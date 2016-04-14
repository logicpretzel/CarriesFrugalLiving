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

namespace CarriesFrugalLiving.Controllers
{
    public class PickListsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecipeRepository _db = new RecipeRepository();

        // GET: PickLists
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            return View(await db.PickLists.OrderBy(r=>r.CatID).ThenBy(r=>r.KeyID).ToListAsync());
        }

        // GET: PickLists/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickList pickList = await db.PickLists.FindAsync(id);
            if (pickList == null)
            {
                return HttpNotFound();
            }
            return View(pickList);
        }

        // GET: PickLists/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            int cat = 0; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");
            return View();
        }

        // POST: PickLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,KeyID,CatID,Value,Description,Void")] PickList pickList)
        {
            if (ModelState.IsValid)
            {
                db.PickLists.Add(pickList);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            int cat = 0; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");
            return View(pickList);
        }

        // GET: PickLists/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickList pickList = await db.PickLists.FindAsync(id);
            if (pickList == null)
            {
                return HttpNotFound();
            }
            int cat = 0; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");
            return View(pickList);
        }

        // POST: PickLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,KeyID,CatID,Value,Description,Void")] PickList pickList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pickList).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            int cat = 0; // See GetPickList Documentation
            ViewBag.ddCat = new SelectList(_db.GetPickList(cat), "KeyID", "Value");
            return View(pickList);
        }

        // GET: PickLists/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickList pickList = await db.PickLists.FindAsync(id);
            if (pickList == null)
            {
                return HttpNotFound();
            }
            return View(pickList);
        }

        // POST: PickLists/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PickList pickList = await db.PickLists.FindAsync(id);
            db.PickLists.Remove(pickList);
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
