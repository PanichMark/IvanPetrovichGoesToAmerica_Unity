using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransferSkinnedMeshRendererArmatureBones : MonoBehaviour
{
	[SerializeField] private Transform _baseArmatureRootBone;
	[SerializeField] private GameObject _meshTorso;
	[SerializeField] private bool _baseMeshTorso = false;
	[SerializeField] private List<GameObject> _meshesLimbs;

	private SkinnedMeshRenderer _torsoSkinnedMeshRenderer;
	private Transform[] _torsoMeshBones;
	private Transform[] _baseArmatureBonesTransform;
	private Dictionary<string, Transform> _baseArmatureBoneNames ;

	private void Start()
	{
		_baseArmatureBonesTransform = _baseArmatureRootBone.GetComponentsInChildren<Transform>();
		_baseArmatureBoneNames = new Dictionary<string, Transform>();

		foreach (var bone in _baseArmatureBonesTransform)
		{
			_baseArmatureBoneNames[bone.name] = bone;
		}

		if (_meshTorso != null)
		{
			_torsoSkinnedMeshRenderer = _meshTorso.GetComponent<SkinnedMeshRenderer>();
			_torsoMeshBones = new Transform[_torsoSkinnedMeshRenderer.bones.Length];

			if (_baseMeshTorso)
			{
				var originalMesh = _torsoSkinnedMeshRenderer.sharedMesh;
				Debug.Log($"[AssignMeshes] Копируем меш '{_meshTorso.name}'...");

				var newMesh = Instantiate(originalMesh);
				_torsoSkinnedMeshRenderer.sharedMesh = newMesh;

				Matrix4x4[] bindPoses = new Matrix4x4[_torsoMeshBones.Length];
				bool hasErrors = false;

				for (int i = 0; i < _torsoMeshBones.Length; i++)
				{
					// Защита от пустых ссылок внутри массива bones самого рендерера
					if (_torsoSkinnedMeshRenderer.bones[i] == null) continue;

					string targetName = _torsoSkinnedMeshRenderer.bones[i].name.Trim();

					Transform foundBone = null;

					// Пытаемся найти кость сначала точным совпадением, затем через Trim() и ToLower()
					if (!_baseArmatureBoneNames.TryGetValue(targetName, out foundBone))
					{
						foreach (var kvp in _baseArmatureBoneNames)
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
						_torsoMeshBones[i] = foundBone;
						bindPoses[i] = foundBone.worldToLocalMatrix * transform.localToWorldMatrix;
					}
					else
					{
						Debug.LogError($"[AssignMeshes] КОСТЬ НЕ НАЙДЕНА: '{targetName}'. Доступные примеры в базе: " +
										$"{string.Join(", ", _baseArmatureBoneNames.Keys.Take(5))}...");
						bindPoses[i] = transform.localToWorldMatrix;
						hasErrors = true;
					}
				}

				if (!hasErrors)
				{
					newMesh.bindposes = bindPoses;
					//Resources.UnloadAsset(originalMesh);
					Debug.Log($"[AssignMeshes] Меш '{_meshTorso.name}' успешно переназначен.");
				}
			}

			for (int i = 0; i < _torsoSkinnedMeshRenderer.bones.Length; i++)
			{
				_torsoMeshBones[i] = _baseArmatureBoneNames[_torsoSkinnedMeshRenderer.bones[i].name];
			}

			_torsoSkinnedMeshRenderer.bones = _torsoMeshBones;
			_torsoSkinnedMeshRenderer.rootBone = _baseArmatureRootBone;

			var armature = _meshTorso.transform.parent.Find("Armature_Humanoid");
			if (armature != null)
			{
				Destroy(armature.gameObject);
			}
		}

		for (int index = 0; index < _meshesLimbs.Count; index++)
		{
			var limbsSkinnedMeshRenderer = _meshesLimbs[index].GetComponent<SkinnedMeshRenderer>();

			var limbsMeshBones = new Transform[limbsSkinnedMeshRenderer.bones.Length];

			for (int i = 0; i < limbsSkinnedMeshRenderer.bones.Length; i++)
			{
				limbsMeshBones[i] = _baseArmatureBoneNames[limbsSkinnedMeshRenderer.bones[i].name];

				if (_meshTorso != null)
				{ 
					_torsoMeshBones[i] = _baseArmatureBoneNames[_torsoSkinnedMeshRenderer.bones[i].name];
				}
			}

			limbsSkinnedMeshRenderer.bones = limbsMeshBones;
			limbsSkinnedMeshRenderer.rootBone = _baseArmatureRootBone;

			var armature = _meshesLimbs[index].transform.parent.Find("Armature_Humanoid");
			if (armature != null)
			{
				Destroy(armature.gameObject);
			}
		}
	}

	public void TransferWeaponEugenicBones(GameObject eugenicArmature, SkinnedMeshRenderer eugenicSkinnedMesh, WeaponHandsEnum weaponHand)
	{
		var _eugenicMeshBones = new Transform[eugenicSkinnedMesh.bones.Length];

		for (int i = 0; i < eugenicSkinnedMesh.bones.Length; i++)
		{
			_eugenicMeshBones[i] = _baseArmatureBoneNames[eugenicSkinnedMesh.bones[i].name];
		}

		eugenicSkinnedMesh.bones = _eugenicMeshBones;
		eugenicSkinnedMesh.rootBone = _baseArmatureRootBone;

		Destroy(eugenicArmature.gameObject);
	}
}