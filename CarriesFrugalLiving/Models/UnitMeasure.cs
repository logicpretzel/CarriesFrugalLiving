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
    public class UnitMeasure
    {
        public int ID { get; set; }
        public string TypeCd { get; set; }
        public string Description { get; set; }
        public string Abbrev { get; set; }
        public decimal EquivalentValue { get; set; }
        public int EquivalentMeasure { get; set; }

    }
}
