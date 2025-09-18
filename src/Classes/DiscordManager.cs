using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Diagnostics;              // Process monitor
using Newtonsoft.Json.Linq;
using DiscordRPC;
using DiscordRPC.Logging;
using GameState;

namespace RichPresenceApp;

public class DiscordManager
{
	public static DiscordRpcClient Client;

	// Elapsed timer start (set once when first valid GSI payload is seen)
	private static DateTime? _gameStartUtc = null;

	// HTTP client + cache for map difficulties
	private static readonly HttpClient _http = new HttpClient();
	private static readonly Dictionary<string, string> _difficultyCache = new();

	// Process watchdog (polls for csgo.exe)
	private static System.Timers.Timer _procWatchdogTimer;
	private static bool _csgoRunningLast = false;

	public static void Initialize()
	{
		// Your Discord Application (Client) ID
		Client = new DiscordRpcClient( "1400566342687522976", autoEvents: false )
		{
			Logger = new ConsoleLogger() { Level = LogLevel.Warning }
		};

		Client.OnReady += ( sender, args ) =>
		{
			Console.WriteLine( "[DISCORD] Received 'ready' from user {0}", args.User.Username );
		};

		Client.OnPresenceUpdate += ( sender, args ) =>
		{
			Console.WriteLine( "[DISCORD] Presence updated!" );
		};

		Client.Initialize();

		// Start a lightweight watchdog that checks csgo.exe every 3s
		_procWatchdogTimer = new System.Timers.Timer( 3000 );
		_procWatchdogTimer.Elapsed += ( s, e ) => CheckCsgoProcess();
		_procWatchdogTimer.AutoReset = true;
		_procWatchdogTimer.Start();
	}

	private static void CheckCsgoProcess()
	{
		bool runningNow = IsProcessRunning( "csgo" ); // Only CS:GO (not CS2)

		// Transition: running -> not running  ==> clear presence immediately
		if ( _csgoRunningLast && !runningNow )
		{
			try
			{
				Client?.ClearPresence();
				_gameStartUtc = null; // reset session start; next launch starts fresh
				Console.WriteLine( "[DISCORD] csgo.exe not found. Presence cleared." );
			}
			catch { /* ignore */ }
		}

		_csgoRunningLast = runningNow;
	}

	private static bool IsProcessRunning( string processNameNoExe )
	{
		try
		{
			// Match by name (without .exe). If multiple, any means "running".
			return Process.GetProcessesByName( processNameNoExe ).Length > 0;
		}
		catch
		{
			return false;
		}
	}

	private static string GetMapDifficulty( string mapName )
	{
		if ( _difficultyCache.TryGetValue( mapName, out var cached ) )
			return cached;

		try
		{
			string url = $"https://kztimerglobal.com/api/v2.0/maps/name/{mapName}";
			string json = _http.GetStringAsync( url ).GetAwaiter().GetResult();
			var obj = JObject.Parse( json );

			int diff = obj["difficulty"]?.Value<int>() ?? 0;
			string tier = diff > 0 ? $"T{diff}" : "T?";
			_difficultyCache[mapName] = tier;
			return tier;
		}
		catch ( Exception ex )
		{
			Console.WriteLine( $"[DISCORD] Failed to fetch difficulty for {mapName}: {ex.Message}" );
			return "T?";
		}
	}

	public static RichPresence BuildPresenceFromData( TopLevel gameData )
	{
		// Default presence = menu
		var presence = new RichPresence
		{
			Details = "Main Menu",
			State = "In Menu",
			Assets = new Assets
			{
				LargeImageKey = "menu",
				LargeImageText = "Menu",
			},
			Timestamps = _gameStartUtc.HasValue ? new Timestamps { Start = _gameStartUtc.Value } : null
		};

		// Guard against partial payloads
		if ( gameData == null || gameData.Provider == null || gameData.Map == null || gameData.Player == null )
			return presence;

		// First valid payload → mark session start once
		if ( _gameStartUtc == null )
			_gameStartUtc = DateTime.UtcNow;

		if ( gameData.Player.Activity == "playing" )
		{
			// timer = kills (seconds)
			long killsSeconds = gameData.Player.MatchStats?.Kills ?? 0;
			var ts = TimeSpan.FromSeconds( killsSeconds );
			string timerText = ts.TotalHours >= 1
				? ts.ToString( @"h\:mm\:ss" )
				: ts.ToString( @"mm\:ss" );

			// tp = deaths
			long tp = gameData.Player.MatchStats?.Deaths ?? 0;

			// progress = score / 10 with one decimal
			double progress = (gameData.Player.MatchStats?.Score ?? 0) / 10.0;
			string progressText = $"{progress:0.0}%";

			// KZ mode from clan tag like "[KZT Semipro]" → "KZT"
			string kzMode = "KZT";
			string clan = gameData.Player.Clan;
			if ( !string.IsNullOrWhiteSpace( clan ) )
			{
				int li = clan.IndexOf( '[' );
				int ri = (li >= 0) ? clan.IndexOf( ']', li + 1 ) : -1;
				if ( li >= 0 && ri > li )
				{
					var inside = clan.Substring( li + 1, ri - li - 1 );
					var firstToken = inside.Split( ' ', StringSplitOptions.RemoveEmptyEntries ).FirstOrDefault();
					if ( !string.IsNullOrWhiteSpace( firstToken ) )
						kzMode = firstToken.ToUpperInvariant();
				}
			}

			// Fetch difficulty tier from KZTimerGlobal API (cached)
			string tier = GetMapDifficulty( gameData.Map.Name );

			presence = new RichPresence
			{
				Details = $"Playing {gameData.Map.Name} {tier}",               // First line
				State = $"[{kzMode}] {timerText} | TP: {tp} | {progressText}", // Second line
				Assets = new Assets
				{
					// If you hit the 300-asset cap, swap to a static KZ logo key here.
					LargeImageKey = gameData.Map.Name,
					LargeImageText = $"Playing on {gameData.Map.Name}"
				},
				// Keep same start so green elapsed timer doesn't reset
				Timestamps = new Timestamps { Start = _gameStartUtc.Value }
			};
		}
		else
		{
			// In menu — keep same timestamp so elapsed shows from game start
			presence = new RichPresence
			{
				Details = "Main Menu",
				State = "In Menu",
				Assets = new Assets
				{
					LargeImageKey = "menu",
					LargeImageText = "Main Menu"
				},
				Timestamps = new Timestamps { Start = _gameStartUtc.Value }
			};
		}

		return presence;
	}
}
