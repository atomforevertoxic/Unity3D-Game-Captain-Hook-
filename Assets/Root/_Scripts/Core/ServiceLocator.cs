using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public interface IService { }

    public class ServiceLocator
    {
        private Dictionary<Type, IService> _services;

        public ServiceLocator()
        {
            _services = new Dictionary<Type, IService>();
        }

        public void RegisterService<T>(IService service) where T : IService
        {
            var key = typeof(T);

            if (_services.ContainsKey(key))
                throw new UnityException("Service already registered");

            _services[key] = service;
        }

        public T GetService<T>() where T : class, IService
        {
            var key = typeof(T);

            if (!_services.ContainsKey(key))
                throw new UnityException("Service not registered");

            T service = _services[key] as T;

            return service;
        }
    }
}