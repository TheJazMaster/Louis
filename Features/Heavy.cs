using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class HeavyManager
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
    private static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;

    internal static ICardTraitEntry HeavyTrait { get; private set; } = null!;
    internal static ICardTraitEntry HeavyUsedTrait { get; private set; } = null!;

    public HeavyManager()
    {
        List<Tooltip> tooltips = [
            
        ];
        HeavyTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("Heavy", new() {
            Icon = (_, _) => Instance.HeavyIcon.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "heavy"]).Localize,
            Tooltips = (_, _) => [
                new GlossaryTooltip($"trait.{GetType().Namespace!}::Heavy") {
                    Icon = Instance.HeavyIcon.Sprite,
                    TitleColor = Colors.action,
                    Title = ModEntry.Instance.Localizations.Localize(["trait", "heavy", "name"]),
                    Description = ModEntry.Instance.Localizations.Localize(["trait", "heavy", "description"]),
                }
            ]
        });
        HeavyUsedTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("HeavyUsed", new() {
            Icon = (_, _) => Instance.HeavyUsedIcon.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "heavy"]).Localize,
            Tooltips = (_, _) => [
                new GlossaryTooltip($"trait.{GetType().Namespace!}::HeavyUsed") {
                    Icon = Instance.HeavyUsedIcon.Sprite,
                    TitleColor = Colors.action,
                    Title = ModEntry.Instance.Localizations.Localize(["trait", "heavy", "nameUsed"]),
                    Description = ModEntry.Instance.Localizations.Localize(["trait", "heavy", "description"]),
                }
            ]
        });

        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DrawCardIdx)),
			postfix: new HarmonyMethod(GetType(), nameof(Combat_DrawCardIdx_Postfix))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AAfterPlayerTurn), nameof(AAfterPlayerTurn.Begin)),
			postfix: new HarmonyMethod(GetType(), nameof(AAfterPlayerTurn_Begin_Postfix))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DiscardHand)),
			transpiler: new HarmonyMethod(GetType(), nameof(Combat_DiscardHand_Transpiler))
		);
    }

	private static IEnumerable<CodeInstruction> Combat_DiscardHand_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		return new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find(
				ILMatches.Call("Where"),
				ILMatches.Call("Reverse"),
				ILMatches.Call("ToList"),
				ILMatches.Stloc<List<Card>>(originalMethod).CreateLdlocInstruction(out var ldLoc).CreateStlocInstruction(out var stLoc)
			)
			.PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
				new CodeInstruction(OpCodes.Ldarg_1),
				ldLoc.Value,
				new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(FilterHeavy))),
				stLoc.Value
			)
			.AllElements();
	}

    private static void AAfterPlayerTurn_Begin_Postfix(G g, State s, Combat c) {
        foreach (Card card in c.hand) {
            if (CardsHelper.IsCardTraitActive(s, card, HeavyTrait)) {
                CardsHelper.SetCardTraitOverride(s, card, HeavyTrait, false, false);
                CardsHelper.SetCardTraitOverride(s, card, HeavyUsedTrait, true, false);
            }
        }
    }

    private static List<Card> FilterHeavy(State s, List<Card> list) {
        return list.Where(card => !CardsHelper.IsCardTraitActive(s, card, HeavyTrait)).ToList();
    }

    private static void Combat_DrawCardIdx_Postfix(State s, int drawIdx, CardDestination from, Card __result) {
        if (CardsHelper.IsCardTraitActive(s, __result, HeavyUsedTrait)) {
            CardsHelper.SetCardTraitOverride(s, __result, HeavyUsedTrait, false, false);
            CardsHelper.SetCardTraitOverride(s, __result, HeavyTrait, true, false);
        }
    }
}