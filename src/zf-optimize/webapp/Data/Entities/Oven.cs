using System;
using System.ComponentModel.DataAnnotations;

namespace webapp.Data.Entities
{
    public class Oven
    {
        [Key] public int Id { get; set; }
        public int CastingCellAmount { get; set; }
        public TimeSpan ChangeDuration { get; set; }
    }
}