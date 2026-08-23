using UnityEngine;

public class InteractionObjectLootMana : InteractionObjectLootAbstract
{
	private bool _isInteractionHintMessageFailActive;
	public override bool IsInteractionHintMessageFailActive => _isInteractionHintMessageFailActive;
	private PlayerManaController _playerResourcesManaManager;

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
		}
		else
		{
			_isInteractionHintMessageFailActive = true;
		}
	}

	public override void InteractCutscene()
	{
		if (_playerResourcesManaManager.CurrentManaReplenishItemsNumber < 9)
		{
			base.InteractCutscene();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			_playerResourcesManaManager.AddManaReplenishItem();
			_isInteractionHintMessageFailActive = false;
		}
		else
		{
			//_isInteractionHintMessageFailActive = true;
		}
	}

	protected override void InitializeLootObject()
	{
		_playerResourcesManaManager = ServiceLocator.Resolve<PlayerManaController>("PlayerResourcesManaManager");
		//InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}