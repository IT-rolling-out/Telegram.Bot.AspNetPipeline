using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures
{
    public static class MvcFeaturesExtensions
    {
        public static ContextMvcFeatures Features(this ActionContext @this)
        {
            const string FeaturesPropName = "_FeaturesProp";
            if (!@this.Properties.ContainsKey(FeaturesPropName))
            {
                var mvcFeaturesProvider=@this.UpdateContext.Services.GetRequiredService<IMvcFeaturesProvider>();
                var mvcFeatures = mvcFeaturesProvider.MvcFeatures;
                @this.Properties[FeaturesPropName] = new ContextMvcFeatures(mvcFeatures, @this);
            }
            return (ContextMvcFeatures)@this.Properties[FeaturesPropName];
        }
    }
}
