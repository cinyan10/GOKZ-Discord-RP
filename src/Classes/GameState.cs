//
// Originally auto-generated from https://app.quicktype.io/
//

namespace GameState;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

public partial class TopLevel
{
	[JsonProperty( "provider" )]
	public Provider Provider { get; set; }

	[JsonProperty( "map" )]
	public Map Map { get; set; }

	[JsonProperty( "player" )]
	public Player Player { get; set; }

	// Optional: present in your sample
	[JsonProperty( "previously", NullValueHandling = NullValueHandling.Ignore )]
	public Previously Previously { get; set; }
}

public partial class Map
{
	[JsonProperty( "mode" )]
	public string Mode { get; set; }

	[JsonProperty( "name" )]
	public string Name { get; set; }

	[JsonProperty( "phase" )]
	public string Phase { get; set; }

	[JsonProperty( "round" )]
	public long Round { get; set; }

	[JsonProperty( "team_ct" )]
	public Team TeamCt { get; set; }

	[JsonProperty( "team_t" )]
	public Team TeamT { get; set; }

	[JsonProperty( "num_matches_to_win_series" )]
	public long NumMatchesToWinSeries { get; set; }

	[JsonProperty( "current_spectators" )]
	public long CurrentSpectators { get; set; }

	[JsonProperty( "souvenirs_total" )]
	public long SouvenirsTotal { get; set; }
}

public partial class Team
{
	[JsonProperty( "score" )]
	public long Score { get; set; }

	[JsonProperty( "consecutive_round_losses" )]
	public long ConsecutiveRoundLosses { get; set; }

	[JsonProperty( "timeouts_remaining" )]
	public long TimeoutsRemaining { get; set; }

	[JsonProperty( "matches_won_this_series" )]
	public long MatchesWonThisSeries { get; set; }
}

public partial class Player
{
	[JsonProperty( "steamid" )]
	public string Steamid { get; set; }

	// NEW: read the clan tag (e.g., "[KZT Semipro]")
	[JsonProperty( "clan", NullValueHandling = NullValueHandling.Ignore )]
	public string Clan { get; set; }

	[JsonProperty( "name" )]
	public string Name { get; set; }

	[JsonProperty( "observer_slot" )]
	public long ObserverSlot { get; set; }

	[JsonProperty( "team" )]
	public string Team { get; set; }

	[JsonProperty( "activity" )]
	public string Activity { get; set; }

	[JsonProperty( "match_stats" )]
	public MatchStats MatchStats { get; set; }

	[JsonProperty( "state" )]
	public State State { get; set; }
}

public partial class MatchStats
{
	[JsonProperty( "kills" )]
	public long Kills { get; set; }

	[JsonProperty( "assists" )]
	public long Assists { get; set; }

	[JsonProperty( "deaths" )]
	public long Deaths { get; set; }

	[JsonProperty( "mvps" )]
	public long Mvps { get; set; }

	[JsonProperty( "score" )]
	public long Score { get; set; }
}

public partial class State
{
	[JsonProperty( "health" )]
	public long Health { get; set; }

	[JsonProperty( "armor" )]
	public long Armor { get; set; }

	[JsonProperty( "helmet" )]
	public bool Helmet { get; set; }

	[JsonProperty( "defusekit" )]
	public bool Defusekit { get; set; }

	[JsonProperty( "flashed" )]
	public long Flashed { get; set; }

	[JsonProperty( "smoked" )]
	public long Smoked { get; set; }

	[JsonProperty( "burning" )]
	public long Burning { get; set; }

	[JsonProperty( "money" )]
	public long Money { get; set; }

	[JsonProperty( "round_kills" )]
	public long RoundKills { get; set; }

	[JsonProperty( "round_killhs" )]
	public long RoundKillhs { get; set; }

	[JsonProperty( "equip_value" )]
	public long EquipValue { get; set; }
}

public partial class Provider
{
	[JsonProperty( "name" )]
	public string Name { get; set; }

	[JsonProperty( "appid" )]
	public long Appid { get; set; }

	[JsonProperty( "version" )]
	public long Version { get; set; }

	[JsonProperty( "steamid" )]
	public string Steamid { get; set; }

	[JsonProperty( "timestamp" )]
	public long Timestamp { get; set; }
}

// NEW: optional "previously" block (as in your sample)
public partial class Previously
{
	[JsonProperty( "player", NullValueHandling = NullValueHandling.Ignore )]
	public PreviouslyPlayer Player { get; set; }
}

public partial class PreviouslyPlayer
{
	[JsonProperty( "match_stats", NullValueHandling = NullValueHandling.Ignore )]
	public PreviouslyMatchStats MatchStats { get; set; }
}

public partial class PreviouslyMatchStats
{
	[JsonProperty( "kills", NullValueHandling = NullValueHandling.Ignore )]
	public long? Kills { get; set; }

	[JsonProperty( "assists", NullValueHandling = NullValueHandling.Ignore )]
	public long? Assists { get; set; }

	[JsonProperty( "deaths", NullValueHandling = NullValueHandling.Ignore )]
	public long? Deaths { get; set; }

	[JsonProperty( "mvps", NullValueHandling = NullValueHandling.Ignore )]
	public long? Mvps { get; set; }

	[JsonProperty( "score", NullValueHandling = NullValueHandling.Ignore )]
	public long? Score { get; set; }
}

public partial class TopLevel
{
	public static TopLevel FromJson( string json ) => JsonConvert.DeserializeObject<TopLevel>( json, Converter.Settings );
}

public static class Serialize
{
	public static string ToJson( this TopLevel self ) => JsonConvert.SerializeObject( self, Converter.Settings );
}

internal static class Converter
{
	public static readonly JsonSerializerSettings Settings = new()
	{
		MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
		DateParseHandling = DateParseHandling.None,
		Converters =
		{
			new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
		},
	};
}
