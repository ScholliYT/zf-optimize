"""
The flask application package.
"""

from flask import Flask
from flask_restplus import Api

app = Flask(__name__)
api = Api(app, validate=True)

import api.endpoints
