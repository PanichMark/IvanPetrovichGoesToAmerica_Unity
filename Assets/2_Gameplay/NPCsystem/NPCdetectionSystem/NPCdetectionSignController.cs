using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCdetectionSignController : MonoBehaviour
{
	private NPCdetectionManager _npcDetectionManager;

	// Ссылки разделены: холст отдельно, картинка отдельно
	private RectTransform _imageRectTransform;

	private Image _imageComponentDetectionSign;
	private Camera _playerCamera;
	private List<Sprite> _detectionSignFrames = new List<Sprite>();

	[SerializeField] private float markerOffset = 20f;
	private float markerHeight;

	public void Initialize(
		NPCdetectionManager npcDetectionManager,
		GameObject canvasNpcStatus, // Оставляем для общего контроля видимости
		GameObject imageDetectionSign,
		List<Sprite> detectionSignFrames,
		Camera playerCamera)
	{
		_npcDetectionManager = npcDetectionManager;

		// Берем RectTransform именно у картинки, а не у Canvas
		_imageRectTransform = imageDetectionSign.GetComponent<RectTransform>();

		_imageComponentDetectionSign = imageDetectionSign.GetComponent<Image>();
		_detectionSignFrames = detectionSignFrames;
		_playerCamera = playerCamera;

		markerHeight = _imageRectTransform.rect.height;

		Debug.Log("[NPC Sign] Initialized. Frames count: " + _detectionSignFrames.Count);

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
		Vector3 targetPosition = transform.position + new Vector3(0f, 2.2f, 0f);
		Vector3 screenPoint = _playerCamera.WorldToViewportPoint(targetPosition);

		Debug.DrawLine(_playerCamera.transform.position, transform.position, Color.cyan);

		if (screenPoint.z <= 0)
		{
			Debug.LogWarning("[NPC Sign] Target is behind the camera or too close.");
			return;
		}

		bool isOnScreenX = screenPoint.x >= 0 && screenPoint.x <= 1;
		bool isOnScreenY = screenPoint.y >= 0 && screenPoint.y <= 1;

		string xState = isOnScreenX ? "Inside" : "Outside";
		string yState = isOnScreenY ? "Inside" : "Outside";
		Debug.Log($"[NPC Sign] Viewport: ({screenPoint.x:F2}, {screenPoint.y:F2}). X: {xState}. Y: {yState}");

		float xPos;
		if (!isOnScreenX)
		{
			if (screenPoint.x < 0.5f)
				xPos = markerOffset;
			else
				xPos = Screen.width - markerOffset;
		}
		else
		{
			xPos = screenPoint.x * Screen.width;
		}

		float yPos;
		if (!isOnScreenY)
		{
			if (screenPoint.y < 0.5f)
				yPos = markerOffset;
			else
				yPos = Screen.height - markerOffset - markerHeight;
		}
		else
		{
			yPos = screenPoint.y * Screen.height;
		}

		// Применяем позицию к картинке, а не к Canvas
		_imageRectTransform.position = new Vector3(xPos, yPos, 0f);

		Debug.Log($"[NPC Sign] Final UI Pos: ({xPos:F0}, {yPos:F0})");
	}

	private void UpdateSpriteByMeter(float meterValue)
	{
		bool shouldShow = meterValue > 0f;
		_imageComponentDetectionSign.gameObject.SetActive(shouldShow); // Используем gameObject от Image

		if (!shouldShow || _detectionSignFrames.Count == 0)
		{
			Debug.LogWarning("[NPC Sign] Tried to update sprite, but frames are missing or value is 0.");
			return;
		}

		float normalizedValue = Mathf.Clamp01(meterValue / 100f);
		int frameIndex = Mathf.RoundToInt(normalizedValue * (_detectionSignFrames.Count - 1));

		Debug.Log($"[NPC Sign] Meter: {meterValue:F1} -> Frame Index: {frameIndex}/{_detectionSignFrames.Count - 1} ({_detectionSignFrames[frameIndex].name})");

		_imageComponentDetectionSign.sprite = _detectionSignFrames[frameIndex];
	}
}