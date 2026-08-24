using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCdetectionSignController : MonoBehaviour
{
	private NPCdetectionManager _npcDetectionManager;

	// Ссылки разделены: холст отдельно, картинка отдельно
	private RectTransform _imageDetectionSignRectTransform;
	private GameObject _imageDetectionSign;
	private Image _imageComponentDetectionSign;
	private Camera _playerCamera;
	private List<Sprite> _detectionSignFrames = new List<Sprite>();

	private float _detectionSignOffset = 20f;
	private float _detectionSignHeight;

	private float _detectionSignBorderOffset = 50f;

	public void Initialize(
		NPCdetectionManager npcDetectionManager,
		GameObject canvasNpcStatus, // Оставляем для общего контроля видимости
		GameObject imageDetectionSign,
		List<Sprite> detectionSignFrames,
		Camera playerCamera)
	{
		_npcDetectionManager = npcDetectionManager;
		_imageDetectionSign = imageDetectionSign;
		// Берем RectTransform именно у картинки, а не у Canvas
		_imageDetectionSignRectTransform = _imageDetectionSign.GetComponent<RectTransform>();

		_imageComponentDetectionSign = _imageDetectionSign.GetComponent<Image>();
		_detectionSignFrames = detectionSignFrames;
		_playerCamera = playerCamera;

		_detectionSignHeight = _imageDetectionSignRectTransform.rect.height;

		//Debug.Log("[NPC Sign] Initialized. Frames count: " + _detectionSignFrames.Count);

		UpdateSpriteByMeter(0f);
		_npcDetectionManager.OnMeterChanged += UpdateSpriteByMeter;
	}

	private void OnDestroy()
	{
		if (_npcDetectionManager != null)
		{
			_npcDetectionManager.OnMeterChanged -= UpdateSpriteByMeter;
		}
	}

	private void Update()
	{
		if (!_imageDetectionSign.activeInHierarchy)
		{
			return;
		}

		
		Vector3 targetPosition = transform.position + new Vector3(0f, 2.2f, 0f);
		Vector3 screenPoint = _playerCamera.WorldToViewportPoint(targetPosition);

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
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width + _detectionSignBorderOffset;
			}
			else
			{
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width - _detectionSignBorderOffset;
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
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height + _detectionSignBorderOffset;
			}
			else
			{
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height - _detectionSignBorderOffset;
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

		_imageDetectionSignRectTransform.anchoredPosition = new Vector2(xPos, yPos);
		
		/*
		Vector3 targetPosition = transform.position + new Vector3(0f, 2.2f, 0f);
		Vector3 screenPoint = _playerCamera.WorldToViewportPoint(targetPosition);

		Debug.DrawLine(_playerCamera.transform.position, transform.position, Color.cyan);

		if (screenPoint.z <= 0)
		{
			//Debug.LogWarning("[NPC Sign] Target is behind the camera or too close.");
			return;
		}

		bool isOnScreenX = screenPoint.x >= 0 && screenPoint.x <= 1;
		bool isOnScreenY = screenPoint.y >= 0 && screenPoint.y <= 1;

		string xState = isOnScreenX ? "Inside" : "Outside";
		string yState = isOnScreenY ? "Inside" : "Outside";
		//Debug.Log($"[NPC Sign] Viewport: ({screenPoint.x:F2}, {screenPoint.y:F2}). X: {xState}. Y: {yState}");

		float xPos;
		if (!isOnScreenX)
		{
			if (screenPoint.x < 0.5f)
				xPos = _detectionSignOffset;
			else
				xPos = Screen.width - _detectionSignOffset;
		}
		else
		{
			xPos = screenPoint.x * Screen.width;
		}

		float yPos;
		if (!isOnScreenY)
		{
			if (screenPoint.y < 0.5f)
				yPos = _detectionSignOffset;
			else
				yPos = Screen.height - _detectionSignOffset - _detectionSignHeight;
		}
		else
		{
			yPos = screenPoint.y * Screen.height;
		}

		// Применяем позицию к картинке, а не к Canvas
		_imageDetectionSignRectTransform.position = new Vector3(xPos, yPos, 0f);

		//Debug.Log($"[NPC Sign] Final UI Pos: ({xPos:F0}, {yPos:F0})");
		*/
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
}