using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace webapp.Data.Entities.Backend
{
    public class BackendForm : Form
    {
        public static BackendForm MakeBackendForm(Form form, int requiredAmount)
        {
            var f = (BackendForm)form;
            f.Backend_RequiredAmount = requiredAmount;
            return f;
        }

        [JsonPropertyName("castingcell_demand")]
        public int Backend_CastingcellDemand
        {
            get
            {
                return (int)CastingCells * CONSTS.SizeScalingFactor;
            }
        }

        [JsonPropertyName("required_amount")]
        public int Backend_RequiredAmount { get; set; }
    }
}
