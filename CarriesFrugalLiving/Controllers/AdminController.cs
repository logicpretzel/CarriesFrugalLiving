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
using System.Web.Helpers;

namespace CarriesFrugalLiving.Controllers
{
    /// <summary>
    /// Admin Controller Class
    /// Author: Dar Dunham
    /// Date: 3/1/16
    /// Revised: 4/3/16
    /// </summary>
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecipeRepository _db = new RecipeRepository();

        //  ******************************************************************/
        //  ADMIN VALIDATION PROCEDURES
        //  ******************************************************************/

        private ActionResult NotAdmin()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["Msg"] = "You need to be and administrator to access this function.";
                return RedirectToAction("NeedLogon", "Home", null);

            }
        }

        private bool IsAdmin()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return false;
            }

            if (User.IsInRole("Admin"))
            {
                return true;
            }
            return false;
        }
        //  END - ADMIN VALIDATION PROCEDURES

// TODO: Extend getrecipedemos to include additional dashboard items.
/// <summary>
/// Method: Index
/// Author: Dar Dunham
/// Model passed is for dashboard items. 
/// Must have the Admin role to access.
/// 
/// </summary>
/// <returns>Recipe Demo View Model</returns>
        public ActionResult Index() {
            if (IsAdmin() == false) { return NotAdmin(); }

            var model = _db.GetRecipeDemos();
            return View(model);

        }


        // GET: UnitMeasures
        public ActionResult UMList()
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            return View(db.UnitMeasures.ToList());
        }

        // GET: UnitMeasures/Details/5
        public ActionResult UMDetails(int? id)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

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
            if (IsAdmin() == false) { return NotAdmin(); }
            return View();
        }

        // POST: UnitMeasures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UMCreate([Bind(Include = "ID,TypeCd,Description,Abbrev,EquivalentValue,EquivalentMeasure")] UnitMeasure unitMeasure)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

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
            if (IsAdmin() == false) { return NotAdmin(); }

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
            if (IsAdmin() == false) { return NotAdmin(); }

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
            if (IsAdmin() == false) { return NotAdmin(); }

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
            if (IsAdmin() == false) { return NotAdmin(); }

            UnitMeasure unitMeasure = db.UnitMeasures.Find(id);
            db.UnitMeasures.Remove(unitMeasure);
            db.SaveChanges();
            return RedirectToAction("UMList");
        }


        /*  [************************************************************************] */
        /*  |           REPORTS, GRAPHS AND DASHBOARDS SECTION                       | */
        /*  [************************************************************************] */
        #region GRAPHS

       
        public ActionResult jsonRecipeTotals()
        {
            if (IsAdmin() == false) { return NotAdmin(); }
            var p = _db.GetRecipeCountsByCategory();
            var s = p.FirstOrDefault().label;
            return Json(p, JsonRequestBehavior.AllowGet); 
        }

        /*
        
        */
        public ReportResult UnitMeasureReport()
        {
            if (IsAdmin() == false) { return null; }
            // Get the data for the report (any IEnumerable will work)
            var query = db.UnitMeasures.ToList();


            var count = query.Count;



            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource());


            // Customize the Text Fields
            report.TextFields.Title = "Unit Measure Report";
            report.TextFields.SubTitle = "Unit Measure Listing - Full";
            report.TextFields.Footer = "Copyright 2016 &copy; Dar Dunham  (Carrie's Frugal Living)";
            report.TextFields.Header = string.Format(@"
             Report Generated: {0}
             Total Items: {1}
            ", DateTime.Now, count);


            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;


            // Customize the data fields
            report.DataFields["ID"].Hidden = false;
            report.DataFields["Description"].DataFormatString = "{0}";
            report.DataFields["TypeCd"].DataFormatString = "{0}";

            // Return the ReportResult
            // the type of report that is rendered will be determined by the extension in the URL (.pdf, .xls, .html, etc)
            //  return new ReportResult(report, new ExcelReportWriter(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileName = "Report.xls" };
            return new ReportResult(report);
        }

        public ReportResult UserListingReport()
        {
            if (IsAdmin() == false) { return null; }
            // Get the data for the report (any IEnumerable will work)
            var query = db.Users.ToList();


            var count = query.Count;

            
           
            // Create the report and turn our query into a ReportSource
            var report = new Report(query.ToReportSource());


            // Customize the Text Fields
            report.TextFields.Title = "User Report";
            report.TextFields.SubTitle = "User Listing - Full";
            report.TextFields.Footer = "Copyright 2016 &copy; Dar Dunham  (Carrie's Frugal Living)";
            report.TextFields.Header = string.Format(@"
             Report Generated: {0}
             Total Users: {1}
            ", DateTime.Now, count);


            // Render hints allow you to pass additional hints to the reports as they are being rendered
            report.RenderHints.BooleanCheckboxes = true;
            report.RenderHints.IncludePageNumbers = true;
            report.RenderHints.Orientation =  ReportOrientation.Landscape;
            report.RenderHints.Margins = new System.Drawing.SizeF(10, 10);
            

            // Customize the data fields
            report.DataFields["Logins"].Hidden = true;
            report.DataFields["Claims"].Hidden = true;
            report.DataFields["SecurityStamp"].Hidden = true;
            report.DataFields["PasswordHash"].Hidden = true;
            report.DataFields["Claims"].Hidden = true;
            report.DataFields["Roles"].Hidden = true;

            report.DataFields["FirstName"].DataFormatString = "{0}";
            report.DataFields["LastName"].DataFormatString = "{0}";

            // Return the ReportResult
            // the type of report that is rendered will be determined by the extension in the URL (.pdf, .xls, .html, etc)
            //  return new ReportResult(report, new ExcelReportWriter(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileName = "Report.xls" };
            return new ReportResult(report);
        }
        #endregion



        //dispose
        /// <summary>
        ///   Required by ContextDB - implements IDisposable
        /// </summary>
        /// <param name="disposing"></param>
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
