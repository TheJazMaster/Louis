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

	public override void Begin(G g, State s, Combat c)
	{
		timer = 0.0;
		List<Card> toExhaust = [];
		foreach (Card card in c.hand) {
			if (CardsHelper.IsCardTraitActive(s, card, FleetingManager.FleetingTrait) && !(ignoreHeavy && CardsHelper.IsCardTraitActive(s, card, HeavyManager.HeavyTrait))) {
				toExhaust.Add(card);
			}
		}
		foreach (Card card in toExhaust) {
			card.ExhaustFX();
			Audio.Play(Event.CardHandling);
			c.hand.Remove(card);
			c.SendCardToExhaust(s, card);
			timer = 0.3;
		}	
	}
}