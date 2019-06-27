using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    /// <summary>
    /// Just wrapper used to separate ASP.NET extensions and extensions in Telegram.Bot.AspNetPipeline library.
    /// </summary>
    public class ServiceCollectionWrapper
    {
        List<Type> _forDispose = new List<Type>();

        public IReadOnlyCollection<Type> ForDispose => _forDispose;

        public IServiceCollection Services { get; }

        public ServiceCollectionWrapper(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// When <see cref="BotManager"/> disposing it will try to resolve current service and dispose it.
        /// Use this to dispose singletons.
        /// </summary>
        public void RegisterForDispose(Type type)
        {
            if (!typeof(IDisposable).IsAssignableFrom(type))
            {
                throw new ArgumentException("Must be IDisposable.", nameof(type));
            }
            _forDispose.Add(type);
        }

        /// <summary>
        /// When <see cref="BotManager"/> disposing it will try to resolve current service and dispose it.
        /// Use this to dispose singletons.
        /// </summary>
        public void RegisterForDispose<Disposable>()
            where Disposable : IDisposable
        {
            RegisterForDispose(typeof(Disposable));
        }
    }
}
