# ForumApp-Posts

Postservice of the forum application

## Testing this application

Local development: Use the locahost connectionstring

Docker compose: Use the postdb connectionstring

## Database

Get image: ```docker pull mongo```

Docker run: ```docker run -d -p 27017:27017 -v data:/data/db --name postdb mongo```

## PostService

### Build docker image

```docker build -t postservice .``` (in project folder)

### Pull image from Dockerhub

```docker pull kvdhoogenhof/forumapp-posts```

## Run image

```docker run -d -e ASPNETCORE_ENVIRONMENT=Development –e ASPNETCORE_URLS="https://+:8080;http://+:8081" –e ASPNETCORE_HTTPS_PORT=9000  -e ASPNETCORE_Kestrel__Certificates__Default__Password=mypass123  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v %USERPROFILE%/.aspnet/https:/https:ro -p 9001:8080 --name api kvdhoogenhof/forumapp-posts```
