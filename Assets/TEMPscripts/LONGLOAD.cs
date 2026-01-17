using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LONGLOAD : MonoBehaviour
{
	public float activationDelay = 5f; // Задержка перед активацией основных объектов

	void Start()
	{
		// Деактивируем основной объект (если нужно)
		gameObject.SetActive(false);

		// Задержка перед активацией
		Invoke(nameof(ActivateObjects), activationDelay);
	}

	void ActivateObjects()
	{
		// Реактивация объектов
		gameObject.SetActive(true);

		// Опционально: делаем видимым Canvas или другие UI-элементы
		GetComponentInChildren<Canvas>()?.gameObject.SetActive(true);
	}
}

