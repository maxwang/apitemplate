using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Website.Models;

namespace Website.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            builder.Entity<ApplicationUser>(au =>
            {
                au.HasKey(x => x.Id);

                //au.HasOne(c => c.MyCompany).WithMany(cu => cu.Users).HasForeignKey(ccu => ccu.CompanyId);

                au.HasMany(e => e.Roles)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                //au.HasMany(e => e.Claims)
                //    .WithOne()
                //    .HasForeignKey(e => e.UserId)
                //    .IsRequired()
                //    .OnDelete(DeleteBehavior.Cascade);

                //au.HasMany(e => e.Logins)
                //    .WithOne()
                //    .HasForeignKey(e => e.UserId)
                //    .IsRequired()
                //    .OnDelete(DeleteBehavior.Cascade);

                
                au.Property(c => c.CreationDate).IsRequired(true).HasDefaultValueSql("getdate()");

                builder.Entity<Resource>(r =>
                {
                    r.HasIndex(m => m.ModuleName).HasName("ix_aspnetresources_modulename");
                    r.HasIndex(m => m.ClaimType).HasName("ix_aspnetresources_claimtype");
                });
                
            });
        }
    }
}
