from ortools.sat.python import cp_model
import math

class ObjectivePrinter(cp_model.CpSolverSolutionCallback):
    """Print intermediate solutions."""

    def __init__(self):
        cp_model.CpSolverSolutionCallback.__init__(self)
        self.__solution_count = 0

    def on_solution_callback(self):
        print('Solution %i, time = %f s, objective = %i' %
              (self.__solution_count, self.WallTime(), self.ObjectiveValue()))
        self.__solution_count += 1

class ZFOptimizer():
    """description of class"""
    def __init__(self, ovens, forms):
        # inputs
        #List of Ovens
        self.ovens = ovens

        #List of Forms - required_amount: Amount of Times this form must be used, castingcell_demand: Amount of space occupied by this form
        # Scale castingcell_demand and oven.size so that they are ints
        self.forms = forms

        self.form_sizes = list(map(lambda f: f['castingcell_demand'], self.forms))
        # TODO: ausrechnen lassen# sum(order_requirements) erstmal
        #max_assignment = 2 # maximal erlaubte Anzahl von Belegungen,
        self.max_assignment = sum(list(map(lambda f: f['required_amount'], self.forms))) # maximal erlaubte Anzahl von Belegungen,
        
        # local vars
        self.model = cp_model.CpModel()
        self.assignments = []        

    def add_all_constrains(self):
        self.add_ct_oven_sizes()
        self.add_ct_all_produced()
        self.add_ct_use_first_assignments()

    # C1: Kein Ofen bei keiner Belegung überfüllt
    def add_ct_oven_sizes(self):
        # check this link for explanation https://developers.google.com/optimization/cp/channeling
        for a in self.assignments:
            form_used_in_oven = {}
            for f in range(len(self.forms)):
                form_f_used_in_oven = [self.model.NewBoolVar('%s_From%i_used_in_Oven%i' % (a['name'], f, o)) for o in range(len(self.ovens))]
                for o in range(len(self.ovens)):
                    self.model.Add(a['form_assignments'][f]['oven'] == o).OnlyEnforceIf(form_f_used_in_oven[o])
                    self.model.Add(a['form_assignments'][f]['oven'] != o).OnlyEnforceIf(form_f_used_in_oven[o].Not())

                    form_used_in_oven[(f,o)] = form_f_used_in_oven[o]

            for o in range(len(self.ovens)):
                self.model.Add(self.ovens[o]['size'] >= sum(form_used_in_oven[(f,o)] * self.form_sizes[f] for f in range(len(self.forms))))

    # C2: Alle Produkte( = Formen) hergestellt ?
    def add_ct_all_produced(self):
        for form in self.forms:
            self.model.Add(sum(assignment['form_assignments'][form['id']]['produced'] for assignment in self.assignments) == form['required_amount'])
    
    # C3: Eine Zeile mit 0 Ticks => alle folgenden Zeilen mit 0 Ticks; also: erste Zeile mit 0 Ticks markiert das Ende der verwendeten Belegungen
    def add_ct_use_first_assignments(self):
        for idx, assignment in enumerate(self.assignments[:-1]):
            next_assignment = self.assignments[idx + 1]
            self.model.AddImplication(assignment['used'].Not(), next_assignment['used'].Not())

    # C4: Formen werden stets ersetzt, wenn sie abgenutzt werden# TODO#
        #for form in range(forms_amount): #for assignment_id in range(rows): #Effizientere Methode ohne diese# Schleife ? #sumUntilNow = sum(data)# self.model.Add(#negative Zahl existiert: Null muss existieren

    def init_model(self):
        for assignment_id in range(self.max_assignment):
            current_assignment = {}
            current_assignment['name'] = 'Assignment' + str(assignment_id)
            current_assignment['used'] = self.model.NewBoolVar(current_assignment['name'] + '_used')
            current_assignment['ticks'] = self.model.NewIntVar(0, self.max_assignment, current_assignment['name'] + '_ticks')
            current_assignment['form_assignments'] = []
            for form in self.forms:
                form_assignment = {}
                form_assignment['oven'] = self.model.NewIntVar(-1, len(self.ovens) -1, current_assignment['name'] + '_Form' + str(form['id']))

                # Declare intermediate boolean variable to track usage of form in this assignment  
                form_assignment['used'] = self.model.NewBoolVar("%s_uses_Form%i" % (current_assignment['name'], form['id']))   
                self.model.Add(form_assignment['oven'] >= 0).OnlyEnforceIf(form_assignment['used'])
                self.model.Add(form_assignment['oven'] == -1).OnlyEnforceIf(form_assignment['used'].Not())
                self.model.AddImplication(form_assignment['used'], current_assignment['used']) # if one form is used, the assignment should be used
                self.model.AddImplication(current_assignment['used'].Not(), form_assignment['used'].Not()) # if the assignment is not used, there should be no form used

                # Forms produced of this type in this assignment
                form_assignment['produced'] = self.model.NewIntVar(0, form['required_amount'], "%s_produces_of_type_form%i" % (current_assignment['name'], form['id']))
                self.model.AddMultiplicationEquality(form_assignment['produced'], (current_assignment['ticks'], form_assignment['used'])) # either "ticks * 0" or "ticks * 1"

                current_assignment['form_assignments'].append(form_assignment)

            # Link produced[] to assignment.used
            self.model.Add(sum(fa['produced'] for fa in current_assignment['form_assignments']) == 0).OnlyEnforceIf(current_assignment['used'].Not())
            self.model.Add(sum(fa['produced'] for fa in current_assignment['form_assignments']) > 0).OnlyEnforceIf(current_assignment['used'])

            self.assignments.append(current_assignment)

        # Link used to ticks
        for assignment in self.assignments:
            self.model.Add(assignment['ticks'] == 0).OnlyEnforceIf(assignment['used'].Not())
            self.model.Add(assignment['ticks'] > 0).OnlyEnforceIf(assignment['used'])

    def optimize(self):
        # Anzahl der Belegungen in denen etwas hergestellt wird minimieren - > Anzahl der Wechselungen der Belegung minimiern
        # TODO: Wechseldauer eines Ofens mit einbeziehen 
        # TODO: Prüfen ob beim Wechsel einer Beleung der Ofen überhaupt veränder wird

        self.model.Minimize(sum([assignment['ticks'] + assignment['used'] for assignment in self.assignments]))
        #self.model.Minimize(sum([assignment['used'] for assignment in self.assignments]))


        objective_printer = ObjectivePrinter()
        solver = cp_model.CpSolver() 
        solver.parameters.max_time_in_seconds = 60 * 5 # TODO: make this configurable via api
        solver.parameters.num_search_workers = 8
        status = solver.SolveWithSolutionCallback(self.model, objective_printer)
        print("Status is: " + solver.StatusName(status))

        if status != cp_model.INFEASIBLE: 
            output = []
            for idx, assignment in enumerate(self.assignments):
                oa = {} # output assignment
                oa = {
                        "name": assignment['name'],
                        "used": True if solver.Value(assignment['used']) else False,
                        "ticks": solver.Value(assignment['ticks']),
                        "assigments": [solver.Value(fa['oven']) for fa in assignment['form_assignments']]
                }
                print(assignment['name'])
                print("   used: %s" % ("yes" if solver.Value(assignment['used']) else "no"))
                print("   ticks: %i" % (solver.Value(assignment['ticks'])))
                print("   assigments:")
                for idx, form_assignment in enumerate(assignment['form_assignments']):
                    used = solver.Value(form_assignment['used'])
                    oven = solver.Value(form_assignment['oven'])
                    produced = solver.Value(form_assignment['produced'])
                    print("       From %i %s used %s" % (idx, "is" if used else "is not", f'in oven {oven} to produce {produced}' if used else ""))

                if oa['used']:
                    output.append(oa)
            print(solver.ResponseStats())
            return output
        return False
