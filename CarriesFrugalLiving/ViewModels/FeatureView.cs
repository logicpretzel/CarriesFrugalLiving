using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.ViewModels
{
    public class FeatureView
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }


        [DataType(DataType.MultilineText)]
        public string FeatureText { get; set; }

        [DataType(DataType.Url)]
        public string url { get; set; }

    }
}
