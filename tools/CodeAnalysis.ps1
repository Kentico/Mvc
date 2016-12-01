<#
.SYNOPSIS
    Runs static code analysis on given solution and imports result to TeamCity

.PARAMETER FxCopPath
    Path to the FxCopCmd.exe, default: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe

.PARAMETER FxRulesetsPath
    Path to the default FxCop rulesets, default: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\Rule Sets

.PARAMETER DllDirectory
    Directory containing compiled binaries

.PARAMETER RulesetFilePath
    Absolute ruleset file path

.PARAMETER FileMask
    Analysed file mask
	
.EXAMPLE
    CodeAnalysis.ps1 -FxCopPath 'C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe' -FxRulesetsPath 'C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\Rule Sets' -DllDirectory 'D:\CodeAnalysis\' -RulesetFilePath 'd:\CodeAnalysis\KenticoMvc.ruleset' -FileMask 'Kentico.*.dll'
#>
Param(
	[string]$FxCopPath = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe",
	[string]$FxRulesetsPath = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\Rule Sets",
	[Parameter(Mandatory=$true)]
	[string]$DllDirectory,
	[Parameter(Mandatory=$true)]
	[string]$RulesetFilePath,
	[Parameter(Mandatory=$true)]
	[string]$FileMask
)

function Copy-RulesetFiles($rulesetFilePath)
{
	$rulesetFolder = [System.IO.Path]::GetDirectoryName($rulesetFilePath)
	& xcopy /I /F /Y $FxRulesetsPath $rulesetFolder
}

function StartAnalysis
{
	$dllFiles = Get-ChildItem $DllDirectory -Include $FileMask -Recurse | Select-Object $_.BaseName
	$outputFile = $DllDirectory + '\codeAnalysis.xml'

	$paremeters = @( "/forceoutput", "/gac", "/ignoregeneratedcode", "/out:$outputFile", "/ruleSet:=$RulesetFilePath" )
	$paremeters += $dllFiles | % { "/f:$_" }

	& $FxCopPath $paremeters

	$message ="##teamcity[importData type='FxCop' path='" + $outputFile + "']"
	Write-Output $message

	return $LASTEXITCODE
}

Copy-RulesetFiles($RulesetFilePath)
StartAnalysis