ApiKey=$1
Version=$2

dotnet pack GraphStructure -v=n /p:PackageVersion=$Version /p:Configuration=Release
dotnet nuget push ./GraphStructure/bin/Release/GraphStructure.*.nupkg -k $ApiKey -s https://api.nuget.org/v3/index.json