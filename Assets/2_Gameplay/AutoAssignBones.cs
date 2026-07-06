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

			// --- Блок для индекса 0: Запекание деформации ---
			if (index != 2)
			{
				BakeDeformationAndReplace(skinnedMeshRenderer);

				var modularMeshBones = new Transform[skinnedMeshRenderer.bones.Length];
				for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
				{
					modularMeshBones[i] = baseArmatureBoneNames[skinnedMeshRenderer.bones[i].name];
				}

				skinnedMeshRenderer.bones = modularMeshBones;
				skinnedMeshRenderer.rootBone = baseArmature;
			}
			// --- Блок для остальных индексов: Переназначение костей из словаря ---
			else
			{
				var modularMeshBones = new Transform[skinnedMeshRenderer.bones.Length];
				for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
				{
					modularMeshBones[i] = baseArmatureBoneNames[skinnedMeshRenderer.bones[i].name];
				}

				skinnedMeshRenderer.bones = modularMeshBones;
				skinnedMeshRenderer.rootBone = baseArmature;
			}
		}
	}

	/// <summary>
	/// Запекает текущую форму меша и заменяет SkinnedMeshRenderer на обычный MeshRenderer
	/// </summary>
	private void BakeDeformationAndReplace(SkinnedMeshRenderer smr)
	{
		// Создаем новый меш, вершины которого вычислены по текущим матрицам костей
		Mesh bakedMesh = new Mesh();
		smr.BakeMesh(bakedMesh);

		// Копируем материалы, чтобы сохранить внешний вид
		Material[] sharedMaterials = smr.sharedMaterials;

		// Отключаем компонент скининга
		smr.enabled = false;

		// Добавляем обычные компоненты рендера
		MeshFilter meshFilter = smr.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = smr.gameObject.AddComponent<MeshRenderer>();

		// Назначаем запеченный меш и материалы
		meshFilter.mesh = bakedMesh;
		meshRenderer.sharedMaterials = sharedMaterials;
	}

}