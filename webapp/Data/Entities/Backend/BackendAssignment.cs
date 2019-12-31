using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace webapp.Data.Entities.Backend
{
    public class BackendAssignment
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("used")]
        public bool Used { get; set; }

        [JsonPropertyName("ticks")]
        public int Ticks { get; set; }

        [JsonPropertyName("assignments")]
        public List<int> FormAssignments { get; set; }

        [JsonPropertyName("oven_has_changed")]
        public List<bool> OvenChanges { get; set; }

        [JsonPropertyName("repaired_here")]
        public List<bool> FormRepairs { get; set; }

    }
}
