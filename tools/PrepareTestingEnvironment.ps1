<#
.SYNOPSIS
    Prepares a testing environment of the Kentico MVC 10 solution for the given branch.
.DESCRIPTION
    This script generates a testing environment for the given branch.
    Testing environment contains Kentico web application, database, MVC application with web farms enabled

    UNABLE TO CONNECT TO GITHUB ?
    If you are not able to connect to GitHub, you need to store your GitHub credentials first:
        git config --global credential.helper store
        git clone https://github.com/Kentico/Mvc-dev.git  --- this saves the password


.EXAMPLE
    .\PrepareTestingEnvironment.ps1 -Branch <branchName> -DbAuthentication SQL -DbServerName SQL-123 -DbName Kentico10MVC -DbUsername sa -DbPassword sa
    .\PrepareTestingEnvironment.ps1 -DbServerName SQL-03\sql2012
    .\PrepareTestingEnvironment.ps1 -DbServerName AA-01-PC -DbAuthentication SQL -DbUsername sa -DbPassword sa -Verbose
#>
param(
    [string] $Branch = 'master',
    [string] $RepositoryUrl = 'https://github.com/Kentico/Mvc-dev.git',
    # Trim the branch name to decrease the length of the directory path (path cannot be more that 260 characters). 7 = Max("CM-1234")
    [string] $TargetLocation = "C:\Rep-Builds\Mvc10\" + $Branch.Substring(0, ($Branch.length, 7 | Measure-Object -Minimum).Minimum),
    [string] $CmsInstallPath = "$TargetLocation\CMS",
    [ValidateSet("Windows", "SQL")]
    [string] $DbAuthentication = "Windows",
    [Parameter(Mandatory=$true)]
    [string] $DbServerName,
    [string] $DbName = "$env:USERNAME-$Branch-Kentico10",
    [string] $DbUsername,
    [string] $DbPassword
)

$latestBuildName = Get-Content "\\kentico\dev\Build\CMS\CMS10\10.0\latest_build.txt"
$latestBuildPath = "\\master.kentico.com\dev\Build\CMS\CMS10\10.0\$latestBuildName"

$nugetFilePath = Resolve-Path ..\.nuget\NuGet.exe
$buildNugetFeed =  "$TargetLocation\generated-packages"
$kenticoNugetFeed = "$latestBuildPath\Testing\NuGetPackages"
$nugetOrgFeed = "https://api.nuget.org/v3/index.json"
$sourceDirPath = "$TargetLocation\source"
$dancingGoatPath = "$sourceDirPath\src\DancingGoat"
$csprojFilePath = "$dancingGoatPath\DancingGoat.csproj"
$webConfigFilePath = "$dancingGoatPath\Web.config"
$iisWebSiteName = "TestDancingGoatMvc"
$iisWebSiteIpAddress = "127.0.0.78"
$siteInstallerSln = "$sourceDirPath\tools\SiteInstaller\SiteInstaller.sln"
$googleMapsApiKey = "AIzaSyAMLtbz8zOr30PFjarqgIETByEZMvMHT-g"

# Stop further execution in case of any error
$ErrorActionPreference = "Stop"

."$PSScriptRoot\SharedFunctions.ps1"

function Ensure-DownloadPath() {
    if (Test-Path $TargetLocation) {
        $confirmation = Read-Host "Testing environment for the branch '$Branch' has been already prepared. Do you want to ovewrite the existing environment? (Y/N)"
        if ($confirmation -eq 'y') {
            Write-Host "Deleting existing environment."

            foreach ($i in 1..5) {
                try {
                    Remove-Item $TargetLocation -Force -Recurse
                    break;
                }
                catch {
                    Write-Host "There was an error while deleting $TargetLocation. Let's try it again! ($i/5)"
                    Start-Sleep -Seconds 3
                }
            }
            Write-Host "Folder $TargetLocation successfully deleted."
        }
        else {
            exit
        }
    } else {
        Write-Host "Creating folder '$TargetLocation'."
        New-Item -ItemType directory -Path $TargetLocation | Write-Verbose
    }
}


function Get-Branch() {
    Write-Host "Getting '$Branch' branch from $RepositoryUrl."
    git clone $RepositoryUrl --branch $Branch $sourceDirPath --quiet --single-branch

    if (-Not ($LASTEXITCODE -eq 0)) {
        Write-Warning "There was an error while cloning repository. Branch '$Branch' probably does not exist."
        exit
    }
}


function Remove-WebConfigAttributes() {
    Write-Host "Removing application settings and connection strings attributes from 'Web.config'."

    [xml]$webconfig = Get-Content $webConfigFilePath
    $webconfig.configuration.connectionStrings.RemoveAttribute("configSource");
    $webconfig.configuration.appSettings.RemoveAttribute("file");

    $webconfig.Save($webConfigFilePath)
}


function Build-DancingGoat() {
    Write-Host "Building DancingGoat"

    $msBuild = Get-MsBuildPath
    &$msBuild $csprojFilePath /t:Build /p:Configuration=Release /p:TargetFramework=v4.5 | Write-Verbose

    if (-Not ($LASTEXITCODE -eq 0)) {
        Write-Warning "DancingGoatMvc build failed!"
        exit
    }
}


function Copy-LatestKIM() {
    $signedKIMPath = "$latestBuildPath\Kentico_10_0.exe"
    $unsignedKIMPath = "$latestBuildPath\Kentico_10_0_not_signed.exe"

    if (Test-Path $signedKIMPath) {
        $latestKimExe = Get-ChildItem $signedKIMPath
    }
    elseif (Test-Path $unsignedKIMPath) {
        $latestKimExe = Get-ChildItem $unsignedKIMPath
    }
    else {
        Write-Host "Could not find KIM executable file in the location '$latestBuildPath'."
        exit
    }

    Write-Host "Copying KIM executable from" $latestKimExe.FullName
    Copy-Item -Path $latestKimExe.FullName -Destination "$TargetLocation\Kentico_10_0_build.exe"
}


function Create-InstallProfile() {
    $installProfile = @"
    <SilentInstall ShowProgress="CommandPrompt" Log="true" OnError="Stop" LogFile="{%programfiles%}\Kentico\10.0\setup.log" CheckRequirements="False">
        <Setup NET="4.5" SetupFolder="{%programfiles%}\Kentico\10.0" Location="Local" InstallOnlyProgramFiles="False" 
            OpenAfterInstall="false" DeleteExisting="True" DoNotOverwriteInstallation="True" RegisterCounters="False" 
            InstallWinServices="False" RegisterApplicationToEventLog="False" WebProject="WebApplication" 
            KillRunningProcesses="False" EnableModuleUsageTracking = "false" />
        <IIS Website="$iisWebSiteName" TargetFolder="$CmsInstallPath" DeleteExisting="True" KillRunningProcesses="True" />
        <Sql InstallDatabase="true" Server="$DbServerName" Authentication="$DbAuthentication" SqlName="$DbUsername" SqlPswd="$DbPassword" 
            Database="$DbName" Operation="New" DeleteExisting="true" />
        <Modules type="InstallAll" />
        <WebTemplates type="InstallAll" />
        <UICultures type="InstallAll" />
        <Dictionaries type="InstallAll" />
        <WebSites></WebSites>
        <Licenses>
            <License domain="localhost"><![CDATA[DOMAIN:localhost
PRODUCT:CX10
EXPIRATION:00000000
SERVERS:0
JFM736MHBjKbPHqrtg66tRG8LINNwSc+VxjQIQ9n8Vi4u6qZlVlbrweW44Hl6ECKTFyXqTFh7nDVtRfps2w2c8/ZOAQ2G6WyQ0W7lCCgX82KxrL3BLDXu0TVdYU3ADPhjlAAnW0uIMoBcx0SYVipeaRqadZ5HjgXLJACAMn+Hs4gix36x211jwLSWlWeD7v6QhcbDFl36PVult6myDt777olkFSZaMcjL8AtRji99ovFIPnAg8sUShSxqVq/ztX6P4q/s9hKA7dkxsOKlj3LbxepyWf/GhqCeYZyC7diGU21lo0GIS3paZEcK4W24iJpfLxW3tWADfUQ7ADw0PiW9g==]]></License>
        </Licenses>
    </SilentInstall>
"@

    Write-Verbose "Saving install profile"
    Write-Verbose $installProfile
    $installProfile | Out-File $TargetLocation\installProfile.xml
}


function Create-WebSite() {
    Write-Host "Creating website $iisWebSiteName"

    New-Website -Name $iisWebSiteName -IPAddress $iisWebSiteIpAddress -PhysicalPath $TargetLocation -Force | Write-Verbose
    Start-Website -Name $iisWebSiteName | Write-Verbose
}


function Install-KenticoCMS() {
    Write-Host "Installing Kentico CMS"
    $installProfile = Get-ChildItem $TargetLocation\installProfile.xml
    &$TargetLocation\Kentico_10_0_build.exe $installProfile | Write-Verbose

    if (-Not ($LASTEXITCODE -eq 0)) {
        Write-Warning "Kentico CMS installation failed!"
        exit
    }
}


function Import-DancingGoatMvc() {
    $siteInstallerExe = "$sourceDirPath\tools\SiteInstaller\SiteInstaller\bin\Release\SiteInstaller.exe"

    $domain = "localhost"
    $package = Get-ChildItem -Path $sourceDirPath\packages\Kentico.MvcDancingGoat* -Filter DancingGoatMvc.zip -Recurse
    $siteName = "DancingGoatMvc"
    $webSitePath = "$CmsInstallPath\CMS"

    Write-Host "Importing $siteName"
    &$siteInstallerExe siteName=$siteName domain=$domain package=$package webSitePath=$webSitePath

    if (!($LASTEXITCODE -eq 0)) {
        Write-Host "$siteInstallerExe exited with code $LASTEXITCODE"
        exit
    }
}


function Set-ApplicationSettings() {
    Write-Host "Copying ConnectionString and HashSalt from CMS web.config and adding GoogleMapsApiKey to MVC web.config"
    
    $cmsWebConfig = New-Object System.Xml.XmlDocument
    $cmsWebConfig.Load("$CmsInstallPath\CMS\web.config")

    $connectionString = $cmsWebConfig.SelectSingleNode("//add[@name='CMSConnectionString']").connectionString
    $hashSalt = $cmsWebConfig.SelectSingleNode("//add[@key='CMSHashStringSalt']").value

    $dgWebConfig = New-Object System.Xml.XmlDocument
    $dgWebConfig.load($webConfigFilePath)

    $dgWebConfig.SelectSingleNode("//add[@name='CMSConnectionString']").connectionString = "$connectionString"
    $dgWebConfig.SelectSingleNode("//add[@key='CMSHashStringSalt']").value = "$hashSalt"
    
    $googleMapsApiKeySetting = $dgWebConfig.CreateElement("add")
    $dgWebConfig.configuration.appSettings.AppendChild($googleMapsApiKeySetting) | Out-Null
    $googleMapsApiKeySetting.SetAttribute("key", "GoogleMapsApiKey")
    $googleMapsApiKeySetting.SetAttribute("value", "$googleMapsApiKey")
    
    $dgWebConfig.Save($webConfigFilePath)
}


function Get-ConnectionString() {
    $cmsWebConfig = New-Object System.Xml.XmlDocument
    $cmsWebConfig.Load("$CmsInstallPath\CMS\web.config")
    $cmsWebConfig.SelectSingleNode("//add[@name='CMSConnectionString']").connectionString
}


function Get-DancingGoatMvcSitePresentationUrl() {
    $SqlCommand = "SELECT SitePresentationUrl FROM CMS_Site WHERE SiteName = 'DancingGoatMvc'"

    $Connection = New-Object System.Data.SQLClient.SQLConnection
    $Connection.ConnectionString = Get-ConnectionString
    $Connection.Open()
    $Command = New-Object System.Data.SQLClient.SQLCommand
    $Command.Connection = $Connection
    $Command.CommandText = $SqlCommand

    [string]$url = $Command.ExecuteScalar()
    
    $Connection.Close() | Out-Null
    
    $url
}


function Enable-WebFarms() {
    Write-Host "Enabling WebFarms"

    # KeyValue = 1 enables WebFarm automatic mode
    $SqlCommand = "UPDATE CMS_SettingsKey SET KeyValue = 1 WHERE KeyName = 'CMSWebFarmMode'"

    $Connection = New-Object System.Data.SQLClient.SQLConnection
    $Connection.ConnectionString = Get-ConnectionString
    $Connection.Open()
    $Command = New-Object System.Data.SQLClient.SQLCommand
    $Command.Connection = $Connection
    $Command.CommandText = $SqlCommand
    $Command.ExecuteNonQuery() | Out-Null
    $Connection.Close() | Out-Null
}


function Set-TestUrl() {
    $url = Get-DancingGoatMvcSitePresentationUrl
    $url = $url.Replace('DancingGoatMvc', $iisWebSiteName);

    Write-Host "Setting web site URL to $url"

    $SqlCommand = "UPDATE CMS_Site SET SitePresentationURL = '$url' FROM CMS_Site WHERE SiteName = 'DancingGoatMvc'"

    $Connection = New-Object System.Data.SQLClient.SQLConnection
    $Connection.ConnectionString = Get-ConnectionString
    $Connection.Open()
    $Command = New-Object System.Data.SQLClient.SQLCommand
    $Command.Connection = $Connection
    $Command.CommandText = $SqlCommand
    $Command.ExecuteNonQuery() | Out-Null
    $Connection.Close() | Out-Null
}


function Start-DancingGoatMvc() {
    Write-Host "Starting DancingGoatMvc"

    $SqlCommand = "UPDATE CMS_Site SET SiteStatus = 'RUNNING' WHERE SiteName = 'DancingGoatMvc'"

    $Connection = New-Object System.Data.SQLClient.SQLConnection
    $Connection.ConnectionString = Get-ConnectionString
    $Connection.Open()
    $Command = New-Object System.Data.SQLClient.SQLCommand
    $Command.Connection = $Connection
    $Command.CommandText = $SqlCommand
    $Command.ExecuteNonQuery() | Out-Null
    $Connection.Close() | Out-Null
}


function Recycle-KenticoCMSAppPool() {
    $webSite = Get-Item "IIS:\sites\$iisWebSiteName"
    $cmsApp = $webSite.Collection | where {$_.GetAttributeValue("path") -eq '/CMS'}
    $poolName = $cmsApp.applicationPool

    Restart-WebAppPool $poolName
}


function Deploy-DancingGoatMvc() {
    $path = "$sourceDirPath\src\DancingGoat"
    
    Write-Host "Deploying Web Application (Name: $name, Site: $site, PysicalPath: $path)"
    
    New-WebApplication -Name $iisWebSiteName -Site $iisWebSiteName -PhysicalPath $path -Force | Out-Null
}


function Open-InBrowser() {
    # Open newly created MVC website in browser
    $mvcSiteUrl = Get-DancingGoatMvcSitePresentationUrl
    Start-Process -FilePath $mvcSiteUrl | Out-Null

    # Wait for the browser to open
    # That way the administration will be opened in a new tab instead of a new window
    Start-Sleep -s 2

    # Open newly installed Kentico administration in browser
    $administrationUrl = "http://$iisWebSiteIpAddress/CMS/Admin"
    Start-Process -FilePath $administrationUrl | Out-Null
}


If (-Not (Check-AdministratorRights -eq $true))
{
    exit
}

# Ensure that IIS cmdlets are available
Import-Module WebAdministration

$startTime = Get-Date


Ensure-DownloadPath
Get-Branch
Restore-NuGetPackages -NugetFilePath $nugetFilePath -SourceDirPath $sourceDirPath -KenticoNugetFeed $kenticoNugetFeed -NugetOrgFeed $nugetOrgFeed
Generate-BranchNuGetPackages -SourceDirPath $sourceDirPath -BuildNugetFeed $buildNugetFeed
Install-BranchNuGetPackages -SourceDirPath $sourceDirPath -CsprojFilePath $csprojFilePath -BuildNugetFeed $buildNugetFeed -KenticoNugetFeed $kenticoNugetFeed
Remove-KenticoProjectReferences -CsprojFilePath $csprojFilePath
Remove-WebConfigAttributes
Build-DancingGoat -CsprojFilePath $csprojFilePath -TargetFolder $dancingGoatFolder
Copy-LatestKIM
Create-InstallProfile
Create-WebSite
Install-KenticoCMS
Restore-NuGetPackagesForSiteInstaller -NugetFilePath $nugetFilePath -SourceDirPath $sourceDirPath -KenticoNugetFeed $kenticoNugetFeed
Build-SiteInstaller -SiteInstallerSolution $siteInstallerSln
Install-DancingGoatMvcNugetPackage -NugetFilePath $nugetFilePath -SourceDirPath $sourceDirPath -KenticoNugetFeed $kenticoNugetFeed
Import-DancingGoatMvc
Set-ApplicationSettings
Enable-WebFarms
Set-TestUrl
Start-DancingGoatMvc
Recycle-KenticoCMSAppPool
Deploy-DancingGoatMvc
Open-InBrowser

$endTime = Get-Date

Write-Host "Deployed in " ($endTime - $startTime).ToString()
