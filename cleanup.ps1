#Set-ExecutionPolicy Unrestricted
$ErrorActionPreference = "Stop"

# Set current directory to script directory
Set-Location -Path $PSScriptRoot
#Get-ChildItem .\ -include bin,obj -Recurse  | format-table
#pause
cls
Get-ChildItem .\ -include bin,obj -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse }
