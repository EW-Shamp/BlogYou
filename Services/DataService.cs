using BlogYou.Data;
using BlogYou.Enums;
using BlogYou.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogYou.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;
        public DataService(ApplicationDbContext dbContext,
                            RoleManager<IdentityRole> roleManager,
                            UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            //Make Database from Migrations
            await _dbContext.Database.MigrateAsync();

            //Seeding Roles into the system
            await SeedRolesAsync();

            //Seed Users into the system
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //If already roles in system DO NOTHING
            if(_dbContext.Roles.Any())
            {
                return;
            }
            //Otherwise create roles
            foreach(var role in Enum.GetNames(typeof(BlogRole)))
            {
                //use Role Manager to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
           

            //If already users in system DO NOTHING
            if (_dbContext.Users.Any())
            {
                return;
            }

            //Otherwise create users

            //Admin User
            //Step 1: Creates a new instance of blog user
            var adminUser = new BlogUser()
            {
                Email = "ew_shamp@post.com",
                UserName = "ew_shamp@post.com",
                PhoneNumber = "(800) 555-1212",
                FirstName = "Ad",
                LastName = "MIN",
                EmailConfirmed = true,
            };

            //Step 2: Use UserManager to create a new user that is defined by adminUser
            await _userManager.CreateAsync(adminUser, "Abc&123!");

            //Step 3: Add this user to administrator role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            //Moderator User
            //Step 1: creates new instance of blog user
            var modUser = new BlogUser()
            {
                Email = "moderator@moderator.com",
                UserName = "Moderator@moderator.com",
                FirstName = "Mod",
                LastName = "Erator",
                EmailConfirmed = true
            };

            //Step 2: Use UserManager to create a new user that is defined by modUser
            await _userManager.CreateAsync(modUser, "Abc&123!");

            //Step 3: Add this user to modUser role
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());


            // User
            //Step 1: creates new instance of blog user
            var userUser = new BlogUser()
            {
                Email = "user@user.com",
                UserName = "user@user.com",
                FirstName = "Use",
                LastName = "Ere",
                EmailConfirmed = true
            };

            //Step 2: Use UserManager to create a new user that is defined by modUser
            await _userManager.CreateAsync(userUser, "Abc&123!");

            //Step 3: Add this user to modUser role
            await _userManager.AddToRoleAsync(userUser, BlogRole.User.ToString());
        }
    }
}
