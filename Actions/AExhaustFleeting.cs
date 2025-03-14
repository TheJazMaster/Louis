using System.Collections.Generic;
using FSPRO;
using Nickel;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis.Actions;

public class AExhaustFleeting : CardAction
{
	static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;
	public bool ignoreHeavy = true;

	private static readonly Pool<OnFleetingExhaustArgs> OnFleetingExhaustPool = new(() => new());

	public override void Begin(G g, State s, Combat c)
	{
		timer = 0;
		List<Card> toExhaust = [];
		foreach (Card card in c.hand) {
			if (CardsHelper.IsCardTraitActive(s, card, FleetingManager.FleetingTrait) && !(ignoreHeavy && CardsHelper.IsCardTraitActive(s, card, HeavyManager.HeavyTrait))) {
				toExhaust.Add(card);
			}
		}
		FleetingManager.IsFleetingHappening = true;
		foreach (Card card in toExhaust) {
			card.ExhaustFX();
			Audio.Play(Event.CardHandling);
			c.hand.Remove(card);
			c.SendCardToExhaust(s, card);
			timer = 0.3;
			
			OnFleetingExhaustPool.Do(args => {
				args.State = s;
				args.Combat = c;
				args.Card = card;
			
				foreach (ILouisApi.IHook hook in ModEntry.Instance.HookManager.GetHooksWithProxies(ModEntry.Instance.Helper.Utilities.ProxyManager, s.EnumerateAllArtifacts())) {
					hook.OnFleetingExhaust(args);
				}
			});
		}
		FleetingManager.IsFleetingHappening = false;
	}

	private sealed class OnFleetingExhaustArgs : ILouisApi.IHook.IOnFleetingExhaustArgs
	{
		public State State { get; set; } = null!;
		public Combat Combat { get; set; } = null!;
		public Card Card { get; set; } = null!;
	}
}
