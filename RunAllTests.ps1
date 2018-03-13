$sqlService = Get-Service MSSQLSERVER

while ($sqlService.Status -ne 'Running')
{
	Write-Warning "SQL Server isn't running.  Trying to start it..."
    Start-Service MSSQLSERVER
    Start-Sleep -seconds 5
    $sqlService.Refresh()
}

$testAssemblies = gci -Recurse -Include "*Test.dll" | where { $_.FullName.Contains("\bin\") }
nunit3-console.exe --out=TestStdout.log --err=TestStderr.log $testAssemblies
