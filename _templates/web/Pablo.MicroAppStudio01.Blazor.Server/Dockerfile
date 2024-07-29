FROM mcr.microsoft.com/dotnet/aspnet:{{common.dotnet_docker_image_version}}
COPY bin/Release/{{common.target_dotnet_framework}}/publish/ app/
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Pablo.MicroAppStudio01.Blazor.Server.dll"]