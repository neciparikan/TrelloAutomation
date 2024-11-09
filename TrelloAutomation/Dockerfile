FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY TrelloAutomation/TrelloAutomation.csproj TrelloAutomation/  
RUN dotnet restore "TrelloAutomation/TrelloAutomation.csproj" 

COPY TrelloAutomation/ TrelloAutomation/


RUN dotnet build "TrelloAutomation/TrelloAutomation.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "TrelloAutomation/TrelloAutomation.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .  
ENTRYPOINT ["dotnet", "TrelloAutomation.dll"]  
