using System;
using System.Collections.Generic;

public class ServiceLocator
{
	private static readonly Dictionary<string, object> _services = new Dictionary<string, object>();

	public static void Register(string key, object serviceInstance)
	{
		if (_services.ContainsKey(key))
		{
			throw new ArgumentException($"Service '{key}' was already registered", nameof(key));
		}
		_services[key] = serviceInstance;
	}

	public static T Resolve<T>(string key)
	{
		if (!_services.TryGetValue(key, out var result))
		{
			throw new KeyNotFoundException($"Service '{key}' not found");
		}
		return (T)result;
	}

	public static void ClearAllServices()
	{
		_services.Clear();
	}

	public static bool RemoveService(string key)
	{
		return _services.Remove(key);
	}
}