using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCdetectionSignController : MonoBehaviour
{
	private GameObject _canvasNPCstatus;
	private GameObject _imageDetectionSign;
	private Image _imageComponentDetectionSign;
	private NPCdetectionManager _NPCdetectionManager;
	private List<Sprite> _frames = new List<Sprite>();

	public void Initialize(NPCdetectionManager NPCdetectionManager, GameObject canvasNPCstatus, GameObject imageDetectionSign)
	{
		_NPCdetectionManager = NPCdetectionManager;
		_canvasNPCstatus = canvasNPCstatus;
		_imageDetectionSign = imageDetectionSign;
		_imageComponentDetectionSign = _imageDetectionSign.GetComponent<Image>();

		LoadFramesFromTexture();
		UpdateSpriteByMeter(0f);
		_NPCdetectionManager.OnMeterChanged += UpdateSpriteByMeter;
	}

	private void LoadFramesFromTexture()
	{
		_frames.Clear();
		Sprite currentSprite = _imageComponentDetectionSign.sprite;

		string baseName = currentSprite.name;
		int padding = currentSprite.packed ? 2 : 0;

		Debug.Log("[Sign] Base sprite name: " + baseName);
		Debug.Log("[Sign] Total sprites in project memory: " + Resources.FindObjectsOfTypeAll<Sprite>().Length);

		// Нативный способ получить ВСЕ куски из этой конкретной текстуры (SpriteSheet)
		Object[] rawAssets = Resources.LoadAll("", typeof(Sprite));
		foreach (var obj in rawAssets)
		{
			Sprite sprite = obj as Sprite;
			if (sprite != null && sprite.texture == currentSprite.texture)
			{
				// Проверяем, что это кусок сетки, а не целая картинка целиком
				if (sprite.rect.width < currentSprite.texture.width - padding)
				{
					_frames.Add(sprite);
				}
			}
		}

		_frames.Sort((a, b) => string.Compare(a.name, b.name));
		Debug.Log("[Sign] Frames loaded successfully. Count: " + _frames.Count);
		for (int i = 0; i < _frames.Count; i++)
		{
			Debug.Log("Frame " + i + ": " + _frames[i].name);
		}
	}

	private void OnDestroy()
	{
		_NPCdetectionManager.OnMeterChanged -= UpdateSpriteByMeter;
	}

	private void UpdateSpriteByMeter(float meterValue)
	{
		bool shouldShow = meterValue > 0f;
		_imageDetectionSign.SetActive(shouldShow);

		if (!shouldShow || _frames.Count == 0) return;

		float step = 100f / _frames.Count;
		int frameIndex = Mathf.FloorToInt(meterValue / step);

		if (frameIndex >= _frames.Count)
		{
			frameIndex = _frames.Count - 1;
		}

		Debug.Log("[Sign] Meter: " + meterValue + ". Step: " + step + ". Index: " + frameIndex + ". Sprite: " + _frames[frameIndex].name);
		_imageComponentDetectionSign.sprite = _frames[frameIndex];
	}
}