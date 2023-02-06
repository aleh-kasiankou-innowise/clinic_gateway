FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Innowise.Clinic.Gateway/Innowise.Clinic.Gateway.csproj", "Innowise.Clinic.Gateway/"]
RUN dotnet restore "Innowise.Clinic.Gateway/Innowise.Clinic.Gateway.csproj"
COPY . .
WORKDIR "/src/Innowise.Clinic.Gateway"
RUN dotnet build "Innowise.Clinic.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Innowise.Clinic.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Innowise.Clinic.Gateway.dll"]
