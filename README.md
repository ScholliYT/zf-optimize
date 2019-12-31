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
- Flask-Restful (Backend Optimazation API)
- Google-OR Tools (Optimazation Framework)

## Testing the Optimization Backend
Get yourself some HTTP Request client like Postman or Curl or use the integrated Swagger solution. Either start the Flask server located at [runserver.py](/orbackend/runserver.py) manually (see requirements.txt for Libraries used) or use the Docker container located at [Dockerfile](/orbackend/docker/Dockerfile). We will show here how to do it with the Docker container.  

- Make sure you are in the project's root directory
- Build the image with: `docker build -t latest -f .\orbackend\docker\Dockerfile .`
- Spinn up a container with: `docker run -p 5000:5000 -e SERVER_HOST=0.0.0.0 -e SERVER_PORT=5000 --name orbackend -d zf-optimize_orbackend:latest`
- Go to: http://localhost:5000/
- Under the default namespace open the POST request to /optimize
- Click the `Try it out` Button
- Paste the following json into the textbox

```json
{
  "ovens": [
    {
      "id": 0,
      "size": 1,
      "changeduration_sec": 5
    },
    {
      "id": 1,
      "size": 2,
      "changeduration_sec": 10
    },
    {
      "id": 2,
      "size": 4,
      "changeduration_sec": 20
    },
    {
      "id": 3,
      "size": 8,
      "changeduration_sec": 40
    }
  ],
  "forms": [
    {
      "id": 0,
      "required_amount": 15,
      "castingcell_demand": 1,
      "current_uses": 1,
      "max_uses": 25
    },
    {
      "id": 1,
      "required_amount": 12,
      "castingcell_demand": 2,
      "current_uses": 1,
      "max_uses": 25
    },
    {
      "id": 2,
      "required_amount": 17,
      "castingcell_demand": 7,
      "current_uses": 1,
      "max_uses": 25
    }
    ,
    {
      "id": 3,
      "required_amount": 20,
      "castingcell_demand": 4,
      "current_uses": 1,
      "max_uses": 25
    }
  ]
}
```
- Depending on your Hardware (and allocation for docker) this will take some minutes to compute. It took 3 min on a machine with i7 4790k 8x4.0 GHz
- As Result you should get the optimal assigments for the ovens like this:

```json
[
    {
        "name": "Assignment0",
        "used": true,
        "ticks": 3,
        "assignments": [
            3,
            -1,
            -1,
            3
        ]
    },
    {
        "name": "Assignment1",
        "used": true,
        "ticks": 5,
        "assignments": [
            -1,
            -1,
            3,
            2
        ]
    },
    {
        "name": "Assignment2",
        "used": true,
        "ticks": 12,
        "assignments": [
            3,
            1,
            3,
            2
        ]
    }
]
```
- `ticks` is the amout of times this configuration of forms will be used in the ovens
- `assignments` is a list of forms and their respective position in oven ids. `-1` meand the form is in no oven == not used.
