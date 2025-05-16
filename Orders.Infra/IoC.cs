using Microsoft.Extensions.DependencyInjection;
using Orders.Domain.Interfaces;
using Orders.Domain.Services;

namespace Orders.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<TokenService>();

            return services;
        }
    }
}
