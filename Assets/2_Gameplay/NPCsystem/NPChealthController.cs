using UnityEngine;

public class NPChealthController : MonoBehaviour, IDamageable
{
	[SerializeField] private ConfigNPCHealth _NPCconfigHealth;

	public delegate void NPChealthHandler(float newHealth);
	public event NPChealthHandler OnNPChealthChanged;

	public ConfigNPCHealth NPCconfigHealth => _NPCconfigHealth;	
	public bool IsObjectDestroyed => false;
	private NPCabstract _NPCabstract;
	private float _currentHealth;
	public float CurrentHealth => _currentHealth;
	private NPCphrasesController _NPCphrasesController;
	public bool CanObjectBeDamaged => throw new System.NotImplementedException();
	private NPCstateMachineController _NPCstateMachineController;
	public void Initialize(
		NPCabstract NPCabstract,
		NPCstateMachineController NPCstateMachineController)
	{
		_NPCabstract = NPCabstract;
		_NPCstateMachineController = NPCstateMachineController;

		_currentHealth = _NPCconfigHealth.NPCcurrentHealth;
	}

	public void TakeDamage(float amount)
	{
		if (_currentHealth > 0)
		{
			Debug.Log($"{_NPCabstract.InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");

			_currentHealth -= amount;

			OnNPChealthChanged?.Invoke(_currentHealth);

			if (_currentHealth <= 0)
			{
				ObjectIsFullyDamaged();
			}
		}
	}

	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{_NPCabstract.InteractionObjectNameSystem} is Dead");

		_currentHealth = 0;

		_NPCstateMachineController.SetNPCState(NPCstateTypes.Dead);

		Destroy(this);
	}
}
