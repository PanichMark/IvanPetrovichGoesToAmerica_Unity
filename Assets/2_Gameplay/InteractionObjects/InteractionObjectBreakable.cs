using UnityEngine;

public class InteractionObjectBreakable : MonoBehaviour, IBreakable
{
	public bool IsObjectBroken => throw new System.NotImplementedException();

	public float CurrentDurability => throw new System.NotImplementedException();

	public float DamageThreshold => throw new System.NotImplementedException();

	public void ObjectIsFullyBroken()
	{
		throw new System.NotImplementedException();
	}

	public void TakeDamage(float amount)
	{
		throw new System.NotImplementedException();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
