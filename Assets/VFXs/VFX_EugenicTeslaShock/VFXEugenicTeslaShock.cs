using UnityEngine;

public class VFXEugenicTeslaShock : MonoBehaviour
{
	private float scrollSpeed = 5f; // Скорость движения текстуры
	private Material material;
	private Vector2 offset;

	void Start()
	{
		Renderer renderer = GetComponent<Renderer>();
		if (renderer != null && renderer.material != null)
			material = renderer.material;

		//Destroy(gameObject, 1f); // Удалить объект через 1 секунду
	}

	void Update()
	{
		if (material == null) return;
		offset.x += Time.deltaTime * scrollSpeed;
		material.SetTextureOffset("_MainTex", offset);
	}
}
