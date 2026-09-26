<#
.SYNOPSIS
    Install the bare-name dispatchers so every script here runs as a plain command.
.DESCRIPTION
    Writes one small .cmd dispatcher per script in this folder into a folder on the
    user PATH, so typing run, build, stop or any other script name in cmd or
    PowerShell runs the matching script of the nearest project: the dispatcher
    walks up from the current folder until it finds <name>.ps1 in a scripts folder
    or, for a project still keeping its scripts at the root, in the folder itself,
    then runs it with pwsh when present and Windows PowerShell otherwise. The
    dispatchers are byte-identical and name nothing of this project, so one
    installation serves every project on the machine; a dispatcher this project
    has no script for is left alone, since another project may own it.

    The destination folder is created when missing and appended to the user PATH
    when absent; the current session's PATH is updated as well. Consoles already
    open elsewhere see the new PATH only after they are reopened.
.PARAMETER Destination
    Folder the dispatchers are written to. Defaults to bin under the user profile.
.PARAMETER Help
    Display this help and exit without writing anything. The alias -? is supported.
.EXAMPLE
    install
    Install the dispatchers into %USERPROFILE%\bin and put that folder on the PATH.
.EXAMPLE
    install -Destination D:\tools
    Install the dispatchers into D:\tools instead.
#>
#requires -Version 5.1
[CmdletBinding()]
param(
    [string]$Destination = (Join-Path $env:USERPROFILE 'bin'),
    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    install.ps1

SYNOPSIS
    Install the bare-name dispatchers so every script here runs as a plain command.

SYNTAX
    install [-Destination <folder>] [-Help]

OPTIONS
    -Destination <folder>
        Folder the dispatchers are written to. Defaults to %USERPROFILE%\bin.

    -Help, -?
        Display this help and exit without writing anything.

EXAMPLES
    install
        Install the dispatchers into %USERPROFILE%\bin and put that folder on the PATH.

    install -Destination D:\tools
        Install the dispatchers into D:\tools instead.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$dispatcher = @'
@echo off
setlocal
set "DIR=%CD%"
:find
if exist "%DIR%\scripts\%~n0.ps1" (set "SCRIPT=%DIR%\scripts\%~n0.ps1" & goto found)
if exist "%DIR%\%~n0.ps1" (set "SCRIPT=%DIR%\%~n0.ps1" & goto found)
for %%I in ("%DIR%\..") do set "PARENT=%%~fI"
if /I "%PARENT%"=="%DIR%" goto missing
set "DIR=%PARENT%"
goto find
:missing
echo No %~n0.ps1 was found in a scripts folder or at the root of this folder or any parent.
exit /b 1
:found
set "PS=powershell"
where pwsh >nul 2>nul && set "PS=pwsh"
%PS% -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT%" %*
exit /b %ERRORLEVEL%
'@

$Destination = [System.IO.Path]::GetFullPath($Destination)
New-Item -ItemType Directory -Path $Destination -Force | Out-Null

function Get-OrdinalKey {
    # Sort-Object compares text by culture, which Windows PowerShell 5.1 and pwsh 7 order differently.
    # Uppercase hexadecimal UTF-16 code units compare alike under every culture, so this key sorts ordinally.
    param([string]$Text)
    return [System.BitConverter]::ToString([System.Text.Encoding]::BigEndianUnicode.GetBytes($Text)).Replace('-', '')
}

$scripts = @(
    Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.ps1' -File |
        Where-Object { $_.Name -notlike '*.config.ps1' } |
        Sort-Object -Property @{ Expression = { Get-OrdinalKey $_.Name } }
)

$content = $dispatcher -replace "`r?`n", "`r`n"
$written = 0
$shadowed = New-Object 'System.Collections.Generic.List[string]'
foreach ($script in $scripts) {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($script.Name)
    $path = Join-Path $Destination ($name + '.cmd')
    $current = if (Test-Path -LiteralPath $path -PathType Leaf) { [System.IO.File]::ReadAllText($path) } else { $null }
    if ($current -ne $content) {
        [System.IO.File]::WriteAllText($path, $content, [System.Text.ASCIIEncoding]::new())
        $written++
    }

    # An alias, function or cmdlet of the same name wins over PATH in PowerShell, and a program
    # earlier on the PATH wins in both shells, so the bare name would never reach the dispatcher.
    # Said aloud rather than discovered later.
    foreach ($shadow in @(Get-Command -Name $name -All -ErrorAction SilentlyContinue)) {
        if ($shadow.Source -eq $path -or $shadow.Source -eq $script.FullName) {
            continue
        }

        $origin = if ($shadow.CommandType -eq 'Application') { $shadow.Source } elseif ($shadow.ModuleName) { "module $($shadow.ModuleName)" } else { 'built in' }
        [void]$shadowed.Add("$name - $($shadow.CommandType.ToString().ToLowerInvariant()), $origin")
    }
}

# The folder serves every project on this machine, so a dispatcher this project has no script for
# is never removed: another project may own it. Dispatchers that older installs wrote in an earlier
# shape are brought up to the current text, since every dispatcher is the same file.
$upgraded = 0
foreach ($other in Get-ChildItem -LiteralPath $Destination -Filter '*.cmd' -File) {
    $text = [System.IO.File]::ReadAllText($other.FullName)
    if ($text -ne $content -and $text -match '(?m)^if exist "%DIR%\\scripts\\%~n0\.ps1"') {
        [System.IO.File]::WriteAllText($other.FullName, $content, [System.Text.ASCIIEncoding]::new())
        $upgraded++
    }
}

$userPath = [System.Environment]::GetEnvironmentVariable('Path', 'User')
$entries = @($userPath -split ';' | Where-Object { $_ -ne '' } | ForEach-Object { $_.TrimEnd('\') })
$onPath = $entries -contains $Destination.TrimEnd('\')
if (-not $onPath) {
    $joined = (($entries + $Destination) -join ';')
    [System.Environment]::SetEnvironmentVariable('Path', $joined, 'User')
}

$sessionEntries = @($env:Path -split ';' | ForEach-Object { $_.TrimEnd('\') })
if ($sessionEntries -notcontains $Destination.TrimEnd('\')) {
    $env:Path = $env:Path.TrimEnd(';') + ';' + $Destination
}

Write-Host 'Dispatchers installed' -ForegroundColor Green
Write-Host ("  {0,-11}{1}" -f 'Folder', $Destination)
Write-Host ("  {0,-11}{1}" -f 'Commands', (($scripts | ForEach-Object { [System.IO.Path]::GetFileNameWithoutExtension($_.Name) }) -join ' '))
Write-Host ("  {0,-11}{1} written, {2} unchanged, {3} other project's upgraded" -f 'Files', $written, ($scripts.Count - $written), $upgraded)
Write-Host ("  {0,-11}{1}" -f 'User PATH', $(if ($onPath) { 'already held the folder' } else { 'folder appended; reopen other consoles to see it' }))
if ($shadowed.Count -gt 0) {
    Write-Host ("  {0,-11}{1}" -f 'Shadowed', 'these bare names reach something else first:') -ForegroundColor Yellow
    foreach ($item in $shadowed) {
        Write-Host ("  {0,-11}  {1}" -f '', $item) -ForegroundColor Yellow
    }
}
