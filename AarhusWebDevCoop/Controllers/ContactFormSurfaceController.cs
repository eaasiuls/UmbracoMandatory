using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using AarhusWebDevCoop.ViewModels;
using System.Net.Mail;

namespace AarhusWebDevCoop.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        // GET: ContactFormSurface
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            //Adding to Umbraco
            IContent comment = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "comment");
            comment.SetValue("email", model.Email);
            comment.SetValue("cname", model.Name);
            comment.SetValue("subject", model.Subject);
            comment.SetValue("message", model.Message);
            Services.ContentService.Save(comment);

            if (!ModelState.IsValid){ return CurrentUmbracoPage(); }

            MailMessage message = new MailMessage();
            message.To.Add("eaasiuls@students.eaaa.dk");
            message.Subject = model.Subject;
            message.From = new MailAddress(model.Email, model.Name);
            message.Body = model.Message + "\n my email is: " + model.Email;

            TempData["success"] = true;

            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = "smtp.gmail.com"; smtp.Port = 587;
              //  smtp.Credentials = new System.Net.NetworkCredential("", "");
                smtp.EnableSsl = true;

                smtp.Send(message);
            }


            return RedirectToCurrentUmbracoPage();
        }
    }
}