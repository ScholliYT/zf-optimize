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
       
        self.max_assignment = sum(list(map(lambda f: f['required_amount'], self.forms))) # maximal erlaubte Anzahl von Belegungen
        #self.max_assignment = 15
        self.max_required_amount = max(list(map(lambda f: f['required_amount'], self.forms))) # maximale geforderte Stückzahl einer spezifischen Form
        self.max_sum = self.max_assignment * 1000 #wieso? wie groß? Häää?
        # local vars
        self.model = cp_model.CpModel()
        self.assignments = []        

    def add_all_constrains(self):
        self.add_ct_oven_sizes()
        self.add_ct_all_produced()
        self.add_ct_use_first_assignments()
        self.add_ct_repair_forms()

    # C1: Kein Ofen bei keiner Belegung überfüllt
    def add_ct_oven_sizes(self):
        # check this link for explanation https://developers.google.com/optimization/cp/channeling
        for a in self.assignments:
            form_used_in_oven = a['form_used_in_oven']
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
        
    
    # C4: Formen werden stets ersetzt, wenn sie abgenutzt werden
    def add_ct_repair_forms(self):
        for form in self.forms:
            for assignment_id, assignment in enumerate(self.assignments): #alle außer dem Ersten Assignment durchgehen
                self.model.Add(assignment['form_assignments'][form['id']]['moduloed_sum_until_now'] + assignment['ticks'] >= form['max_uses']).OnlyEnforceIf(assignment['form_assignments'][form['id']]['repaired_here'])
                self.model.Add(assignment['form_assignments'][form['id']]['moduloed_sum_until_now'] + assignment['ticks'] <  form['max_uses']).OnlyEnforceIf(assignment['form_assignments'][form['id']]['repaired_here'].Not())

    def init_model(self):
        for assignment_id in range(self.max_assignment):
            current_assignment = {}
            current_assignment['name'] = 'Assignment' + str(assignment_id)
            current_assignment['used'] = self.model.NewBoolVar(current_assignment['name'] + '_used')
            current_assignment['ticks'] = self.model.NewIntVar(0, self.max_required_amount, current_assignment['name'] + '_ticks')
            current_assignment['form_assignments'] = []
            for form in self.forms:
                form_assignment = {}
                current_assignment['form_assignments'].append(form_assignment)
                form_assignment['oven'] = self.model.NewIntVar(-1, len(self.ovens) -1, current_assignment['name'] + '_Form' + str(form['id'])) # in which oven is this form used in this assignment (-1 == unused)

                # Declare intermediate boolean variable to track usage of form in this assignment  
                form_assignment['used'] = self.model.NewBoolVar("%s_uses_Form%i" % (current_assignment['name'], form['id']))   
                self.model.Add(form_assignment['oven'] >= 0).OnlyEnforceIf(form_assignment['used'])
                self.model.Add(form_assignment['oven'] == -1).OnlyEnforceIf(form_assignment['used'].Not())
                self.model.AddImplication(form_assignment['used'], current_assignment['used']) # if one form is used, the assignment should be used
                self.model.AddImplication(current_assignment['used'].Not(), form_assignment['used'].Not()) # if the assignment is not used, there should be no form used

                # Forms produced of this type in this assignment
                form_assignment['produced'] = self.model.NewIntVar(0, form['required_amount'], "%s_produces_of_type_form%i" % (current_assignment['name'], form['id']))
                self.model.AddMultiplicationEquality(form_assignment['produced'], (current_assignment['ticks'], form_assignment['used'])) # either "ticks * 0" or "ticks * 1"

                # is this form repaired prior to this assignment?
                form_assignment['repaired_here'] = self.model.NewBoolVar('Form%i_is_repaired_in_%s' % (form['id'], current_assignment['name']))

                
                form_assignment['moduloed_sum_until_now'] = self.model.NewIntVar(0, form['max_uses'], 'Modulo_Helper_Form%i %s' % (form['id'], current_assignment['name']))
                sum_until_now = self.model.NewIntVar(0, self.max_sum, 'sum of all ticks that happened so far')
                self.model.Add(sum_until_now == form['current_uses'] + sum([self.assignments[assignment_hlp_index]['form_assignments'][form['id']]['produced'] for assignment_hlp_index in range(assignment_id)]))
                self.model.AddModuloEquality(form_assignment['moduloed_sum_until_now'], sum_until_now , form['max_uses'])
                self.model.Add(form_assignment['moduloed_sum_until_now'] + current_assignment['ticks'] == form['max_uses']).OnlyEnforceIf(form_assignment['repaired_here'])
                self.model.Add(form_assignment['moduloed_sum_until_now'] + current_assignment['ticks'] != form['max_uses']).OnlyEnforceIf(form_assignment['repaired_here'].Not())
            
                
               
            # init form_used_in_oven
            current_assignment['form_used_in_oven'] = {}
            for f in range(len(self.forms)):
                form_f_used_in_oven = [self.model.NewBoolVar('%s_Form%i_used_in_Oven%i' % (current_assignment['name'], f, o)) for o in range(len(self.ovens))]
                for o in range(len(self.ovens)):
                    self.model.Add(current_assignment['form_assignments'][f]['oven'] == o).OnlyEnforceIf(form_f_used_in_oven[o])
                    self.model.Add(current_assignment['form_assignments'][f]['oven'] != o).OnlyEnforceIf(form_f_used_in_oven[o].Not())

                    current_assignment['form_used_in_oven'][(f,o)] = form_f_used_in_oven[o]
                    

            # init oven_has_changed
            # whose sum shall be minimized
            current_assignment['oven_has_changed'] = []
            current_assignment['oven_needs_special_treatment'] = []
            for oven in self.ovens:
                current_oven_has_changed = self.model.NewBoolVar('%s_oven%i_has_changed' %(current_assignment['name'], oven['id']))
                current_oven_needs_special_treetment = self.model.NewBoolVar('%s_oven%i_needs_special_treatment' %(current_assignment['name'], oven['id']))
                current_assignment['oven_has_changed'].append(current_oven_has_changed)
                current_assignment['oven_needs_special_treatment'].append(current_oven_needs_special_treetment)
                self.model.AddImplication(current_oven_has_changed, current_oven_needs_special_treetment)
                

                form_used_in_oven = current_assignment['form_used_in_oven']
               
                if assignment_id == 0: # first assignment
                    self.model.Add(sum([form_used_in_oven[(f,oven['id'])] for f in range(len(self.forms))]) > 0).OnlyEnforceIf(current_oven_has_changed)
                    self.model.Add(sum([form_used_in_oven[(f,oven['id'])] for f in range(len(self.forms))]) == 0).OnlyEnforceIf(current_oven_has_changed.Not())
                else: #for each oven, link its current_assignment['oven_has_changed'] value to the actual change in comparison to the prior assignment
                    prev_assignment = self.assignments[assignment_id-1]
                    form_used_in_oven_last = prev_assignment['form_used_in_oven'] # TODO: Delete
                    prev_form_used_in_oven = prev_assignment['form_used_in_oven']
                 
                    form_changed_in_oven = [self.model.NewIntVar(-1, 1, "%s_oven%i_form%i_has_changed" % (current_assignment['name'], oven['id'], f)) for f in range(len(self.forms))]
                    form_changed_in_oven_abs = [self.model.NewBoolVar("%s_oven%i_form%i_has_changed_abs" % (current_assignment['name'], oven['id'], f)) for f in range(len(self.forms))]
                    for f in range(len(self.forms)):
                        self.model.Add(form_changed_in_oven[f] == form_used_in_oven[(f,oven['id'])] - form_used_in_oven_last[(f,oven['id'])])

                    for f in range(len(self.forms)):
                        self.model.AddAbsEquality(form_changed_in_oven_abs[f], form_changed_in_oven[f])
                    
                    # Mindestens ein Unterschied <=> Maximum aller Differenzen oder deren Inverse ist 1 <=> Mindestens eine Differenz ist != 0
                    self.model.Add(sum(form_changed_in_oven_abs) > 0).OnlyEnforceIf(current_oven_has_changed) #TODO Ersetzen oder so
                    # Gar kein Unterschied <=> Maximum aller Differenzen oder deren Inverse ist 0 <=> Alle Differenzen sind 0
                    self.model.Add(sum(form_changed_in_oven_abs) == 0).OnlyEnforceIf(current_oven_has_changed.Not())
                    #oven needs to be worked on if (1) its forms are exchanged
                for f in range(len(self.forms)): #id_vs counter
                    self.model.AddImplication(current_oven_needs_special_treetment.Not(), current_assignment['form_assignments'][f]['repaired_here'].Not())
                


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

        self.model.Minimize(sum([sum(assignment['oven_needs_special_treatment']) for assignment in self.assignments]) + sum([assignment['ticks'] + assignment['used'] for assignment in self.assignments]))
        #self.model.Minimize(sum([sum(assignment['oven_needs_special_treatment']) for assignment in self.assignments]) + sum([assignment['ticks']  for assignment in self.assignments]))
        #old#self.model.Minimize(sum([sum(assignment['oven_has_changed']) for assignment in self.assignments]))


        objective_printer = ObjectivePrinter()
        solver = cp_model.CpSolver() 
        solver.parameters.max_time_in_seconds = 60 * 5 # TODO: make this configurable via api
        solver.parameters.num_search_workers = 8
        status = solver.SolveWithSolutionCallback(self.model, objective_printer)
        print("Status is: " + solver.StatusName(status))
        if status == cp_model.MODEL_INVALID:
            #The given CpModelProto didn't pass the validation step. You can get a detailed error by calling ValidateCpModel(model_proto).
            print("Model is invalid:")
            print(self.model.Validate())
        elif status != cp_model.INFEASIBLE: 
            output = []
            for idx, assignment in enumerate(self.assignments):
                oa = {} # output assignment
                oa = {
                        "name": assignment['name'],
                        "used": True if solver.Value(assignment['used']) else False,
                        "ticks": solver.Value(assignment['ticks']),
                        "assignments": [solver.Value(fa['oven']) for fa in assignment['form_assignments']]
                }
                if oa['used']:
                    print(assignment['name'])
                    print("   used: %s" % ("yes" if solver.Value(assignment['used']) else "no"))
                    print("   ticks: %i" % (solver.Value(assignment['ticks'])))
                    print("   assignments:")
                    for idx, form_assignment in enumerate(assignment['form_assignments']):
                        used = solver.Value(form_assignment['used'])
                        oven = solver.Value(form_assignment['oven'])
                        produced = solver.Value(form_assignment['produced'])
                        print("       Form %i %s used %s" % (idx, "is" if used else "is not", f'in oven {oven} to produce {produced}' if used else "") + '    repaired_here: ' + str(solver.Value(form_assignment['repaired_here'])))
                    print("   oven_changes:")
                    for idx, oven_has_changed in enumerate(assignment['oven_has_changed']):
                        print("       Oven %i has %schanged" % (idx, "" if solver.Value(oven_has_changed) else "not "))
                    output.append(oa)
            # STATS
            print("Optimizaion goals:")
            print("    oven_changes %i" %     (sum([sum([solver.Value(ovh) for ovh in assignment['oven_has_changed']]) for assignment in self.assignments])))
            print("    oven_needs_special_treatment %i" %     (sum([sum([solver.Value(ovh) for ovh in assignment['oven_needs_special_treatment']]) for assignment in self.assignments])))
            print("    ticks %i" %            (sum([solver.Value(assignment['ticks'])                 for assignment in self.assignments])))
            print("    assignments_used %i" % (sum([solver.Value(assignment['used'])                  for assignment in self.assignments])))

            print(solver.ResponseStats())
            return output
        return False


