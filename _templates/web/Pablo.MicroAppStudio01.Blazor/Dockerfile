FROM nginx:alpine
COPY ./nginx.conf  /etc/nginx/conf.d/default.conf
WORKDIR /usr/share/nginx/html
COPY bin/Release/{{common.target_dotnet_framework}}/publish/wwwroot .