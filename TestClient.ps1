#!/usr/bin/env pwsh

$pipeClient = New-Object System.IO.Pipes.NamedPipeClientStream -arg @(".", "PipeSample", [System.IO.Pipes.PipeDirection]::In)
$pipeClient.Connect()

$pipeReader = New-Object System.IO.StreamReader($pipeClient)
$result = $pipeReader.ReadLine()
Write-Output "Got from the server: [" $result "]"
