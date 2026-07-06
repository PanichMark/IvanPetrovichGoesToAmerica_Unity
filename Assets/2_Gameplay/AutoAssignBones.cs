using System.Collections.Generic;
using UnityEngine;

public class AssignMeshes : MonoBehaviour
{
	[SerializeField] private Transform baseArmature;
	[SerializeField] private List<GameObject> meshes;

	private void Start()
	{
		var baseArmatureBonesTransform = baseArmature.GetComponentsInChildren<Transform>();

		var baseArmatureBoneNames = new Dictionary<string, Transform>();
		foreach (var bone in baseArmatureBonesTransform)
		{
			baseArmatureBoneNames[bone.name] = bone;
		}

		for (int index = 0; index < meshes.Count; index++)
		{
			var skinnedMeshRenderer = meshes[index].GetComponent<SkinnedMeshRenderer>();

			if (index == 0)
			{
				Debug.Log(meshes[index].name);
			}

			var modularMeshBones = new Transform[skinnedMeshRenderer.bones.Length];
			for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
			{
				modularMeshBones[i] = baseArmatureBoneNames[skinnedMeshRenderer.bones[i].name];
			}

			skinnedMeshRenderer.bones = modularMeshBones;
			skinnedMeshRenderer.rootBone = baseArmature;

			Destroy(meshes[index].transform.parent.Find("Armature_Humanoid").gameObject);
		}
	}
}