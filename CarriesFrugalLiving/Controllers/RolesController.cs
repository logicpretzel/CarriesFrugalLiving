using CarriesFrugalLiving.DAL;
using CarriesFrugalLiving.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace CarriesFrugalLiving.Controllers
{
    /// <summary>
    /// RolesController
    /// Description: Manage User Roles Controller Class
    /// 
    /// Author: Dar Dunham
    /// Date: 3/1/16
    /// Revised: 4/3/16
    /// </summary>
    public class RolesController : Controller
    {
        RecipeRepository _rp = new RecipeRepository();
        ApplicationDbContext context = new ApplicationDbContext();

        
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
        //




        // GET: /Roles/
        public ActionResult Index()
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            var roles = context.Roles.ToList();

            return View(roles);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            return View();
        }



        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (IsAdmin() == false) { return NotAdmin(); }


            try
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }



        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        //
        // GET: /Roles/Delete/5
        public ActionResult Delete(string RoleName)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }



        public ActionResult Manage()
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            var ulist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
            ViewBag.UserName = ulist;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            string mEmail = "dardunham@live.com";

            var manager = System.Web.HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var adminUser = manager.FindByEmail(mEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    
                    EmailConfirmed = true,
                    FirstName = "Dar",
                    LastName = "Dunham",
                    UserName = mEmail,
                    Email = mEmail
                };
                manager.Create(adminUser, "I81ou812");
            }
            // adding roles to the user if necessary 

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string roleName = "Admin";
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
            
            if (!manager.IsInRole(adminUser.Id, "Admin"))
            {
                manager.AddToRole(adminUser.Id, "Admin");
                context.SaveChanges();
            }

            //manager.AddToRole(user.Id, RoleName);
            if (_rp.AddRoleToUser(user.Id, RoleName))
            {
                ViewBag.ResultMessage = "Role created successfully !";
            }
            // prepopulat roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            var ulist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
            ViewBag.UserName = ulist;


            return View("Manage");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var account = new AccountController();

                ViewBag.RolesForThisUser = account.UserManager.GetRoles(user.Id);

                // prepopulat roles for the view dropdown
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                var ulist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
                ViewBag.UserName = ulist;

            }

            return View("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            if (IsAdmin() == false) { return NotAdmin(); }

            var account = new AccountController();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (account.UserManager.IsInRole(user.Id, RoleName))
            {
                account.UserManager.RemoveFromRole(user.Id, RoleName);
                ViewBag.ResultMessage = "Role removed from this user successfully !";
            }
            else
            {
                ViewBag.ResultMessage = "This user doesn't belong to selected role.";
            }
            // prepopulat roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            var ulist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
            ViewBag.UserName = ulist;

            return View("Manage");
        }

    }
}