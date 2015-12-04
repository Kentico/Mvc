$outputFolderPath = Resolve-Path .
$nugetFilePath = Resolve-Path ..\.nuget\nuget.exe

foreach ($projectFile in Get-ChildItem -Path ..\src -Include *.csproj -Recurse -Exclude DancingGoat.csproj)
{
	&$nugetFilePath pack $projectFile -Build -Properties Configuration=Release -OutputDirectory $outputFolderPath
}