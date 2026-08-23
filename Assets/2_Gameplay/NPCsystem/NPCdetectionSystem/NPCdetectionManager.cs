using UnityEngine;
using System.Collections;

public class NPCdetectionManager : MonoBehaviour
{
	public delegate void DetectionMeterHandler(float currentValue);
	public event DetectionMeterHandler OnMeterChanged;
	public float NPCdetectionMeter { get; private set; }
	private Coroutine _meterRoutine;

	public void Initialize()
	{
		NPCdetectionMeter = 0f;
		OnMeterChanged?.Invoke(NPCdetectionMeter);
		if (_meterRoutine != null)
		{
			StopCoroutine(_meterRoutine);
		}
		_meterRoutine = StartCoroutine(MeterPulse());
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
				float currentValue = Mathf.Lerp(startValue, endValue, forward ? t : 1f - t);
				NPCdetectionMeter = currentValue;
				OnMeterChanged?.Invoke(NPCdetectionMeter);
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
				float currentValue = Mathf.Lerp(startValue, endValue, t);
				NPCdetectionMeter = currentValue;
				OnMeterChanged?.Invoke(NPCdetectionMeter);
				yield return null;
			}
		}
	}
}