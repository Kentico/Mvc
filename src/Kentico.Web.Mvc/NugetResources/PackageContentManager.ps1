param (
    [string]$ProjectDir,
    [string]$SolutionDir
)

Function Prepare-Content
{
    $applicationConfig = [System.IO.Path]::GetFullPath((Join-Path $SolutionDir 'MvcDemo.Web\App_Start\ApplicationConfig.cs'))
    $applicationConfigPp = [System.IO.Path]::GetFullPath((Join-Path $ProjectDir 'NugetResources\ApplicationConfig.cs.pp'))
	$mimeTypes = [System.IO.Path]::GetFullPath((Join-Path $SolutionDir 'MvcDemo.Web\App_Data\mimetypes.txt'))

    (Get-Content $applicationConfig) |
    %{
        $_ -replace '^namespace .*', 'namespace $rootnamespace$'
    } |
    Out-File $applicationConfigPp

	if (Test-Path $mimeTypes)
	{
		Copy-Item $mimeTypes "$ProjectDir\NugetResources\" -Force
	}
}

Prepare-Content
