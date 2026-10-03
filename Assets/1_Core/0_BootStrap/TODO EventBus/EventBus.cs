using System;
using System.Collections.Generic;

public sealed class EventBus
{
	private readonly Dictionary<Enum, Delegate> _subscribersByEvent = new Dictionary<Enum, Delegate>();

	public void Subscribe<TEvent>(TEvent eventType, Action handler) where TEvent : struct, Enum
	{
		AddSubscriber(eventType, handler);
	}

	public void Subscribe<TEvent, TArgument>(TEvent eventType, Action<TArgument> handler) where TEvent : struct, Enum
	{
		AddSubscriber(eventType, handler);
	}

	public void Subscribe<TEvent, TArgument1, TArgument2>(TEvent eventType, Action<TArgument1, TArgument2> handler) where TEvent : struct, Enum
	{
		AddSubscriber(eventType, handler);
	}

	public void Subscribe<TEvent, TArgument1, TArgument2, TArgument3>(TEvent eventType, Action<TArgument1, TArgument2, TArgument3> handler) where TEvent : struct, Enum
	{
		AddSubscriber(eventType, handler);
	}

	public void Unsubscribe<TEvent>(TEvent eventType, Action handler) where TEvent : struct, Enum
	{
		RemoveSubscriber(eventType, handler);
	}

	public void Unsubscribe<TEvent, TArgument>(TEvent eventType, Action<TArgument> handler) where TEvent : struct, Enum
	{
		RemoveSubscriber(eventType, handler);
	}

	public void Unsubscribe<TEvent, TArgument1, TArgument2>(TEvent eventType, Action<TArgument1, TArgument2> handler) where TEvent : struct, Enum
	{
		RemoveSubscriber(eventType, handler);
	}

	public void Unsubscribe<TEvent, TArgument1, TArgument2, TArgument3>(TEvent eventType, Action<TArgument1, TArgument2, TArgument3> handler) where TEvent : struct, Enum
	{
		RemoveSubscriber(eventType, handler);
	}

	public void Publish<TEvent>(TEvent eventType) where TEvent : struct, Enum
	{
		GetSubscribers<TEvent, Action>(eventType)?.Invoke();
	}

	public void Publish<TEvent, TArgument>(TEvent eventType, TArgument argument) where TEvent : struct, Enum
	{
		GetSubscribers<TEvent, Action<TArgument>>(eventType)?.Invoke(argument);
	}

	public void Publish<TEvent, TArgument1, TArgument2>(TEvent eventType, TArgument1 argument1, TArgument2 argument2) where TEvent : struct, Enum
	{
		GetSubscribers<TEvent, Action<TArgument1, TArgument2>>(eventType)?.Invoke(argument1, argument2);
	}

	public void Publish<TEvent, TArgument1, TArgument2, TArgument3>(TEvent eventType, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3) where TEvent : struct, Enum
	{
		GetSubscribers<TEvent, Action<TArgument1, TArgument2, TArgument3>>(eventType)?.Invoke(argument1, argument2, argument3);
	}

	private void AddSubscriber<TEvent>(TEvent eventType, Delegate handler) where TEvent : struct, Enum
	{
		if (handler == null)
		{
			throw new ArgumentNullException(nameof(handler));
		}

		Enum eventKey = eventType;
		if (_subscribersByEvent.TryGetValue(eventKey, out Delegate subscribers))
		{
			if (subscribers.GetType() != handler.GetType())
			{
				throw new InvalidOperationException("An event key cannot be used with handlers that have different signatures.");
			}

			_subscribersByEvent[eventKey] = Delegate.Combine(subscribers, handler);
			return;
		}

		_subscribersByEvent.Add(eventKey, handler);
	}

	private void RemoveSubscriber<TEvent>(TEvent eventType, Delegate handler) where TEvent : struct, Enum
	{
		if (handler == null)
		{
			return;
		}

		Enum eventKey = eventType;
		if (!_subscribersByEvent.TryGetValue(eventKey, out Delegate subscribers))
		{
			return;
		}

		Delegate remainingSubscribers = Delegate.Remove(subscribers, handler);
		if (remainingSubscribers == null)
		{
			_subscribersByEvent.Remove(eventKey);
			return;
		}

		_subscribersByEvent[eventKey] = remainingSubscribers;
	}

	private TDelegate GetSubscribers<TEvent, TDelegate>(TEvent eventType)
		where TEvent : struct, Enum
		where TDelegate : Delegate
	{
		if (!_subscribersByEvent.TryGetValue(eventType, out Delegate subscribers))
		{
			return null;
		}

		if (subscribers is TDelegate typedSubscribers)
		{
			return typedSubscribers;
		}

		throw new InvalidOperationException("The event was published with arguments that do not match its subscribed handler signature.");
	}
}
