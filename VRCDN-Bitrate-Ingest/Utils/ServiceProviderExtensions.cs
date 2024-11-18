using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LucHeart.VRCDN.BI.Utils;

public static class ServiceProviderExtensions
{
    public static T GetOptions<T>(this IServiceProvider serviceProvider) where T : class
    {
        return serviceProvider.GetRequiredService<IOptions<T>>().Value;
    }
}