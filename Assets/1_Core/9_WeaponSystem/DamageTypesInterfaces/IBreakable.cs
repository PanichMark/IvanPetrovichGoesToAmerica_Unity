using UnityEngine;
using System.Collections;

public interface IBreakable
{
	bool CanObjectBeBroken {  get; }
	bool IsObjectDestroyed { get; }
	float CurrentDurability { get; }
	float DuribilityThreshold { get; }

	GameObject Normal3Dmodel { get; }
	GameObject Damaged3Dmodel { get; }
	GameObject Broken3Dmodel { get; }

	void TakeBreakDamage(float amount);

	void ObjectIsFullyBroken();

	IEnumerator ModelBreakingAnimation();
}