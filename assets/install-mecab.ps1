#!/usr/bin/env pwsh

$mecab_installer_exe = Join-Path $PSScriptRoot 'mecab-0.996-64.exe'
$mecab_config_ps1 = Join-Path $PSScriptRoot 'mecab-config.ps1'

$mecab_bin = Join-Path $Env:programfiles 'MeCab' 'bin'

Write-Host "Install: Installing MeCab by '$mecab_installer_exe'."
& $mecab_installer_exe '/VERYSILENT'
Write-Host "Install: Done."
