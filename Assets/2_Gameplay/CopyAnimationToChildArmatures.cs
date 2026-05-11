using System.Collections.Generic;
using UnityEngine;

public class CopyAnimationToChildArmatures : MonoBehaviour
{
	[SerializeField] private Transform _parentArmatureRoot;
	[SerializeField] private List<Transform> _childArmatureRoots = new List<Transform>(); 

	private List<Dictionary<string, Transform>> _childBonesMaps = new List<Dictionary<string, Transform>>();

	void Start()
	{
		BuildChildBonesMaps();
	}

	void LateUpdate()
	{
		if (!_parentArmatureRoot || _childArmatureRoots.Count == 0)
			return;

		Transform[] parentBones = _parentArmatureRoot.GetComponentsInChildren<Transform>(true);

		for (int i = 0; i < _childArmatureRoots.Count; i++)
		{
			Transform childRoot = _childArmatureRoots[i];
			Dictionary<string, Transform> childBonesMap = _childBonesMaps[i];

			if (!childRoot)
				continue;

			childRoot.position = _parentArmatureRoot.position;
			childRoot.rotation = _parentArmatureRoot.rotation;

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
		foreach (var childRoot in _childArmatureRoots)
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