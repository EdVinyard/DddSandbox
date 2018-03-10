$testAssemblies = gci -Recurse -Include "*Test.dll" | where { $_.FullName.Contains("\bin\") }
nunit3-console.exe --out=TestStdout.log --err=TestStderr.log $testAssemblies
