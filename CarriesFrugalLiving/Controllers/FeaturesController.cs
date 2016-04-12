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
using CarriesFrugalLiving.ViewModels;

namespace CarriesFrugalLiving.Controllers
{
    public class FeaturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecipeRepository _db = new RecipeRepository();
     


        // GET: Features
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Index()
        {
          
            return View(await db.Features.ToListAsync());
        }

        // GET: Features/Details/5
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            return View(feature);
        }

        [AllowAnonymous]
        public  ActionResult Featured(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeatureView model = _db.GetFeaturedByCategory((int)id,null);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult Intro(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeatureView model = _db.GetFeaturedByCategory((int)id, null);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }





        // GET: Features/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            int cat = 1; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");

            return View();
        }

        // POST: Features/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "ID,Title,FeatureText,StartDate,EndDate,Category,url")] Feature feature)
        {
            
            if (ModelState.IsValid)
            {
                db.Features.Add(feature);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            int cat = 1; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");
            return View(feature);
        }

        // GET: Features/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            int cat = 1; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");
            return View(feature);
        }

        // POST: Features/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Title,FeatureText,StartDate,EndDate,Category,url")] Feature feature)
        {
           
            if (ModelState.IsValid)
            {
                db.Entry(feature).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            int cat = 1; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");
            return View(feature);
        }

        // GET: Features/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature feature = await db.Features.FindAsync(id);
            if (feature == null)
            {
                return HttpNotFound();
            }
            return View(feature);
        }

        // POST: Features/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            
            Feature feature = await db.Features.FindAsync(id);
            db.Features.Remove(feature);
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
