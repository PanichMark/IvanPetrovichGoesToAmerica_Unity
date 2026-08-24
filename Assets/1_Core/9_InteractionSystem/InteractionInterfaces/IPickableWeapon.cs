public interface IPickableWeapon
{
	public string WeaponRightMouseButtonAttackMessage { get; }
	public string WeaponLeftMouseButtonAttackMessage { get; }
	void AttackRight();
	void AttackLeft();
}