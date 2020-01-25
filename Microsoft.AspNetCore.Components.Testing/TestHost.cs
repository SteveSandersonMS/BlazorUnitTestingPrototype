﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Components.Testing
{
    public class TestHost
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Lazy<TestRenderer> _renderer;
        private readonly Lazy<IServiceProvider> _serviceProvider;

        public TestHost():this(new ServiceCollection())
        {
            
        }
        public TestHost(IServiceCollection services)
        {
            _serviceCollection = services;
            _serviceProvider = new Lazy<IServiceProvider>(() =>
            {
                return _serviceCollection.BuildServiceProvider();
            });

            _renderer = new Lazy<TestRenderer>(() =>
            {
                var loggerFactory = Services.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
                return new TestRenderer(Services, loggerFactory);
            });
        }

        public IServiceProvider Services => _serviceProvider.Value;

        public void AddService<T>(T implementation)
            => AddService<T, T>(implementation);

        public void AddService<TContract, TImplementation>(TImplementation implementation) where TImplementation: TContract
        {
            if (_renderer.IsValueCreated)
            {
                throw new InvalidOperationException("Cannot configure services after the host has started operation");
            }

            _serviceCollection.AddSingleton(typeof(TContract), implementation);
        }

        public void WaitForNextRender(Action trigger)
        {
            var task = Renderer.NextRender;
            trigger();
            task.Wait(millisecondsTimeout: 1000);

            if (!task.IsCompleted)
            {
                throw new TimeoutException("No render occurred within the timeout period.");
            }
        }

        public RenderedComponent<TComponent> AddComponent<TComponent>() where TComponent: IComponent
        {
            var result = new RenderedComponent<TComponent>(Renderer);
            result.SetParametersAndRender(ParameterView.Empty);
            return result;
        }

        public RenderedComponent<TComponent> AddComponent<TComponent>(ParameterView parameters) where TComponent : IComponent
        {
            var result = new RenderedComponent<TComponent>(Renderer);
            result.SetParametersAndRender(parameters);
            return result;
        }

        public RenderedComponent<TComponent> AddComponent<TComponent>(IDictionary<string, object> parameters) where TComponent : IComponent
        {
            return AddComponent<TComponent>(ParameterView.FromDictionary(parameters));
        }

        private TestRenderer Renderer => _renderer.Value;
    }
}
