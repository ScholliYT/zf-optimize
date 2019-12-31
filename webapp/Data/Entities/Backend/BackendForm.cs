using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace webapp.Data.Entities.Backend
{
    public class BackendForm : Form
    {
        [JsonPropertyName("castingcell_demand")]
        public int Backend_CastingcellDemand { get
            {
                return (int)CastingCells * CONSTS.SizeScalingFactor;
            }
        }

        [JsonPropertyName("required_amount")]
        public int Backend_RequiredAmount { get; set; }
    }
}
