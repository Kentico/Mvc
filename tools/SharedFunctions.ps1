<#
.SYNOPSIS
	Checks that script runs with administrator rights, if so returns true, otherwise false

.PARAMETER ShowWarning
	If true, writes warning to console output
#>
function Check-AdministratorRights([boolean]$ShowWarning = $false)
{
	$hasRights = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
	
	if (-Not $hasRights -and $ShowWarning)
	{
		Write-Warning "You do not have Administrator rights to run this script! Please re-run this script as an Administrator!"
	}
	
	return $hasRights
}

<#
.SYNOPSIS
	Restores nuget package with given parameters

.PARAMETER NugetFilePath
	Path to nuget.exe

.PARAMETER SourceDirPath
	Working folder location

.PARAMETER KenticoNugetFeed
	Kentico nuget feed address

.PARAMETER NugetOrgFeed
	Official nuget feed address
#>
function Restore-NuGetPackages(
	[Parameter(Mandatory=$true)][string] $NugetFilePath, 
	[Parameter(Mandatory=$true)][string] $SourceDirPath,
	[Parameter(Mandatory=$true)][string] $KenticoNugetFeed,
	[Parameter(Mandatory=$true)][string] $NugetOrgFeed
)
{
	Write-Host "Restoring nuget packages."

	Push-Location $SourceDirPath
	&$NugetFilePath restore -Source "$KenticoNugetFeed;$NugetOrgFeed" | Write-Verbose
	Pop-Location

	if (-Not ($LASTEXITCODE -eq 0)) 
	{
		Write-Warning "Error while restoring nuget packages."
		exit
	}
}

<#
.SYNOPSIS
	Generates Kentico nuget packages to given folder

.PARAMETER BuildNugetFeed
	Target folder where packages will be created

.PARAMETER $SourceDirPath
	Working folder location
#>
function Generate-BranchNuGetPackages(
	[Parameter(Mandatory=$true)][string] $SourceDirPath,
	[Parameter(Mandatory=$true)][string] $BuildNugetFeed
)
{
	Write-Host "Generating Kentico.* nuget packages from the branch."

	Push-Location "$SourceDirPath\tools"
	.\CreatePackages.ps1 -OutputFolderPath $BuildNugetFeed -Verbose | Write-Verbose
	Pop-Location
}

<#
.SYNOPSIS
	Returns all project references to Kentico libraries in given project file

.PARAMETER ProjectFile
	Project file
#>
function Get-KenticoProjectReferences([xml] $ProjectFile) 
{
	# Find all Kentico.* project references
	return $ProjectFile.Project.ItemGroup.ProjectReference | where { $_.Name -like "Kentico.*"}
}

<#
.SYNOPSIS
	Installs Kentico nuget packages

.PARAMETER SourceDirPath
	Working folder location

.PARAMETER CsprojFilePath
	Path to project file

.PARAMETER BuildNugetFeed
	Target folder where packages will be created

.PARAMETER KenticoNugetFeed
	Kentico nuget feed address
#>
function Install-BranchNuGetPackages(
	[Parameter(Mandatory=$true)][string] $SourceDirPath,
	[Parameter(Mandatory=$true)][string] $CsprojFilePath,
	[Parameter(Mandatory=$true)][string] $BuildNugetFeed,
	[Parameter(Mandatory=$true)][string] $KenticoNugetFeed
)
{
	$NugetOrgFeed = "https://api.nuget.org/v3/index.json"
	
	Push-Location $SourceDirPath
	foreach ($projectReference in Get-KenticoProjectReferences(Get-Content $CsprojFilePath)) 
	{
		$projectName = $projectReference.Name

		Write-Host "Installing package '$projectName'."
		&$nugetFilePath install $projectName -OutputDirectory .\packages -Prerelease -Source "$NugetOrgFeed;$BuildNugetFeed;$KenticoNugetFeed" | Write-Verbose
		&$nugetFilePath update $CsprojFilePath -id $projectName -Prerelease -FileConflictAction Ignore -Source "$BuildNugetFeed;$KenticoNugetFeed" | Write-Verbose
	}
	Pop-Location

	if (-Not ($LASTEXITCODE -eq 0)) 
	{
		Write-Warning "Error while installing nuget packages."
		exit
	}
}

<#
.SYNOPSIS
	Removes Kentico project references from given project file

.PARAMETER CsprojFilePath
	Path to project file
#>
function Remove-KenticoProjectReferences(
	[Parameter(Mandatory=$true)][string] $CsprojFilePath
) 
{
	[xml]$csproj = Get-Content $CsprojFilePath
	foreach ($projectReference in Get-KenticoProjectReferences($csproj)) 
	{
		$projectName = $projectReference.Name
		Write-Host "Removing project reference '$projectName'."
		$projectReference.ParentNode.RemoveChild($projectReference) | Out-Null
	}

	$csproj.Save($CsprojFilePath)
}

<#
.SYNOPSIS
	Restores nuget package for site installer utility
	
.PARAMETER NugetFilePath
	Path to nuget.exe

.PARAMETER SourceDirPath
	Working folder location

.PARAMETER KenticoNugetFeed
	Kentico nuget feed address
#>
function Restore-NuGetPackagesForSiteInstaller(
	[Parameter(Mandatory=$true)][string] $NugetFilePath,
	[Parameter(Mandatory=$true)][string] $SourceDirPath,
	[Parameter(Mandatory=$true)][string] $KenticoNugetFeed
) 
{
	Write-Host "Restoring nuget packages for SiteInstaller."

	Push-Location $SourceDirPath\tools\SiteInstaller
	&$NugetFilePath restore -Source $KenticoNugetFeed | Write-Verbose
	Pop-Location

	if (-Not ($LASTEXITCODE -eq 0)) 
	{
		Write-Warning "Restoring nuget packages for SiteInstaller failed!"
		exit
	}
}

<#
.SYNOPSIS
	Returns path to installed MsBuild
#>
function Get-MsBuildPath() 
{
	Write-Host "Getting MSBuild path"

	$msBuildVersion = "14.0"
	$regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$msBuildVersion"
	$regProperty = "MSBuildToolsPath"

	$msbuildExe = Join-Path -Path (Get-ItemProperty $regKey).$regProperty -ChildPath "msbuild.exe"

	Write-Host "MSBuild path is $msbuildExe"
	
	$msbuildExe
}

<#
.SYNOPSIS
	Runs compilation on site installer utility solution

.PARAMETER SiteInstallerSolution
	Path to site installer utility solution file
#>
function Build-SiteInstaller(
	[Parameter(Mandatory=$true)][string] $SiteInstallerSolution
)
{
	Write-Host "Building SiteInstaller"

	$msBuild = Get-MsBuildPath
	&$msBuild $SiteInstallerSolution /t:Build /p:Configuration=Release /p:TargetFramework=v4.5 | Write-Verbose

	if (-Not ($LASTEXITCODE -eq 0)) 
	{
		Write-Warning "Building SiteInstaller failed!"
		exit
	}
}

<#
.SYNOPSIS
	Installs Dancing Goat nuget packages
	
.PARAMETER NugetFilePath
	Path to nuget.exe

.PARAMETER SourceDirPath
	Working folder location

.PARAMETER KenticoNugetFeed
	Kentico nuget feed address
#>
function Install-DancingGoatMvcNugetPackage(
	[Parameter(Mandatory=$true)][string] $NugetFilePath,
	[Parameter(Mandatory=$true)][string] $SourceDirPath,
	[Parameter(Mandatory=$true)][string] $KenticoNugetFeed
)
{
	Push-Location $SourceDirPath
	$dancingGoatNugetPackage = "Kentico.MVCDancingGoat.ImportPackage"
	Write-Host "Installing package '$dancingGoatNugetPackage'."
	&$NugetFilePath install $dancingGoatNugetPackage -OutputDirectory .\packages -Prerelease -Source $KenticoNugetFeed | Write-Verbose
	Pop-Location

	if (-Not ($LASTEXITCODE -eq 0)) 
	{
		Write-Warning "Instalation of ImportPackage failed with code $LASTEXITCODE"
		exit
	}
}