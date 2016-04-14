using CarriesFrugalLiving.Models;
using CarriesFrugalLiving.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.SqlTypes;
using System.Collections;

namespace CarriesFrugalLiving.DAL
{
    partial class  RecipeRepository
    {
        //FrugalLivingDB _dc = new FrugalLivingDB();
        //public object Images { get; private set; }
       

        ApplicationDbContext _dc = new ApplicationDbContext();


        /// <summary>
        /// GetPickList
        /// The picklists database table is used to store data used in various dropdown lists
        /// The catid is used to determine the type of picklist. Catid 0 is the master index of picklist types, and
        /// is used to document the piclists only.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        internal IEnumerable GetPickList(int categoryID)
        {
            IQueryable<PickList> _pl = _dc.PickLists;
            _pl = (from r in _pl where r.CatID == categoryID select r).Where(r => r.Void == false).OrderBy(r => r.CatID).ThenBy(r=>r.KeyID);
            return _pl.ToList();
        }



        #region RECIPES
        public IEnumerable<Recipe> SelectRecipes(string kw = "") {
           
                // IngredientListView ingredients = new IngredientListView();
                var idParam = new SqlParameter
                {
                    ParameterName = "kw",
                    Value = kw
                };
                var _recipes = _dc.Database.SqlQuery<Recipe>(
                    "spSelectRecipesByKW @kw", idParam);

                return _recipes.ToList();

            
            
        }


        public Recipe GetRecipeByID(int id)
        {
            IQueryable<Recipe> recipe = _dc.Recipes;
            if (id >= 1)
                recipe = recipe.Where(s => s.ID == id);

            return recipe.FirstOrDefault();
        }

        public IEnumerable<RecipeGraphData> GetRecipeDemos()
        {
            IEnumerable<RecipeGraphData> rc = _dc.Database.SqlQuery<RecipeGraphData>("Select * from vwRecipeDemos");
            return rc;
        }

        public IEnumerable<RecipeCountsByCategoryGraphData> GetRecipeCountsByCategory()
        {
            IEnumerable<RecipeCountsByCategoryGraphData> rc = _dc.Database.SqlQuery<RecipeCountsByCategoryGraphData>( "Select CategoryNm as label, RecipeCount as data from vwRecipeDemos");
            return rc;
        }



        public IEnumerable<Recipe> SelectRecipesByCategory(Recipe.eCategory category = Recipe.eCategory.NONE)
        {

            IQueryable<Recipe> _recipes = _dc.Recipes;
            //if (category != Recipe.eCategory.NONE)
            //    _recipes = _recipes.Where(s => s.Category == category).Take(100);

            if (category != Recipe.eCategory.NONE)
            {
                _recipes = (from r in _recipes where r.Category == category select r).Take(100);
                _recipes.OrderByDescending(o => o.Rating);
            }
            return _recipes.ToList();


        }

        #endregion


        #region REVIEWS
        /*
     CREATE PROCEDURE spGetReviewsList 
	 @UserCD varchar(100) = null
	,@Rating int = null
	,@Offensive int = null
	,@RecipeID int = null
	,@kw varchar(20) = null
        */
        public IEnumerable<ReviewListView> 
            GetReviewList(
             string     userCD      =   "",
             int        rating      =   0,
             int        offensive   =   0,
             int        recipeID    =   0,
             string     kw          =   ""
            )
        {
            

            var idParam1 = new SqlParameter
            {
                ParameterName = "UserCD",
                Value = userCD.Length > 0 ? userCD : SqlString.Null
            };

            var idParam2 = new SqlParameter
            {
                ParameterName = "Rating",
                Value = rating > 0 ? rating : SqlInt32.Null
            };

            var idParam3 = new SqlParameter
            {
                ParameterName = "Offensive",
                Value = offensive > 0 ? offensive : SqlInt32.Null
            };

            var idParam4 = new SqlParameter
            {
                ParameterName = "RecipeID",
                Value = recipeID > 0 ? recipeID : SqlInt32.Null
            };

            var idParam5 = new SqlParameter
            {
                ParameterName = "kw",
                Value = kw.Length > 0 ? kw : SqlString.Null
            };

            var _model = _dc.Database.SqlQuery<ReviewListView>(
                "spGetReviewsList @UserCD, @Rating, @Offensive, @RecipeID, @kw", idParam1, idParam2, idParam3, idParam4, idParam5);

            return _model.ToList();



        }

        public IEnumerable<Review> SelectReviews(int recipeID)
        {

            IQueryable<Review> _reviews = _dc.Reviews;
            if (recipeID >= 1)
                _reviews = (from r in _reviews where r.RecipeID == recipeID select r).Where(r => r.Offensive == false);


            return _reviews.ToList();

        }

        public int GetReviewCount(int recipeID)
        {
            int rc = 0;

            if (recipeID >= 1)
            {
                IQueryable<Review> _reviews = _dc.Reviews;
                _reviews = (from r in _reviews where r.RecipeID == recipeID select r).Where(r => r.Offensive == false); 
                

                rc = _reviews.Count();
            }
            return rc;
        }


        public double GetAverageRating(int recipeID)
        {
            double rc = 0;

            if (recipeID >= 1)
            {    try {
                    IQueryable<Review> _reviews = _dc.Reviews;
                    _reviews = (from r in _reviews where r.RecipeID == recipeID select r).Where(r => r.Offensive == false);
                    var t = _reviews.Average(f => (double?)f.Rating);
                    if (t != null) rc = (double)t;
                } catch 
                {
                    rc = 0;
                }
            }
            return rc;
        }


        /// <summary>
        /// GetReviewDisplayString
        ///   used in review display form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetReviewDisplayString(int id)
        {
            string rc = "";

            // get count of reviews and average rating
            if (id >= 1)
            {
                try
                {
                    Recipe _recipe = (from i in _dc.Recipes
                                      where i.ID == id
                                      select i).FirstOrDefault();

                    if (_recipe != null)
                    {  // make sure we have a valid recipe

                        int reviewCount = GetReviewCount(id);
                        double ar = GetAverageRating(id);

                        rc = String.Format("Total Reviews: {0}\n  Average Rating: {1:n2} stars", reviewCount, ar);
                    }
                }
                catch (Exception e)
                {
                    rc = String.Format("error: {0}", e.Message.ToString());
                }
            }

            return rc;
        } // END GetReviewDisplayString

        #endregion



        #region IMAGEPROCESSING


        public int GetImageIdFromRecipeID(int recipeID) {
            Image image = (from i in _dc.Images
                           where i.RecipeID == recipeID
                           select i).FirstOrDefault();
            return image != null ? image.Id : 0;
        }

        public FileContentResult GetImageContent(int imageID)
        {
            Image image = (from i in _dc.Images
                           where i.Id == imageID
                           select i).FirstOrDefault();

            return image != null ? new FileContentResult(image.ImageData, image.ContentType): null;
        }

        public Image GetImage(int imageID)
        {
            Image image = (from i in _dc.Images
                           where i.Id == imageID
                           select i).FirstOrDefault();

            return image ;
        }

        public  void RemoveImagesForRecipe(int rId)
        {
            //Image image = (from i in _dc.Images
            //               where i.RecipeID == rId
            //               select i).FirstOrDefault();
            //if (image != null)
            //{
                _dc.Images.RemoveRange(from i in _dc.Images
                                       where i.RecipeID == rId
                                       select i );
                _dc.SaveChanges();
            //}
        }


        #endregion

        #region INGREDIENTS

        public IEnumerable<Ingredient> GetIngredientList(int recipeID)
        {

            IQueryable<Ingredient> _ingredients = _dc.Ingredients;
            if (recipeID >= 1)
                _ingredients = _ingredients.Where(s => s.RecipeID == recipeID);

            return _ingredients.ToList();

        }

        public IEnumerable<IngredientListView> GetIngredientViewList(int recipeID)
        {
            // IngredientListView ingredients = new IngredientListView();
            var idParam = new SqlParameter {
                ParameterName = "recipeid",  
                Value = recipeID
                };
            var ingredients = _dc.Database.SqlQuery<IngredientListView>(
                "pDisplayIngredients @recipeid",
            idParam).ToList<IngredientListView>();

       
            return ingredients;

        }


        #endregion

        #region GROCERYCARTS

        public GroceryCart GetGroceryCart(int id)
        {
            GroceryCart rc = new GroceryCart();
            IQueryable<GroceryCart> _gc = _dc.GroceryCarts;
            _gc = _gc.Where(s => s.ID == id);
            rc = _gc.FirstOrDefault();

            return rc;

        }


        public IEnumerable<GroceryCart> GetGroceryCarts()
        {
            
            IQueryable<GroceryCart> _gc = _dc.GroceryCarts;
            _gc = _gc.Take(100);
            return _gc.ToList();
        }


        public IEnumerable<GroceryCart> GetGroceryCarts(int take = 100, int skip = 0)
        {
            IQueryable<GroceryCart> _gc = _dc.GroceryCarts;
            _gc = _gc.Skip(skip).Take(take);
            return _gc.ToList();
        }

        public IEnumerable<GroceryCart> GetGroceryCarts(string userCD)
        {
            IQueryable<GroceryCart> _gc = _dc.GroceryCarts;
            _gc = _gc.Where(s => s.UserCD.Trim().ToLower() == userCD.Trim().ToLower());
            return _gc.ToList();

        }

        public string GetCartName(int cartID)
        {
            string rc = "";
            try
            {
                IQueryable<GroceryCart> _gc = _dc.GroceryCarts;
                _gc = _gc.Where(s => s.ID == cartID);
                rc = _gc.FirstOrDefault().Name.ToString();

            }
            catch (Exception e) {
                // 
                rc = e.Message;
                //TODO: do something with errors
                rc = "";
            }

            return rc;

        }

        public string GetCartOwnerID(int cartID)
        {
            string rc = "";
            try
            {
                IQueryable<GroceryCart> _gc = _dc.GroceryCarts;
                _gc = _gc.Where(s => s.ID == cartID);
                rc = _gc.FirstOrDefault().UserCD.ToString();

            }
            catch (Exception e)
            {
                // 
                rc = e.Message;
                //TODO: do something with errors
                rc = "";
            }

            return rc;

        }

        public IEnumerable<GCartItem> GetGroceryList(int cartID)
        {
            IQueryable<GCartItem> _q = _dc.GCartItems;
            if (cartID >= 1)
                _q = _q.Where(s => s.GroceryCartID == cartID);

         

            return _q.ToList();
        }

        public IEnumerable<vCartItem> GetvCartList(int cartID)
        {
            
            var idParam = new SqlParameter
            {
                ParameterName = "ID",
                Value = cartID
            };
            var results = _dc.Database.SqlQuery<vCartItem>(
                "pCartItems @ID",
            idParam).ToList<vCartItem>();

            return results.ToList();
        }


        public int AddIngredientsToGroceryCart(int recipeID, string userCD, string cartName)
        {
            int rc = 0;
            string sErr = "";
            var idParam1 = new SqlParameter
            {
                ParameterName = "RecipeID",
                Value = recipeID
            };
            var idParam2 = new SqlParameter
            {
                ParameterName = "UserCD",
                Value = userCD
            };
            var idParam3 = new SqlParameter
            {
                ParameterName = "Name",
                Value = cartName
            };

            try {
                rc = _dc.Database.ExecuteSqlCommand("spAddRecipeToCart @UserCD,@recipeid,@Name",
                  idParam1, idParam2, idParam3);
            } catch (Exception e)
            {
                sErr = e.Message;
            }


            return rc;


        }
        #endregion



        #region MEASURES



        public IList<UnitMeasure> UnitMeasureList()
        {
            IEnumerable<UnitMeasure> um = _dc.UnitMeasures;
            return um.ToList();

        }

        public string GetUnitMeasureName(int id) {
           
            IQueryable<UnitMeasure> um = _dc.UnitMeasures;
            if (id >= 1)
              um = um.Where(s => s.ID == id);

            string rc = um.FirstOrDefault().Abbrev.ToString();

            return rc;

        }

        public decimal GetFractionDecimalAmount(int id)
        {
            mFraction f = _dc.mFractions.Find(id);
            decimal rc = 0;
            if (f != null) rc = f.num; 
            return rc;

        }


        public string GetUserCdByRecipeID(int id)
        {
            string rc = "";
            IQueryable<Recipe> recipe = _dc.Recipes;
            if (id >= 1)
                recipe = recipe.Where(s => s.ID == id);
            rc = recipe.FirstOrDefault().UserCd;

            return rc;
        }

        #endregion

        #region FEATURES


        public FeatureView GetFeatured(int id)
        {

            var rc = new FeatureView();

            {
                rc = _dc.Features.Where(x=>x.ID== id)
                    .Select( t => new FeatureView
                    {
                      Title = t.Title,
                      FeatureText = t.FeatureText,
                      ID = t.ID
                    }  ).FirstOrDefault();

             };

            return rc;

        }


        public FeatureView GetFeaturedByCategory(int category, DateTime? dt)
        {
            DateTime seedDt = DateTime.Today;

            if (dt != null) { seedDt = (DateTime)dt; } 

            var idParam = new SqlParameter
            {
                ParameterName = "Category",
                Value = category
            };

            var idParam2 = new SqlParameter
            {
                ParameterName = "seeddt",
                Value = seedDt != null ? seedDt : SqlDateTime.Null
            };

            FeatureView rc = _dc.Database.SqlQuery<FeatureView>(
             "spGetFeaturesByCategory @Category, @seeddt ", idParam, idParam2
             ).FirstOrDefault();

            
 

            return rc;

        }


        #endregion

    }


}
