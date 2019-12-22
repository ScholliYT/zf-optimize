"""
This script runs the application using a development server.
"""

from os import environ
from api import app

if __name__ == '__main__':
    HOST = environ.get('SERVER_HOST', 'localhost')
    try:
        PORT = int(environ.get('SERVER_PORT', '5050'))
    except ValueError:
        PORT = 5050
    app.run(HOST, PORT, debug=True)
   