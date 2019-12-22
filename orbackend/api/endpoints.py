"""
Routes for the flask application.
"""

from flask_restplus import Resource
from api import api
from ortools.sat.python import cp_model

@api.route('/hello')
class HelloWorld(Resource):
    def get(self):
        return {'hello': 'world'}

@api.route('/optimize')
class HelloWorld(Resource):
    def get(self):
        #inputs
        forms_amount = 3 #Anzahl Formen
        form_castingcell_demand = (1.2, 0.8, 1.8) #Gießzellenbedarf einer Form
        rows = 10 #maximal erlaubte Anzahl von Belegungen
        max_assignment = 100 #maximal erlaubte Anzahl von Belegungen
        ovens_amount = 2 #anzahl verwendeter Öfen
        ovens_size = (20, 30)
        

        #local vars
        columns = 1 + forms_amount
        data = []

        
        model = cp_model.CpModel()

        for i in range(rows):
            data[i * colums] = model.NewIntVar(0, max_assignment, str(i) + '_av')
            for j in range (forms_amount):
                data[i * colums + j] = model.NewIntVar(0, forms_amount, str(i) + '_' + str(j))

         # 
        for oven_id in range(ovens_amount):
             for belegung_id in range(rows):
                 #Für Jede Belegung und jeden Oven die Verwendung zählen, muss kleiner gleich der Ovengröße sein.
                 model.Add(sum((form_castingcell_demand[form_id] if data[belegung_id * colums + form_id + 1] == oven_id else 0) for form_id in range(forms_amount)) <= ovens_size[oven_id])
       
           


        return {'hello': 'world'}