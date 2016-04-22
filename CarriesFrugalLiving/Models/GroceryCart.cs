using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.Models
{
    public class GroceryCart
    {
        [Key]
        public int ID { get; set; }

       
        [MaxLength(100)]
        public string UserID { get; set; }

        
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }

        [MaxLength(80)]
        public string Name { get; set; }

        public ICollection<GCartItem> GCartItems { get; set; }


    }
}
