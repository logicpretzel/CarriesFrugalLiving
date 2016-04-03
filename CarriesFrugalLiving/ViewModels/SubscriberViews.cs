using CarriesFrugalLiving.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.ViewModels
{
    public class SubscriberEditView 
    {
        [Key]
        public int ID { get; set; }

        [MaxLength(50, ErrorMessage = "Max length = 50.")]
        public string LastName { get; set; }

        [MaxLength(50, ErrorMessage = "Max length = 50.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Max length = 50.")]
        public string City { get; set; }

        public string State { get; set; }

        [MaxLength(150, ErrorMessage = "Max length = 150.")]
        public string Email { get; set; }


    }
}
