using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CarriesFrugalLiving.Models
{
    public class Feature
    {   [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(120)]
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string FeatureText { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",  ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [DefaultValue(1)]
        public int Category { get; set; }

        [DataType(DataType.Url)]
        public string url { get; set; }

    }
}
