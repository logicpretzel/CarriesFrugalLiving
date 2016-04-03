using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CarriesFrugalLiving.Models
{
    /// <summary>
    /// Author: Dar Dunham
    /// Date: 2/15/16
    /// Model
    /// </summary>
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int? RecipeID { get; set; }
        public byte[] ImageData { get; set; }
       

        public int? Size { get; set; }
        public string FileName { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string ContentType { get; set; }

        [AllowHtml]
        public string Contents { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select file")]
        public HttpPostedFileBase File { get; set; }
    }

}
