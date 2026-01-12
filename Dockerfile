# -------- Build / publish --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "IdleGarageBackend.csproj"
RUN dotnet publish "IdleGarageBackend.csproj" -c Release -o /app/publish

# -------- Migrator (EF Core migrations) --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator
WORKDIR /src

COPY . .
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

CMD ["sh", "-c", "dotnet ef database update --project IdleGarageBackend.csproj --startup-project IdleGarageBackend.csproj"]

# -------- Runtime --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 5026
ENV ASPNETCORE_URLS=http://+:5026
ENTRYPOINT ["dotnet", "IdleGarageBackend.dll"]
