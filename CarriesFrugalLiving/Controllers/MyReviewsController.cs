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
    public class MyReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecipeRepository _dc = new RecipeRepository();



        private ActionResult AccessError()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["Msg"] = "Access Not Authorized.  You must be the owner of this review in order to edit or delete.";
                return RedirectToAction("NeedLogon", "Home", null);

            }
        }

        // GET: MyReviews

        [Authorize(Roles = "Admin, Reviewer, Moderator, Contributer")]
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var model = _dc.SelectReviewsForUser(userID);
            //
            return View(model);

        }


        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.Reviews.ToListAsync());
        //}

        // GET: MyReviews/Details/5
        [Authorize(Roles = "Admin, Reviewer, Moderator, Contributer")]
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
            var userID = User.Identity.GetUserId();
            if (userID != review.UserID) return AccessError();

            return View(review);
        }


        // GET: MyReviews/Edit/5
        [Authorize(Roles = "Admin, Reviewer, Moderator, Contributer")]
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
            var userID = User.Identity.GetUserId();
            if (userID != review.UserID) return AccessError();
            return View(review);
        }

        // POST: MyReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Reviewer, Moderator, Contributer")]
        public async Task<ActionResult> Edit([Bind(Include = "ID,RecipeID,ReviewerDisplayName,ReviewText,Rating,TriedRecipe")] Review review)
        {
            if (ModelState.IsValid)
            {

                db.Entry(review).State = EntityState.Modified;

                if (review.ReviewerDisplayName == null) review.ReviewerDisplayName = "Anon.";
                if (review.UserCd == null) review.UserCd = User.Identity.GetUserName();
                if (review.UserID == null) review.UserID = User.Identity.GetUserId();
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(review);
        }

        // GET: MyReviews/Delete/5
        [Authorize(Roles = "Admin, Reviewer, Moderator, Contributer")]
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
            var userID = User.Identity.GetUserId();
            if (userID != review.UserID) return AccessError();
            return View(review);
        }

        // POST: MyReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Reviewer, Moderator, Contributer")]
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
