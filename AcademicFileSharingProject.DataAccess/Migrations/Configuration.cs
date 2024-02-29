//namespace AcademicFileSharingProject.DataAccess.Migrations
//{
//    using AcademicFileSharingProject.Core.ExtensionsMethods;
//    using AcademicFileSharingProject.Entities;
//    using System;
//    using Microsoft.EntityFrameworkCore;
//    using System.Data.Entity.Migrations;
//    using System.Data.Entity.Migrations.Model;
//    using System.Linq;

//    internal sealed class Configuration : DbMigrationsConfiguration<AcademicFileSharingProject.DataAccess.EntityFramework.DatabaseContext>
//    {
//        public Configuration()
//        {
//            AutomaticMigrationsEnabled = false;
//        }

//        protected override void Seed(AcademicFileSharingProject.DataAccess.EntityFramework.DatabaseContext context)
//        {
//            //  This method will be called after migrating to the latest version.

//            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
//            //  to avoid creating duplicate seed data.


//            if(context.Users.Any())
//            {
//                return;
//            }

//            //Add Admin
//            var adminUser = new UserEntity
//            {
//                CreatedTime = DateTime.Now,
//                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Cupiditate, excepturi. ",
//                Email = "halilcinar1260@gmail.com",
//                Email2 = "halilcinar1260@outlook.com",
//                IsDeleted = false,
//                Name = "Admin",
//                PhoneNumber = "05516788915",
//                Surname = "Admin",
//                Title = "System Admin"

//            };

//            context.Users.Add(adminUser);
//            var salt = ExtensionsMethods.GenerateRandomString(64);
//            var adminIdentity = new IdentityEntity
//            {
//                IsDeleted = false,
//                CreatedTime = DateTime.Now,
//                IsValid = true,
//                UserId = adminUser.Id,
//                PasswordSalt = salt,
//                PasswordHash = ExtensionsMethods.CalculateMD5Hash(salt + "12345" + salt)

//            };
//            context.Identities.Add(adminIdentity);
//            var adminRole = new UserRoleEntity
//            {
//                CreatedTime = DateTime.Now,
//                IsDeleted = false,
//                Role = Entities.Enums.ERoles.Admin,
//                UserId = adminUser.Id,

//            };
//            context.UserRoles.Add(adminRole);
//            context.SaveChanges();
//        }
//    }
//}
