param (
	$ChartName="microappstudio01",
	$Namespace="microappstudio01-local",
	$ReleaseName="microappstudio01-local",
	$DotnetEnvironment="Staging",
    $User = ""
)

$VersionLog = Get-Date -Format "yyyyMMdd.HHmmss"

# Create values.localdev.yaml if not exists
$localDevFilePath = Join-Path $PSScriptRoot "microappstudio01/values.azure.st.yaml"
if (!(Test-Path $localDevFilePath)) {
	New-Item -ItemType File -Path $localDevFilePath | Out-Null
}

$FinalReleaseName = $ReleaseName
if([string]::IsNullOrEmpty($User) -eq $false)
{
    $Namespace += '-' + $User
    $FinalReleaseName += '-' + $User
}

# Install (or upgrade) the Helm chart / en modo debug
helm upgrade --install ${FinalReleaseName} ${ChartName} --namespace ${Namespace} --create-namespace --set global.dotnetEnvironment=${DotnetEnvironment} -f "microappstudio01/values.azure.st.yaml"  --dry-run=client --debug > "debug-helm-${VersionLog}.yaml"