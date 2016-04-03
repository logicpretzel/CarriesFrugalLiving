using CarriesFrugalLiving.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CarriesFrugalLiving.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult NeedLogon() {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ContactEmail model = new ContactEmail();
            ViewBag.Message = "Your contact page.";

            return View(model);
        }

        // POST: 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact([Bind(Include = "Name,Description,Email, Phone")] ContactEmail model)
        {
            
            if (ModelState.IsValid)
            {

                try
                {
                  //  utils.EmailSender eSender = new utils.EmailSender();
                   
                    if (model.Phone == null) model.Phone = "none";
                    if (model.Description == null) model.Description = "none";

                    string sBody = "<b>Feedback from CarriesFrugal Living.</b>"
                            + "<br /><b><i>Summary:</i></b><hr/>" 
                            + HttpUtility.HtmlEncode(model.Description).Replace(Environment.NewLine, "<br />")
                            + "<hr/><br/>Name: " 
                            + HttpUtility.HtmlEncode(model.Name )
                            + "<br/><br/>Phone: " 
                            + HttpUtility.HtmlEncode(model.Phone)
                             + "<br/><br/>Email: "
                            + HttpUtility.HtmlEncode(model.Email)
                            ;

                    string subj = "Notification from CarriesFrugalLiving.com";

                    var es = new EmailService();
                    var msg = new IdentityMessage();

                    msg.Destination = "dar@ccssllc.com";
                    msg.Body = sBody;
                    msg.Subject = subj;
                    await es.SendAsync(msg);

             

                    return RedirectToAction("ContactResponse");


                }
                catch (Exception emailException) {
                    ViewBag.ErrMsg = emailException.Message;
                }
                
            }
            //something went wrong


            return View(model);
        }

        public ActionResult ContactResponse() {
            return View();
        }


    }
}