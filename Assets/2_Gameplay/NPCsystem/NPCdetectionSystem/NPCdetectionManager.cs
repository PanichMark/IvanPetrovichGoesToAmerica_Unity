using UnityEngine;
using System.Collections;

public class NPCdetectionManager : MonoBehaviour
{
	// Делегат и событие теперь принимают int
	public delegate void DetectionMeterHandler(int currentValue);
	public event DetectionMeterHandler OnMeterChanged;

	// Публичное свойство возвращает только целое число
	public int NPCdetectionMeter {  get; private set; }

	private Coroutine _meterRoutine;

	private NPCstateMachineController _NPCstateMachineController;

	public void Initialize(NPCstateMachineController NPCstateMachineController)
	{
		_NPCstateMachineController = NPCstateMachineController;

		UpdateDetectionMeter(0f);

		if (_meterRoutine != null)
		{
			StopCoroutine(_meterRoutine);
		}
		//_meterRoutine = StartCoroutine(MeterPulse());
	}

	// Новый метод-обработчик, отвечающий за логику округления
	private void UpdateDetectionMeter(float rawValue)
	{
		// Mathf.CeilToInt всегда округляет к большему (потолок). 
		// Например: 45.1 -> 46, 45.9 -> 46, 46.0 -> 46.
		int newRoundedValue = Mathf.CeilToInt(rawValue);

		// Ограничиваем диапазон строго от 0 до 100
		newRoundedValue = Mathf.Clamp(newRoundedValue, 0, 100);

		// Вызываем событие только если целое число действительно изменилось
		if (newRoundedValue != NPCdetectionMeter)
		{
			NPCdetectionMeter = newRoundedValue;

			ProcessDetectionMeter();

			OnMeterChanged?.Invoke(NPCdetectionMeter);
		}
	}

	private void ProcessDetectionMeter()
	{
		if (NPCdetectionMeter >= 50)
		{
			_NPCstateMachineController.SetNPCState(NPCstateTypes.Alarmed);
		}
	}

	// Отдельный метод для увеличения значения
	public void IncreaseMeter(int amount)
	{
		if (amount == 0) return;

		//Debug.Log($"METER INCREASED BY: {NPCdetectionMeter}");

		if (_meterRoutine != null)
		{
			StopCoroutine(_meterRoutine);
			_meterRoutine = null;
		}

		int newValue = Mathf.Clamp(NPCdetectionMeter + amount, 0, 100);

		if (newValue != NPCdetectionMeter)
		{
			UpdateDetectionMeter(newValue);
		}
	}

	// Отдельный метод для уменьшения значения
	public void DecreaseMeter(int amount)
	{
		if (amount == 0) return;

		//Debug.Log($"METER DECREASED BY: {NPCdetectionMeter}");

		if (_meterRoutine != null)
		{
			StopCoroutine(_meterRoutine);
			_meterRoutine = null;
		}

		// Уменьшаем, но Clamp следит, чтобы не уйти ниже 0
		int newValue = Mathf.Clamp(NPCdetectionMeter - amount, 0, 100);

		if (newValue != NPCdetectionMeter)
		{
			UpdateDetectionMeter(newValue);
		}
	}

	private IEnumerator MeterPulse()
	{
		while (true)
		{
			float duration = 3f;
			float elapsed = 0f;
			float startValue = 0f;
			float endValue = 100f;
			bool forward = true;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / duration);

				// Считаем плавающее значение
				float currentFloatValue = Mathf.Lerp(startValue, endValue, forward ? t : 1f - t);

				// Передаем его в обработчик округления
				UpdateDetectionMeter(currentFloatValue);

				yield return null;
			}

			forward = false;
			elapsed = 0f;
			startValue = 100f;
			endValue = 0f;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / duration);
				float currentFloatValue = Mathf.Lerp(startValue, endValue, t);
				UpdateDetectionMeter(currentFloatValue);
				yield return null;
			}
		}
	}
}