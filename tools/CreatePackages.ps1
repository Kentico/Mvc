[CmdletBinding()]
param(
    [string] $OutputFolderPath = 'C:\LocalNugetFeed',
    [string] $MSBuildPath
)

$nugetFilePath = Resolve-Path ..\.nuget\nuget.exe


function Ensure-Output-Path() {
    if (-Not (Test-Path $OutputFolderPath))
    {
        New-Item -ItemType directory -Path $OutputFolderPath
    }
}


function Create-Packages() {

    foreach ($projectFile in Get-ChildItem -Path ..\src -Include *.csproj -Recurse -Exclude DancingGoat.csproj)
    {
        $cmd_params = 'pack', $projectFile, '-Build', '-Properties', 'Configuration=Release', '-OutputDirectory', $OutputFolderPath
        
        if ($MSBuildPath)
        {
            $cmd_params += '-MSBuildPath', $MSBuildPath
        }
        
        &$nugetFilePath $cmd_params | Write-Verbose
    
        if (-Not ($LASTEXITCODE -eq 0)) 
        {
            Write-Warning "$projectFile build failed!"
            exit($LASTEXITCODE)
        }
    }
}


Ensure-Output-Path
Create-Packages