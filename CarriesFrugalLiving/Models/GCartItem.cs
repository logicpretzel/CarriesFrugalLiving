using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.Models
{
    /// <summary>
    /// Author: Dar Dunham
    /// Date: 2/15/16
    /// Model
    /// </summary>
    public class GCartItem
    {
        [Key]
        public int ID { get; set; }
        
        public int GroceryCartID { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }

        public int IngredientID { get; set; }
        public decimal Quantity { get; set; }
        public int UnitsID { get; set; }


    }
}
