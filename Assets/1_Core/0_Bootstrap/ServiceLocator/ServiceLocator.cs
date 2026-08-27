using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();
    private static readonly Dictionary<ServiceLocatorGameObjectsEnum, GameObject> _gameObjects = new();
    private static readonly Dictionary<ServiceLocatorAudioSourcesEnum, AudioSource> _audioSources = new();

	public static void Register<T>(object instance)
	{
		var type = typeof(T);

		// ПРОВЕРКА: Не затираем ли мы старый сервис новым?
		if (_services.ContainsKey(type))
		{
			//Debug.LogError($"[SL] OVERWRITING existing service '{type.Name}'. Old Instance ID: {_services[type].GetHashCode()}, New Instance ID: {instance.GetHashCode()}");
		}

		//Debug.Log($"[SL] REGISTERED: '{type.Name}' | Hash: {instance.GetHashCode()}");
		_services[type] = instance;
	}

	public static void Register(ServiceLocatorGameObjectsEnum tag, GameObject go)
    {
        if (_gameObjects.ContainsKey(tag))
        {
            throw new InvalidOperationException($"GameObject for {tag} already registered.");
        }
        _gameObjects[tag] = go;
    }

    public static void Register(ServiceLocatorAudioSourcesEnum key, AudioSource source)
    {
        if (_audioSources.ContainsKey(key))
        {
            throw new InvalidOperationException($"AudioSource for {key} already registered.");
        }
        _audioSources[key] = source;
    }

    public static T Resolve<T>()
    {
        var type = typeof(T);
        if (!_services.TryGetValue(type, out var result))
        {
            throw new KeyNotFoundException($"Contract '{type.Name}' not found");
        }
        return (T)result;
    }

    public static GameObject Resolve(ServiceLocatorGameObjectsEnum tag)
    {
        if (!_gameObjects.TryGetValue(tag, out var go))
        {
            throw new KeyNotFoundException($"GameObject for {tag} not found");
        }
        return go;
    }

    public static AudioSource Resolve(ServiceLocatorAudioSourcesEnum key)
    {
        if (!_audioSources.TryGetValue(key, out var source))
        {
            throw new KeyNotFoundException($"AudioSource for {key} not found");
        }
        return source;
    }

    public static T GetComponent<T>(ServiceLocatorGameObjectsEnum tag) where T : Component
    {
        var go = Resolve(tag);
        return go.GetComponent<T>();
    }

    public static void ClearServices()
    {
        _services.Clear();
    }

    public static void ClearGameObjects()
    {
        _gameObjects.Clear();
    }

    public static void ClearAudioSources()
    {
        _audioSources.Clear();
    }

    public static void ClearAllServices()
    {
        ClearServices();
        ClearGameObjects();
        ClearAudioSources();
    }

    public static bool Remove<T>()
{
    var type = typeof(T);
    return _services.Remove(type);
}
}