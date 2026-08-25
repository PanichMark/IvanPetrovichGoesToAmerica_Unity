using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractionObjectLightSwitchController : GameplayObjectSaveLoad
{
	[SerializeField] protected List<GameObject> _lightObjectsList = new List<GameObject>();
	[SerializeField] protected Color _lightEmissionColor = Color.white;

	public bool IsLightTurnedOn { get; protected set; }

	private readonly List<Material> _cachedMaterials = new List<Material>();
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

	public void TurnOn()
	{
		IsLightTurnedOn = true;
		ApplyEmission(_lightEmissionColor);
	}

	public  void TurnOff()
	{
		IsLightTurnedOn = false;
		ApplyEmission(Color.black);
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

	public override IEnumerator SaveData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.LightsData == null || !data.LightsData.ContainsKey(currentScene))
			yield break;

		var targetList = data.LightsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.LightIndex == GameplayObjectIndex);

		var updatedItem = new LightData
		{
			LightIndex = GameplayObjectIndex,
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

		yield return null;
	}

	public override IEnumerator LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.LightsData == null || !data.LightsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.LightIndex == GameplayObjectIndex);

		if (savedState.Equals(default(LightData))) yield break;

		if (savedState.IsLightTurnedOn)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}

		yield return null;
	}
}