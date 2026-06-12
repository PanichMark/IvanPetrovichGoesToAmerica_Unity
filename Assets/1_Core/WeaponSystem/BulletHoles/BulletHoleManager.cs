using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleManager : MonoBehaviour
{
	// --- ДОБАВЛЕНЫ ДВА ПОЛЯ ДЛЯ СПРАЙТОВ ---
	private Sprite _decalSpriteDefault; // Спрайт для обычных поверхностей
	private Sprite _decalSpriteDamageable; // Спрайт для объектов с IDamageable

	private int _maxInstances = 50;
	private List<SpriteRenderer> _decalList = new List<SpriteRenderer>();
	private Transform _decalParent;
	private int _currentIndex = 0;

	public void Initialize()
	{
		// Загружаем ОБА спрайта из папки Resources
		_decalSpriteDefault = Resources.Load<Sprite>("Sprites/Sprites_BulletHoles/Sprite_BulletHole_Solid");
		_decalSpriteDamageable = Resources.Load<Sprite>("Sprites/Sprites_BulletHoles/Sprite_BulletHole_Blood");

		if (_decalSpriteDefault == null || _decalSpriteDamageable == null)
		{
			Debug.LogError("[BulletHoleManager] Один из спрайтов не найден! Проверьте пути в Assets/Resources.");
			return;
		}

		_decalParent = new GameObject("Decals").transform;

		for (int i = 0; i < _maxInstances; i++)
		{
			GameObject go = new GameObject($"Pooled_Decal_{i}", typeof(SpriteRenderer));
			go.transform.SetParent(_decalParent);

			SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

			// Устанавливаем спрайт по умолчанию при создании
			sr.sprite = _decalSpriteDefault;
			sr.sortingOrder = -1;
			go.transform.localScale = Vector3.one * 0.05f;

			_decalList.Add(sr);
		}
	}

	public void SpawnDecal(Vector3 position, Quaternion rotation, bool isDamageableTarget, Transform parentTransform)
	{
		SpriteRenderer sr = _decalList[_currentIndex];

		sr.transform.position = position;
		sr.transform.rotation = rotation * Quaternion.Euler(-90f, 0, 0);
		sr.transform.Translate(0, 0, 0.01f, Space.Self);

		// Выбираем нужный спрайт
		sr.sprite = isDamageableTarget ? _decalSpriteDamageable : _decalSpriteDefault;

		// --- НОВАЯ СТРОКА ---
		// Делаем декаль дочерним объектом от цели попадания
		sr.transform.SetParent(parentTransform);

		sr.enabled = true;

		_currentIndex++;
		if (_currentIndex >= _maxInstances)
		{
			_currentIndex = 0;
		}
	}
}