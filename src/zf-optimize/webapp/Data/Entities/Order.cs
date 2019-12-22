using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Data.Entities
{
    public class Order
    {
        [Key] public int Id { get; set; }
        public DateTime Date { get; set; }

    }
}
