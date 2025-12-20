using UnityEngine;
public class PlayerCapluseCollider : MonoBehaviour
{
    //public Transform playerTransform;
    //public GameObject player;

    PlayerMovementController playerMovementController;
    void Start()
    {
		playerMovementController = transform.parent.GetComponent<PlayerMovementController>();
	}
    void Update()
    {
		if (playerMovementController.IsPlayerCrouching == true)
        {
            transform.position = transform.parent.position+new Vector3(0f, 0.5f, 0f); ;
            transform.localScale = new Vector3(1f,  0.5f, 1f);
        }

		if (playerMovementController.IsPlayerCrouching == false)
		{
			transform.position = transform.parent.position+new Vector3(0f, 1f, 0f);
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}
}

