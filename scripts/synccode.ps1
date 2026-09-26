#!/usr/bin/env pwsh
<#
.SYNOPSIS
Synchronize the repository's CodeGraph index and report what changed.

.DESCRIPTION
Displays the current CodeGraph index statistics, applies an incremental sync,
then displays updated statistics and file, node, and edge deltas. Use -Rebuild to
rebuild the entire index instead. The script requires the codegraph CLI and an
initialized .codegraph directory.

.PARAMETER Rebuild
Rebuild the complete index with codegraph index instead of applying an
incremental codegraph sync.

.PARAMETER Help
Display this help and exit without changing the index. The alias -? is supported.

.EXAMPLE
synccode
Synchronize pending repository changes into the index.

.EXAMPLE
synccode -Rebuild
Rebuild the complete index.

.NOTES
This script carries no project-specific value, so the file is identical in
every project at the same generation.
#>
#requires -Version 5.1
# SNAPSHOT GENERATION 1 - synccode.ps1.
# A generation is not a revision count. It names functionality, not edits, so editing one of these
# files is never on its own a reason to raise it. Raise it only when the executed outcome changes.
# A generation names how the family archives the source, fetches archives from GitHub, cleans the
# tree and maintains the code index. Two projects on the same generation produce the same archive
# from the same tree and lay out snapshots/ the same, whatever else differs between the files.
# Generation 1: zip.ps1 archives the Git-visible file set as snapshots/<version>/<project>-V<version>.zip,
# refuses to overwrite and refuses a tree that no longer matches an executable snapshot; gitdownload.ps1
# fetches every version-labelled commit above the threshold from GitHub into the same layout;
# clean.ps1 removes obj/, bin/, TestResults/ and WPF temp projects outside the protected trees and
# maintains the editor exclusions; synccode.ps1 syncs the CodeGraph index and reports the delta.
# Every project-specific value lives in snapshot.json, so this file is identical in every project at
# this generation.
[CmdletBinding()]
param(
    [switch]$Rebuild,
    [Alias('?')]
    [switch]$Help
)

if ($Help) {
    @'
NAME
    synccode.ps1

SYNOPSIS
    Synchronize the repository's CodeGraph index and report what changed.

SYNTAX
    synccode [-Rebuild] [-Help]

OPTIONS
    -Rebuild
        Rebuild the complete index with codegraph index. Without this option,
        the script applies an incremental codegraph sync.

    -Help, -?
        Display this help and exit without changing the index.

EXAMPLES
    synccode
        Synchronize pending repository changes into the index.

    synccode -Rebuild
        Rebuild the complete index.

NOTES
    Requires the codegraph CLI and an initialized .codegraph directory.
'@ | Write-Host
    exit 0
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# The codegraph CLI works on the current folder, so the project root is entered for the run.
Set-Location -LiteralPath ([System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..')))

function Read-Status {
    $json = codegraph status --json 2>$null
    if (-not $json) { return $null }
    return $json | ConvertFrom-Json
}

function Format-Age([string]$iso) {
    if (-not $iso) { return 'never' }
    $span = (Get-Date).ToUniversalTime() - ([datetimeoffset]$iso).UtcDateTime
    if ($span.TotalMinutes -lt 1)  { return 'just now' }
    if ($span.TotalHours   -lt 1)  { return "{0:n0}m ago" -f $span.TotalMinutes }
    if ($span.TotalDays    -lt 1)  { return "{0:n0}h ago" -f $span.TotalHours }
    return "{0:n0}d ago" -f $span.TotalDays
}

function Write-Stats($s) {
    Write-Host ("  Files:  {0,7:n0}   Nodes: {1,7:n0}   Edges: {2,7:n0}" -f $s.fileCount, $s.nodeCount, $s.edgeCount)
    Write-Host ("  DB:     {0,6:n1} MB   Indexed: {1}" -f ($s.dbSizeBytes / 1MB), (Format-Age $s.lastIndexed)) -ForegroundColor DarkGray
}

function Write-Delta([int]$before, [int]$after, [string]$label) {
    $d = $after - $before
    $sign = if ($d -gt 0) { "+$d" } elseif ($d -lt 0) { "$d" } else { '0' }
    $color = if ($d -gt 0) { 'Green' } elseif ($d -lt 0) { 'Red' } else { 'DarkGray' }
    Write-Host ("  {0,-6} {1,7:n0} -> {2,7:n0}  ({3})" -f $label, $before, $after, $sign) -ForegroundColor $color
}

# --- (1) Before ---
Write-Host "`n[1] Current situation" -ForegroundColor Cyan
$before = Read-Status
if (-not $before) {
    Write-Host "  CodeGraph not initialized here. Run: codegraph init" -ForegroundColor Yellow
    exit 1
}
Write-Stats $before
$p = $before.pendingChanges
$pending = $p.added + $p.modified + $p.removed
if ($pending -gt 0) {
    Write-Host ("  Pending: +{0} added, ~{1} modified, -{2} removed" -f $p.added, $p.modified, $p.removed) -ForegroundColor Yellow
} else {
    Write-Host "  Pending: none (index up to date)" -ForegroundColor DarkGray
}

# --- (2) Update ---
Write-Host "`n[2] Updating" -ForegroundColor Cyan
if ($Rebuild) {
    Write-Host "  Full rebuild (codegraph index)..." -ForegroundColor DarkGray
    codegraph index
} else {
    codegraph sync
}
if ($LASTEXITCODE -ne 0) {
    throw "codegraph update failed ($LASTEXITCODE)."
}

# --- (3) After ---
Write-Host "`n[3] New situation" -ForegroundColor Cyan
$after = Read-Status
if (-not $after) {
    throw 'codegraph status failed after the update.'
}
Write-Stats $after
Write-Host "  Delta:" -ForegroundColor Cyan
Write-Delta $before.fileCount $after.fileCount 'files'
Write-Delta $before.nodeCount $after.nodeCount 'nodes'
Write-Delta $before.edgeCount $after.edgeCount 'edges'
Write-Host ""
