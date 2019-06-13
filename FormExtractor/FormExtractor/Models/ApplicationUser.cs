using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FormExtractor.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual ApplicationUserInfo ApplicationUserInfo { get; set; }
    }

    public class ApplicationUserInfo
    {
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Company")]
        public string Company { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Vendor Number")]
        public string VendorNumber { get; set; }
    }

}