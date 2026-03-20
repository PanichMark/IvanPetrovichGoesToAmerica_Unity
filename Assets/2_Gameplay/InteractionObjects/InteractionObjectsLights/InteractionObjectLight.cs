using UnityEngine;

public class InteractionObjectLight : MonoBehaviour, IInteractable
{
	// Перетаскиваем сюда объект, у которого нужно менять эмиссию материала
	public GameObject lightObject;
	private bool isLightTurnedOn = false; // Начальное состояние - выключено

	// Закешированный материал для быстрого доступа
	private Material cachedMaterial;

	// Цвет свечения при включении (можно настроить в инспекторе)
	public Color emissionColorOn = Color.white;

	// Свойства интерфейса IInteractable
	public string InteractionObjectNameSystem => "LightSwitch";
	public string InteractionObjectNameUI => "Свет";
	public string InteractionHintMessageMain => $"Включить/выключить {InteractionObjectNameUI}";
	public string HintAction => "Взаимодействие";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;

	public string InteractionHintAction => "";

	// Кешируем материал при старте сцены и устанавливаем начальное состояние
	void Start()
	{
		cachedMaterial = lightObject.GetComponent<Renderer>().material;
		// Выключаем свет при старте
		cachedMaterial.SetColor("_EmissionColor", Color.black);
	}

	// Метод взаимодействия: переключает эмиссию материала
	public void Interact()
	{
		if (isLightTurnedOn)
		{
			isLightTurnedOn = false;
			cachedMaterial.SetColor("_EmissionColor", Color.black);
		}
		else
		{
			isLightTurnedOn = true;
			cachedMaterial.SetColor("_EmissionColor", emissionColorOn);
		}
	}
}