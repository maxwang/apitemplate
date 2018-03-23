using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Website.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website.Data;

namespace Website.Extensions
{
    

    public class SMSUserStore<TUser> : UserStore<TUser>
        where TUser : ApplicationUser, new()
    {
        public SMSUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            
        }
        

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var aContext = Context as ApplicationDbContext;
            var results = from ur in aContext.UserRoles
                          join r in aContext.Roles on ur.RoleId equals r.Id
                          where ur.UserId == userId
                          select r.Name;

            return await results.ToListAsync(); 
        }

       
        public async Task<bool> IsInRoleAsync(string userId, string roleName)
        {
            var aContext = Context as ApplicationDbContext;
            var role = await aContext.Roles.FirstOrDefaultAsync(
                x => x.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));

            if (role != null)
            {
                return await aContext.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == role.Id);
            }

            return false;

        }
        
        

        public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roleNames)
        {
            var aContext = Context as ApplicationDbContext;
            foreach (var roleName in roleNames)
            {
                var role = await aContext.Roles.FirstOrDefaultAsync(x => x.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
                if (role != null)
                {
                    var userRole = await aContext.UserRoles.FirstOrDefaultAsync( x => x.UserId == user.Id && x.RoleId == role.Id);
                    if (userRole != null)
                    {
                        aContext.UserRoles.Remove(userRole);
                    }
                }
                
            }

            await aContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roleNames)
        {
            var aContext = Context as ApplicationDbContext;
            foreach (var roleName in roleNames)
            {
                var role = await aContext.Roles.SingleOrDefaultAsync(x => x.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
                var result = await aContext.UserRoles.AddAsync(new IdentityUserRole<string> { RoleId = role.Id, UserId = user.Id });

            }
            
            await aContext.SaveChangesAsync();
            return IdentityResult.Success;

        }
    }
}
