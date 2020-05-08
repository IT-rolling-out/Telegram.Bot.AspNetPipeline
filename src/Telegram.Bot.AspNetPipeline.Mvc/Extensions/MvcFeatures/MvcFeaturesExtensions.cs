using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions
{ 
    public static class MvcFeaturesExtensions
    {
        public static ContextMvcFeatures Features(this ActionContext @this)
        {
            const string FeaturesPropName = "_FeaturesProp";
            if (!@this.Properties.ContainsKey(FeaturesPropName))
            {
                var mvcFeaturesProvider = @this.UpdateContext.Services.GetRequiredService<IMvcFeaturesProvider>();
                var mvcFeatures = mvcFeaturesProvider.MvcFeatures;
                @this.Properties[FeaturesPropName] = new ContextMvcFeatures(mvcFeatures, @this);
            }
            return (ContextMvcFeatures)@this.Properties[FeaturesPropName];
        }
    }
}
