using UnityEngine;

public class InteractionObjectPickableThrowable : InteractionObjectPickableAbstract, IThrowable, IDamageable
{
	private bool _wasObjectDestroyed;

	private bool _canObjectBeDestroyedOnImpact;

	public float ObjectThrowPower => 10f;

	// Поле для здоровья, регулируемое в инспекторе, min=0
	[SerializeField, Min(0)]
	private float _health;

	public float Health
	{
		get => _health;
		set
		{
			_health = value;
			if (_health <= 0)
				Die(); // Вызываем метод уничтожения, если здоровье стало <= 0
		}
	}

	// Внутреннее скрытое поле для состояния разрушения

	

	// Соответствует интерфейсу IDamageable
	public bool WasObjectDestroyed => _wasObjectDestroyed;

	
	private void OnCollisionEnter(Collision collision)
	{
		if (_canObjectBeDestroyedOnImpact)
		{
			RigidBody.isKinematic = true;

			_wasObjectDestroyed = true;
			Destroy(gameObject);
			Debug.Log($"{InteractionObjectNameSystem} was destroyed on impact!");
		}
	}
	
	public void ThrowObject()
	{
		Debug.Log($"Throwed {InteractionObjectNameSystem}");
		gameObject.tag = "Interactable";
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		

		_canObjectBeDestroyedOnImpact = true;
		// Отцепляем объект от игрока
		transform.parent = null;

		//
		//RigidBody.AddForce(CachedPlayer.transform.forward * ObjectThrowPower, ForceMode.Impulse);
		//


		////////////////
		// FIX!!!!!!!!!!!!!!!!!!!!!!!
		Vector3 throwDirection = CachedPlayer.transform.forward - Camera.main.transform.up * Mathf.Tan(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);
		throwDirection.Normalize();

		RigidBody.AddForce(throwDirection * ObjectThrowPower, ForceMode.Impulse);
		//FIX!!!!!!!!!!!
		/////////////
		


	}

	public void TakeDamage(float amount)
	{

		Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {Health - amount}");

		Health -= amount; // Уменьшаем здоровье на указанное количество единиц

	}

	public void Die()
	{
		Debug.Log($"{InteractionObjectNameSystem} was destroyed!");
		_wasObjectDestroyed = true; // Устанавливаем флаг, что объект разрушен
		Destroy(gameObject); // Уничтожаем объект
	}
}


