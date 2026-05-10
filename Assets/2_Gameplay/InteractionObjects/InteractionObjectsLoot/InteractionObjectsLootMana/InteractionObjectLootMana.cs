using UnityEngine;

public class InteractionObjectLootMana : InteractionObjectLootAbstract
{
	private bool _isAdditionalInteractionHintActive;
	public override bool IsInteractionHintMessageAdditionalActive => _isAdditionalInteractionHintActive;
	private PlayerResourcesManaManager _playerResourcesManaManager;

	public override string InteractionHintMessageAdditional => $"Maximum {InteractionObjectNameUI}";

	public override void Interact()
	{
		if (_playerResourcesManaManager.CurrentManaReplenishItemsNumber < 9)
		{
			base.Interact();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			_playerResourcesManaManager.AddManaReplenishItem();
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
		_playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}