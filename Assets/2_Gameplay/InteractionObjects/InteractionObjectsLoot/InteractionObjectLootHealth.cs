using UnityEngine;

public class InteractionObjectLootHealth : InteractionObjectLootAbstract
{
	private PlayerResourcesHealthManager _playerResourcesHealthManager;
	private bool _isInteractionHintMessageFailActive;

	public override bool IsInteractionHintMessageFailActive => _isInteractionHintMessageFailActive;


	[SerializeField] Sprite _lootObjectIcon;
	public override Sprite LootObjectIcon => _lootObjectIcon;

	public override void Interact()
	{
		if (_playerResourcesHealthManager.CurrentHealingItemsNumber < 9)
		{
			base.Interact();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			_playerResourcesHealthManager.AddHealingItem();
			_isInteractionHintMessageFailActive = false;
			WasLootItemCollected = true;
		}
		else
		{
			_isInteractionHintMessageFailActive = true;
		}
	}

	public override void InteractCutscene()
	{
		if (_playerResourcesHealthManager.CurrentHealingItemsNumber < 9)
		{
			base.InteractCutscene();
			Debug.Log($"You picked up {InteractionObjectNameUI}");

			_playerResourcesHealthManager.AddHealingItem();
			_isInteractionHintMessageFailActive = false;
			WasLootItemCollected = true;
		}
		else
		{
			//_isInteractionHintMessageFailActive = true;
		}
	}

	protected override void SetUpLootObjectReferences()
	{
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");

		//InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	}
}