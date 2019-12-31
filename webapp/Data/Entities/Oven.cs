using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace webapp.Data.Entities
{
    public class Oven
    {
        [JsonPropertyName("id")]
        [Key] public int Id { get; set; }

        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        [JsonIgnore]
        public double CastingCellAmount { get; set; }

        [Required]
        [JsonIgnore]
        public TimeSpan ChangeDuration { get; set; }

        [JsonPropertyName("changeduration_sec")]
        public int ChangedurationSec
        {
            get
            {
                return ChangeDuration.Seconds;
            }
        }

        [JsonIgnore]
        public string ChangeDurationDisplay
        {
            get
            {
                return ChangeDuration.ToString(@"hh\:mm\:ss");
            }
        }
    }
}