using System.Collections.Generic;
using System;

public class ServiceLocator
{
	private static Dictionary<Type, object> services = new Dictionary<Type, object>();

	public static void Register<T>(T serviceInstance)
	{
		services[typeof(T)] = serviceInstance;
	}

	public static T Resolve<T>()
	{
		if (!services.TryGetValue(typeof(T), out var result))
		{
			throw new InvalidOperationException($"Тип {typeof(T)} не зарегистрирован.");
		}
		return (T)result;
	}
}