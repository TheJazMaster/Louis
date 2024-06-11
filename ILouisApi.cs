using System.Collections.Generic;
using Nickel;

#nullable enable
namespace TheJazMaster.Louis;

public interface ILouisApi
{
	int GemHandCount(State s, Combat c);
	AAttack MakeEnfeebleAttack(AAttack attack, int strength);

	Deck LouisDeck { get; }
	Status OnslaughtStatus { get; }
	Status SteadfastStatus { get; }
	ICardTraitEntry HeavyTrait { get; }
	ICardTraitEntry GemTrait { get; }
	ICardTraitEntry PreciousGemTrait { get; }
	ICardTraitEntry FleetingTrait { get; }
}
