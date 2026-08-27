using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectOpenableDoorFoldableController : InteractionObjectOpenableDoorUndestructable
{


	private InteractionObjectOpenableDoorFoldablePart _parentComponent;
	private InteractionObjectOpenableDoorFoldablePart _childComponent;

	private Transform _partParentTransform;
	private Transform _partChildTransform;

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


		_parentComponent = doorParts[0].gameObject.GetComponent<InteractionObjectOpenableDoorFoldablePart>();
		_childComponent = doorParts[1].gameObject.GetComponent<InteractionObjectOpenableDoorFoldablePart>();

		_parentComponent.Initialze();
		_childComponent.Initialze();

		// Присваиваем ссылки. Порядок может быть любым, главное - сохранить их для анимации.
		_partParentTransform = doorParts[0];
		_partChildTransform = doorParts[1];

		// Запоминаем начальные вращения
		_closedRotationParent = _partParentTransform.localRotation;
		_closedRotationChild = _partChildTransform.localRotation;
	}

	public override void InteractCutscene()
	{
		//throw new System.NotImplementedException();
	}

	public override void SetDoorToOpenedPosition()
	{
	
			Quaternion targetParent = _closedRotationParent * Quaternion.Euler(0, _doorOpenAngle, 0);
			Quaternion targetChild = _closedRotationChild * Quaternion.Euler(0, _doorOpenAngle * 1.8f, 0);

			_partParentTransform.localRotation = targetParent;
			_partChildTransform.localRotation = targetChild;
		
	}

	public override void SetDoorToClosedPosition()
	{

			_partParentTransform.localRotation = _closedRotationParent;
			_partChildTransform.localRotation = _closedRotationChild;
		
	}


	protected override IEnumerator OpenDoor()
	{
		Quaternion startParent = _closedRotationParent;
		Quaternion startChild = _closedRotationChild;

		Quaternion targetParent = startParent * Quaternion.Euler(0, _doorOpenAngle, 0);
		Quaternion targetChild = startChild * Quaternion.Euler(0, _doorOpenAngle * 1.8f, 0);

		float elapsedTime = 0f;
		float duration = _doorOpenAngle / _doorOpeningSpeed;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			_partParentTransform.localRotation = Quaternion.LerpUnclamped(startParent, targetParent, t);
			_partChildTransform.localRotation = Quaternion.LerpUnclamped(startChild, targetChild, t);
			yield return null;
		}

		SetDoorToOpenedPosition();
		_currentAnimation = null;
	}

	protected override IEnumerator CloseDoor()
	{
		_isObjectOpened = false;

		Quaternion startParent = _partParentTransform.localRotation;
		Quaternion startChild = _partChildTransform.localRotation;

		Quaternion targetRotationParent = _closedRotationParent;
		Quaternion targetRotationChild = _closedRotationChild;

		float elapsedTime = 0f;
		float duration = _doorOpenAngle / _doorOpeningSpeed;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			_partParentTransform.localRotation = Quaternion.LerpUnclamped(startParent, targetRotationParent, t);
			_partChildTransform.localRotation = Quaternion.LerpUnclamped(startChild, targetRotationChild, t);
			yield return null;
		}

		SetDoorToClosedPosition();
		_currentAnimation = null;
	}
}