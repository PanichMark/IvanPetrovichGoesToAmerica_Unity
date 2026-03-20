using UnityEngine;
using System.Collections.Generic;

public class InteractionObjectLightSwitch : MonoBehaviour, IInteractable
{
	[SerializeField] private List<GameObject> lightsList = new List<GameObject>();
	[SerializeField] private bool isButtonActivate = true; // true = только включает, false = только выключает

	public Color emissionColorOn = Color.white;

	// --- Свойства интерфейса IInteractable ---
	public string InteractionObjectNameSystem => "LightSwitch";
	public string InteractionObjectNameUI => "Свет";
	public string InteractionHintMessageMain => $"Включить/выключить {InteractionObjectNameUI}";
	public string HintAction => "Взаимодействие";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;
	public string InteractionHintAction => "";

	// Кэшируем материалы и рендереры для быстрого доступа
	private List<Material> cachedMaterials = new List<Material>();

	void Start()
	{
		// Очищаем списки на случай повторного вызова
		cachedMaterials.Clear();

		foreach (var obj in lightsList)
		{
			if (obj == null) continue;

			Renderer renderer = obj.GetComponent<Renderer>();
			if (renderer != null)
			{
				// Кэшируем ИСХОДНЫЙ материал объекта
				cachedMaterials.Add(renderer.material);


			}
		}
	}

	public void Interact()
	{
		// Определяем действие на основе флага
		bool shouldTurnOn = isButtonActivate;
		bool shouldTurnOff = !isButtonActivate;

		for (int i = 0; i < cachedMaterials.Count; i++)
		{
			if (cachedMaterials[i] == null) continue;

			if (shouldTurnOn)
			{
				cachedMaterials[i].SetColor("_EmissionColor", emissionColorOn);
			}
			else if (shouldTurnOff)
			{
				cachedMaterials[i].SetColor("_EmissionColor", Color.black);
			}

			// Обновляем GI. Мы знаем, что у i-го объекта есть Renderer, так как он был в списке.
			lightsList[i].GetComponent<Renderer>().UpdateGIMaterials();
		}
	}
}