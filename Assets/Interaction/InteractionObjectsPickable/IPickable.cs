public interface IPickable
{
	public bool IsObjectPickedUp { get; }
	void PickUpObject();

	void DropOffObject();
}

