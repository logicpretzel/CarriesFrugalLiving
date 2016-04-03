using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CarriesFrugalLiving.Models;
using System.Collections;

namespace CarriesFrugalLiving.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? System.Web.HttpContext.Current.Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); // HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }

            // get; private set;

        }




        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        public ActionResult Index() {
            ApplicationDbContext db = new ApplicationDbContext();
            var model = UserManager.Users.ToList();
            //ApplicationUser usr = db.Users.Find(userID);
            //IEnumerable<ApplicationUsers> model = db.Users.ToList();
            return View(model);
        }



        public ActionResult UserProfile() {

            var userID = User.Identity.GetUserId();
            
            var model = new ProfileViewModel();
            if (userID == null)
            {
                // Don't reveal that the user does not exist
                //return RedirectToAction("Index", "Home");  //could also go here...
               
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            else
            {
                //TODO: Put this in repository
                ApplicationDbContext db = new ApplicationDbContext();
                var usr = db.Users.Find(userID);
                model.Email =usr.Email;
                model.FirstName = usr.FirstName;
                model.LastName = usr.LastName;
                model.Address = usr.Address;
                model.City = usr.City;
                model.State = usr.State;
                model.Zip = usr.Zip;
                db.Dispose();
                return View(model);
            };
           // return RedirectToAction("Index", "Home");
        }



        //
        // POST: /Account/UserProfile
        [HttpPost]
         [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.City = model.City;
                    user.State = model.State;
                    user.Zip = model.Zip;
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    //TODO: Check result ....

                    return RedirectToAction("Index", "Home");
                };
                
              
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        public ActionResult Edit(string Id) {
            var model =  UserManager.FindById(Id);
            if (model == null )
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }
            return View(model);

        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserName, Email, FirstName, LastName, Address, City, State, Zip, EmailConfirmed,PhoneNumber,PhoneNumberConfirmed, LockoutEnabled, AccessFailedCount")]ApplicationUser model)
        {
            //if (ModelState.IsValid)
            //{
            //    //var user = new ApplicationUser
            //{
            //    UserName = model.Email,
            //    Email = model.Email,
            //    FirstName = model.FirstName,
            //    LastName = model.LastName,
            //    Address = model.Address,
            //    City = model.City,
            //    State = model.State,
            //    Zip = model.Zip
            //};

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.City = model.City;
                    user.State = model.State;
                    user.Zip = model.Zip;
                    user.EmailConfirmed = model.EmailConfirmed;
                    user.PhoneNumber = model.PhoneNumber;
                    user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
                    user.LockoutEnabled = model.LockoutEnabled;
                    user.AccessFailedCount = model.AccessFailedCount;

                    IdentityResult result = await UserManager.UpdateAsync(user);

                    if (result.Succeeded == true ) {
                        
                        
                        utils.EmailSender eSender = new utils.EmailSender();

                        string sBody = eSender.GetNotifyMsgBody("Your account has changed for Carries Frugal Living website (CarriesFrugalLiving.com)"
                            , "If you received this message unexpectedly please contact us. For security reasons we will not provide a link, but simply access the main site and click Contact Us. Thank you.");
                        var e = eSender.Send(user.Email, "Account changed at CarriesFrugalLiving.com", sBody, true, null);
                        eSender = null;
                        ViewBag.ErrMsg = e;
                    }
                  
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Delete(string id)
        {
            var user = UserManager.FindById(id);
            return View(user);
        }
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete([Bind(Include = "UserName, Email")]ApplicationUser model)
        {

            if (ModelState.IsValid)
            {
              

                var user = await UserManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    user = await UserManager.FindByNameAsync(model.Email);
                    if (user == null) return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {

                    string email = user.Email;
                    // IdentityResult result = await UserManager.UpdateAsync(user);
                    IdentityResult result = await UserManager.DeleteAsync(user);
                    if (result.Succeeded == true)
                    {

                       // email = "dar@ccssllc.com";
                        utils.EmailSender eSender = new utils.EmailSender();
                        string subject = String.Format("Account {0} has been removed for CarriesFrugalLiving.com", email);

                        string sBody = eSender.GetNotifyMsgBody(subject
                            , "If you received this message unexpectedly please contact us. For security reasons we will not provide a link, but simply access the main site and click Contact Us. Thank you.");
                        var msg = eSender.Send(email, subject, sBody, true, null);
                        eSender = null;
                        ViewBag.ErrMsg = msg;
                    }

                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

         // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind(Include="Email, Password, FirstName, LastName, Address, City, State, Zip")]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email,
                        FirstName =model.FirstName, LastName=model.LastName, Address=model.Address,
                        City = model.City, State=model.State, Zip=model.Zip };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                     string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                     var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                     var es = new EmailService();
                     var msg = new IdentityMessage();

                    
                    msg.Destination = model.Email;
                   
                    msg.Subject = "Carries Frugal Living Account Confirmation";
                    msg.Body = "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>";
                    await es.SendAsync( msg );

                    msg = null;
                    es = null;

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register([Bind(Include="Email, Password, FirstName, LastName, Address, City, State, Zip")]RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email,
        //                FirstName =model.FirstName, LastName=model.LastName, Address=model.Address,
        //                City = model.City, State=model.State, Zip=model.Zip };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                 string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                 var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //  await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                var es = new EmailService();
                var msg = new IdentityMessage();

                //TODO: Uncomment;
                msg.Destination = model.Email;
                
                msg.Subject = "Carries Frugal Living Password Reset Confirmation";

                string sbody = "<table style=\"Margin:0;background:#f3f3f3;border-collapse:collapse;border-spacing:0;color:#0a0a0a;font-family:Candara,Arial,sans-serif;font-size:16px;font-weight:400;height:100%;line-height:19px;margin:0;padding:0;text-align:left;vertical-align:top;width:100%\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><td class=\"center\" align=\"center\" valign=\"top\" style =\"-moz-hyphens:auto;-webkit-hyphens:auto;Margin:0;border-collapse:collapse!important;color:#0a0a0a;font-size:16px;font-weight:400;hyphens:auto;line-height:19px;margin:0;padding:0;text-align:left;vertical-align:top;word-wrap:break-word\" ><center><table style=\"Margin:0 auto;background:#fefefe;border-collapse:collapse;border-spacing:0;margin:0 auto;padding:0;text-align:center;vertical-align:top;width:580px\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><td style=\"-moz-hyphens:auto;-webkit-hyphens:auto;Margin:0;border-collapse:collapse!important;color:#0a0a0a;font-size:16px;font-weight:400;hyphens:auto;line-height:19px;margin:0;padding:0;text-align:left;vertical-align:top;word-wrap:break-word\"><table style=\"border-collapse:collapse;border-spacing:0;display:table;padding:0;position:relative;text-align:left;vertical-align:top;width:100%\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><th style=\"Margin:0 auto;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0 auto;padding:0;padding-bottom:0;padding-left:16px;padding-right:16px;text-align:left;width:564px\"><table style=\"border-collapse:collapse;border-spacing:0;padding:0;text-align:left;vertical-align:top;width:100%\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><th style=\"Margin:0;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0;padding:0;text-align:left\"><h4 style=\"Margin:0;Margin-bottom:10px;color:inherit;font-size:24px;font-weight:400;line-height:1.3;margin:0;margin-bottom:10px;padding:0;text-align:center;word-wrap:normal\">Carrie's Frugal Living</h4></th><th style=\"Margin:0;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0;padding:0!important;text-align:left;visibility:hidden;width:0\"></th></tr></tbody></table></th></tr></tbody></table><table style=\"border-collapse:collapse;border-spacing:0;display:table;padding:0;position:relative;text-align:left;vertical-align:top;width:100%\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><th  style=\"Margin:0 auto;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0 auto;padding:0;padding-bottom:16px;padding-left:16px;padding-right:16px;text-align:left;width:564px\"><table style=\"border-collapse:collapse;border-spacing:0;padding:0;text-align:left;vertical-align:top;width:100%\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><th style=\"Margin:0;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0;padding:0;text-align:left\"><center style=\"min-width:532px;width:100%\"><img src=\"http://www.carriesfrugalliving.com/images/forgot_password.png\" align=\"center\"  style=\"-ms-interpolation-mode:bicubic;Margin:0 auto;clear:both;display:block;float:none;margin:0 auto;max-width:100%;outline:0;text-align:center;text-decoration:none;width:auto\"></center><h1 style=\"Margin:0;Margin-bottom:10px;color:inherit;font-size:34px;font-weight:400;line-height:1.3;margin:0;margin-bottom:10px;padding:0;text-align:center;word-wrap:normal\">Forgot Your Password?</h1><p  style=\"Margin:0;Margin-bottom:10px;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0;margin-bottom:10px;padding:0;text-align:center\">It happens. Click the link below to reset your password.</p><table class=\"button large expand\" style=\"Margin:0 0 16px 0;border-collapse:collapse;border-spacing:0;margin:0 0 16px 0;padding:0;text-align:left;vertical-align:top;width:100%!important\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><td style=\"-moz-hyphens:auto;-webkit-hyphens:auto;Margin:0;border-collapse:collapse!important;color:#0a0a0a;font-size:16px;font-weight:400;hyphens:auto;line-height:19px;margin:0;padding:0;text-align:left;vertical-align:top;word-wrap:break-word\"><table style=\"border-collapse:collapse;border-spacing:0;padding:0;text-align:left;vertical-align:top;width:100%\"><tbody><tr style=\"padding:0;text-align:left;vertical-align:top\"><td style=\"-moz-hyphens:auto;-webkit-hyphens:auto;Margin:0;background:#2199e8;border:2px solid #2199e8;border-collapse:collapse!important;color:#fefefe;font-size:16px;font-weight:400;hyphens:auto;line-height:19px;margin:0;padding:0;text-align:left;vertical-align:top;width:auto!important;word-wrap:break-word\"><center data-parsed=\"\" style=\"min-width:0;width:100%\">";
                sbody += "<a href=\"" + callbackUrl;
                sbody += "\" align=\"center\" style=\"Margin:0;border:0 solid #2199e8;border-radius:3px;color:#fefefe;display:inline-block;font-size:20px;font-weight:700;line-height:1.3;margin:0;padding:10px 20px 10px 20px;text-align:center;text-decoration:none;width:calc(100%-20px)\">";
                sbody += "Reset Password";
                sbody += "</a></center></td><td  style=\"-moz-hyphens:auto;-webkit-hyphens:auto;Margin:0;background:#2199e8;border:2px solid #2199e8;border-collapse:collapse!important;color:#fefefe;font-size:16px;font-weight:400;hyphens:auto;line-height:19px;margin:0;padding:0!important;text-align:left;vertical-align:top;visibility:hidden;width:auto!important;word-wrap:break-word\"></td></tr></tbody></table></td></tr></tbody></table><hr style=\"Margin:20px auto;border-bottom:1px solid #cacaca;border-left:0;border-right:0;border-top:0;clear:both;height:0;margin:20px auto;max-width:580px\"><p style=\"Margin:0;Margin-bottom:10px;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0;margin-bottom:10px;padding:0;text-align:left\"><small style=\"color:#cacaca;font-size:80%\">You're getting this email because you've signed up for email updates. If you want to opt-out of future emails, <a href=\"http://www.carriesfrugalliving.com/home/unsubscribe\" style=\"Margin:0;color:#2199e8;font-weight:400;line-height:1.3;margin:0;padding:0;text-align:left;text-decoration:none\">unsubscribe here</a>.</small></p></th><th class=\"expander\" style=\"Margin:0;color:#0a0a0a;font-size:16px;font-weight:400;line-height:19px;margin:0;padding:0!important;text-align:left;visibility:hidden;width:0\"></th></tr></tbody></table></th></tr></tbody></table></td></tr></tbody></table></center></td></tr></tbody></table>";

                msg.Body = sbody;

                await es.SendAsync(msg);

                msg = null;
                es = null;

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}