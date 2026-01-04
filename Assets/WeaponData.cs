using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
	public GameObject WeaponPrefab;
	public string WeaponName;
	public Sprite WeaponIcon;
}
