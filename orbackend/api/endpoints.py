"""
Routes for the flask application.
"""

from flask_restplus import Resource
from api import api
from ortools.sat.python import cp_model

@api.route('/hello')
class HelloWorld(Resource):
    def get(self):
        return {'hello': 'helpo9'}

@api.route('/optimize')
class Optimization(Resource):
    def get(self):
        #inputs
        forms_amount = 3 #Anzahl Formen
        form_castingcell_demand = (1.2, 0.8, 1.8) #Gießzellenbedarf einer Form
        rows = 10 #maximal erlaubte Anzahl von Belegungen
        ovens_amount = 2 #anzahl verwendeter Öfen (input oder len(ovens_size))
        ovens_size = (40, 50) #Größe jedes Ovens
        order_requirements = (100, 50, 10) #Bestellung eines Monats, bereits nach Formen sortiert
        

        
        #TODO: ausrechnen lassen
        max_assignment = 100 #maximal erlaubte Anzahl von Belegungen, estimate

        #local vars
        columns = 1 + forms_amount
        data = [] # Matrix: x: Belegung, 'y': Formen, values: In welchem Oven befindet sich die Form bei dieser Belegung?

#       Belegung |Anzahl Druckvorgänge / Belegungsdauer | Ofenposition Form 1     | Ofenpostion Form 2      |   ...
#       _________|_________________________________________________________________________________________________
#       0        |                  3                   | 0: Form 1 unbenutzt     | 1: Form 2 in Ofen 0     |
#       1        |                  1                   | 1: Form 1 in Ofen 0     | 0: Form 2 unbenutzt     |
#       2        |                  5                   | 1: Form 1 in Ofen 0     | 2: Form 2 in Ofen 1     |
#       ...      |

# Umsetzung in der data-Liste:
#       _________|_________________________________________________________________________________________________
# 0              |      data[0]   |         data[1]         | data[2]     |
# 1              |      data[3]   |         data[4]         | data[5]     |
# 2              |      data[6]   |         data[7]         | data[8]     |

# Aufruf einer bestimmten Dauer der Belegung b: | Aufruf der Ofenpositionierung einer Form f in der Belegung b
#               | data[b * rows]                | data[b * rows + 1 + f]  
#
#

     
        model = cp_model.CpModel()

        #Variable initialization
        for i in range(rows):
            data.append(model.NewIntVar(0, max_assignment, str(i) + '_av'))
            for j in range (forms_amount):
                data.append(model.NewIntVar(0, forms_amount, str(i) + '_' + str(j)))

        #constraints - Wenn es funktioniert, gerne verkürzen und einige fors rausschmeißen
        #C1: Kein Ofen bei keiner Belebung überfüllt?
        for oven_id in range(ovens_amount):
             for assignment_id in range(rows):
                 model.Add(sum((form_castingcell_demand[form_id] if data[index(assignment_id, form_id + 1, columns)] == oven_id else 0) for form_id in range(forms_amount)) <= ovens_size[oven_id])
        
        #C2: Alle Produkte ( = Formen) hergestellt?
        for form in range(forms_amount):
             model.Add(sum((data[index(assignment_id, 0, columns)] if data[index(assignment_id, form + 1, columns)] else 0) for assignment_id in range(rows)) == order_requirements[form])
        
        model.Minimize(sum([(0 if x == 0 else 1) for x in range(len(data) // 5)]))
       
       #C3: Formen werden stets ersetzt, wenn sie abgenutzt werden
        #TODO
#        for form in range(forms_amount):
#           for assignment_id in range(rows): #Effizientere Methode ohne diese Schleife?
#               sumUntilNow = sum(data)
#               model.Add( #negative Zahl existiert: Null muss existieren


        solver = cp_model.CpSolver()
        status = solver.Solve(model)
        out = []
        print('Minimale Anzahl Belegungen: ' + str(solver.ObjectiveValue()))
        for i in data: 
           out.append(solver.Value(str(i)))
        print(out)
        print(solver.ResponseProto())
        return [str(x) for x in out]
    
def index(row, column, row_size):
    return row * row_size + column