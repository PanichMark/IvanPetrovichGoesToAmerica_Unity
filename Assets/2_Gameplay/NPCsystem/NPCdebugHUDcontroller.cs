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
		NPCstateMachineController NPCstateMachineController)
	{
		_NPChealthController = NPChealthController;
		_NPCstateMachineController = NPCstateMachineController;

		_canvasNPCstatus = transform.Find("CanvasNPCstatus").gameObject;
		_textNPCcurrentState = _canvasNPCstatus.transform.Find("TextNPCcurrentState").gameObject;
		_textComponentNPCcurrentState = _textNPCcurrentState.GetComponent<TextMeshProUGUI>();
		_textNPCcurrentHealth = _canvasNPCstatus.transform.Find("TextNPCcurrentHealth").gameObject;
		_textComponentNPCcurrentHealth = _textNPCcurrentHealth.GetComponent<TextMeshProUGUI>();

		_textComponentNPCcurrentHealth.text = _NPChealthController.NPCconfigHealth.NPCcurrentHealth.ToString();

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
