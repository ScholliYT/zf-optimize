using System.ComponentModel.DataAnnotations;

namespace webapp.Data.Entities
{
    public class Form
    {
        [Key] public int Id { get; set; }
        
        [Range(0,int.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        public int Actions { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        public int ActionsMax { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        public float CastingCells { get; set; }

    }
}