FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Build Runtime Image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

# Use a non-root port
ENV ASPNETCORE_URLS=http://+:8080

# Env Variables required at run time.
ENV ApplicationInsights__InstrumentationKey=00000000-0000-0000-0000-000000000000
ENV ServiceHealthApiConfiguration__TenantHost=contoso.onmicrosoft.com
ENV ServiceHealthApiConfiguration__TenantId=00000000-0000-0000-0000-000000000000
ENV ServiceHealthApiConfiguration__ClientId=00000000-0000-0000-0000-000000000000
ENV ServiceHealthApiConfiguration__ClientSecret=00000000-0000-0000-0000-000000000000
ENV ServiceHealthApiConfiguration__CacheDurationInSeconds=60
ENV CompanyConfiguration__CompanyName=Contoso
ENV CompanyConfiguration__SupportEmail=support@contoso.com
ENV CompanyConfiguration__SupportPhone="+1 000-000-000"

# ... and run as non-root user.
RUN chown -R www-data:www-data /app
USER www-data

ENTRYPOINT ["dotnet", "O365StatusDashboard.dll"]