using UnityEngine;
using System.Collections;

public class NPCvfxEffectsController : MonoBehaviour
{
	private MeshRenderer[] _NPCvfxEffectMeshRenderers;

	public void Initialize(MeshRenderer[] NPCvfxEffectMeshRenderers)
	{
		_NPCvfxEffectMeshRenderers = NPCvfxEffectMeshRenderers;
	}

	public void ShowElectifiedVFX()
	{
		StopAllCoroutines();
		StartCoroutine(ElectrifyRoutine());
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