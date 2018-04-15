Get-ChildItem -r * -Include *.cs, *.md, *.config | Select-String "TODO: "
