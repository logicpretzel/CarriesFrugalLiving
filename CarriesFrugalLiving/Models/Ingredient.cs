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
    public class Ingredient
    {
        

        public int ID { get; set; }
        public int RecipeID { get; set; }
        public decimal Quantity { get; set; }
        public int UnitsID { get; set; }
        [MaxLength(4000)]
        [DataType(DataType.MultilineText)]
        [Display(ShortName = "Review Comment(optional)")]
        public string Description { get; set; }

        
        [Display(ShortName = "Quantity (Whole Number)")]
        public int qtyWhole { get; set; }
        [Display(ShortName = "and/or (Fractional Quantity )")]
        public int qtyFraction { get; set; }
    }
}
