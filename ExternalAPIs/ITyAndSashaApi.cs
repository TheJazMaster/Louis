using System.Collections.Generic;
using Nickel;

namespace TheJazMaster.Louis;

public interface ITyAndSashaApi
{
	int GetXBonus(Card card, List<CardAction> actions, State s, Combat c);

	int CountWildsInHand(State s, Combat c);

	Deck TyDeck { get; }
	Status PredationStatus { get; }
	Status XFactorStatus { get; }
	Status ExtremeMeasuresStatus { get; }

	ICardTraitEntry WildTrait { get; }
}
