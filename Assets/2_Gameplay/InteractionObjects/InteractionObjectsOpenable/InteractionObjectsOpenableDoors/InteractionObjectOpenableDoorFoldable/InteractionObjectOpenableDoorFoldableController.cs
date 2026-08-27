using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectOpenableDoorFoldableController : InteractionObjectOpenableDoorUndestructable
{

	// Ссылки на части двери, которые мы найдем сами
	private Transform _partParent;
	private Transform _partChild;

	// Сохраняем закрытые позиции
	private Quaternion _closedRotationParent;
	private Quaternion _closedRotationChild;


	public override void InitializeDoor()
	{

	

		// Находим все компоненты-трансформы у себя в детях (на любом уровне вложенности)
		Transform[] allChildren = GetComponentsInChildren<Transform>();

		// Игнорируем сам корневой объект (this.transform), оставляем только настоящих детей
		List<Transform> doorParts = new List<Transform>();
		foreach (var child in allChildren)
		{
			if (child != transform) // Пропускаем сам Door_Root
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

	public override void InteractCutscene()
	{
		//throw new System.NotImplementedException();
	}

	public override void SetDoorToOpenedPosition()
	{
	
			Quaternion targetParent = _closedRotationParent * Quaternion.Euler(0, _doorOpenAngle, 0);
			Quaternion targetChild = _closedRotationChild * Quaternion.Euler(0, -_doorOpenAngle * 1.8f, 0);

			_partParent.localRotation = targetParent;
			_partChild.localRotation = targetChild;
		
	}

	public override void SetDoorToClosedPosition()
	{

			_partParent.localRotation = _closedRotationParent;
			_partChild.localRotation = _closedRotationChild;
		
	}


	protected override IEnumerator OpenDoor()
	{
		Quaternion startParent = _closedRotationParent;
		Quaternion startChild = _closedRotationChild;

		Quaternion targetParent = startParent * Quaternion.Euler(0, _doorOpenAngle, 0);
		Quaternion targetChild = startChild * Quaternion.Euler(0, -_doorOpenAngle * 1.8f, 0);

		float elapsedTime = 0f;
		float duration = _doorOpenAngle / _doorOpeningSpeed;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			_partParent.localRotation = Quaternion.LerpUnclamped(startParent, targetParent, t);
			_partChild.localRotation = Quaternion.LerpUnclamped(startChild, targetChild, t);
			yield return null;
		}

		SetDoorToOpenedPosition();
		_currentAnimation = null;
	}

	protected override IEnumerator CloseDoor()
	{
		_isObjectOpened = false;

		Quaternion startParent = _partParent.localRotation;
		Quaternion startChild = _partChild.localRotation;

		Quaternion targetRotationParent = _closedRotationParent;
		Quaternion targetRotationChild = _closedRotationChild;

		float elapsedTime = 0f;
		float duration = _doorOpenAngle / _doorOpeningSpeed;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			_partParent.localRotation = Quaternion.LerpUnclamped(startParent, targetRotationParent, t);
			_partChild.localRotation = Quaternion.LerpUnclamped(startChild, targetRotationChild, t);
			yield return null;
		}

		SetDoorToClosedPosition();
		_currentAnimation = null;
	}
}