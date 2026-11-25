public interface IPickable
{
	public bool IsObjectPickedUp { get; }
	void PickUpObject(bool isPickedUpByLoadSafeFile);
	void DropOffObject();

	public InteractionObjectsPickableTypes PickableType { get; }
}