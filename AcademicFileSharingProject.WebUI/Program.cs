using AcademicFileSharingProject.Business;
using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Core.DependencyInjection.AutoMapper;
using AcademicFileSharingProject.DataAccess.Repository.EntityFramework;
using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Abstract;
using AcademicFileSharingProject.Entities.Validators;
using AcademicFileSharingProject.WebUI.Hubs;
using Microsoft.AspNetCore.Http.Features;
using NToastNotify;
using System.Reflection;

namespace AcademicFileSharingProject.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));



            builder.Services.AddScoped<IChatService, ChatManager>();
            builder.Services.AddScoped<IEntityRepository<ChatEntity>, EfChatDal>();
            builder.Services.AddScoped<BaseEntityValidator<ChatEntity>, ChatValidator>();

            builder.Services.AddScoped<IChatUserService, ChatUserManager>();
            builder.Services.AddScoped<IEntityRepository<ChatUserEntity>, EfChatUserDal>();
            builder.Services.AddScoped<BaseEntityValidator<ChatUserEntity>, ChatUserValidator>();

            builder.Services.AddScoped<IPostCommentService, PostCommentManager>();
            builder.Services.AddScoped<IEntityRepository<PostCommentEntity>, EfPostCommentDal>();
            builder.Services.AddScoped<BaseEntityValidator<PostCommentEntity>, CommentValidator>();

            builder.Services.AddScoped<IIdentityService, IdentityManager>();
            builder.Services.AddScoped<IEntityRepository<IdentityEntity>, EfIdentityDal>();
            builder.Services.AddScoped<BaseEntityValidator<IdentityEntity>, IdentityValidator>();

            builder.Services.AddScoped<IMediaService, MediaManager>();
            builder.Services.AddScoped<IEntityRepository<MediaEntity>, EfMediaDal>();
            builder.Services.AddScoped<BaseEntityValidator<MediaEntity>, MediaValidator>();

            builder.Services.AddScoped<IMessageService, MessageManager>();
            builder.Services.AddScoped<IEntityRepository<MessageEntity>, EfMessageDal>();
            builder.Services.AddScoped<BaseEntityValidator<MessageEntity>, MessageValidator>();

            builder.Services.AddScoped<IPostService, PostManager>();
            builder.Services.AddScoped<IEntityRepository<PostEntity>, EfPostDal>();
            builder.Services.AddScoped<BaseEntityValidator<PostEntity>, PostValidator>();

            builder.Services.AddScoped<IPostMediaService, PostMediaManager>();
            builder.Services.AddScoped<IEntityRepository<PostMediaEntity>, EfPostMediaDal>();
            builder.Services.AddScoped<BaseEntityValidator<PostMediaEntity>, PostMediaValidator>();

            builder.Services.AddScoped<IUserService, UserManager>();
            builder.Services.AddScoped<IEntityRepository<UserEntity>, EfUserDal>();
            builder.Services.AddScoped<BaseEntityValidator<UserEntity>, UserValidator>();

            builder.Services.AddScoped<IUserRoleService, UserRoleManager>();
            builder.Services.AddScoped<IEntityRepository<UserRoleEntity>, EfUserRoleDal>();
            builder.Services.AddScoped<BaseEntityValidator<UserRoleEntity>, UserRoleValidator>();

            builder.Services.AddScoped<IBlogService, BlogManager>();
            builder.Services.AddScoped<IEntityRepository<BlogEntity>, EfBlogDal>();
            builder.Services.AddScoped<BaseEntityValidator<BlogEntity>, BlogValidator>();


            builder.Services.AddScoped<IBlogCommentService, BlogCommentManager>();
            builder.Services.AddScoped<IEntityRepository<BlogCommentEntity>, EfBlogCommentDal>();
            builder.Services.AddScoped<BaseEntityValidator<BlogCommentEntity>, BlogCommentValidator>();
            
            builder.Services.AddScoped<ISessionService, SessionManager>();
            builder.Services.AddScoped<IEntityRepository<SessionEntity>, EfSessionDal>();
            builder.Services.AddScoped<BaseEntityValidator<SessionEntity>, SessionValidator>();


            builder.Services.AddScoped<IRoleMethodService, RoleMethodManager>();
            builder.Services.AddScoped<IEntityRepository<RoleMethodEntity>, EfRoleMethodDal>();
            builder.Services.AddScoped<BaseEntityValidator<RoleMethodEntity>, RoleMethodValidator>();

            builder.Services.AddScoped<INotificationService, NotificationManager>();
            builder.Services.AddScoped<IEntityRepository<NotificationEntity>, EfNotificationDal>();
            builder.Services.AddScoped<BaseEntityValidator<NotificationEntity>, NotificationValidator>();


            builder.Services.AddScoped<IAccountService,AccountManager>();   

            //Toast
            builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
            {
                ProgressBar = true,
                Timeout = 5000
            });

            builder.Services.AddSignalR();

            //Form file size changed
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;

            });
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = long.MaxValue;

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseNToastNotify();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<MessageHub>("/messageHub");

            app.Run();
        }
    }
}