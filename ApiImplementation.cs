using Nickel;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis;

public sealed class ApiImplementation : ILouisApi
{
	readonly ModEntry Instance = ModEntry.Instance;

	public int GemHandCount(State s, Combat c) => GemManager.GetGemHandCount(s, c);
	public int GemHandCount(State s, Combat c, int? excludedId) => GemManager.GetGemHandCount(s, c, excludedId);
	public AAttack MakeEnfeebleAttack(AAttack attack, int strength) => EnfeebleManager.MakeEnfeebleAttack(attack, strength);

	public Deck LouisDeck => Instance.LouisDeck.Deck;
	public Status OnslaughtStatus => Instance.OnslaughtStatus.Status;
	public Status SteadfastStatus => Instance.SteadfastStatus.Status;
	public ICardTraitEntry HeavyTrait => HeavyManager.HeavyTrait;
	public ICardTraitEntry GemTrait => GemManager.GemTrait;
	public ICardTraitEntry PreciousGemTrait => GemManager.PreciousGemTrait;
	public ICardTraitEntry FleetingTrait => FleetingManager.FleetingTrait;
}
