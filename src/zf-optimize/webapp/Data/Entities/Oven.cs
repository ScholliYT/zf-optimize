using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Data.Entities
{
    public class Oven
    {
        [Key] public int Id { get; set; }
        public int CastingCellAmount { get; set; }
        public TimeSpan ChangeDuration { get; set; }
    }
}
