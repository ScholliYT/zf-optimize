using System.ComponentModel.DataAnnotations;

namespace webapp.Data.Entities
{
    public class Form
    {
        [Key] public int Id { get; set; }
        public int Actions { get; set; }
        public int ActionsMax { get; set; }
        public float CastingCells { get; set; }
    }
}