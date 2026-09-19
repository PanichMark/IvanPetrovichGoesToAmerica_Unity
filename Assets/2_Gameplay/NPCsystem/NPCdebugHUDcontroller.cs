using TMPro;
using UnityEngine;

public class NPCdebugHUDcontroller : MonoBehaviour
{
	private GameObject _canvasNPCstatus;

	private GameObject _textNPCcurrentState;
	private RectTransform _textNPCcurrentStateRectTransform;
	private float _textNPCcurrentStateHeight;
	private TextMeshProUGUI _textComponentNPCcurrentState;

	private GameObject _textNPCcurrentHealth;
	private RectTransform _textNPCcurrentHealthRectTransform;
	private float _textNPCcurrentHealthHeight;
	private TextMeshProUGUI _textComponentNPCcurrentHealth;

	private NPChealthController _NPChealthController;
	private NPCstateMachineController _NPCstateMachineController;

	private Camera _playerCameraComponent;

	private float _NPCdebugMessageOffset = 20f;
	
	private float _NPCdebugMessageBorderOffsetX = 40f;
	private float _NPCdebugMessageBorderOffsetY = 55f;

	public void Initialize(
		GameObject playerCameraComponent,
		NPChealthController NPChealthController,
		NPCstateMachineController NPCstateMachineController,
		GameObject canvasNPCstatus,
		GameObject textNPCcurrentState,
		GameObject textNPCcurrentHealth)
	{
		_playerCameraComponent = playerCameraComponent.GetComponent<Camera>();
		_NPChealthController = NPChealthController;
		_NPCstateMachineController = NPCstateMachineController;
		_canvasNPCstatus = canvasNPCstatus;
		_textNPCcurrentState = textNPCcurrentState;
		_textNPCcurrentStateRectTransform = _textNPCcurrentState.GetComponent<RectTransform>();
		_textNPCcurrentStateHeight = _textNPCcurrentStateRectTransform.rect.height;
		_textComponentNPCcurrentState = _textNPCcurrentState.GetComponent<TextMeshProUGUI>();

		_textNPCcurrentHealth = textNPCcurrentHealth;
		_textNPCcurrentHealthRectTransform = _textNPCcurrentHealth.GetComponent<RectTransform>();
		_textNPCcurrentHealthHeight = _textNPCcurrentHealthRectTransform.rect.height;
		_textComponentNPCcurrentHealth = _textNPCcurrentHealth.GetComponent<TextMeshProUGUI>();

		//_textNPCcurrentState.SetActive(false);
		//_textNPCcurrentHealth.SetActive(false);

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
			_textNPCcurrentHealth.SetActive(false);
		}
	}

	private void Update()
	{
		ProcessNPCdebugMEssageCanvasTransform(_textNPCcurrentState, _textNPCcurrentStateRectTransform, _textNPCcurrentStateHeight, 2.6f);

		ProcessNPCdebugMEssageCanvasTransform(_textNPCcurrentHealth, _textNPCcurrentHealthRectTransform, _textNPCcurrentHealthHeight, 2.8f);
	}

	private void ProcessNPCdebugMEssageCanvasTransform(
		GameObject NPCdebugMessage,
		RectTransform NPCdebugMessageRectTransform,
		float NPCdebugMessageHeight,
		float NPCdebugMessageAbove)
	{
		if (!NPCdebugMessage.activeInHierarchy)
		{
			return;
		}

		Vector3 targetPosition = transform.position + new Vector3(0f, NPCdebugMessageAbove, 0f);
		Vector3 screenPoint = _playerCameraComponent.WorldToViewportPoint(targetPosition);

		if (screenPoint.z <= 0)
		{
			return;
		}

		bool isOnScreenX = screenPoint.x >= 0 && screenPoint.x <= 1;
		bool isOnScreenY = screenPoint.y >= 0 && screenPoint.y <= 1;

		float xPos;
		if (!isOnScreenX)
		{
			if (screenPoint.x < 0)
			{
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width + _NPCdebugMessageBorderOffsetX;
			}
			else
			{
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width - _NPCdebugMessageBorderOffsetX;
			}
		}
		else
		{
			xPos = screenPoint.x * Screen.width;
		}

		float yPos;
		if (!isOnScreenY)
		{
			if (screenPoint.y < 0)
			{
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height + _NPCdebugMessageBorderOffsetY;
			}
			else
			{
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height - _NPCdebugMessageBorderOffsetY;
			}
		}
		else
		{
			yPos = screenPoint.y * Screen.height;
		}

		if (isOnScreenX)
		{
			if (screenPoint.x < 0)
			{
				xPos -= _NPCdebugMessageOffset;
			}
			else if (screenPoint.x > 1)
			{
				xPos += _NPCdebugMessageOffset;
			}
		}

		if (isOnScreenY)
		{
			if (screenPoint.y < 0)
			{
				yPos -= _NPCdebugMessageOffset;
			}
			else if (screenPoint.y > 1)
			{
				yPos += NPCdebugMessageHeight + _NPCdebugMessageOffset;
			}
		}

		NPCdebugMessageRectTransform.anchoredPosition = new Vector2(xPos - Screen.width / 2, yPos - Screen.height / 2);
	}
}