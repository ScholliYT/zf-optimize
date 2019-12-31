using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace webapp.Data.Entities.Backend
{
    public class BackendOven : Oven
    {
        public static BackendOven MakeBackendOven(Oven oven)
        {
            return (BackendOven)oven;
        }

        [JsonPropertyName("size")]
        public int Backend_CastingCellAmount { get => (int)CastingCellAmount * CONSTS.SizeScalingFactor; }
    }
}
