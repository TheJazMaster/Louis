using System.Collections.Generic;
using FSPRO;
using Nickel;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis.Actions;

public class AMakePreciousGem : CardAction
{
	static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;
	public bool permanent = false;

	public override void Begin(G g, State s, Combat c)
	{
		if (selectedCard != null && CardsHelper.IsCardTraitActive(s, selectedCard, GemManager.GemTrait)) {
			CardsHelper.SetCardTraitOverride(s, selectedCard, GemManager.GemTrait, false, permanent);
			CardsHelper.SetCardTraitOverride(s, selectedCard, GemManager.PreciousGemTrait, true, permanent);
			Audio.Play(Event.Status_PowerUp);
		}
	}

	public override Icon? GetIcon(State s) => new Icon(ModEntry.Instance.UpgradeGemIcon.Sprite, null, Colors.textMain);

    public string GetDuration()
    {
        return permanent ? Loc.T("actionShared.durationForever") : Loc.T("actionShared.durationCombat");
    }

	public override List<Tooltip> GetTooltips(State s)
	{
        return [
            new GlossaryTooltip($"action.{GetType().Namespace!}::MakeGemPrecious") {
                Icon = ModEntry.Instance.UpgradeGemIcon.Sprite,
                TitleColor = Colors.action,
                Title = ModEntry.Instance.Localizations.Localize(["action", "makeGemPrecious", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(["action", "makeGemPrecious", "description"], new { Duration = GetDuration() })
			},
            .. GemManager.GemTrait.Configuration.Tooltips!(s, null),
			.. GemManager.PreciousGemTrait.Configuration.Tooltips!(s, null)
        ];
	}
}