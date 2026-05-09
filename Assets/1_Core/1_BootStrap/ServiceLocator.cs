using System;
using System.Collections.Generic;

public class ServiceLocator
{
	private static readonly Dictionary<string, object> services = new Dictionary<string, object>();

	public static void Register(string key, object serviceInstance)
	{
		if (services.ContainsKey(key))
		{
			throw new ArgumentException($"Service '{key}' was already registered", nameof(key));
		}
		services[key] = serviceInstance;
	}

	public static T Resolve<T>(string key)
	{
		if (!services.TryGetValue(key, out var result))
		{
			throw new KeyNotFoundException($"Service '{key}' not found");
		}
		return (T)result;
	}

	public static void ClearAllServices()
	{
		services.Clear();
	}

	public static bool RemoveService(string key)
	{
		return services.Remove(key);
	}
}