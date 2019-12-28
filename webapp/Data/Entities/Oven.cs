using System;
using System.ComponentModel.DataAnnotations;

namespace webapp.Data.Entities
{
    public class Oven
    {
        [Key] public int Id { get; set; }

        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double CastingCellAmount { get; set; }

        [Required]
        public TimeSpan ChangeDuration { get; set; }

        public string ChangeDurationDisplay
        {
            get
            {
                return ChangeDuration.ToString(@"hh\:mm\:ss");
            }
        }
    }
}