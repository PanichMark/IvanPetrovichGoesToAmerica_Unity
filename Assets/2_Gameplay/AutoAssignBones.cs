using System.Collections.Generic;
using UnityEngine;

public class AssignMeshes : MonoBehaviour
{
	// Базовая арматура (Root Bone)
	[SerializeField] private Transform baseArmature;

	// Список игровых объектов с мешами
	[SerializeField] private List<GameObject> meshes;

	private void Start()
	{
		if (baseArmature == null)
		{
			Debug.LogError("Не указана базовая арматура!");
			return;
		}

		if (meshes.Count == 0)
		{
			Debug.LogError("Нет объектов с мешами!");
			return;
		}

		// Собираем все кости в базе
		var allBones = baseArmature.GetComponentsInChildren<Transform>();

		// Строим словарь соответствий
		var boneMap = new System.Collections.Generic.Dictionary<string, Transform>();
		foreach (var bone in allBones)
		{
			boneMap[bone.name] = bone;
		}

		// Перепривязываем все меши
		foreach (var go in meshes)
		{
			var smr = go.GetComponent<SkinnedMeshRenderer>();
			if (smr == null)
			{
				Debug.LogWarning($"{go.name} не имеет SMR. Пропускаю.");
				continue;
			}

			// Создаем новый массив костей
			var newBones = new Transform[smr.bones.Length];
			for (int i = 0; i < smr.bones.Length; i++)
			{
				var oldBone = smr.bones[i];
				if (boneMap.TryGetValue(oldBone.name, out var newBone))
				{
					newBones[i] = newBone;
				}
				else
				{
					Debug.LogWarning($"Кость '{oldBone.name}' не найдена в базе. Оставляю старую.");
					newBones[i] = oldBone;
				}
			}

			smr.bones = newBones;
			smr.rootBone = baseArmature;
		}
	}
}