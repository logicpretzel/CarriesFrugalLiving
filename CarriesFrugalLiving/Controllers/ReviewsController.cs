using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarriesFrugalLiving.Models;
using CarriesFrugalLiving.DAL;
using Microsoft.AspNet.Identity;

namespace CarriesFrugalLiving.Controllers
{
    /// <summary>
    /// ReviewsController
    /// Description: Reviews Controller Class
    /// 
    /// Author: Dar Dunham
    /// Date: 3/1/16
    /// Revised: 4/3/16
    /// </summary>
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecipeRepository _dc = new RecipeRepository();

        // GET: Reviews
        public ActionResult Index(string userCD = "",int rating = 0,int recipeID = 0,int offensive = 0, string kw = "")
        {
            if (User.Identity.IsAuthenticated == false) {
                return RedirectToAction("Login", "Account");
            } else
            {
                if (!User.IsInRole("Admin"))
                {
                    
                    TempData["Msg"] = "You need to be and administrator to access this function."; 
                    return RedirectToAction("NeedLogon", "Home", null);
                }
            }
            var model = _dc.GetReviewList(userCD, rating, offensive, recipeID, kw);
            
            return View(model);
        }

        // POST: Reviews
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
          
            

            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (!User.IsInRole("Admin"))
                {

                    TempData["Msg"] = "You need to be and administrator to access this function.";
                    return RedirectToAction("NeedLogon", "Home", null);
                }
            }

            string userCD = "", kw = "";
            int rating = 0, recipeID = 0, offensive = 0;

            kw = fc["kw"] != null ? fc["kw"] : "";

            if (!int.TryParse(fc["rating"].ToString(), out rating))
            {
                rating = 0;
            }

            if (!int.TryParse(fc["recipeid"].ToString(), out recipeID))
            {
                recipeID = 0;
            }
            var model = _dc.GetReviewList( userCD, rating,  offensive, recipeID, kw);

            return View(model);
        }

       

        // GET: Reviews/Details/5
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "Admin, Reviewer")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Reviewer")]
        public async Task<ActionResult> Create([Bind(Include = "ID,RecipeID,ReviewerDisplayName,ReviewText,Rating,TriedRecipe,IsUser,UserCd,UserID, IsValidated,Offensive,ReportedByUserCD")] Review review)
        {
            if (ModelState.IsValid)
            {
                var userID = User.Identity.GetUserId();
                review.UserID = userID;
                review.UserCd = User.Identity.GetUserName();
                db.Reviews.Add(review);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "Admin, Reviewer, Moderator")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Reviewer, Moderator")]
        public async Task<ActionResult> Edit([Bind(Include = "ID,RecipeID,ReviewerDisplayName,ReviewText,Rating,TriedRecipe,IsUser,UserCd,IsValidated,Offensive,ReportedByUserCD")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "Admin, Reviewer, Moderator")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = await db.Reviews.FindAsync(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Reviewer, Moderator")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Review review = await db.Reviews.FindAsync(id);
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
