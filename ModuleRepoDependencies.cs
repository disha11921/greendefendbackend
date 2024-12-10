using GreenDefined.Repository.IRepo;
using GreenDefined.Repository.Repo;

namespace GreenDefined.Repository
{
    public static class ModuleRepoDependencies
    {
        public static IServiceCollection AddRepoDependencies(this IServiceCollection services)
        {
            services.AddTransient<IClassficationRepository, ClassficationRepository>();
            services.AddTransient<IReactRepository, ReactRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<INotificationTransactionRepository, NotificationTransactionRepository>();

            return services;
        }
    }
}
