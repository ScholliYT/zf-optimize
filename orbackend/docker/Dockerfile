FROM python:3.7-slim-buster

# We copy just the requirements.txt first to leverage Docker cache
COPY ["./orbackend/requirements.txt", "/app/requirements.txt"]

WORKDIR /app

RUN pip3 install -r requirements.txt

COPY ["orbackend/", "/app"]
EXPOSE 5000

CMD ["python3","runserver.py"]