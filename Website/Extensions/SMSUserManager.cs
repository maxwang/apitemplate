using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Website.Extensions
{
    public class SMSUserManager<TUser> : UserManager<ApplicationUser>
    {
        public SMSUserManager(IUserStore<ApplicationUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<ApplicationUser> passwordHasher, 
            IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<ApplicationUser>> logger) : 
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public override async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.RemoveFromRolesAsync(user, roles);
        }

        public override async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.AddToRolesAsync(user, roles);
        }

        public override async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if(string.IsNullOrEmpty(user?.Id))
            {
                throw new ArgumentException("User could not be empty");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.GetUserRolesAsync(user.Id);
        }


        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return base.CreateAsync(user, password);
        }
        

        public async Task<IdentityResult> RestPasswordAndForceChangeAsync(string userId, string password)
        {
            var user = await FindByIdAsync(userId);
            if(user == null)
            {
                return IdentityResult.Failed(new IdentityError[] {
                    new IdentityError
                    {
                        Code = "100001",
                        Description = "Could not find user"
                    }
                });
            }
            user.PasswordHash = PasswordHasher.HashPassword(user, password);
            //user.LastPasswordChangedDate = user.CreationDate;
            return await UpdateAsync(user);
        }
        
        public override async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            if (user == null || string.IsNullOrEmpty(user.Id) || string.IsNullOrEmpty(role))
            {
                throw new ArgumentException("User Id and role name is required fields");
            }

            var uStore = Store as SMSUserStore<ApplicationUser>;
            return await uStore.IsInRoleAsync(user.Id, role);
        }

        public override async Task<IdentityResult> AddPasswordAsync(ApplicationUser user, string password)
        {
            var result = await base.AddPasswordAsync(user, password);
            
            if (result.Succeeded)
            {
                var uStore = Store as SMSUserStore<ApplicationUser>;
                
                uStore.Context.SaveChanges();
            }

            return result;

        }
        public override async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {  
            var result =  await base.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                var uStore = Store as SMSUserStore<ApplicationUser>;
                
                uStore.Context.SaveChanges();
            }

            return result;
        }


    }
}
