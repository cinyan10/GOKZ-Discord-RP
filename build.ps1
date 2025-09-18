# 1) Clean build outputs
dotnet clean src/CS2-Discord-RP.csproj
Remove-Item -Recurse -Force src\bin, src\obj

# 2) Publish as fully portable single file into .\bin (repo root)
dotnet publish "src/CS2-Discord-RP.csproj" `
  -c Release `
  -r win-x64 `
  -o .\bin `
  -p:PublishSingleFile=true `
  -p:SelfContained=true `
  -p:DebugType=None `
  -p:DebugSymbols=false `
  -p:EnableCompressionInSingleFile=true
