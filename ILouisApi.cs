using System.Collections.Generic;
using Nickel;

#nullable enable
namespace TheJazMaster.Louis;

public interface ILouisApi
{
	int GemHandCount(State s, Combat c);
	int GemHandCount(State s, Combat c, int? excludedId);
	AAttack MakeEnfeebleAttack(AAttack attack, int strength);
	AAttack MakeEnfeebleAttack(AAttack attack, int strength, Card card);
	(int amount, int baseAmount, Card? fromCard) GetEnfeeble(State s, AAttack attack, bool duringRendering = false);
	
	// Returns whether it succeeded in enfeebling an attack intent
	bool EnfeeblePart(State s, Combat c, Part part, int amount, Card? fromCard = null);
	GlossaryTooltip GetEnfeebleGlossary(int amount);

	Deck LouisDeck { get; }
	Status OnslaughtStatus { get; }
	Status SteadfastStatus { get; }
	ICardTraitEntry HeavyTrait { get; }
	ICardTraitEntry GemTrait { get; }
	ICardTraitEntry PreciousGemTrait { get; }
	ICardTraitEntry FleetingTrait { get; }

	void RegisterHook(IHook hook, double priority = 0);
	void UnregisterHook(IHook hook);

	public interface IHook
	{
		// Returning false means you skip future hooks
		bool BeforeEnfeeble(IBeforeEnfeebleArgs args) => true;

		public interface IBeforeEnfeebleArgs
		{
			State State { get; }
			Combat Combat { get; }
			Part Part { get; }
			int Amount { get; }
			int WorldX { get; }
			Card? FromCard { get; }
		}

		// This is called on each AAttack. If args.Amount is 0, enfeeble won't apply. You can add or remove enfeeble from arbitrary cards with this
		int AdjustEnfeeble(IAdjustEnfeebleArgs args) => 0;

		public interface IAdjustEnfeebleArgs
		{
			State State { get; }
			int BaseAmount { get; }
			int Amount { get; set; }
			bool DuringRendering { get; }
			Card? FromCard { get; set; }
			AAttack Attack { get; }
		}

		void BeforeFleetingExhaust(IBeforeFleetingExhaustArgs args) {}

		public interface IBeforeFleetingExhaustArgs
		{
			State State { get; }
			Combat Combat { get; }
			List<Card> Cards { get; }
		}

		void OnFleetingExhaust(IOnFleetingExhaustArgs args) {}

		public interface IOnFleetingExhaustArgs
		{
			State State { get; }
			Combat Combat { get; }
			List<Card> Cards { get; }
		}

		int AddToPassiveShardPay(IAddToPassiveShardPayArgs args) => 0;

		public interface IAddToPassiveShardPayArgs
		{
			State State { get; }
			Combat Combat { get; }
			int Amount { get; }
		}
	}
}
