using UnityEngine;
using System.Collections;

public class NPCvfxEffectsController : MonoBehaviour
{
	private MeshRenderer[] _NPCvfxEffectMeshRenderers;
	private NPCstateMachineController _NPCstateMachineController;

	public void Initialize(
		NPCstateMachineController npcstateMachineController,
		MeshRenderer[] NPCvfxEffectMeshRenderers)
	{
		_NPCstateMachineController = npcstateMachineController;
		_NPCvfxEffectMeshRenderers = NPCvfxEffectMeshRenderers;

		_NPCstateMachineController.OnNewNPCstate += ShowElectifiedVFX;
	}

	public void ShowElectifiedVFX(NPCstateTypes newNPCstate)
	{
		if (newNPCstate == NPCstateTypes.ElectroShocked)
		{
			StopAllCoroutines();
			StartCoroutine(ElectrifyRoutine());
		}
	}

	private IEnumerator ElectrifyRoutine()
	{
		foreach (var renderer in _NPCvfxEffectMeshRenderers)
		{
			renderer.enabled = true;
		}

		yield return new WaitForSeconds(2f);

		foreach (var renderer in _NPCvfxEffectMeshRenderers)
		{
			renderer.enabled = false;
		}
	}
}