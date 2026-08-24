using TMPro;
using UnityEngine;

public class NPCdebugHUDcontroller : MonoBehaviour
{
	private GameObject _canvasNPCstatus;
	private GameObject _textNPCcurrentState;
	private TextMeshProUGUI _textComponentNPCcurrentState;
	private GameObject _textNPCcurrentHealth;
	private TextMeshProUGUI _textComponentNPCcurrentHealth;

	private NPChealthController _NPChealthController;
	private NPCstateMachineController _NPCstateMachineController;

	public void Initialize(
		NPChealthController NPChealthController,
		NPCstateMachineController NPCstateMachineController,
		GameObject canvasNPCstatus,
		GameObject textNPCcurrentState,
		GameObject textNPCcurrentHealth)
	{
		_NPChealthController = NPChealthController;
		_NPCstateMachineController = NPCstateMachineController;
		_canvasNPCstatus = canvasNPCstatus;
		_textNPCcurrentState = textNPCcurrentState;
		_textComponentNPCcurrentState = _textNPCcurrentState.GetComponent<TextMeshProUGUI>();
		_textNPCcurrentHealth = textNPCcurrentHealth;
		_textComponentNPCcurrentHealth = _textNPCcurrentHealth.GetComponent<TextMeshProUGUI>();

		_textNPCcurrentState.SetActive(false);
		_textNPCcurrentHealth.SetActive(false);

		_textComponentNPCcurrentHealth.text = _NPChealthController.NPCconfigHealth.NPCcurrentHealth.ToString();

		ShowNewNPCstate(_NPCstateMachineController.CurrentNPCState);

		_NPCstateMachineController.OnNewNPCstate += ShowNewNPCstate;
		_NPChealthController.OnNPChealthChanged += ShowNewNPChealth;
	}

	public void ShowNewNPCstate(NPCstateTypes newState)
	{
		_textComponentNPCcurrentState.text = newState.ToString();
	}

	public void ShowNewNPChealth(float newHealth)
	{
		_textComponentNPCcurrentHealth.text = newHealth.ToString();

		if (newHealth <= 0)
		{
			_canvasNPCstatus.SetActive(false);
		}
	}
}
