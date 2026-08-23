using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCdetectionSignController : MonoBehaviour
{
	private Camera _playerCamera;
	private GameObject _canvasNPCstatus;
	private GameObject _imageDetectionSign;
	private Image _imageComponentDetectionSign;
	private NPCdetectionManager _NPCdetectionManager;
	private List<Sprite> _detectionSignFrames = new List<Sprite>();
	public void Initialize(
		NPCdetectionManager npcDetectionManager,
		GameObject canvasNPCstatus,
		GameObject imageDetectionSign,
		List<Sprite> detectionSignFrames,
		Camera playerCamera)
	{
		_NPCdetectionManager = npcDetectionManager;
		_canvasNPCstatus = canvasNPCstatus;
		_imageDetectionSign = imageDetectionSign;
		_imageComponentDetectionSign = _imageDetectionSign.GetComponent<Image>();
		_detectionSignFrames = detectionSignFrames;

		//_canvasNPCstatus.GetComponent<Canvas>().worldCamera = playerCamera;
		_playerCamera = playerCamera;
		UpdateSpriteByMeter(0f);
		_NPCdetectionManager.OnMeterChanged += UpdateSpriteByMeter;
	}

	private void Update()
	{
		_canvasNPCstatus.transform.LookAt(_playerCamera.transform);
	}

	private void OnDestroy()
	{
		if (_NPCdetectionManager != null)
		{
			_NPCdetectionManager.OnMeterChanged -= UpdateSpriteByMeter;
		}
	}

	private void UpdateSpriteByMeter(float meterValue)
	{
		bool shouldShow = meterValue > 0f;
		_imageDetectionSign.SetActive(shouldShow);

		if (!shouldShow || _detectionSignFrames.Count == 0) return;

		float normalizedValue = Mathf.Clamp01(meterValue / 100f);
		int frameIndex = Mathf.RoundToInt(normalizedValue * (_detectionSignFrames.Count - 1));

		_imageComponentDetectionSign.sprite = _detectionSignFrames[frameIndex];
	}
}