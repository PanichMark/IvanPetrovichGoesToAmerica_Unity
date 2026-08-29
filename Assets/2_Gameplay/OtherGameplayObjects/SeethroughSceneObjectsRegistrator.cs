using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeethroughSceneObjectsRegistrator : MonoBehaviour
{
	List<GameObject> _seethroughSceneObject = new List<GameObject>();

	public List<GameObject> SeethroughSceneObject => _seethroughSceneObject;

	void Start()
	{
		RegisterSceneSeethoughObjects();

		Debug.Log(_seethroughSceneObject.Count);
	}

	private void RegisterSceneSeethoughObjects()
	{
		_seethroughSceneObject.Clear();

		// Unity сам найдет все активные компоненты, которые являются NPCabstract или его наследниками
		NPCabstract[] allNpcs = FindObjectsOfType<NPCabstract>(true);

		foreach (var npc in allNpcs)
		{
			if (npc != null)
			{
				_seethroughSceneObject.Add(npc.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		_seethroughSceneObject.Clear();
	}

	/*
	private void RegisterSceneSeethoughObjects()
	{
		_seethoughSceneObject.Clear();

		IInteractable[] components = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>().ToArray();

		foreach (IInteractable seethoughObjects in components)
		{
			if (seethoughObjects != null && seethoughObjects is Component comp)
			{
				_seethoughSceneObject.Add(comp.gameObject);
			}
		}
	}
	*/

}