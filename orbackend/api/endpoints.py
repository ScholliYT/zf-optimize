"""
Routes for the flask application.
"""

from flask import request
from flask_restplus import Resource, fields
from api import api
from ortools.sat.python import cp_model
from services.zfoptimizer import ZFOptimizer

@api.route('/hello')
class HelloWorld(Resource):
    def get(self):
        return {'hello': 'helpo9'}

""" Model for documenting the API """
oven_model = api.model('Oven', {
'id': fields.Integer(required=True),
'size': fields.Integer(required=True),
'changeduration_sec': fields.Integer(required=True),
})
form_model = api.model('Form', {
'id': fields.Integer(required=True),
'required_amount': fields.Integer(required=True),
'castingcell_demand': fields.Integer(required=True),
'current_uses': fields.Integer(required=True),
'max_uses': fields.Integer(required=True),
})

optimization_request = api.model("Optimization", {
"ovens": 
fields.List(fields.Nested(oven_model), description="list of ovens", required=True),
"forms": 
fields.List(fields.Nested(form_model), description="list of forms", required=True)
})

@api.route('/optimize')
class Optimization(Resource):
    @api.expect(optimization_request)
    def post(self):
        json_data = request.json
        ovens = json_data['ovens']
        forms = json_data['forms']

        #ovens = [{"id": 0, "size": 20, "changeduration_sec": 50},
        #              {"id": 1, "size": 25, "changeduration_sec": 70}]

        #forms = [{"id": 0,'required_amount':2, 'castingcell_demand':1},
        #              {"id": 1,'required_amount':3, 'castingcell_demand':12},
        #              {"id": 2,'required_amount':3, 'castingcell_demand':18}]
        op = ZFOptimizer(ovens, forms)
        op.init_model()
        op.add_all_constrains()
        solution = op.optimize()
        return solution
        
