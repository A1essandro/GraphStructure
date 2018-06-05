ApiKey=$1
Version=$2

dotnet pack GraphStructure -v=n /p:PackageVersion=$Version
dotnet nuget push GraphStructure.*.nupkg -k $ApiKey -s https://api.nuget.org/v3/index.json