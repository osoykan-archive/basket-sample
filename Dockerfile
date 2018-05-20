FROM microsoft/aspnetcore-build AS build-env
WORKDIR /workdir
COPY . /workdir
 
RUN dotnet restore ./BasketContext.sln
RUN dotnet build   ./BasketContext.sln
RUN dotnet test    ./test/BasketContext.Domain.Tests/BasketContext.Domain.Tests.csproj
RUN dotnet test    ./test/BasketContext.Domain.IntegrationTests/BasketContext.Domain.IntegrationTests.csproj
RUN dotnet test    ./test/BasketContext.Api.IntegrationTests/BasketContext.Api.IntegrationTests.csproj
RUN dotnet publish ./src/BasketContext.Api/BasketContext.Api.csproj -o /publish -c Release -r linux-x64

FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-env ./publish .

ENV ASPNETCORE_ENVIRONMENT="Development"
ENV ASPNETCORE_URLS="http://*:5000"

EXPOSE 5000
ENTRYPOINT ["dotnet", "BasketContext.Api.dll"]