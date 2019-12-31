using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace webapp.Data.Entities
{
    public class Form
    {
        [Key] 
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [Range(0,int.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        [JsonPropertyName("current_uses")]
        public int Actions { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        [JsonPropertyName("max_uses")]
        public int ActionsMax { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "{0} muss zwischen {2} und {1} sein")]
        [JsonIgnore]
        public float CastingCells { get; set; }

        [JsonIgnore]
        public double Wear
        {
            get
            {
                return (double)Actions / ActionsMax;
            }
        }

        [JsonIgnore]
        public string WearDisplay
        {
            get
            {
                return string.Format("{0:0.00}%", Wear*100d);
            }
        }

    }
}