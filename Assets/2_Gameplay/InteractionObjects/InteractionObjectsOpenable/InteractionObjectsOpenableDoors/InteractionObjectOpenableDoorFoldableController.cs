using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectOpenableDoorFoldableController : InteractionObjectOpenableAbstract
{
	[SerializeField] private float _openAngleParent = 106f;
	[SerializeField] private float _openAngleChild = -164f;
	[SerializeField] private float _doorOpeningSpeed = 200f; // Увеличил скорость для наглядности

	private bool _isOpened;

	// Ссылки на части двери, которые мы найдем сами
	private Transform _partParent;
	private Transform _partChild;

	// Сохраняем закрытые позиции
	private Quaternion _closedRotationParent;
	private Quaternion _closedRotationChild;

	private Coroutine _activeCoroutine;

	void Start()
	{
		// Находим все компоненты-трансформы у себя в детях (на любом уровне вложенности)
		Transform[] allChildren = GetComponentsInChildren<Transform>();

		// Игнорируем сам корневой объект (this.transform), оставляем только настоящих детей
		List<Transform> doorParts = new List<Transform>();
		foreach (var child in allChildren)
		{
			if (child != this.transform) // Пропускаем сам Door_Root
			{
				doorParts.Add(child);
			}
		}

		// Проверяем, нашли ли мы хотя бы две части
		if (doorParts.Count < 2)
		{
			Debug.LogError("У контроллера должно быть как минимум два дочерних объекта!", this);
			return;
		}

		// Присваиваем ссылки. Порядок может быть любым, главное - сохранить их для анимации.
		_partParent = doorParts[0];
		_partChild = doorParts[1];

		// Запоминаем начальные вращения
		_closedRotationParent = _partParent.localRotation;
		_closedRotationChild = _partChild.localRotation;
	}

	public override void Interact()
	{
		if (_activeCoroutine != null)
		{
			StopCoroutine(_activeCoroutine);
			_activeCoroutine = null;
		}

		_activeCoroutine = !_isOpened ? StartCoroutine(OpenDoor()) : StartCoroutine(CloseDoor());
	}

	public override void InteractCutscene()
	{
		//throw new System.NotImplementedException();
	}

	private IEnumerator OpenDoor()
	{
		_isOpened = true;

		Quaternion targetRotationParent = _closedRotationParent * Quaternion.Euler(0, _openAngleParent, 0);
		Quaternion targetRotationChild = _closedRotationChild * Quaternion.Euler(0, _openAngleChild, 0);

		float angleToRotateParent = Quaternion.Angle(_partParent.localRotation, targetRotationParent);
		float angleToRotateChild = Quaternion.Angle(_partChild.localRotation, targetRotationChild);
		float maxAngle = Mathf.Max(angleToRotateParent, angleToRotateChild);
		float duration = maxAngle / _doorOpeningSpeed;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;

			_partParent.localRotation = Quaternion.LerpUnclamped(_partParent.localRotation, targetRotationParent, elapsedTime / duration);
			_partChild.localRotation = Quaternion.LerpUnclamped(_partChild.localRotation, targetRotationChild, elapsedTime / duration);

			yield return null;
		}

		// Финальное присваивание для точности
		_partParent.localRotation = targetRotationParent;
		_partChild.localRotation = targetRotationChild;

		_activeCoroutine = null;
	}

	private IEnumerator CloseDoor()
	{
		_isOpened = false;

		Quaternion targetRotationParent = _closedRotationParent;
		Quaternion targetRotationChild = _closedRotationChild;

		float angleToRotateParent = Quaternion.Angle(_partParent.localRotation, targetRotationParent);
		float angleToRotateChild = Quaternion.Angle(_partChild.localRotation, targetRotationChild);
		float maxAngle = Mathf.Max(angleToRotateParent, angleToRotateChild);
		float duration = maxAngle / _doorOpeningSpeed;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;

			_partParent.localRotation = Quaternion.LerpUnclamped(_partParent.localRotation, targetRotationParent, elapsedTime / duration);
			_partChild.localRotation = Quaternion.LerpUnclamped(_partChild.localRotation, targetRotationChild, elapsedTime / duration);

			yield return null;
		}

		_partParent.localRotation = targetRotationParent;
		_partChild.localRotation = targetRotationChild;

		_activeCoroutine = null;
	}
}