using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Application.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKingdomApplication(this IServiceCollection services)
    {
        services.AddSingleton<IKingdomCatalogService, PreviewKingdomCatalogService>();

        return services;
    }
}
