using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using ReksadanaRekon.Models.Master;
using ReksadanaRekon.Models.Data;
using ReksadanaRekon.Models.Trans;

namespace ReksadanaRekon.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Nama Lengkap")]
        public string Nama { get; set; }

        //[Required]
        //[Display(Name = "Kelompok")]
        //public string Kelompok { get; set; }

        [Required]
        [Display(Name = "NPP")]
        public string NPP { get; set; }

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
        public DbSet<Kelompok> Kelompok { get; set; }

        //Model Transaction Data
        public DbSet<Transaction> Transaction { get; set; }
        //Subscription
        public DbSet<DataAplikasi> DataAplikasi { get; set; }
        public DbSet<DataAplikasiPasif> DataAplikasiPasif { get; set; }
        public DbSet<DataFund> DataFund { get; set; }
        public DbSet<DataFundPasif> DataFundPasif { get; set; }
        //Redemption & Switch
        public DbSet<DataAplikasiRed> DataAplikasiRed { get; set; }
        public DbSet<DataRedemp> DataRedemp { get; set; }
        public DbSet<DataFundRed> DataFundRed { get; set; }
        //Retur
        public DbSet<DataRetur> DataRetur { get; set; }


        //Model Master Data
        public DbSet<Matching> Matching { get; set; }
        public DbSet<SA> SA { get; set; }
        public DbSet<Fund> Fund { get; set; }
        public DbSet<FundType> FundType { get; set; }
        public DbSet<MI> MI { get; set; }
        public DbSet<Rekening> Rekening { get; set; }
        public DbSet<Bank> Bank { get; set; }

        //Model Transaksi
        public DbSet<Transaksi> Transaksi { get; set; }
        public DbSet<TransaksiPasif> TransaksiPasif { get; set; }
        public DbSet<TrDataAplikasiPasif> TrDataAplikasiPasif { get; set; }
        public DbSet<TrDataAplikasi> TrDataAplikasi { get; set; }
        public DbSet<TrDataFundPasif> TrDataFundPasif { get; set; }
        public DbSet<TrDataFund> TrDataFund { get; set; }

        public DbSet<TransRedemp> TransRedemp { get; set; }
        public DbSet<TrRedAplikasi> TrRedAplikasi { get; set; }
        public DbSet<TrRedFund> TrRedFund { get; set; }

        public DbSet<TrDataRetur> TrDataRetur { get; set; }


        public ApplicationDbContext()
            : base("ReksadanaRekon", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}