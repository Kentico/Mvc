<#
.SYNOPSIS
    Prepares a copies new deployment package with Kentico MVC website and with MVC site installer to desired folder.
	
.EXAMPLE
    .\New-DeploymentPackage.ps1 -WorkingFolder "D:\Deploy" -TargetLocation "\\kentico\dev\Build\MVC\"
#>
param(
	[string] $WorkingFolder,
    [string] $TargetLocation
)

$nugetFilePath = Resolve-Path .nuget\NuGet.exe
$nugetOrgFeed = "https://api.nuget.org/v3/index.json"
$kenticoNugetFeed = "\\kentico\dev\Build\NugetFeed"
$buildNugetFeed = Join-Path $WorkingFolder "generated-packages"
$dancingGoatPath = Join-Path $WorkingFolder "src\DancingGoat"
$csprojFilePath = Join-Path $dancingGoatPath "DancingGoat.csproj"
$dancingGoatFolder = Join-Path $WorkingFolder "publish"
$dancingGoatZip = Join-Path $TargetLocation "DancingGoatPublish.zip"
$installerSolution = Join-Path $WorkingFolder "tools\SiteInstaller\SiteInstaller.sln"
$installerFolder = Join-Path $WorkingFolder "tools\SiteInstaller\SiteInstaller\bin\Release"
$installerZip = Join-Path $TargetLocation "SiteInstaller.zip"

# Stop further execution in case of any error
$ErrorActionPreference = "Stop"

."$PSScriptRoot\SharedFunctions.ps1"

function Create-DancingGoatPackage([string]$CsprojFilePath, [string]$TargetFolder) 
{
    Write-Host "Building DancingGoat deployment package"

	$msBuildArgs = $CsprojFilePath , "/t:Build;PipelinePreDeployCopyAllFilesToOneFolder" , "/p:TargetFramework=v4.5", "/p:Configuration=Release" , "/p:_PackageTempDir=$TargetFolder"
	
    $msBuild = Get-MsBuildPath
    &$msBuild $msBuildArgs | Write-Verbose

    if (-Not ($LASTEXITCODE -eq 0)) 
	{
        Write-Warning "DancingGoatMvc build failed!"
        exit
    }
}

function Create-ZipFile([string]$SourceFolder, [string]$ZipFile)
{
	If (Test-Path $ZipFile)
	{
		Remove-Item $ZipFile
	}

	Add-Type -assembly "system.io.compression.filesystem"
	[io.compression.zipfile]::CreateFromDirectory($SourceFolder, $ZipFile) 
}

If (-Not (Check-AdministratorRights -eq $true))
{
    exit
}

# MVC site processing
Restore-NuGetPackages -NugetFilePath $nugetFilePath -SourceDirPath $WorkingFolder -KenticoNugetFeed $kenticoNugetFeed -NugetOrgFeed $nugetOrgFeed
Generate-BranchNuGetPackages -SourceDirPath $WorkingFolder -BuildNugetFeed $buildNugetFeed
Install-BranchNuGetPackages -SourceDirPath $WorkingFolder -CsprojFilePath $csprojFilePath -BuildNugetFeed $buildNugetFeed -KenticoNugetFeed $kenticoNugetFeed
Remove-KenticoProjectReferences -CsprojFilePath $csprojFilePath
Create-DancingGoatPackage -CsprojFilePath $csprojFilePath -TargetFolder $dancingGoatFolder
Restore-NuGetPackagesForSiteInstaller -NugetFilePath $nugetFilePath -SourceDirPath $WorkingFolder -KenticoNugetFeed $kenticoNugetFeed
Build-SiteInstaller -SiteInstallerSolution $installerSolution
Install-DancingGoatMvcNugetPackage -NugetFilePath $nugetFilePath -SourceDirPath $WorkingFolder -KenticoNugetFeed $kenticoNugetFeed

# DancingGoat website zip package
Create-ZipFile -SourceFolder $dancingGoatFolder -ZipFile $dancingGoatZip

# SiteInstaller utility zip package
Create-ZipFile -SourceFolder $installerFolder -ZipFile $installerZip

# MVC site import zip package
$mvcSitePackage = Get-ChildItem -Path $WorkingFolder\packages\Kentico.MvcDancingGoat* -Filter DancingGoatMvc.zip -Recurse
Copy-Item $mvcSitePackage $TargetLocation
