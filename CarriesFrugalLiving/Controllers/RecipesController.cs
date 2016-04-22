using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarriesFrugalLiving.DAL;
using CarriesFrugalLiving.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace CarriesFrugalLiving.Controllers
{
    /// <summary>
    /// RecipesController
    /// Description: Recipes Controller Class
    /// 
    /// Author: Dar Dunham
    /// Date: 3/1/16
    /// Revised: 4/3/16
    /// </summary>
    public class RecipesController : Controller
    {
        private const int MAXPICWIDTH = 300;
        private const int MAXPICHEIGHT = 500;


        private RecipeRepository _db = new RecipeRepository();
        private ApplicationDbContext db = new ApplicationDbContext();
 
        // GET: Recipes
        public ActionResult Index()
        {
            var model = db.Recipes.ToList();
            ViewBag.TotalRecipes = model.Count().ToString("#,##0");
            return View(model);
        }


        #region RECIPE_DETAILS
        // GET: Recipes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }
            Session["LastRecipeID"] = (int)id;
            ViewBag.RatingString = _db.GetReviewDisplayString((int)id);
            ViewBag.ReviewCount = _db.GetReviewCount((int)id);
            ViewBag.AverageRating = _db.GetAverageRating((int)id).ToString("#,##0.0##");
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            
            return View(recipe);
        }
        #endregion

        #region PRINT_RECIPE
        public ActionResult RecipePrint(int? id)
        {
            if (id == null || id == 0)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }
            Session["LastRecipeID"] = (int)id;
            ViewBag.RatingString = _db.GetReviewDisplayString((int)id);
            ViewBag.ReviewCount = _db.GetReviewCount((int)id);
            ViewBag.AverageRating = _db.GetAverageRating((int)id).ToString("#,##0.0##");
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }

            return View(recipe);
        }
        #endregion

        #region LIST_RECIPES

        [AllowAnonymous]
        public ActionResult List(string scategory = "")
        {
            int i = -1;
            Recipe.eCategory category = Recipe.eCategory.NONE;
            List<Recipe> model ;
            

            if (scategory != "")
            {
                if (int.TryParse(scategory, out i))
                {
                    if (Enum.IsDefined(typeof(Recipe.eCategory), i))
                    {
                        category = (Recipe.eCategory)i;
                        model = (List<Recipe>)_db.SelectRecipesByCategory(category);
                    }
                    else {
                        model = (List<Recipe>)_db.SelectRecipes("(nullcategory)");
                    }

                }
                else {

                    if (Enum.TryParse<Recipe.eCategory>(scategory, out category))
                    {
                        model = (List<Recipe>)_db.SelectRecipesByCategory(category);
                    }
                    else {
                        model = (List<Recipe>)_db.SelectRecipes("(nullcategory)");
                        //
                    }
                }
            }
            else {
                model = (List<Recipe>)_db.SelectRecipes("");
            }
            ViewBag.TotalRecipes = model.Count();
            return View(model);
        }

        string swatch = "";

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult List(FormCollection fc)
        {
            string kw = fc["kw"].ToString();
            kw = utils.HtmlEncodeDecode.Encode(kw);
            swatch = kw;
            var model = _db.SelectRecipes(kw);

            ViewBag.TotalRecipes = model.Count();
            return View(model);
        }

        #endregion


        #region CREATE_RECIPE


        // GET: Recipes/Create
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult Create([Bind(Include = "Name,ShortDescription,UserCd")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                recipe.UserId = User.Identity.GetUserId();
                recipe.UserCd = User.Identity.GetUserName();
                db.Recipes.Add(recipe);
                db.SaveChanges();

                int id = recipe.ID;
                
                return RedirectToAction("Edit", new { id = id });
            }

            return View(recipe);
        }
        #endregion

        #region CARTS

        /// <summary>
        /// AddToCart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult AddToCart(int? recipeID) {
           
            if (recipeID == null)
            {
                return RedirectToAction("Index");
            }
            else {
                string userID = User.Identity.GetUserId();
                var model = _db.GetIngredientViewList((int)recipeID);
                Session["LastRecipeID"] = (int)recipeID;
                ViewBag.RecipeID = (int)recipeID;
                ViewBag.GroceryCarts = new SelectList(_db.GetGroceryCarts(userID), "ID", "Name");
                return View(model); 
            }

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult AddToCart(FormCollection fc)
        {

            int recipeID = 0;
            int cartID = 0;

            if (!int.TryParse(fc["recipeID"].ToString(), out recipeID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (recipeID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            if (!int.TryParse(fc["CartID"].ToString(), out cartID))
            {
                cartID = 0;
            }
            var cartName = _db.GetCartName(cartID);
            if (cartName == null) {
                cartName = "Default";
            }

            string userID = User.Identity.GetUserId();
            

            Session["LastRecipeID"] = (int)recipeID;
            ViewBag.RecipeID = (int)recipeID;

           

            _db.AddIngredientsToGroceryCart(recipeID, userID, cartName);

            return RedirectToAction("MyGroceryList", "GroceryCarts", new { id = cartID });


         

        }


        // GET: GroceryCarts/Create
        [Authorize]
        public ActionResult CreateNewCart()
        {
            
            return View();
        }

        // POST: GroceryCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateNewCart([Bind(Include = "Email,UserID,Name,CreateDate")] GroceryCart groceryCart)
        {
            int recipeID = 0;
            if (Session["LastRecipeID"] != null) {
                recipeID = (int)Session["LastRecipeID"];
            }

            if (ModelState.IsValid)
            {

                groceryCart.Email = User.Identity.Name;  
                groceryCart.UserID = User.Identity.GetUserId();
                groceryCart.CreateDate = DateTime.Today;


                db.GroceryCarts.Add(groceryCart);
                db.SaveChanges();
                if (recipeID > 0)
                {
                    return RedirectToAction("AddToCart", "Recipes", new { recipeID = recipeID });
                } else
                {
                    return RedirectToAction("List", "Recipes");

                }
            }

            return View(groceryCart);
        }

        #endregion

        #region EDIT_RECIPE

        // GET: Recipes/Edit/5
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");

            }
            Session["LastRecipeID"] = (int)id;
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public ActionResult Edit([Bind(Include = "ID,Name,ShortDescription,Category,IngredientString,Preparation,CookingInstructions,CookingMethod,ServingInstructions,NutritionInformation")] Recipe recipe)



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult Edit(FormCollection fc)
        {

            int id = 0;
            if (!int.TryParse(fc["id"].ToString(), out id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session["LastRecipeID"] = (int)id;
            var _model = db.Recipes.Find(id);

            if (TryUpdateModel(_model, "",
               new string[] { "Name", "ShortDescription"
               , "Preparation","CookingInstructions", "CookingMethod", "ServingInstructions"
               , "NutritionInformation", "Category"
               }))
            {
                try
                {
                    db.SaveChanges();
                    if (fc["addingredient"] == "Add Ingredient")
                    {
                        return RedirectToAction("EditIngredient", new { id = id });
                    }
                    else {
                        return RedirectToAction("Details", new { id = id });
                    }
                }
                catch (DataException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(_model);
        }

        #endregion

        #region DELETE_RECIPE

        // GET: Recipes/Delete/5
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult DeleteConfirmed(int id)
        {
            Session["LastRecipeID"] = 0;
            Recipe recipe = db.Recipes.Find(id);

            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        #endregion


        #region IMAGE_PROCESSING

  
       

        public ActionResult Picture(int? recipeId) {
            int id = 0;
            
            if (recipeId != null ){
                Image image = new Image();
                int rId = (int)recipeId;
                ViewBag.UserCd = _db.GetUserCdByRecipeID(rId);

                id = _db.GetImageIdFromRecipeID(rId);
                if (id >= 1) {
                    image = _db.GetImage(id);

                } else
                {
                    image.RecipeID = recipeId;
                }
                 
                 return View(image);
             }       
             else return null ;
        }

        //iMAGE UPLOAD
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult Upload(int id)
        {
            if (db.Recipes.Find(id) == null)
            {
                return new HttpNotFoundResult("Recipe not found");
            }
            ViewBag.RecipeID = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult Upload(Image image)
        {
            // Apply Validation Here
            int? recipeid = image.RecipeID;

            if (recipeid == null) recipeid = 0;
            // remove images if exist 
            _db.RemoveImagesForRecipe((int)recipeid);
            ViewBag.UserCd = _db.GetUserCdByRecipeID((int)recipeid);
            if (image.File.ContentLength > (3 * 1024 * 1024))
            {
                ModelState.AddModelError("CustomError", "File size must be less than 3 MB");
                return View();
            }
            if (!(image.File.ContentType == "image/jpeg" || image.File.ContentType == "image/gif" || image.File.ContentType == "image/png"))
            {
                ModelState.AddModelError("CustomError", "Only png, jpeg and gif file types are allowed!");
                return View();
            }

            image.FileName = image.File.FileName;
            image.Size = image.File.ContentLength;
            image.ContentType = image.File.ContentType;
            

            byte[] data = new byte[image.File.ContentLength];
            image.File.InputStream.Read(data, 0, image.File.ContentLength);


            int w = 0;
            int h = 0;

            utils.StaticImageResizer.GetDimensions(data, out w, out h);

            if (w > MAXPICWIDTH || h > MAXPICHEIGHT) {
                //desired max width = 400, want to scale proportionally
                if (w > MAXPICWIDTH)
                {
                    decimal nHeight = MAXPICWIDTH * ((decimal)h / w);
                    //call resize 

                    data = utils.StaticImageResizer.GetScaledDownByteArray(data, MAXPICWIDTH, (int)nHeight);
                }
                else if (h > MAXPICHEIGHT) {
                    decimal nWidth = MAXPICHEIGHT * ((decimal)w / h);
                    //call resize 

                    data = utils.StaticImageResizer.GetScaledDownByteArray(data, (int)nWidth, MAXPICHEIGHT);

                }
            }

            image.ImageData = data;
            db.Images.Add(image);
            db.SaveChanges();
            
            return RedirectToAction("Details", new { id = recipeid } ); // if recipeid = 0 will return to list

        }
        #endregion


        #region INGREDIENTS



        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult UpdateIngredient(int? id)
        {
            
            if (id == null)
            {
                //  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");

            }

            Ingredient model  = db.Ingredients.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            

            ViewBag.UMList = new SelectList(db.UnitMeasures, "ID", "Description", model.UnitsID);

            ViewBag.Fractions = new SelectList(db.mFractions, "id", "text", model.qtyFraction);



            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult UpdateIngredient(FormCollection fc)
        {
            string sErr = "";

            int id;
            if (!int.TryParse(fc["id"].ToString(), out id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var _model = db.Ingredients.Find(id);

            if (TryUpdateModel(_model, "",
               new string[] { "RecipeID", "Description"
               , "UnitsID","qtyWhole", "qtyFraction"
               }))
            {
                try
                {
                    db.SaveChanges();
                   
                }
                catch (DataException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

                try
                {
                   
                    return RedirectToAction("Details", new { id = _model.RecipeID });
                   
                } catch (Exception e)
                {
                    sErr = e.Message;
                }
            }

            return View(_model);

        }

        /// <summary>
        /// Add Ingredient
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult AddIngredient(int? id) {

            Ingredient ingredient = new Ingredient();
            if (id == null)
            {
                //  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");

            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ingredient.RecipeID = (int)id;

            ViewBag.UMList = new SelectList(db.UnitMeasures, "ID", "Description", ingredient.UnitsID);

            ViewBag.Fractions = new SelectList(db.mFractions, "id", "text", ingredient.qtyFraction);

           

            return View(ingredient);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult AddIngredient([Bind(Include = "RecipeID,Description,UnitsID,qtyFraction,qtyWhole")] Ingredient ingredient)
        {

            //TODO: create save recipe in repository
            if (ingredient.RecipeID <= 0) {
                return HttpNotFound();
            }

            Recipe recipe = db.Recipes.Find(ingredient.RecipeID);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                ingredient.Quantity = (decimal)(ingredient.qtyWhole) + _db.GetFractionDecimalAmount(ingredient.qtyFraction);
                db.Ingredients.Add(ingredient);
                db.SaveChanges();
                ingredient.sortorder = ingredient.ID;
                db.SaveChanges();
               // return RedirectToAction("Details", new { id = ingredient.RecipeID });
            }

            ViewBag.UMList = new SelectList(db.UnitMeasures, "ID", "Description", ingredient.UnitsID);

            ViewBag.Fractions = new SelectList(db.mFractions, "id", "text", ingredient.qtyFraction);



            return View(ingredient);

            //return RedirectToAction("Details", new { id = ingredient.RecipeID });
        }

        /// <summary>
        /// Add Ingredient
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult EditIngredient(int? id)
        {

            Ingredient ingredient = new Ingredient();
            if (id == null)
            {
                //  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");

            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ingredient.RecipeID = (int)id;

            ViewBag.UMList = new SelectList(db.UnitMeasures, "ID", "Description", ingredient.UnitsID);

            ViewBag.Fractions = new SelectList(db.mFractions, "id", "text", ingredient.qtyFraction);

          

            return View(ingredient);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult EditIngredient([Bind(Include = "RecipeID,Description,UnitsID,qtyFraction,qtyWhole")] Ingredient ingredient)
        {

            //TODO: create save recipe in repository
            if (ingredient.RecipeID <= 0)
            {
                return HttpNotFound();
            }

            Recipe recipe = db.Recipes.Find(ingredient.RecipeID);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                ingredient.Quantity = (decimal)(ingredient.qtyWhole) + _db.GetFractionDecimalAmount(ingredient.qtyFraction);
                db.Ingredients.Add(ingredient);

                db.SaveChanges();
                ingredient.sortorder = ingredient.ID;
                db.SaveChanges();
                // return RedirectToAction("Details", new { id = ingredient.RecipeID });
            }

            ViewBag.UMList = new SelectList(db.UnitMeasures, "ID", "Description", ingredient.UnitsID);

            ViewBag.Fractions = new SelectList(db.mFractions, "id", "text", ingredient.qtyFraction);



            return View(ingredient);

            //return RedirectToAction("Details", new { id = ingredient.RecipeID });
        }

        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult MoveIngredientUp (int id)
        {
            string sErr = "";
            Ingredient ingredient =  db.Ingredients.Find(id);
            int recipeID = 0;
            if (ingredient != null)
            {
                recipeID = ingredient.RecipeID;
               sErr =  _db.IngredientMove(id, recipeID, 0);

            }
            var model = _db.GetIngredientViewList(recipeID);
            if (model != null)
            {
                ViewBag.RecipeID = recipeID;
                ViewBag.ERR = sErr;
                return PartialView("IngredientList", model);
            }
            else {
                return null;
            }
        }


        [Authorize(Roles = "Admin, Contributer")]
        public ActionResult MoveIngredientDn(int id)
        {
            string sErr = "";
            Ingredient ingredient = db.Ingredients.Find(id);
            int recipeID = 0;
            if (ingredient != null)
            {
                
                recipeID = ingredient.RecipeID;
                sErr =  _db.IngredientMove(id, recipeID, 1);

            }
            var model = _db.GetIngredientViewList(recipeID);
            if (model != null)
            {
                ViewBag.RecipeID = recipeID;
                ViewBag.ERR = sErr;
                return PartialView("IngredientList", model);
            }
            else {
                return null;
            }
        }

        [Authorize(Roles = "Admin, Contributer")]
        public async Task<ActionResult> RemoveIngredient(int id)
        {
            Ingredient ingredient = await db.Ingredients.FindAsync(id);
            int recipeID = 0;
            if (ingredient != null)
            {
                recipeID = ingredient.RecipeID;
                db.Ingredients.Remove(ingredient);
                var x = await db.SaveChangesAsync();
            }
            var model = _db.GetIngredientViewList(recipeID);
            if (model != null)
            {
                ViewBag.RecipeID = recipeID;

                return PartialView("IngredientList", model);
            }
            else {
                return null;
            }
        }

       
        public  ActionResult IngredientList(int id)
        {
            var model = _db.GetIngredientViewList(id);
            if (model != null)
            {
                ViewBag.RecipeID = id;

                return PartialView("IngredientList", model);
            }
            else {
                return null;
            }
        }

       
        public ActionResult IngredientDetails(int id)
        {
            var model = _db.GetIngredientViewList(id);
            // var model = _db.GetRecipeByID(id);

            ViewBag.RecipeID = id;
            return PartialView(model);
        }




        #endregion

        #region REVIEWS

        public ActionResult ReviewList(int id)
        {
            var model = _db.SelectReviews(id);
            ViewBag.TotalReviews = _db.GetReviewCount(id);
            ViewBag.AverageRating = _db.GetAverageRating(id).ToString("#,##0.0##");

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ReviewList(FormCollection fc)
        {
            int id = 0;
            string sID = fc["sfRecipeID"] == null ? "" : fc["sfRecipeID"];
            string sRating = fc["sfRating"] == null ? "" : fc["sfRating"];

            Review.eReviewStarRating eRating = Review.eReviewStarRating.NONE;

            if (!int.TryParse(sID, out id))
            {
                id = 0;
            }

            else {
                if (!Enum.TryParse<Review.eReviewStarRating>(sRating, out eRating))
                {
                    eRating = Review.eReviewStarRating.NONE;
                }
            }

            var model = _db.SelectReviews(id);

            if (eRating != Review.eReviewStarRating.NONE)
            {
                model = model.Where(s => s.Rating == eRating);
            }

            ViewBag.TotalReviews = _db.GetReviewCount(id);
            ViewBag.AverageRating = _db.GetAverageRating(id).ToString("#,##0.0##");

            return PartialView(model);
        }

        //Reviews
        [Authorize(Roles = "Admin, Reviewer, Contributer, Moderator")]
        public ActionResult AddReview(int? id) {
            if (id == null)
            {
                //  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");

            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            Review review = new Review();
            review.RecipeID = (int)id;
            return View(review);
            
        }

        [HttpPost, ActionName("AddReview")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Reviewer, Contributer, Moderator")]
        public ActionResult AddReview([Bind(Include = "ReviewerDisplayName,ReviewText,Rating,TriedRecipe,RecipeID")] Review review)
        {

            var userID = User.Identity.GetUserId();
            review.UserID = userID;
            review.UserCd = User.Identity.GetUserName();
            if (ModelState.IsValid)
            {
                 
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = review.RecipeID });
            }

            return View(review);
        }


        //Reviews
        [Authorize(Roles = "Admin, Reviewer, Contributer, Moderator")]
        public ActionResult ReportReview(int? id)
        {
            if (id == null)
            {
                //  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");

            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }

            AbuseReport abuseReport = new AbuseReport();
            abuseReport.RecipeID = (int)review.RecipeID;
            abuseReport.ReviewID = (int)id;
            abuseReport.UserCD = User.Identity.GetUserId();
            abuseReport.CreateDate = DateTime.Now;
            abuseReport.AbuseType = AbuseReport.eAbuseType.REVIEW;

            return View(abuseReport);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Reviewer, Contributer, Moderator")]
        public async Task<ActionResult> ReportReview([Bind(Include = "ID,RecipeID,ReviewID,UserCD,CreateDate,AbuseType,Comment")] AbuseReport abuseReport)
                {
                    if (User.Identity.IsAuthenticated == false)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    
                    if (ModelState.IsValid)
                    {
                        abuseReport.UserCD = User.Identity.GetUserId(); //prevent forgery
                        db.AbuseReports.Add(abuseReport);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = abuseReport.RecipeID });
                    }

                    return View(abuseReport);
                }


        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }


    

}


//To-Do:
/*
Confirmation Email for Registrants
Check Subscribed/Confirmed prior to allowing add/edit.
Add report this link to disable content

*/


/*
//RESEARCH:
//http://www.asp.net/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
//https://msdn.microsoft.com/en-us/library/bb351562.aspx
//http://codetunnel.io/should-you-return-iqueryablet-from-your-repositories/

//https://www.youtube.com/watch?v=QJJSnUVX6PA

//http://patrickdesjardins.com/blog/how-to-bind-sql-view-to-an-entity-framework-object

//https://msdn.microsoft.com/en-us/library/ms229862(v=vs.100).aspx#findingthecorrectversion
//http://bitoftech.net/2015/01/21/asp-net-identity-2-with-asp-net-web-api-2-accounts-management/
//https://msdn.microsoft.com/en-us/library/ee358769(v=vs.100).aspx
//http://johnatten.com/2013/11/11/extending-identity-accounts-and-implementing-role-based-authentication-in-asp-net-mvc-5/#Simplifying-AccountController---Remove-the-Clutter
//http://stackoverflow.com/questions/263486/how-to-get-current-user-in-asp-net-mvc
//http://www.asp.net/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#overpost
//http://www.c-sharpcorner.com/UploadFile/b696c4/how-to-upload-and-display-image-in-mvc/
//
//http://www.mikesdotnetting.com/article/268/how-to-send-email-in-asp-net-mvc
//http://stackoverflow.com/questions/23612614/mvc-5-identity-2-0-registration-email-confirmation-issue
//http://www.c-sharpcorner.com/UploadFile/sourabh_mishra1/sending-an-e-mail-using-Asp-Net-mvc/
//http://www.asp.net/mvc/overview/security/create-an-aspnet-mvc-5-web-app-with-email-confirmation-and-password-reset

//
//http://www.codeproject.com/Tips/738090/ASP-NET-MVC-Confirm-Registration-Email
//
//http://rateit.codeplex.com/
//http://stackoverflow.com/questions/10608488/how-do-insert-row-into-child-table

//Complex types (stored procrdures)
    //https://msdn.microsoft.com/en-us/data/jj691402.aspx
    //https://blogs.msdn.microsoft.com/diego/2012/01/09/stored-procedures-with-output-parameters-using-sqlquery-in-the-dbcontext-api/

    //AJAX and partial views
    //https://cmatskas.com/update-an-mvc-partial-view-with-ajax/

    //Image Stuff http://stackoverflow.com/questions/8946846/converting-a-byte-array-to-png-jpg
    //http://www.radioactivethinking.com/rateit/example/example.htm
//Email
//http://www.rtur.net/blog/post/2007/10/12/SMTP-with-GoDaddy


    //https://www.simple-talk.com/dotnet/asp.net/revisiting-partial-view-rendering-in-asp.net-mvc/

    //TODO: JSON to flot
    http://stackoverflow.com/questions/23604504/asp-net-mvc-passing-json-to-view-from-controller
        // by Se0ng11
        //
        //    //Controller
        //   [httpPost]
        //    public JsonResult something(string userGuid)
        //    {
        //        var p = GetUserProducts(userGuid);
        //        return Json(p, JsonRequestBehavior.AllowGet);
        //    }
        //
        //   //call with ajax
        //$.post( "../something", {userGuid: "foo"}, function( data ) {
        //  console.log(data)
        //});

    //http://www.pikemere.co.uk/blog/flot-how-to-create-pie-charts/

//http://stackoverflow.com/questions/4907422/asp-net-mvc-get-current-host
//
//User Security Stamp
//http://stackoverflow.com/questions/19487322/what-is-asp-net-identitys-iusersecuritystampstoretuser-interface
//good tutorial site
//http://www.tutorialsteacher.com/linq/linq-sorting-operators-orderby-orderbydescending
*/
