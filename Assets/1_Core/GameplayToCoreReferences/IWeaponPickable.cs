public interface IWeaponPickable
{
	public string WeaponRightMouseButtonAttackMessage { get; }
	public string WeaponLeftMouseButtonAttackMessage { get; }
	void AttackRight();
	void AttackLeft();
}