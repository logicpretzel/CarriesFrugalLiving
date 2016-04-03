using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarriesFrugalLiving.ViewModels
{
    public class ReviewListView
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

 

        [MaxLength(50)]
        [Display(Name = "Display Name")]
        public string ReviewerDisplayName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Comment")]
        private string _reviewShortened;

        public string ReviewShortened
        {
            get { return _reviewShortened; }
            set { _reviewShortened = value; }
        }

        public eReviewStarRating Rating { get; set; }

       


        [Display(Name = "Offensive")]
        public bool Offensive { get; set; }

        public int ID { get; set; }
        public int RecipeID { get; set; }


    }
}