using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.Models
{
    public class AbuseReport
    {
        public enum eAbuseType {
            NONE = 0,
            REVIEW = 1,
            RECIPE = 2,
            PICTURE = 4
        }

        [Key]
        public int ID { get; set; }


        public int RecipeID { get; set; }

        public int ReviewID { get; set; }
        public string  UserCD { get; set; }


        public DateTime CreateDate { get; set; }


        public eAbuseType AbuseType { get; set; }

        [MaxLength(1024)]
        public string Comment { get; set; }

    }
}
