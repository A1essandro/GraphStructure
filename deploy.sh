ApiKey=$1

dotnet pack GraphStructure -v=n
dotnet nuget push ./GraphStructure/bin/Debug/GraphStructure.0.1.0-alpha.nupkg -k $ApiKey -s https://api.nuget.org/v3/index.json