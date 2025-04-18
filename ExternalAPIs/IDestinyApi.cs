namespace TheJazMaster.Louis;

public interface IDestinyApi
{
	void RegisterHook(IHook hook, double priority = 0);
	void UnregisterHook(IHook hook);
	
	public interface IHook
	{
		void ModifyExplosiveDamage(IModifyExplosiveDamageArgs args) { }

		public interface IModifyExplosiveDamageArgs
		{
			State State { get; }
			Combat Combat { get; }
			Card? Card { get; }
			int BaseDamage { get; }
			int CurrentDamage { get; set; }
		}
	}
}