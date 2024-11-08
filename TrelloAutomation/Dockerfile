# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /src

# Copy the project file and restore dependencies
COPY TrelloAutomation/TrelloAutomation.csproj TrelloAutomation/  # Copy the project file
RUN dotnet restore "TrelloAutomation/TrelloAutomation.csproj"  # Restore dependencies

# Copy the entire project into the container
COPY TrelloAutomation/ TrelloAutomation/  # Copy the entire project directory

# Build the application
RUN dotnet build "TrelloAutomation/TrelloAutomation.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TrelloAutomation/TrelloAutomation.csproj" -c Release -o /app/publish

# Final stage: use the ASP.NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .  # Copy published output
ENTRYPOINT ["dotnet", "TrelloAutomation.dll"]  # Set the command to run the application
