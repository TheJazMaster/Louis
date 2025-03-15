using Nickel;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis;

public sealed class ApiImplementation : ILouisApi
{
	readonly ModEntry Instance = ModEntry.Instance;

	public int GemHandCount(State s, Combat c) => GemManager.GetGemHandCount(s, c);
	public int GemHandCount(State s, Combat c, int? excludedId) => GemManager.GetGemHandCount(s, c, excludedId);
	public AAttack MakeEnfeebleAttack(AAttack attack, int strength) => EnfeebleManager.MakeEnfeebleAttack(attack, strength, null);
	public AAttack MakeEnfeebleAttack(AAttack attack, int strength, Card card) => EnfeebleManager.MakeEnfeebleAttack(attack, strength, card);
	public (int amount, int baseAmount, Card? fromCard) GetEnfeeble(State s, AAttack attack, bool duringRendering = false) => EnfeebleManager.GetEnfeeble(s, attack, duringRendering);
	public bool EnfeeblePart(State s, Combat c, Part part, int amount, Card? fromCard = null) => EnfeebleManager.EnfeeblePart(s, c, part, amount, fromCard);
	public GlossaryTooltip GetEnfeebleGlossary(int amount) => new(
            $"action.{typeof(EnfeebleManager).Namespace!}::Enfeeble") {
            Icon = ModEntry.Instance.EnfeebleIcon.Sprite,
            TitleColor = Colors.action,
            Title = ModEntry.Instance.Localizations.Localize(["action", "enfeeble", "name"]),
            Description = ModEntry.Instance.Localizations.Localize(["action", "enfeeble", "description"], new { Amount = amount })
        };

	public void RegisterHook(ILouisApi.IHook hook, double priority = 0) {
		ModEntry.Instance.HookManager.Register(hook, priority);
	}

	public void UnregisterHook(ILouisApi.IHook hook) {
		ModEntry.Instance.HookManager.Unregister(hook);
	}

	public Deck LouisDeck => Instance.LouisDeck.Deck;
	public Status OnslaughtStatus => Instance.OnslaughtStatus.Status;
	public Status SteadfastStatus => Instance.SteadfastStatus.Status;
	public ICardTraitEntry HeavyTrait => HeavyManager.HeavyTrait;
	public ICardTraitEntry GemTrait => GemManager.GemTrait;
	public ICardTraitEntry PreciousGemTrait => GemManager.PreciousGemTrait;
	public ICardTraitEntry FleetingTrait => FleetingManager.FleetingTrait;
}
