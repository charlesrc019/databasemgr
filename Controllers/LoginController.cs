using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using DatabaseManager.Models;

namespace DatabaseManager.Controllers
{
    public class LoginController : Controller
    {
        // VIEWS //
        [AllowAnonymous]
        public virtual ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "View");
            else
                return View();
        }

        // FUNCTIONS //
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Index(LoginSubmission loginSubmission)
        {
            // Validate HTML form.
            if (!ModelState.IsValid)
                return View(loginSubmission);

            // Process login.
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            var idService = new IdentityService(authenticationManager);
            var authenticationResult = idService.SignIn(loginSubmission.Username, loginSubmission.Password);
            if (authenticationResult.IsSuccess)
                return RedirectToAction("Index", "View");

            // Process errors.
            System.Web.HttpContext.Current.Session["StatusMessage"] = authenticationResult.ErrorMessage;
            return View(loginSubmission);
        }

        [ValidateAntiForgeryToken]
        public virtual ActionResult Logoff()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(Authentication.ApplicationCookie);

            return RedirectToAction("Index");
        }
    }
}