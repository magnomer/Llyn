#requires -Version 5.1
# UNICODEWIDTH - UNICODEWIDTH GENERATION 1.
[CmdletBinding()]
param(
    [string]$DataDirectory,
    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    UnicodeWidth.ps1

SYNOPSIS
    Write the console cell-width table src/Llyn.UIDemeanor/Kit/BCanvasRange.cs
    from the Unicode data of the version the .NET runtime carries.

SYNTAX
    unicodewidth -DataDirectory <path> [-Help]

OPTIONS
    -DataDirectory <path>
        Folder holding the two data files of the pinned Unicode version:
          https://www.unicode.org/Public/16.0.0/ucd/EastAsianWidth.txt
          https://www.unicode.org/Public/16.0.0/ucd/emoji/emoji-data.txt
        A file of another version is refused.

    -Help
        Display this help and exit without reading or writing anything.

OUTPUT
    One line naming the version, the run count and the file written.
    The exit code is 0 on success and 1 on a missing or refused file.

EXAMPLES
    unicodewidth -DataDirectory D:\temp\unicode-16.0.0
        Regenerate the table from the downloaded files.
'@ | Write-Host
    exit 0
}

Write-Host 'UNICODEWIDTH - UNICODEWIDTH GENERATION 1' -ForegroundColor Blue

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [Console]::OutputEncoding

$unicodeVersion = '16.0.0'
$emojiVersion = '16.0'
$root = Split-Path -Parent $PSScriptRoot
$outputPath = Join-Path $root 'src/Llyn.UIDemeanor/Kit/BCanvasRange.cs'

if (-not $DataDirectory) {
    Write-Host 'UnicodeWidth: -DataDirectory is required. Run with -Help for the files it needs.' -ForegroundColor Red
    exit 1
}

$widthPath = Join-Path $DataDirectory 'EastAsianWidth.txt'
$emojiPath = Join-Path $DataDirectory 'emoji-data.txt'
foreach ($path in @($widthPath, $emojiPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        Write-Host "UnicodeWidth: missing $path" -ForegroundColor Red
        exit 1
    }
}

$utf8 = [System.Text.UTF8Encoding]::new($false)
$widthLines = [System.IO.File]::ReadAllLines($widthPath, $utf8)
$emojiLines = [System.IO.File]::ReadAllLines($emojiPath, $utf8)

if ($widthLines.Length -eq 0 -or $widthLines[0] -cne "# EastAsianWidth-$unicodeVersion.txt") {
    Write-Host "UnicodeWidth: $widthPath is not EastAsianWidth $unicodeVersion." -ForegroundColor Red
    exit 1
}

if (-not ($emojiLines | Where-Object { $_.StartsWith("# Used with Emoji Version $emojiVersion ", [StringComparison]::Ordinal) })) {
    Write-Host "UnicodeWidth: $emojiPath is not emoji data $emojiVersion." -ForegroundColor Red
    exit 1
}

$kind = New-Object 'byte[]' 0x110000
$pattern = '^([0-9A-F]{4,6})(?:\.\.([0-9A-F]{4,6}))?\s*;\s*(\w+)'

function Set-Kind([int]$First, [int]$Last, [byte]$Value) {
    for ($point = $First; $point -le $Last; $point++) {
        $kind[$point] = $Value
    }
}

function Read-Range([string]$Line) {
    $match = [regex]::Match($Line, $pattern)
    if (-not $match.Success) {
        return $null
    }
    $first = [Convert]::ToInt32($match.Groups[1].Value, 16)
    $last = $first
    if ($match.Groups[2].Success) {
        $last = [Convert]::ToInt32($match.Groups[2].Value, 16)
    }
    return @($first, $last, $match.Groups[3].Value)
}

foreach ($line in $widthLines) {
    $range = Read-Range $line
    if ($null -eq $range) {
        continue
    }
    switch -CaseSensitive ($range[2]) {
        'W' { Set-Kind $range[0] $range[1] 1 }
        'F' { Set-Kind $range[0] $range[1] 1 }
        'A' { Set-Kind $range[0] $range[1] 2 }
    }
}

foreach ($line in $emojiLines) {
    $range = Read-Range $line
    if ($null -ne $range -and $range[2] -ceq 'Emoji_Presentation') {
        Set-Kind $range[0] $range[1] 1
    }
}

$firsts = New-Object 'System.Collections.Generic.List[string]'
$narrow = New-Object 'System.Collections.Generic.List[string]'
$wide = New-Object 'System.Collections.Generic.List[string]'
$narrowCells = @(1, 2, 1)
$wideCells = @(1, 2, 2)
$previous = -1
for ($point = 0; $point -lt 0x110000; $point++) {
    $value = $kind[$point]
    if ($value -ne $previous) {
        $firsts.Add('0x' + $point.ToString('X6'))
        $narrow.Add([string]$narrowCells[$value])
        $wide.Add([string]$wideCells[$value])
        $previous = $value
    }
}

function Format-Rows($Items, [int]$PerRow) {
    $rows = New-Object 'System.Collections.Generic.List[string]'
    for ($index = 0; $index -lt $Items.Count; $index += $PerRow) {
        $count = [Math]::Min($PerRow, $Items.Count - $index)
        $rows.Add('        ' + (($Items.GetRange($index, $count)) -join ', ') + ',')
    }
    return $rows -join "`n"
}

$text = @"
using System;

namespace Llyn.UIDemeanor;

internal static class BCanvasRange
{
    public static ReadOnlySpan<int> BCanvasRangeFirst =>
    [
$(Format-Rows $firsts 8)
    ];

    public static ReadOnlySpan<byte> BCanvasRangeNarrow =>
    [
$(Format-Rows $narrow 32)
    ];

    public static ReadOnlySpan<byte> BCanvasRangeWide =>
    [
$(Format-Rows $wide 32)
    ];
}

"@ -replace "`r`n", "`n"

[System.IO.File]::WriteAllText($outputPath, $text, $utf8)
Write-Host "Unicode $unicodeVersion | runs $($firsts.Count) | $outputPath"
exit 0
