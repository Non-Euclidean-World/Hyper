<#

.SYNOPSIS
Creates a new release of the Hyper project.
    
.PARAMETER ReleaseName
Name of the release.

.PARAMETER MainBranchCheck
Indicates whether to check if the release is being done from the main branch.

.PARAMETER CreateSourceArchive
Indicates whether to create an archive with the source code.

.EXAMPLE

PS> ./Scripts/New-Release.ps1 -ReleaseName Hyper-v0.0.0

.EXAMPLE

PS> ./Scripts/New-Release.ps1 -ReleaseName Hyper-v0.0.0 -CreateSourceArchive $false -MainBranchCheck $false

#>

param(
    [Parameter(Mandatory)]
    [string]$ReleaseName,
    [bool]$MainBranchCheck = $true,
    [bool]$CreateSourceArchive = $true
)

function CheckIfMainBranchIsTracked {
    $gitBranchOut = @(git branch -vv)
    foreach ($branch in $gitBranchOut) {
        if ($branch -match "\* main\s+\w+ \[origin\/main].*") {
            return $true
        }
    }
    $false
}

function GitCheck {
    # make sure main is the current branch
    $currentBranch = git rev-parse --abbrev-ref HEAD
    if ($currentBranch -cne "main") {
        Write-Host "You're not on the main branch."
        exit
    }

    # check if main is tracked
    if ((CheckIfMainBranchIsTracked) -eq $false) {
        Write-Host "Main not tracked."
        exit
    }

    # update refs
    git fetch

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Couldn't fetch from origin main"
        exit
    }

    # check if main is up-to-date
    $gitStatus = git status -uno
    $expectedGitStatusOutput = "Your branch is up to date with 'origin/main'."

    if ($gitStatus[1] -cne $expectedGitStatusOutput) {
        Write-Host "Local main not up-to-date with the remote."
        exit
    }
}

$originalDirectory = Get-Location
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition
$rootDirectory = Split-Path -Parent $scriptDirectory
$publishDirectory = "publish"
$publishPath = Join-Path -Path $rootDirectory -ChildPath $publishDirectory # | Join-Path -ChildPath $ReleaseName

Set-Location $rootDirectory

$configuration = "Release"
$runtimes = @("win-x64", "win-x86")

if ($MainBranchCheck -eq $true) {
    GitCheck
}

# build the solution
dotnet build -c $configuration $silenceOutputFlag

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build completed successfully."
}
else {
    Write-Host "Build failed. Check the error messages for details."
    exit
}

# run tests
dotnet test -c $configuration --no-build

if ($LASTEXITCODE -eq 0) {
    Write-Host "All tests pass."
}
else {
    Write-Host "At least one test failed. Check the error messages for details."
    exit
}


$hyperProjectPath = Join-Path -Path $rootDirectory -ChildPath "Hyper"

$releaseDir = "releases"
$releasePath = Join-Path -Path $rootDirectory -ChildPath $releaseDir
New-Item -ItemType Directory -Force -Path $releasePath

foreach ($runtime in $runtimes) {
    $runtimePublishPath = Join-Path -Path $publishPath -ChildPath $runtime | Join-Path -ChildPath $ReleaseName
    dotnet publish $hyperProjectPath -c $configuration -r $runtime --sc -o $runtimePublishPath

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully published to $runtimePublishPath."
    }
    else {
        Write-Host "Publishing failed. Check the error messages for details."
        exit
    }

    # copy the readme file
    $readmePath = Join-Path -Path $rootDirectory -ChildPath "README.md"
    Copy-Item -Path $readmePath -Destination $runtimePublishPath

    # create a zip archive
    $releaseArchive = Join-Path -Path $releasePath -ChildPath "$runtime.zip"
    Compress-Archive -Path $runtimePublishPath -DestinationPath $releaseArchive
}

if ($CreateSourceArchive -eq $true) {
    # create a new directory with fresh version of the source code
    $repoCopy = Join-Path -Path $publishPath -ChildPath "src" | Join-Path -ChildPath $ReleaseName
    New-Item -ItemType Directory -Force -Path $repoCopy
    git clone "https://github.com/Non-Euclidean-World/Hyper.git" $repoCopy

    # create a zip archive of the source code
    $releaseArchive = Join-Path -Path $releasePath -ChildPath "source.zip"
    Compress-Archive -Path $repoCopy -DestinationPath $releaseArchive
}

Write-Host "The release archives created in $releasePath."

Set-Location $originalDirectory
