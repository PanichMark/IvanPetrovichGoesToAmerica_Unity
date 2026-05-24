using UnityEngine;

public class InteractionObjectLootMana : InteractionObjectLootAbstract
{
	private bool _isInteractionHintMessageFailActive;
	public override bool IsInteractionHintMessageFailActive => _isInteractionHintMessageFailActive;
	private PlayerResourcesManaManager _playerResourcesManaManager;

	[SerializeField] Sprite _lootObjectIcon;
	public override Sprite LootObjectIcon => _lootObjectIcon;
	public override void Interact()
	{
		if (_playerResourcesManaManager.CurrentManaReplenishItemsNumber < 9)
		{
			base.Interact();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			_playerResourcesManaManager.AddManaReplenishItem();
			_isInteractionHintMessageFailActive = false;
			WasLootItemCollected = true;
		}
		else
		{
			_isInteractionHintMessageFailActive = true;
		}
	}

	protected override void ThisMethodSetsActionName()
	{
		_playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		//InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}