using System.Collections.Generic;
using FSPRO;
using Nickel;

#nullable enable
namespace TheJazMaster.Louis.Actions;

public class ATurnCardIntoShard : DynamicWidthCardAction
{
	internal int amount = 1;

	public override void Begin(G g, State s, Combat c)
	{
		if (selectedCard != null) {
			selectedCard.ExhaustFX();
			Audio.Play(Event.CardHandling);
			s.RemoveCardFromWhereverItIs(selectedCard.uuid);
			c.SendCardToExhaust(s, selectedCard);
			timer = 0.3;

			c.QueueImmediate(new AStatus {
				status = Status.shard,
				statusAmount = amount,
				targetPlayer = true
			});
		}
	}

	public override Icon? GetIcon(State s) => new Icon(ModEntry.Instance.ExhaustCardIntoShard.Sprite, null, Colors.textMain);

	public override List<Tooltip> GetTooltips(State s)
	{
        return [
            new GlossaryTooltip($"action.{GetType().Namespace!}::TurnCardIntoShard") {
                Icon = ModEntry.Instance.ExhaustCardIntoShard.Sprite,
				IsWideIcon = true,
                TitleColor = Colors.action,
                Title = ModEntry.Instance.Localizations.Localize(["action", "exhaustForShard", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(["action", "exhaustForShard", "description"], new { Amount = amount })
			},
            new TTGlossary("cardtrait.exhaust")
        ];
	}
}