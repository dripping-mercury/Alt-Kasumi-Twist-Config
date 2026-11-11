# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/p5rpc.event.altkasumitwistr&v/*" -Force -Recurse
dotnet publish "./AltTwist.csproj" -c Release -o "$env:RELOADEDIIMODS/p5rpc.event.altkasumitwistr&v" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location