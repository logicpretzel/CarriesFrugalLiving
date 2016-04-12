using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.Models
{
    public class PickList
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int KeyID { get; set; }

        [Required]
        public int CatID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Value { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        [ScaffoldColumn(false)]
        public bool Void { get; set; }
    }
}
