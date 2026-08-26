using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectSafeDestructableController : InteractionObjectSafeUndestructableController
{
	private bool _isSafeBroken;

	private GameObject _safeBody;
	private Rigidbody _safeBodyRb;

	public bool IsFalling { get; private set; }
	private float _fallStartTime;
	[SerializeField] private float _fallSpeedThreshold;
	[SerializeField] private float _fallDurationLimit;
	[SerializeField] private float _impactForceMultiplier;
	private InteractionObjectSafeFallSensor _interactionObjectSafeFallSensor;
	protected override void InitializeSafe()
	{
		_safeBody = _safeDoor.transform.parent.gameObject;
		_safeBodyRb = _safeBody.GetComponent<Rigidbody>();
		_interactionObjectSafeFallSensor = _safeBody.GetComponent<InteractionObjectSafeFallSensor>();

		_interactionObjectSafeFallSensor.Initialize(this, _safeDoor);
	}

	void Update()
	{
		if (!_isSafeBroken)
		{
			CheckForFall();
		}
	}

	private void CheckForFall()
	{
		if (_safeBodyRb == null) return;

		float verticalVelocity = _safeBodyRb.linearVelocity.y;

		if (IsFalling)
		{
			// Сбрасываем флаг, если сейф остановился или подпрыгнул (ударился), но не сломался сразу
			if (verticalVelocity >= -0.1f || verticalVelocity > 0f)
			{
				IsFalling = false;
			}
		}
		// Начинаем отсчет времени ТОЛЬКО если скорость превысила порог вниз
		else if (verticalVelocity < -_fallSpeedThreshold)
		{
			IsFalling = true;
			_fallStartTime = Time.time;
		}
	}

	// Добавьте этот НОВЫЙ МЕТОД в класс InteractionObjectSafeController
	public void OnSafeBodyCollided(Collision collision)
	{
		//Debug.Log("SAFE BODY COLLIDED! " + collision.collider.name);

		// Ваша логика проверки времени падения остается здесь
		if (IsFalling && (Time.time - _fallStartTime) >= _fallDurationLimit)
		{
			BreakSafeFromImpact();
		}

		// Сбрасываем флаг в любом случае
		IsFalling = false;
	}

	private void BreakSafeFromImpact()
	{
		_isSafeBroken = true;
		_handleCollider.enabled = false;
		_safeDoor.transform.SetParent(null);
		_safeDoor.tag = "Interactable";
		Rigidbody doorRigidbody = _safeDoor.AddComponent<Rigidbody>();
		doorRigidbody.AddForce(transform.forward * _impactForceMultiplier, ForceMode.Impulse);
		_safeRotatorySection1.tag = "Untagged";
		_safeRotatorySection2.tag = "Untagged";
		_safeRotatorySection3.tag = "Untagged";
		gameObject.tag = "Untagged";
		Debug.Log("SAFE BROKEN!!!!");

		enabled = false;
	}

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.SafesDestructableData == null)
		{
			data.SafesDestructableData = new Dictionary<GameScenesGameplayDataEnum, List<SafeDestructableData>>();
		}
		if (!data.SafesDestructableData.ContainsKey(currentScene))
		{
			data.SafesDestructableData[currentScene] = new List<SafeDestructableData>();
		}

		var targetList = data.SafesDestructableData[currentScene];

		int indexInList = targetList.FindIndex(item => item.SafeDestructableIndex == GameplayObjectIndex);

		var updatedItem = new SafeDestructableData
		{
			SafeDestructableIndex = GameplayObjectIndex,
			SafeDestructableNameSystem = InteractionObjectNameSystem,
			IsSafeDestructableOpened = _isSafeOpened,
			IsSafeDestructableDestroyed = _isSafeBroken,
			SafeDestructableRotationSection_1_Position = _section1.currentSectionPosition,
			SafeDestructableRotationSection_2_Position = _section2.currentSectionPosition,
			SafeDestructableRotationSection_3_Position = _section3.currentSectionPosition
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.SafesDestructableData == null || !data.SafesDestructableData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.SafeDestructableIndex == GameplayObjectIndex);

		if (savedState.Equals(default(SafeDestructableData))) yield break;

		_isSafeOpened = savedState.IsSafeDestructableOpened;
		_isSafeBroken = savedState.IsSafeDestructableDestroyed;

		_section1.SetLoadedPosition(savedState.SafeDestructableRotationSection_1_Position);
		_section2.SetLoadedPosition(savedState.SafeDestructableRotationSection_2_Position);
		_section3.SetLoadedPosition(savedState.SafeDestructableRotationSection_3_Position);

		if (_isSafeOpened)
		{
			gameObject.tag = "Untagged";
			_safeRotatorySection1.tag = "Untagged";
			_safeRotatorySection2.tag = "Untagged";
			_safeRotatorySection3.tag = "Untagged";

			if (!_isSafeBroken)
			{
				_safeDoorTransform.localRotation = _safeDoorOpenedPosition;
			}
		}

		if (_isSafeBroken)
		{
			_handleCollider.enabled = false;
			_safeDoor.transform.SetParent(null);
			_safeDoor.tag = "Interactable";
			Rigidbody doorRigidbody = _safeDoor.AddComponent<Rigidbody>();
			gameObject.tag = "Untagged";
			_safeRotatorySection1.tag = "Untagged";
			_safeRotatorySection2.tag = "Untagged";
			_safeRotatorySection3.tag = "Untagged";
		}

		yield return null;
	}
}
