using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.ViewModels
{
    public class ContactEmail
    {   [Key]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MaxLength(100)]
        [Required]
        public String Name { get; set; }

        [MaxLength(50)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [MaxLength(1024)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

    }
}
