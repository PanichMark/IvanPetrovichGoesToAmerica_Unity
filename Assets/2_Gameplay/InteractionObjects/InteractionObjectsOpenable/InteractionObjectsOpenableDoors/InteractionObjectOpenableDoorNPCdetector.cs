using UnityEngine;

public class InteractionObjectOpenableDoorNPCdetector : MonoBehaviour
{
    private InteractionObjectOpenableDoorUndestructable _doorController;
    private Collider _collider;
    private Collider _mainNPCCollider; // Кэш главного коллайдера


    public void Initialize(InteractionObjectOpenableDoorUndestructable doorController)
    {
        _doorController = doorController;
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_doorController.CanNPCopenThis)
        {
            // Ищем главный коллайдер NPC (тот, что на корне)
            if (other.transform.root.TryGetComponent<NPClivingBeing>(out _))
            {
                // Кэшируем его при первом входе
                if (_mainNPCCollider == null)
                {
                    _mainNPCCollider = other.transform.root.GetComponentInChildren<Collider>();
                   //Debug.Log($"NPC Detector Enter: {_mainNPCCollider.name}");
                    _doorController.NPCtryOpeningDoor();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_doorController.CanNPCopenThis)
        {
            // Проверяем выход ТОЛЬКО по закешированному главному коллайдеру
            if (_mainNPCCollider != null)
            {
                //Debug.Log($"NPC Detector Exit: {_mainNPCCollider.name}");
                _mainNPCCollider = null; // Сбрасываем кэш
               // Debug.Log(_doorController);
                if (_doorController.IsObjectOpened)
                {
                    _doorController.NPCtryClosingDoor();
                }
            }
        }
    }

	private void Update()
	{
        //Debug.Log(_mainNPCCollider);
	}
}