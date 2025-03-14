using Nickel;

namespace TheJazMaster.Louis;

public interface IJohnsonApi
{
	IDeckEntry JohnsonDeck { get; }
	IStatusEntry CrunchTimeStatus { get; }

	ICardTraitEntry StrengthenCardTrait { get; }
	Tooltip GetStrengthenTooltip(int amount);
	int GetStrengthen(Card card);
	void SetStrengthen(Card card, int value);
	void AddStrengthen(Card card, int value);
	CardAction MakeStrengthenAction(int cardId, int amount);
	CardAction MakeStrengthenHandAction(int amount);
}