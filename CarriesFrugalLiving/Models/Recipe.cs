using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CarriesFrugalLiving.Models
{

    /// <summary>
    /// Author: Dar Dunham
    /// Date: 2/15/16
    /// Model
    /// </summary>
    public class Recipe
    {

        public enum eCookingMethod
        {
            NONE = 0,
            STOVETOP,
            SIMMER,
            BAKE,
            BROIL,
            BOIL,
            PRESSURE_COOK,
            ROAST,
            TOAST,
            GRILL,
            FRY,
            DEEPFRY,
            CHILL,
            FREEZE
        }

        public enum eReleasedFlag
        {
            NEW = 0,
            EDITED,
            REVIEWING,
            RELEASED,
            REJECTED
        }

        public enum eCategory {
            NONE = 0,
            ENTREE,
            BREAD,
            SIDEDISH,
            DESSERT,
            SALAD,
            SOUP
        }

        private eReleasedFlag _releasedFlag;


        public int ID { get; set; }

        [Display(Name = "Title")]
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Display(Name="Description")]
        [MaxLength(256)]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [Display(Name = "Ingredients")]
        [MaxLength(4000)]
        public string IngredientString { get; set; }

        [MaxLength(4000)]
        [DataType(DataType.MultilineText)]
        public string Preparation { get; set; }

        [Display(Name = "Cooking Instructions")]
        [DataType(DataType.MultilineText)]
        public string CookingInstructions { get; set; }

        [Display(Name = "Cooking Method")]
        public eCookingMethod?  CookingMethod { get; set; }

        [MaxLength(4000)]
        [Display(Name = "Serving Suggestion")]
        [DataType(DataType.MultilineText)]
        public string ServingInstructions { get; set; }

        [MaxLength(4000)]
        [Display(Name = "Nutrition Information")]
        [DataType(DataType.MultilineText)]
        public string NutritionInformation { get; set; }

        public int? NumberOfReviews { get; set; }

        public decimal? Rating { get; set; }

        public eCategory? Category { get; set; }

        private int? _submittedBy;
        public int? SubscriberID
        {
            get { return _submittedBy; }
            set { if (value == null) { value = 0; }
                _submittedBy = value;
            }
        }

        [MaxLength(4)]
        public string SubmittedInitials { get; set; }

        [MaxLength(100)]
        public string UserCd { get; set; }
        public eReleasedFlag ReleasedFlag
        {
            get { return _releasedFlag; }
            set { _releasedFlag = value; }
        }
        public string PictureURL { get; set; }
        public string PictureCaption { get; set; }
        public int? PicWidth { get; set; }
        public int? PicHeight { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
        public ICollection<AbuseReport> AbuseReport { get; set; }


    }

}
