using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Data.Entities
{
    public class Form
    {

    [Key] public int Id { get; set; }
        public int Actions { get; set; }
        public int ActionsMax{ get; set; }
        public float CastingCells { get; set; }
    }
}
