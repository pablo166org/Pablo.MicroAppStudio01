FROM nginx:alpine
COPY ./nginx.conf  /etc/nginx/conf.d/default.conf
WORKDIR /usr/share/nginx/html
COPY bin/Release/net8.0/publish/wwwroot .