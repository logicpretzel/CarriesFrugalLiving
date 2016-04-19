using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.Models
{
    public class Review
    {

        public enum eReviewStarRating
        {
            NONE = 0,
            ONE_STAR,
            TWO_STARS,
            THREE_STARS,
            FOUR_STARS,
            FIVE_STARS
        }

        public int ID { get; set; }
        public int RecipeID { get; set; }

        [MaxLength(50)]
        [Display (Name = "Display Name (Alias, Handle)",ShortName ="Display Name (alias)")]
        public string ReviewerDisplayName { get; set; }

        [MaxLength(4000)]
        [DataType(DataType.MultilineText)]
        [Display (Name ="Review Comment(optional)")]
        public string ReviewText { get; set; }

        
        public eReviewStarRating Rating { get; set; }

        [Display(Name = "I tried this recipe")]
        public bool TriedRecipe { get; set; }

        public bool IsUser { get; set; }


        [MaxLength(100)]
        public string UserID { get; set; }

        [MaxLength(100)]
        public string UserCd { get; set; }

        public bool IsValidated { get; set; }

        [Display(Name = "Flag as offensive")]
        public bool Offensive { get; set; }


        [MaxLength(100)]
        public string ReportedByUserCD { get; set; }


    }
}
