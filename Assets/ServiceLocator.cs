using System;
using System.Collections.Generic;

public class ServiceLocator
{
	private static readonly Dictionary<string, object> services = new Dictionary<string, object>();

	public static void Register(string key, object serviceInstance)
	{
		if (services.ContainsKey(key))
		{
			throw new ArgumentException($"Служба с ключом '{key}' уже зарегистрирована.", nameof(key));
		}
		services[key] = serviceInstance;
	}

	public static T Resolve<T>(string key)
	{
		if (!services.TryGetValue(key, out var result))
		{
			throw new KeyNotFoundException($"Не найдено службы с ключом '{key}'.");
		}
		return (T)result;
	}

	// Новый публичный метод для очистки
	public static void ClearServices()
	{
		services.Clear();
	}
}