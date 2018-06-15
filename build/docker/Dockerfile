FROM microsoft/dotnet:2.1-runtime-alpine
ADD console/any /docgen
ENTRYPOINT ["dotnet", "/docgen/DocGen.Cons.dll"]
WORKDIR /work