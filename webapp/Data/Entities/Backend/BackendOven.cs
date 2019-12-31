using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace webapp.Data.Entities.Backend
{
    public class BackendOven : Oven
    {
        [JsonPropertyName("size")]
        public double Backend_CastingCellAmount { get => CastingCellAmount * CONSTS.SizeScalingFactor; }
    }
}
