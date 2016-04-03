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
    public enum eMPDay
    {
        NONE = 0,
        SUN,
        MON,
        TUE,
        WED,
        THU,
        FRI,
        SAT
    }

    public class MPlanItem
    {
        public int ID { get; set; }
        public int MealPlanID { get; set; }
        public string Description { get; set; }
        public DateTime MealDate { get; set; }
        public eMPDay MealDay { get; set; }


    }
}
