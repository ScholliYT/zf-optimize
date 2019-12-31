# zf-optimize
![Build Status Webapp](https://github.com/ScholliYT/zf-optimize/workflows/Publish-Dockerimage-Webapp/badge.svg)
![Build Status Orbackend](https://github.com/ScholliYT/zf-optimize/workflows/Publish-Dockerimage-Orbackend/badge.svg)
[![forthebadge](https://forthebadge.com/images/badges/fuck-it-ship-it.svg)](https://forthebadge.com)

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
- [Chart.js for Blazor](https://github.com/mariusmuntean/ChartJs.Blazor) (Frontend Charts)
- SQL Server (Database)
- Docker / Docker Compose (Container)
- Flask / [Flask-Restplus](https://flask-restplus.readthedocs.io/en/stable/) (Backend Optimazation API)
- [Google OR Tools](https://developers.google.com/optimization/) (Optimazation Framework)

## Architecture
![system architecture diagram](/docs/img/system_architecture_diagram.png)

## Optimization Backend
The optimization backend uses Flask + [Flask-Restplus](https://flask-restplus.readthedocs.io/en/stable/) as REST-API interface for the optimization logic made with [Google OR Tools](https://developers.google.com/optimization/).
Docs on how to run this are located in the [wiki](https://github.com/ScholliYT/zf-optimize/wiki/Optimization-Backend).
