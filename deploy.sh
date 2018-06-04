ApiKey=$1

dotnet nuget pack ./GraphStructure -v=n
dotnet nuget push ./GraphStructure.1.0.0.nupkg -k $ApiKey -s https://api.nuget.org/v3/index.json