# zf-optimize
![Build Status Webapp](https://github.com/ScholliYT/zf-optimize/workflows/Publish-Dockerimage-Webapp/badge.svg)
![Build Status Orbackend](https://github.com/ScholliYT/zf-optimize/workflows/Publish-Dockerimage-Orbackend/badge.svg)

https://www.it-talents.de/foerderung/code-competition/zf-code-competition-11-2019

## Docker
### Docker-compose
Start all containers:  
`docker-compose up --build -d`

Delete all containers:  
`docker-compose down --rmi local`

Find the __WebApp__ at: https://localhost:443/.  
Find the __Swagger__(orbackend) docs at: http://localhost:5000/.


## Tech-Stack
- Blazor ASP.NET Core (Frontend)
- Syncfusion Essential JS2 Blazor UI Components (Frontend)
- SQL Server (Database)
- Docker / Docker Compose (Container)
- Flask-Restful (Backend Optimazation API)
- Google-OR Tools (Optimazation Framework)
