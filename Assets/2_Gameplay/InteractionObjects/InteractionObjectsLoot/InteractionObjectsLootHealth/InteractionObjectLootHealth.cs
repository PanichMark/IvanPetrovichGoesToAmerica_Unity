using UnityEngine;

public class InteractionObjectLootHealth : InteractionObjectLootAbstract
{
	private PlayerResourcesHealthManager _playerResourcesHealthManager;
	private bool _isAdditionalInteractionHintActive;

	public override bool IsInteractionHintMessageFailActive => _isAdditionalInteractionHintActive;

	public override string InteractionHintMessageFail => $"Максимум {InteractionObjectNameUI}";

	public override void Interact()
	{
		if (_playerResourcesHealthManager.CurrentHealingItemsNumber < 9)
		{
			base.Interact();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			_playerResourcesHealthManager.AddHealingItem();
			_isAdditionalInteractionHintActive = false;
			WasLootItemCollected = true;
		}
		else
		{
			_isAdditionalInteractionHintActive = true;
		}
	}

	protected override void ThisMethodSetsActionName()
	{
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");

		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}