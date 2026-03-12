using UnityEngine;

[System.Serializable]
public struct AnchorPointStop
{
	public GameObject anchorPoint; // Сам объект анкорной точки
	public float waitDuration;     // Длительность ожидания
}