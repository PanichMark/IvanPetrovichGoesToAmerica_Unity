public interface IGameController
{
	bool IsPlayerControllable { get; }
	bool IsPlayerDead { get; }
	bool IsGameAbleToSave { get; }

	void PlayerIsDead();
	void MakePlayerNonControllable();
	void MakePlayerControllable();
}