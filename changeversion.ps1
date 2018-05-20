 param (
    [int]$buildNumber = 999,
    [string]$file = "Directory.build.props"
 )

(Get-Content $file) | %{$_ -replace "<Version>.*<\/Version>","<Version>1.0.$buildNumber</Version>"} | Set-Content $file 