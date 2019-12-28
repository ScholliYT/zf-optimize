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
        order_requirements = (100, 50, 10) #Bestellung eines Monats, bereits nach Formen sortiert
        form_castingcell_demand = (1.2, 0.8, 1.8) #Gießzellenbedarf einer Form
        
        ovens_amount = 2 #anzahl verwendeter Öfen (input oder len(ovens_size))
        ovens_size = (40, 50) #Größe jedes Ovens

        ovens = [{"id": 0, "size": 2, "changeduration_sec": 50},
                 {"id": 1, "size": 3, "changeduration_sec": 70}]

        #List of Forms - required_amount: Amount of Times this form must be used, castingcell_demand: Amount of space occupied by this form
        forms = [{"id": 0,'required_amount':100, 'castingcell_demand':1.2},
                 {"id": 1,'required_amount':50, 'castingcell_demand':0.8},
                 {"id": 2,'required_amount':10, 'castingcell_demand':1.8}]

        

        
        #TODO: ausrechnen lassen
        #sum(order_requirements) erstmal
        max_assignment = 160 #maximal erlaubte Anzahl von Belegungen, 
        rows = max_assignment #TODO: Remove
        max_ticks = max_assignment

        #local vars
        columns = 1 + forms_amount
        data = [] # Matrix: x: Belegung, 'y': Formen, values: In welchem Oven befindet sich die
                  # Form bei dieser Belegung?

#       Belegung |Anzahl Druckvorgänge / Belegungsdauer | Ofenposition Form 0 |
#       Ofenpostion Form 1 | ...
#       _________|_________________________________________________________________________________________________
#       0 | 3 | 0: Form 1 unbenutzt | 1: Form 2 in Ofen 0 |
#       1 | 1 | 1: Form 1 in Ofen 0 | 0: Form 2 unbenutzt |
#       2 | 5 | 1: Form 1 in Ofen 0 | 2: Form 2 in Ofen 1 |
#       ...  |

# Umsetzung in der data-Liste:
#       _________|_________________________________________________________________________________________________
# 0 | data[0] | data[1] | data[2] |
# 1 | data[3] | data[4] | data[5] |
# 2 | data[6] | data[7] | data[8] |

# Aufruf einer bestimmten Dauer der Belegung b: | Aufruf der Ofenpositionierung
# einer Form f in der Belegung b
#               | data[b * columns] | data[b * columns + 1 + f]
#

        model = cp_model.CpModel()

        #neue Datenstruktur
        output = []
        for assignment_id in range(rows):
            current_assignment = {}
            current_assignment['name'] = 'Assignment ' + str(assignment_id)
            current_assignment['used'] = model.NewBoolVar(current_assignment['name'] + '_used')
            current_assignment['ticks'] = model.NewIntVar(0, max_ticks, current_assignment['name'] + '_ticks')
            current_assignment['assignments'] = []
            for form_id in range(forms_amount):
                 current_assignment['assignments'].append(model.NewIntVar(-1, ovens_amount - 1, current_assignment['name'] + '_Form' + str(form_id)))
            output.append(current_assignment)
     
        

        #Variable initialization
#        for i in range(rows):
#           data.append(model.NewIntVar(0, max_assignment, str(i) + '_av')) #Anzahl Vorgänge
#           for j in range(forms_amount):
#               data.append(model.NewIntVar(0, ovens_amount, str(i) + '_' + str(j)))

        #constraints - Wenn es funktioniert, gerne verkürzen und einige fors
        #rausschmeißen

        #C0: Umsetzung der used-Variable in den Tick-Variablengrenzen
        for assignment in output:
            model.Add(assignment['ticks'] == 0).OnlyEnforceIf(assignment['used'].Not())
            model.Add(assignment['ticks'] > 0).OnlyEnforceIf(assignment['used'])

        #C1: Kein Ofen bei keiner Belegung überfüllt
        for assignment in output:
            for oven in ovens:
                model.Add(sum((form_castingcell_demand[form_id] if assignment['assignments'][form_id] == oven['id'] else 0) for form_id in range(forms_amount) <= oven['size']))
                
        #for oven_id in range(ovens_amount):
        #     for assignment_id in range(rows):
        #         model.Add(sum((form_castingcell_demand[form_id] if data[index(assignment_id, form_id + 1, columns)] == oven_id else 0) for form_id in range(forms_amount)) <= ovens_size[oven_id])
        

        #C2: Alle Produkte ( = Formen) hergestellt?
        for form_id, form in enumerate(forms):
            model.Add(sum((assignment['ticks'] if assignment['assignments'][form_id] != 0 else 0) for assignment in output) == form['required_amount'])

        #for form in range(forms_amount):
        #     model.Add(sum((data[index(assignment_id, 0, columns)] if data[index(assignment_id, form + 1, columns)] else 0) for assignment_id in range(rows)) == order_requirements[form])
       
        #Eine Zeile mit 0 Ticks => alle folgenden Zeilen mit 0 Ticks; also: erste Zeile mit 0 Ticks markiert das Ende der verwendeten Belegungen

        for idx, assignment in enumerate(assignments[:-1]):
            next_assignment = assignments[idx + 1]
            model.AddImplication(assignment['used'].Not(), assignment['used'].Not())


       #for assignment_id in range(len(assignments) - 1): #not python-y
        #    model.Add(assignment[assignment_id + 1]['ticks'] == 0).OnlyEnforceIf(assignment[assignment_id] == 0)
            


        #for assignment_id in range(rows - 1):
        #    model.Add(data[index(assignment_id + 1,0,columns)] == 0).OnlyEnforceIf(data[index(assignment_id, 0, columns)] == 0)

       #C3: Formen werden stets ersetzt, wenn sie abgenutzt werden
        #TODO
#        for form in range(forms_amount):
#           for assignment_id in range(rows): #Effizientere Methode ohne diese
#           Schleife?
#               sumUntilNow = sum(data)
#               model.Add( #negative Zahl existiert: Null muss existieren


        # Anzahl der Belegungen in denen etwas hergestellt wird minimieren -> Anzahl der Wechselungen der Belegung minimiern
        # TODO: Wechseldauer eines Ofens mit einbeziehen
        model.Minimize(sum([1 if assignment['used'] else 0]) for assignment in output)

        #model.Minimize(sum([1 if data[index(assignment_id, 0, columns)] > 0 else 0] for assignment_id in range(rows))) 
       

        solver = cp_model.CpSolver()
        status = solver.Solve(model)
        out = {}
        print('Minimale Anzahl Belegungen: ' + str(solver.ObjectiveValue()))
       # for i in data: 
       #    out.append(solver.Value(str(i)))
        print(out)
        print(solver.ResponseProto())
        return [str(x) for x in out]
    
def index(row, column, row_size):
    return row * row_size + column