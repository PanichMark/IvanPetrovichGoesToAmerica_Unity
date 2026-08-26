using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();
    private static readonly Dictionary<EnumServiceLocatorGameObjects, GameObject> _gameObjects = new();
    private static readonly Dictionary<EnumServiceLocatorAudioSources, AudioSource> _audioSources = new();

    public static void Register<T>(object instance)
    {
        if (instance is GameObject || instance is Component)
        {
            Debug.LogError($"[ServiceLocator] Attempt to register a scene/engine object ({instance.GetType().Name}) as contract {typeof(T).Name}. Use the enum overload instead.", instance as Object);
            return;
        }

        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            throw new InvalidOperationException($"Contract '{type.Name}' is already registered.");
        }
        
        _services[type] = instance;
    }

    public static void Register(EnumServiceLocatorGameObjects tag, GameObject go)
    {
        if (_gameObjects.ContainsKey(tag))
        {
            throw new InvalidOperationException($"GameObject for {tag} already registered.");
        }
        _gameObjects[tag] = go;
    }

    public static void Register(EnumServiceLocatorAudioSources key, AudioSource source)
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

    public static GameObject Resolve(EnumServiceLocatorGameObjects tag)
    {
        if (!_gameObjects.TryGetValue(tag, out var go))
        {
            throw new KeyNotFoundException($"GameObject for {tag} not found");
        }
        return go;
    }

    public static AudioSource Resolve(EnumServiceLocatorAudioSources key)
    {
        if (!_audioSources.TryGetValue(key, out var source))
        {
            throw new KeyNotFoundException($"AudioSource for {key} not found");
        }
        return source;
    }

    public static T GetComponent<T>(EnumServiceLocatorGameObjects tag) where T : Component
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

    public static void ClearAll()
    {
        ClearServices();
        ClearGameObjects();
        ClearAudioSources();
    }
}