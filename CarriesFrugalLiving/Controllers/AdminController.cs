using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarriesFrugalLiving.Models;
using DoddleReport.Web;
using DoddleReport;
using CarriesFrugalLiving.DAL;

namespace CarriesFrugalLiving.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecipeRepository _db = new RecipeRepository();

        public ActionResult Index() {
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

            var model = _db.GetRecipeDemos();
            return View(model);

        }


        // GET: UnitMeasures
        public ActionResult UMList()
        {
            return View(db.UnitMeasures.ToList());
        }

        // GET: UnitMeasures/Details/5
        public ActionResult UMDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitMeasure unitMeasure = db.UnitMeasures.Find(id);
            if (unitMeasure == null)
            {
                return HttpNotFound();
            }
            return View(unitMeasure);
        }

        // GET: UnitMeasures/Create
        public ActionResult UMCreate()
        {
            return View();
        }

        // POST: UnitMeasures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UMCreate([Bind(Include = "ID,TypeCd,Description,Abbrev,EquivalentValue,EquivalentMeasure")] UnitMeasure unitMeasure)
        {
            if (ModelState.IsValid)
            {
                db.UnitMeasures.Add(unitMeasure);
                db.SaveChanges();
                return RedirectToAction("UMList");
            }

            return View(unitMeasure);
        }

        // GET: UnitMeasures/Edit/5
        public ActionResult UMEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitMeasure unitMeasure = db.UnitMeasures.Find(id);
            if (unitMeasure == null)
            {
                return HttpNotFound();
            }
            return View(unitMeasure);
        }

        // POST: UnitMeasures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UMEdit([Bind(Include = "ID,TypeCd,Description,Abbrev,EquivalentValue,EquivalentMeasure")] UnitMeasure unitMeasure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unitMeasure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unitMeasure);
        }

        // GET: UnitMeasures/Delete/5
        public ActionResult UMDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitMeasure unitMeasure = db.UnitMeasures.Find(id);
            if (unitMeasure == null)
            {
                return HttpNotFound();
            }
            return View(unitMeasure);
        }

        // POST: UnitMeasures/Delete/5
        [HttpPost, ActionName("UMDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnitMeasure unitMeasure = db.UnitMeasures.Find(id);
            db.UnitMeasures.Remove(unitMeasure);
            db.SaveChanges();
            return RedirectToAction("UMList");
        }

        public ReportResult UnitMeasureReport()
        {
            // Get the data for the report (any IEnumerable will work)
            var query = db.UnitMeasures.ToList();


            var totalProducts = query.Count;
           


            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource());
            

            // Customize the Text Fields
            report.TextFields.Title = "Unit Measure Report";
            report.TextFields.SubTitle = "This is a sample report showing how Doddle Report works";
            report.TextFields.Footer = "Copyright 2011 &copy; The Doddle Project";
            report.TextFields.Header = string.Format(@"
        Report Generated: {0}
        Total Products: {1}
       ", DateTime.Now, totalProducts);


            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;


            // Customize the data fields
            report.DataFields["ID"].Hidden = true;
            report.DataFields["Description"].DataFormatString = "{0}";
            report.DataFields["TypeCd"].DataFormatString = "{0}";

            // Return the ReportResult
            // the type of report that is rendered will be determined by the extension in the URL (.pdf, .xls, .html, etc)
            //  return new ReportResult(report, new ExcelReportWriter(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileName = "Report.xls" };
            return new ReportResult(report);
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
