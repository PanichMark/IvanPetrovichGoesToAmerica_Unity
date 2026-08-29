using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCdetectionSignController : MonoBehaviour
{
	private NPCdetectionManager _npcDetectionManager;
	private GameObject _playerCameraGameObject;
	// Ссылки разделены: холст отдельно, картинка отдельно
	private RectTransform _imageDetectionSignRectTransform;
	private GameObject _imageDetectionSign;
	private Image _imageComponentDetectionSign;
	private Camera _playerCameraComponent;
	private List<Sprite> _detectionSignFrames = new List<Sprite>();
	private GameObject _canvasNpcStatus;
	private float _detectionSignOffset = 20f;
	private float _detectionSignHeight;

	private float _detectionSignBorderOffsetX = 40f;
	private float _detectionSignBorderOffsetY = 55f;


	private float _minDistanceForScale = 6f; // Минимальное расстояние до начала эффекта
	private float _scaleMultiplier = 6f;     // Во сколько раз увеличится размер на минимальном расстоянии

	private MenuManager _menuManager;

	public void Initialize(
		NPCdetectionManager npcDetectionManager,
		GameObject canvasNpcStatus, // Оставляем для общего контроля видимости
		GameObject imageDetectionSign,
		List<Sprite> detectionSignFrames,
		GameObject playerCameraGameObject)
	{
		_canvasNpcStatus = canvasNpcStatus;
		_npcDetectionManager = npcDetectionManager;
		_imageDetectionSign = imageDetectionSign;
		// Берем RectTransform именно у картинки, а не у Canvas
		_imageDetectionSignRectTransform = _imageDetectionSign.GetComponent<RectTransform>();

		_imageComponentDetectionSign = _imageDetectionSign.GetComponent<Image>();
		_detectionSignFrames = detectionSignFrames;
		_playerCameraGameObject = playerCameraGameObject;
		_playerCameraComponent = _playerCameraGameObject.GetComponent<Camera>();

		_detectionSignHeight = _imageDetectionSignRectTransform.rect.height;

		//Debug.Log("[NPC Sign] Initialized. Frames count: " + _detectionSignFrames.Count);


		_menuManager = ServiceLocator.Resolve<MenuManager>();
		_menuManager.OnOpenAnyMenu += HideCanvasNPC;
		//_menuManager.OnCloseAnyMenu += ShowCanvasNPC;

		UpdateSpriteByMeter(0f);
		_npcDetectionManager.OnMeterChanged += UpdateSpriteByMeter;

		HideCanvasNPC();
	}

	private void ShowCanvasNPC()
	{
		_canvasNpcStatus.SetActive(true);
	}

	private void HideCanvasNPC()
	{
		_canvasNpcStatus.SetActive(false);
	}

	private void OnDestroy()
	{
		_npcDetectionManager.OnMeterChanged -= UpdateSpriteByMeter;

		_menuManager.OnOpenAnyMenu -= HideCanvasNPC;
		_menuManager.OnCloseAnyMenu -= ShowCanvasNPC;
	}

	private void Update()
	{
		if (!_imageDetectionSign.activeInHierarchy)
		{
			return;
		}

		Vector3 targetPosition = transform.position + new Vector3(0f, 2.2f, 0f);
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
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width + _detectionSignBorderOffsetX;
			}
			else
			{
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width - _detectionSignBorderOffsetX;
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
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height + _detectionSignBorderOffsetY;
			}
			else
			{
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height - _detectionSignBorderOffsetY;
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
				xPos -= _detectionSignOffset;
			}
			else if (screenPoint.x > 1)
			{
				xPos += _detectionSignOffset;
			}
		}

		if (isOnScreenY)
		{
			if (screenPoint.y < 0)
			{
				yPos -= _detectionSignOffset;
			}
			else if (screenPoint.y > 1)
			{
				yPos += _detectionSignHeight + _detectionSignOffset;
			}
		}

		_imageDetectionSignRectTransform.anchoredPosition = new Vector2(xPos  - Screen.width / 2, yPos - Screen.height / 2);

		UpdateScaleByDistance();
	}

	private void UpdateSpriteByMeter(float meterValue)
	{
		bool shouldShow = meterValue > 0f;
		_imageComponentDetectionSign.gameObject.SetActive(shouldShow); // Используем gameObject от Image

		if (!shouldShow || _detectionSignFrames.Count == 0)
		{
			//Debug.LogWarning("[NPC Sign] Tried to update sprite, but frames are missing or value is 0.");
			return;
		}

		float normalizedValue = Mathf.Clamp01(meterValue / 100f);
		int frameIndex = Mathf.RoundToInt(normalizedValue * (_detectionSignFrames.Count - 1));

		//Debug.Log($"[NPC Sign] Meter: {meterValue:F1} -> Frame Index: {frameIndex}/{_detectionSignFrames.Count - 1} ({_detectionSignFrames[frameIndex].name})");

		_imageComponentDetectionSign.sprite = _detectionSignFrames[frameIndex];
	}

	private void UpdateScaleByDistance()
	{
		float distance = Vector3.Distance(_playerCameraComponent.transform.position, transform.position);

		// Если дальше минимальной дистанции — возвращаем стандартный масштаб
		if (distance >= _minDistanceForScale)
		{
			_imageDetectionSignRectTransform.localScale = Vector3.one;
			return;
		}

		float t = Mathf.InverseLerp(_minDistanceForScale, 0f, distance);
		float currentScaleValue = Mathf.Lerp(1f, _scaleMultiplier, t);

		_imageDetectionSignRectTransform.localScale = new Vector3(currentScaleValue, currentScaleValue, currentScaleValue);
	}
}