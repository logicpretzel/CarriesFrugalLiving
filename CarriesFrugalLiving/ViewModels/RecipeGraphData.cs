using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.ViewModels
{
    public class RecipeGraphData
    {
        public string CategoryNm { get; set; }
        public int Category { get; set; }

        
        private int recipeCount;

        public int RecipeCount
        {
            get { return recipeCount; }
            set { recipeCount = value; }
        }

        public int UserCount { get; set; }
        public int ReviewCount { get; set; }
       
    
    }

    public class RecipeCountsByCategoryGraphData
    {
        public string label { get; set; }
        public int data { get; set; }
     }
}
