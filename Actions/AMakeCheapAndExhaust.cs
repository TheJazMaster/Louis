using Nickel;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis.Actions;

public class AMakeCheapAndExhaust : CardAction
{
	static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;
	public int amount = 0;

	public override Route? BeginWithRoute(G g, State s, Combat c)
	{
		timer = 0;
		if (selectedCard != null) {
			ModEntry.Instance.Helper.ModData.SetModData(selectedCard, CheapManager.CheapKey, amount);
			CardsHelper.SetCardTraitOverride(s, selectedCard, CardsHelper.ExhaustCardTrait, true, true);

			return new CustomShowCards {
				message = ModEntry.Instance.Localizations.Localize(["action", "makeCheapAndExhaust", "showCardText"]),
				cardIds = [selectedCard.uuid]
			};	
		}
		return null;
	}

	public override Icon? GetIcon(State s) => new Icon(StableSpr.icons_energy, amount, Colors.textMain);

	public override string? GetCardSelectText(State s)
	{
		return ModEntry.Instance.Localizations.Localize(["action", "makeCheapAndExhaust", "cardSelectText"]);
	}
}