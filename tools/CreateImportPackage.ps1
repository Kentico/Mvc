param([string] $SolutionFolderPath)

$solutionFolderPath = Resolve-Path $SolutionFolderPath;
$temporaryFolderPath = New-Item -ItemType directory -Path ".\Temp\" -Force;
$outputZipFilePath = (Resolve-Path ".\..\webtemplates\").Path + "DancingGoatMvc.zip";

# Run default data export
& "$solutionFolderPath\DefaultDataExport\bin\Debug\DefaultDataExport.exe" "-webtemplatedata" "-targetpath:$temporaryFolderPath" "-webprojectpath:$solutionFolderPath\CMS\" "-exportincludeprefixes:DancingGoatMvc" "-sites:DancingGoatMvc";

# Zip package
If (Test-Path $outputZipFilePath){
	Remove-Item $outputZipFilePath -Force -Recurse;
}
Add-Type -assembly "system.io.compression.filesystem";
[io.compression.zipfile]::CreateFromDirectory("$temporaryFolderPath\DancingGoatMvc\", $outputZipFilePath); 

# Clean up temporary folder
If (Test-Path $temporaryFolderPath){
	Remove-Item $temporaryFolderPath -Force -Recurse;
}