using AcademicFileSharingProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.EntityFramework
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext():base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=.;initial catalog=AcademicFileSharingProjectDB;integrated security=True"
           );
            base.OnConfiguring(optionsBuilder);

        }

        public DbSet<ChatEntity> Chats { get; set; }
        public DbSet<ChatUserEntity> ChatUsers { get; set; }
        public DbSet<PostCommentEntity> PostComments { get; set; }
        public DbSet<IdentityEntity> Identities { get; set; }
        public DbSet<MediaEntity> Medias { get; set; }
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<PostMediaEntity> PostMedias { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<BlogEntity> Blogs { get; set; }
        public DbSet<BlogCommentEntity> BlogComments { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<SubscribeEntity> Subscribes { get; set; }
        public DbSet<RoleMethodEntity> RoleMethods { get; set; }
        public DbSet<SessionEntity> Sessions { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<SystemSettingsEntity> SystemSettings { get; set; }



    }
}
