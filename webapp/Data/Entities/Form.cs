using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.Data.Entities
{
    public class Form
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        public int Actions { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        public int ActionsMax { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        public float CastingCells { get; set; }

        public double Wear => (double)Actions / ActionsMax;

        public string WearDisplay => $"{Wear * 100d:0.00}%";
    }
}