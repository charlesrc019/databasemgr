using System;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Security.Principal;


namespace DatabaseManager.Models
{
    // DATA STRUCTURES //
    public class LoginSubmission
    {
        [Required(ErrorMessage="Please enter your username.")]
        [AllowHtml]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [AllowHtml]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    // FUNCTIONS //
    public class IdentityService
    {
        // Variables.
        public class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null)
            { ErrorMessage = errorMessage; }

            public String ErrorMessage { get; private set; }
            public Boolean IsSuccess => String.IsNullOrEmpty(ErrorMessage);
        }
        private readonly IAuthenticationManager authenticationManager;
        public IdentityService(IAuthenticationManager authenticationManager)
        { this.authenticationManager = authenticationManager; }
        public string ADusername { get; set; }

        // Methods.
        public AuthenticationResult SignIn(String username, String password)
        {
            // Set the domain to authenticate against.
            PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, "LD");

            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;
            try
            {
                isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                if (isAuthenticated)
                    userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
            }
            catch (Exception)
            {
                isAuthenticated = false;
                userPrincipal = null;
            }

            // Catch errors.
            if (!isAuthenticated || userPrincipal == null)
                return new AuthenticationResult("Your username or password is incorrect.");
            if (userPrincipal.IsAccountLockedOut())
                return new AuthenticationResult("Your account is locked.");
            if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
                return new AuthenticationResult("Your account is disabled.");

            // Login information is correct. Verify that they have access.
            //GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(principalContext, "Devops-Infrastructure");
            //if (!userPrincipal.IsMemberOf(groupPrincipal))
            //    return new AuthenticationResult("Access denied.");

            var identity = CreateIdentity(userPrincipal);

            authenticationManager.SignOut(Authentication.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);

            return new AuthenticationResult();
        }
        private ClaimsIdentity CreateIdentity(UserPrincipal userPrincipal)
        {
            var identity = new ClaimsIdentity(Authentication.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Active Directory"));
            identity.AddClaim(new Claim(ClaimTypes.Name, userPrincipal.DisplayName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userPrincipal.SamAccountName));
            identity.AddClaim(new Claim("ADUsername", userPrincipal.SamAccountName));
            if (!String.IsNullOrEmpty(userPrincipal.EmailAddress))
                identity.AddClaim(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress));

            return identity;
        }
    }

    // EXTENSIONS //
    public static class IdentityExtensions
    {
        // Methods.
        public static string GetADUsername(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("ADUsername");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}