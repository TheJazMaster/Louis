using System.Collections.Generic;
using FSPRO;
using Nickel;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis.Actions;

public class AGemRightmost : CardAction
{
	static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;
	public int amount = 1;
	public bool permanent = false; 

	public override void Begin(G g, State s, Combat c)
	{
		int i = c.hand.Count - 1;
		int j = 0;
		while (i >= 0 && j < amount) {
			Card card = c.hand[i];
			if (!CardsHelper.IsCardTraitActive(s, card, GemManager.PreciousGemTrait) && !CardsHelper.IsCardTraitActive(s, card, GemManager.GemTrait)) {
				CardsHelper.SetCardTraitOverride(s, card, GemManager.GemTrait, true, permanent);
				j++;
			}
			i--;
		}
	}

	public override Icon? GetIcon(State s) => new Icon(ModEntry.Instance.MakeGemIcon.Sprite, null, Colors.textMain);

    public string GetDuration()
    {
        return permanent ? Loc.T("actionShared.durationForever") : Loc.T("actionShared.durationCombat");
    }

	public override List<Tooltip> GetTooltips(State s)
	{
        return [
            new GlossaryTooltip($"action.{GetType().Namespace!}::GemRightmost") {
                Icon = ModEntry.Instance.MakeGemIcon.Sprite,
                TitleColor = Colors.action,
                Title = ModEntry.Instance.Localizations.Localize(["action", "applyGem", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(["action", "applyGem", "description"], new { Amount = amount, Duration = GetDuration() })
			},
            .. GemManager.GemTrait.Configuration.Tooltips!(s, null)
        ];
	}
}