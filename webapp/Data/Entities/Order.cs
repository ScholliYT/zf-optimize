using System;
using System.ComponentModel.DataAnnotations;

namespace webapp.Data.Entities
{
    public class Order
    {
        [Key] public int Id { get; set; }
        public DateTime Date { get; set; }

        public string DateDisplay
        {
            get
            {
                return Date.ToString("dd.MM.yyyy");
            }
        }
    }
}