using System.Collections.Generic;
using UnityEngine;

public class CopyAnimationToChildArmatures : MonoBehaviour
{
	public Transform parentArmatureRoot;
	public List<Transform> childArmatureRoots = new List<Transform>(); 

	private List<Dictionary<string, Transform>> _childBonesMaps = new List<Dictionary<string, Transform>>();

	void Start()
	{
		BuildChildBonesMaps();
	}

	void LateUpdate()
	{
		if (!parentArmatureRoot || childArmatureRoots.Count == 0)
			return;

		Transform[] parentBones = parentArmatureRoot.GetComponentsInChildren<Transform>(true);

		for (int i = 0; i < childArmatureRoots.Count; i++)
		{
			Transform childRoot = childArmatureRoots[i];
			Dictionary<string, Transform> childBonesMap = _childBonesMaps[i];

			if (!childRoot)
				continue;

			childRoot.position = parentArmatureRoot.position;
			childRoot.rotation = parentArmatureRoot.rotation;

			foreach (Transform bone in parentBones)
			{
				string name = bone.name;
				if (childBonesMap.TryGetValue(name, out var childBone))
				{
					childBone.localPosition = bone.localPosition;
					childBone.localRotation = bone.localRotation;
				}
			}
		}
	}

	private void BuildChildBonesMaps()
	{
		_childBonesMaps.Clear();
		foreach (var childRoot in childArmatureRoots)
		{
			if (childRoot == null) continue;

			var bonesMap = new Dictionary<string, Transform>();
			foreach (var transform in childRoot.GetComponentsInChildren<Transform>(true))
			{
				bonesMap[transform.name] = transform;
			}
			_childBonesMaps.Add(bonesMap);
		}
	}
}