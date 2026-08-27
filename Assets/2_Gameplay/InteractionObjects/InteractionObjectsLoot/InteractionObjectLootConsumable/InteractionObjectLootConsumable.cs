using UnityEngine;

public class InteractionObjectLootConsumable : InteractionObjectLootAbstract
{
	[SerializeField] private InteractionObjectLootConsumableTypes _interactionObjectConsumableTypes;
	[SerializeField] private float _healthAffected;
	[SerializeField] private bool _isRotten;
	private PlayerHealthController _playerHealthController;
	protected override void InitializeLootObject()
	{
		_playerHealthController = ServiceLocator.Resolve<PlayerHealthController>();

		if (_interactionObjectConsumableTypes == InteractionObjectLootConsumableTypes.Food)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Eat", gameObject.name); 
		}
		else if (_interactionObjectConsumableTypes == InteractionObjectLootConsumableTypes.Drink) 
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Drink", gameObject.name); 
		}
	}

	protected override void OnAfterLooted()
	{
		if (!_isRotten)
		{
			_playerHealthController.ReceiveHealth(_healthAffected);
		}
		else
		{
			_playerHealthController.TakeDamage(_healthAffected);
		}
	}

	public override void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (_interactionObjectConsumableTypes == InteractionObjectLootConsumableTypes.Food)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Eat", gameObject.name);
		}
		else if (_interactionObjectConsumableTypes == InteractionObjectLootConsumableTypes.Drink)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Drink", gameObject.name);
		}
	}
}