using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace CarriesFrugalLiving.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        
        [Display(Name = "Last Name")]
        [MaxLength(50)]
        public string LastName { get; set; }

        
        [Display(Name = "First Name")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        [MaxLength(50)]
        public string City { get; set; }
       
        [Display(Name = "State")]
        [MaxLength(50)]
        public string State { get; set; }

        [Display(Name = "Zip")]
        [MaxLength(10)]
        public string Zip { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }



        
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<UnitMeasure> UnitMeasures { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<mFraction> mFractions { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<GroceryCart> GroceryCarts { get; set; }

        public DbSet<MealPlan> MealPlans { get; set; }

        public DbSet<GCartItem> GCartItems { get; set; }

        public DbSet<AbuseReport> AbuseReports { get; set; }
        public DbSet<Category> Categories { get; set; }

        // public System.Data.Entity.DbSet<CarriesFrugalLiving.ViewModels.UserList> UserLists { get; set; }

        //   public System.Data.Entity.DbSet<CarriesFrugalLiving.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}