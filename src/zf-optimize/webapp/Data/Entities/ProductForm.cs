using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Data.Entities
{
    public class ProductForm
    {
        public int ProductId { get; set; }
        public int FormId { get; set; }
        public Product Product { get; set; }
        public Form Form { get; set; }
        public float Amount { get; set; }
    }
}
