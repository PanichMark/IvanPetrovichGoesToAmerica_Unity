using System.Collections.Generic;
using UnityEngine;

public class BulletHoleManager : MonoBehaviour
{
	private Sprite _decalSpriteDefault;
	private Sprite _decalSpriteDamageable;
	private bool _isBloodVisible;
	private int _maxInstances = 50;
	private List<SpriteRenderer> _decalList = new List<SpriteRenderer>();
	private Transform _decalParent;
	private int _currentIndex = 0;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;

	private GameScenesManager _gameSceneManager;

	public void Initialize(GameScenesManager gameSceneManager, PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController)
	{
		_gameSceneManager = gameSceneManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;

		_decalSpriteDefault = Resources.Load<Sprite>("Sprites/Sprites_BulletHoles/Sprite_BulletHole_Solid");
		_decalSpriteDamageable = Resources.Load<Sprite>("Sprites/Sprites_BulletHoles/Sprite_BulletHole_Blood");

		if (_decalSpriteDefault == null || _decalSpriteDamageable == null)
		{
			Debug.LogError("[BulletHoleManager] Один из спрайтов не найден! Проверьте пути в Assets/Resources.");
			return;
		}
		_isBloodVisible = true;
		RecreatePool();

		_gameSceneManager.OnBeginLoadingGameplayScene += RecreatePool;
		_gameSceneManager.OnBeginLoadingMainMenuScene += RecreatePool;

		_pauseSubMenuSettingsSectionGeneralController.OnShowBlood += ShowBloodDecals;
		_pauseSubMenuSettingsSectionGeneralController.OnHideBlood += HideBloodDecals;

		Debug.Log("BulletHoleManager Initialized");
	}

	private void RecreatePool()
	{
		if (_decalParent != null)
		{
			Destroy(_decalParent.gameObject);
		}

		_decalList = new List<SpriteRenderer>();

		_decalParent = new GameObject("Decals").transform;

		for (int i = 0; i < _maxInstances; i++)
		{
			GameObject go = new GameObject($"Pooled_Decal_{i}", typeof(SpriteRenderer));
			go.transform.SetParent(_decalParent);

			SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
			sr.sprite = _decalSpriteDefault;
			sr.sortingOrder = -1;
			go.transform.localScale = Vector3.one * 0.05f;

			go.SetActive(false);

			_decalList.Add(sr);
		}

		_currentIndex = 0;
	}

	public void SpawnDecal(Vector3 position, Quaternion rotation, bool isDamageableTarget, Transform parentTransform)
	{
		if (_currentIndex < _maxInstances && _decalList[_currentIndex] != null)
		{
			SpriteRenderer sr = _decalList[_currentIndex];
			sr.gameObject.SetActive(true);
			sr.transform.position = position;
			sr.transform.rotation = rotation * Quaternion.Euler(-90f, 0, 0);
			sr.transform.Translate(0, 0, 0.01f, Space.Self);

			if (isDamageableTarget)
			{
				sr.sprite = _decalSpriteDamageable;
				sr.enabled = _isBloodVisible;
			}
			else
			{
				sr.sprite = _decalSpriteDefault;
				sr.enabled = true;
			}

			sr.transform.SetParent(parentTransform);

			_currentIndex++;
			if (_currentIndex >= _maxInstances)
			{
				_currentIndex = 0;
			}
		}
	}

	public void HideBloodDecals()
	{
		_isBloodVisible = false;
		foreach (var sr in _decalList)
		{
			if (sr.gameObject.activeSelf && sr.sprite == _decalSpriteDamageable)
			{
				sr.enabled = false;
			}
		}
	}

	public void ShowBloodDecals()
	{
		_isBloodVisible = true;
		foreach (var sr in _decalList)
		{
			if (sr.gameObject.activeSelf && sr.sprite == _decalSpriteDamageable)
			{
				sr.enabled = true;
			}
		}
	}
}