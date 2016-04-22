using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarriesFrugalLiving.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using CarriesFrugalLiving.DAL;
using PagedList;

namespace CarriesFrugalLiving.Controllers
{
    /// <summary>
    /// GroceryCartsController
    /// Description: Grocery Carts Controller Class
    /// 
    /// Author: Dar Dunham
    /// Date: 3/1/16
    /// Revised: 4/3/16
    /// </summary>
    public class GroceryCartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecipeRepository _dc = new RecipeRepository();



        // GET: GroceryCarts
        public ActionResult Index(string sortOrder = "", int? page = 1)
        {

            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }
            IEnumerable<GroceryCart> model;

            if (User.IsInRole("Admin"))
            {
                 model = _dc.GetGroceryCarts().ToList();
                
            }
            else {
                var userCD = User.Identity.GetUserId();
                if (userCD != null)
                {
                     model = _dc.GetGroceryCarts(userCD).ToList();
                    
                } else
                {
                    //Should never get here
                    return HttpNotFound();
                }
            }

            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.EmailSortParm =  String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";

            switch (sortOrder)
            {
                case "name_desc":
                    model = model.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    model = model.OrderBy(s => s.Name);
                    break;
                case "Date":
                    model = model.OrderBy(s => s.CreateDate);
                    break;
                case "date_desc":
                    model = model.OrderByDescending(s => s.CreateDate);
                    break;
                case "email_desc":
                    model = model.OrderByDescending(s => s.Email);
                    break;
                default:
                    model = model.OrderBy(s => s.Email);
                    break;
            }
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));

           // return View(model);

        }

        // GET: GroceryCarts/Details/5
        public ActionResult Details(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == null)
            {
                return RedirectToAction("Index");
            }

            GroceryCart groceryCart = new GroceryCart();


            if (User.IsInRole("Admin"))
            {
                groceryCart = _dc.GetGroceryCart((int)id);
                
            }
            else {
                var userCD = User.Identity.GetUserId();
                if (userCD != null)
                {
                    groceryCart = _dc.GetGroceryCart((int)id);
                    if (userCD != groceryCart.UserID)
                    {
                        groceryCart = null;
                    }
                    
                }
                else
                {
                    //Should never get here
                    return HttpNotFound();
                }
            }

            
            if (groceryCart == null)
            {
                return HttpNotFound();
            }
            return View(groceryCart);
        }

        public ActionResult GCartItemList(int? id) //cart id
        {
            if (id == null) return null;

            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            IEnumerable<GCartItem> model   = _dc.GetGroceryList((int)id);

            if (!User.IsInRole("Admin"))
            {
              
                var userCD = User.Identity.GetUserId();
                var cartOwnerID = _dc.GetCartOwnerID((int)id);
                if (userCD != null)
                {
                    
                    if (userCD != cartOwnerID)
                    {
                        model = null;
                        return null;
                    }

                }
                else
                {
                    //Should never get here
                    return HttpNotFound();
                }
            }

            

            return View(model);
        }


        public ActionResult AddGroceryItem(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == null) {
                RedirectToAction("Index");
            }

            var model = new GCartItem();

            model.GroceryCartID = (int)id;

            return View(model);
        }
        // POST: GroceryCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult AddGroceryItem([Bind(Include = "GroceryCartID, Description")] GCartItem gcartItem)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (ModelState.IsValid)
                {
                    db.GCartItems.Add(gcartItem);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = gcartItem.GroceryCartID });
            }
               
            
            // sumpin went wrong
            return View(gcartItem);
        }

        // GET: GroceryCarts/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }


            return View();
            
        }

        // POST: GroceryCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CreateDate,Name,Email")] GroceryCart groceryCart)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }


            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var user = await UserManager.FindByEmailAsync(groceryCart.Email);
                groceryCart.UserID = user.Id;
                if (user != null)
                {
                    if (ModelState.IsValid)
                    {
                        db.GroceryCarts.Add(groceryCart);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    UserManager.Dispose();
                }
                // sumpin went wrong
                return View(groceryCart);
           
        }

        // GET: GroceryCarts/Create
        public ActionResult CreateCart()
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }
            return View();
        }

        // POST: GroceryCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCart([Bind(Include = "email,UserCD,Name")] GroceryCart groceryCart)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (ModelState.IsValid)
            {
              
                groceryCart.UserID = User.Identity.GetUserId();
                groceryCart.CreateDate = DateTime.Today;

                db.GroceryCarts.Add(groceryCart);
                db.SaveChanges();
                return RedirectToAction("Index","Recipes");
            }

            return View(groceryCart);
        }

        // GET: GroceryCarts/Edit/5
        public ActionResult EditCartItem(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GCartItem groceryCartItem = db.GCartItems.Find(id);
            if (groceryCartItem == null)
            {
                return HttpNotFound();
            }
            return View(groceryCartItem);
        }

        // POST: GroceryCarts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCartItem([Bind(Include = "ID,GroceryCartID, Description")] GCartItem groceryCartItem)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (ModelState.IsValid)
            {
                db.Entry(groceryCartItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = groceryCartItem.GroceryCartID });
            }
            return View(groceryCartItem);
        }



        // GET: GroceryCarts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroceryCart groceryCart = db.GroceryCarts.Find(id);
            if (groceryCart == null)
            {
                return HttpNotFound();
            }
            return View(groceryCart);
        }

        // POST: GroceryCarts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection fc)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            int id = 0;
            if (!int.TryParse(fc["ID"].ToString(), out id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var _model = db.GroceryCarts.Find(id);

            if (TryUpdateModel(_model, "",
               new string[] { "Name"  }))
            {
                try
                {
                    db.SaveChanges();
                   
                    return RedirectToAction("Index");
                    
                }
                catch (DataException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            //Something went wrong
            return View(_model);

            
        }

        // GET: GroceryCarts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroceryCart groceryCart = db.GroceryCarts.Find(id);
            if (groceryCart == null)
            {
                return HttpNotFound();
            }
            return View(groceryCart);
        }

        // POST: GroceryCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            GroceryCart groceryCart = db.GroceryCarts.Find(id);
            db.GroceryCarts.Remove(groceryCart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /*
        [HttpGet]
public async Task<ActionResult> GetCategoryProducts(string categoryId)  
{
    var lookupId = int.Parse(categoryId);
    var model = await this.GetFullAndPartialViewModel(lookupId);
    return PartialView("CategoryResults", model);
}
            */
        [HttpGet]
        public async Task<ActionResult>  DeleteCartItem(int id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GCartItem model = await db.GCartItems.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            else {
                int cartID = model.GroceryCartID;
                db.GCartItems.Remove(model);
                int x = await db.SaveChangesAsync();
                var mdl  = _dc.GetGroceryList(cartID);
                return PartialView("GCartItemList", mdl);
            }

            
        }


        public ActionResult MyGroceryList(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == null)
            {
                return RedirectToAction("Index");
            }
            GroceryCart groceryCart = db.GroceryCarts.Find(id);
            if (groceryCart == null)
            {
                return HttpNotFound();
            }
            return View(groceryCart);
        }


        public ActionResult MyGroceryListItems(int? id) //cart id
        {
            if (User.Identity.IsAuthenticated == false)
            {

                return RedirectToAction("NeedLogon", "Home");
            }

            if (id == null) return null;
            // var model = _dc.GetGroceryList((int)id);
            var model = _dc.GetvCartList((int)id);
            return View(model);
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
