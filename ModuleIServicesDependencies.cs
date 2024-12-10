using GreenDefined.Service.IServices;
using GreenDefined.Service.Services;

namespace GreenDefined.Service
{
    public static class ModuleIServicesDependencies
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IClassficationService, ClassficationService>();
            services.AddScoped<IUploadingImage, UploadingImage>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IReactService, ReactService>();
            services.AddScoped<INotificationTransactionService, NotificationTransactionService>();


            return services;
        }
    }

}
