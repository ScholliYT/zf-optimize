"""
Routes for the flask application.
"""

from flask_restplus import Resource
from api import api

@api.route('/hello')
class HelloWorld(Resource):
    def get(self):
        return {'hello': 'world'}