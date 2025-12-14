FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["Stackbuld.Assessment.CSharp.Presentation/Stackbuld.Assessment.CSharp.Presentation.csproj", "Stackbuld.Assessment.CSharp.Presentation/"]
RUN dotnet restore "Stackbuld.Assessment.CSharp.Presentation/Stackbuld.Assessment.CSharp.Presentation.csproj"
COPY . .
WORKDIR "/src/Stackbuld.Assessment.CSharp.Presentation"
RUN dotnet build "./Stackbuld.Assessment.CSharp.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Stackbuld.Assessment.CSharp.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stackbuld.Assessment.CSharp.Presentation.dll"]
