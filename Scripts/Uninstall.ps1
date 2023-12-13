<#

.SYNOPSIS
Uninstalls the Hyper game.

.DESCRIPTION
This script is meant to be invoked from the installation folder of the game,
i.e. the unzipped release folder.

.EXAMPLE

PS> ./Uninstall.ps1

#>

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition
$rootDirectory = Split-Path -Parent $scriptDirectory

# remove game saves
$hyperSavesFolder = "Hyper"
$hyperSavesPath = Join-Path -Path $env:APPDATA -ChildPath $hyperSavesFolder

if (Test-Path -Path $hyperSavesPath) {
    $confirmation = Read-Host "Do you want to delete the game saves (y/n)? This will remove the $hyperSavesPath directory and its contents."

    if ($confirmation -eq "y") {
        Remove-Item -Recurse -Force $hyperSavesPath
    }
}
else {
    Write-Host "Couldn't find the directory with game saves. It looks like you didn't have any."
}

# remove the game installation
$confirmation = Read-Host "Do you want to remove the game (y/n)? This will remove contents of the $scriptDirectory directory."

Set-Location $rootDirectory
if ($confirmation -eq "y") {
    $scriptDirName = Split-Path -Path $scriptDirectory -Leaf
    if ($scriptDirName -notmatch "Hyper-v\d+\.\d+\.\d+") {
        Write-Host "$scriptDirName not recognized as the installation directory. Aborting."
        exit
    }
    Get-ChildItem -Path $scriptDirectory -Recurse | Foreach-object { Remove-item -Recurse -path $_.FullName }
}