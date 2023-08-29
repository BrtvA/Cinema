#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Cinema.PL/Cinema.PL.csproj", "src/Cinema.PL/"]
COPY ["src/Cinema.BLL/Cinema.BLL.csproj", "src/Cinema.BLL/"]
COPY ["src/Cinema.DAL/Cinema.DAL.csproj", "src/Cinema.DAL/"]
RUN dotnet restore "src/Cinema.PL/Cinema.PL.csproj"
COPY . .
WORKDIR "/src/src/Cinema.PL"
RUN dotnet build "Cinema.PL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cinema.PL.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cinema.PL.dll"]