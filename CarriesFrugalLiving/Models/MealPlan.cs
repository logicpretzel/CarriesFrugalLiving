using System;
using System.Collections.Generic;
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
    public class MealPlan
    {
        public int ID { get; set; }
        public string UserCD { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public ICollection<MPlanItem> PlanItems { get; set; }
    }
}
