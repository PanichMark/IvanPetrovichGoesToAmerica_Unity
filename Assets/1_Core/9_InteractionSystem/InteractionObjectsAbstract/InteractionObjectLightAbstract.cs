using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectLightAbstract : MonoBehaviour, ISaveLoad
{
	[SerializeField] protected List<GameObject> _lightObjectsList = new List<GameObject>();
	[SerializeField] protected Color _lightEmissionColor = Color.white;

	public bool IsLightTurnedOn { get; protected set; }

	private readonly List<Material> _cachedMaterials = new List<Material>();
	public int LightIndex { get; protected set; }
	protected virtual void Start()
	{
		CacheMaterials();
	}

	protected void CacheMaterials()
	{
		_cachedMaterials.Clear();
		foreach (var obj in _lightObjectsList)
		{
			if (obj == null) continue;
			var renderer = obj.GetComponent<Renderer>();
			if (renderer != null)
			{
				_cachedMaterials.Add(renderer.material);
			}
		}
	}

	public void AssignLightsIndexes(int index)
	{
		LightIndex = index;
	}

	public virtual void TurnOn()
	{
	}

	public virtual void TurnOff()
	{
	}

	protected void ApplyEmission(Color color)
	{
		for (int i = 0; i < _cachedMaterials.Count; i++)
		{
			if (_cachedMaterials[i] == null) continue;
			_cachedMaterials[i].SetColor("_EmissionColor", color);
			_cachedMaterials[i].DisableKeyword("_EMISSION");
			_cachedMaterials[i].EnableKeyword("_EMISSION");
			_cachedMaterials[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		}
	}

	public void SaveData(ref GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		if (data.LightsData == null || !data.LightsData.ContainsKey(currentScene))
			return;

		var targetList = data.LightsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.LightIndex == LightIndex);

		var updatedItem = new LightData
		{
			LightIndex = LightIndex,
			LightNameSystem = name,
			IsLightTurnedOn = IsLightTurnedOn
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}
	}

	public void LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		if (data.LightsData == null || !data.LightsData.TryGetValue(currentScene, out var sourceList)) return;

		var savedState = sourceList.Find(item => item.LightIndex == LightIndex);

		if (savedState.Equals(default(LightData))) return;

		if (savedState.IsLightTurnedOn)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}
}