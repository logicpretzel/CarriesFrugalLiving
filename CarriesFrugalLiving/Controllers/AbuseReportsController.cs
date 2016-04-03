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

namespace CarriesFrugalLiving.Controllers
{
    public class AbuseReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AbuseReports
        public async Task<ActionResult> Index()
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
            return View(await db.AbuseReports.ToListAsync());
        }

        // GET: AbuseReports/Details/5
        public async Task<ActionResult> Details(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AbuseReport abuseReport = await db.AbuseReports.FindAsync(id);
            if (abuseReport == null)
            {
                return HttpNotFound();
            }
            return View(abuseReport);
        }

        // GET: AbuseReports/Create
        public ActionResult Create()
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
            return View();
        }

        // POST: AbuseReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,RecipeID,ReviewID,UserCD,CreateDate,AbuseType,Comment")] AbuseReport abuseReport)
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
            if (ModelState.IsValid)
            {
                db.AbuseReports.Add(abuseReport);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(abuseReport);
        }

        // GET: AbuseReports/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AbuseReport abuseReport = await db.AbuseReports.FindAsync(id);
            if (abuseReport == null)
            {
                return HttpNotFound();
            }
            return View(abuseReport);
        }

        // POST: AbuseReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,RecipeID,ReviewID,UserCD,CreateDate,AbuseType,Comment")] AbuseReport abuseReport)
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
            if (ModelState.IsValid)
            {
                db.Entry(abuseReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(abuseReport);
        }

        // GET: AbuseReports/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AbuseReport abuseReport = await db.AbuseReports.FindAsync(id);
            if (abuseReport == null)
            {
                return HttpNotFound();
            }
            return View(abuseReport);
        }

        // POST: AbuseReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
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
            AbuseReport abuseReport = await db.AbuseReports.FindAsync(id);
            db.AbuseReports.Remove(abuseReport);
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
