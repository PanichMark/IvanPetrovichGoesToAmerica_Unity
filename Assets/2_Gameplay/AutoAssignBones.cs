using System;
using System.Collections.Generic;
using System.Linq;
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

			var modularMeshBones = new Transform[skinnedMeshRenderer.bones.Length];

			if (index == 0)
			{
				if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null) return;

				var originalMesh = skinnedMeshRenderer.sharedMesh;
				Debug.Log($"[AssignMeshes] Копируем меш '{meshes[index].name}'...");

				var newMesh = Instantiate(originalMesh);
				skinnedMeshRenderer.sharedMesh = newMesh;

				Matrix4x4[] bindPoses = new Matrix4x4[modularMeshBones.Length];
				bool hasErrors = false;

				for (int i = 0; i < modularMeshBones.Length; i++)
				{
					// Защита от пустых ссылок внутри массива bones самого рендерера
					if (skinnedMeshRenderer.bones[i] == null) continue;

					string targetName = skinnedMeshRenderer.bones[i].name.Trim();

					Transform foundBone = null;

					// Пытаемся найти кость сначала точным совпадением, затем через Trim() и ToLower()
					if (!baseArmatureBoneNames.TryGetValue(targetName, out foundBone))
					{
						foreach (var kvp in baseArmatureBoneNames)
						{
							if (kvp.Key.Trim().ToLower() == targetName.ToLower())
							{
								foundBone = kvp.Value;
								break;
							}
						}
					}

					if (foundBone != null)
					{
						modularMeshBones[i] = foundBone;
						bindPoses[i] = foundBone.worldToLocalMatrix * transform.localToWorldMatrix;
					}
					else
					{
						Debug.LogError($"[AssignMeshes] КОСТЬ НЕ НАЙДЕНА: '{targetName}'. Доступные примеры в базе: " +
									   $"{string.Join(", ", baseArmatureBoneNames.Keys.Take(5))}...");
						bindPoses[i] = transform.localToWorldMatrix;
						hasErrors = true;
					}
				}

				if (!hasErrors)
				{
					newMesh.bindposes = bindPoses;
					//Resources.UnloadAsset(originalMesh);
					Debug.Log($"[AssignMeshes] Меш '{meshes[index].name}' успешно переназначен.");
				}
			}

			for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
			{
				modularMeshBones[i] = baseArmatureBoneNames[skinnedMeshRenderer.bones[i].name];
			}

			skinnedMeshRenderer.bones = modularMeshBones;
			skinnedMeshRenderer.rootBone = baseArmature;

			var armature = meshes[index].transform.parent.Find("Armature_Humanoid");
			if (armature != null)
			{
				Destroy(armature.gameObject);
			}
		}
	}
}