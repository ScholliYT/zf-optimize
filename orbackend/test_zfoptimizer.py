import unittest
from services.zfoptimizer import ZFOptimizer

class Test_ZFOptimizer(unittest.TestCase):
    def run_optimizaion(self, forms, ovens):
        op = ZFOptimizer(ovens, forms)
        op.init_model()
        op.add_all_constrains()
        solution = op.optimize()
        return solution


    def test_almost_same_amounts(self):
        ovens = [{"id": 0, "size": 5, "changeduration_sec": 50}]

        forms = [{"id": 0, "required_amount": 12, "castingcell_demand": 1},
                 {"id": 1, "required_amount": 13, "castingcell_demand": 1}]

        solution = self.run_optimizaion(forms, ovens)
        
        # Verify
        self.assertEqual(len(solution), 2) # there should just be 2 assignments

    def test_only_one_oven_fits(self):
        ovens = [{"id": 0, "size": 1, "changeduration_sec": 50},
                 {"id": 1, "size": 2, "changeduration_sec": 50},
                 {"id": 2, "size": 3, "changeduration_sec": 50},
                 {"id": 3, "size": 4, "changeduration_sec": 50},
                 {"id": 4, "size": 5, "changeduration_sec": 50}]

        forms = [{"id": 0, "required_amount": 12, "castingcell_demand": 5}]

        solution = self.run_optimizaion(forms, ovens)

        # Verify
        self.assertEqual(len(solution), 1)
        
        a = solution[0]
        self.assertIsNotNone(a)
        self.assertEqual(a['ticks'], 12)
        self.assertTrue(a['used'])

        fa = a['assigments']
        self.assertIsNotNone(fa)
        self.assertEqual(len(fa), 1)
        self.assertEqual(fa[0], 4)

    def test_form_too_big(self):
        ovens = [{"id": 0, "size": 5, "changeduration_sec": 50}]

        forms = [{"id": 0, "required_amount": 12, "castingcell_demand": 6},
                 {"id": 1, "required_amount": 13, "castingcell_demand": 1}]

        solution = self.run_optimizaion(forms, ovens)

        # Verify
        self.assertFalse(solution)


if __name__ == '__main__':
    unittest.main()
