using System;
using System.Collections.Generic;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using TheJazMaster.Louis.Actions;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class GemManager
{
    private static ModEntry Instance => ModEntry.Instance;
    private static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;
    private static IModData ModData => ModEntry.Instance.Helper.ModData;

    internal static int usedGems = 0;
    internal static int bonusGems = 0;

    internal static readonly string LastGemmedTurnKey = "LastGemmedTurn";
    internal static readonly string DontUseGemKey = "DontUseGem";
    internal static readonly string AvailableGemKey = "AvailableGem";

    internal static ICardTraitEntry GemTrait { get; private set; } = null!;
    internal static ICardTraitEntry PreciousGemTrait { get; private set; } = null!;



    public GemManager()
    {   
        GemTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("Gem", new() {
            Icon = (_, _) => Instance.GemIcon.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "gem"]).Localize,
            Tooltips = (_, _) => [
                new GlossaryTooltip($"trait.{GetType().Namespace!}::Gem")
				{
					Icon = Instance.GemIcon.Sprite,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["trait", "gem", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["trait", "gem", "description"]),
				}
            ]
        });
        PreciousGemTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("PreciousGem", new() {
            Icon = (_, _) => Instance.PreciousGemIcon.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "preciousGem"]).Localize,
            Tooltips = (_, _) => [
                new GlossaryTooltip($"trait.{GetType().Namespace!}::PreciousGem")
				{
					Icon = Instance.PreciousGemIcon.Sprite,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["trait", "preciousGem", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["trait", "preciousGem", "description"]),
				}
            ]
        });

        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
			postfix: new HarmonyMethod(GetType(), nameof(Combat_TryPlayCard_Postfix)),
            transpiler: new HarmonyMethod(GetType(), nameof(Combat_TryPlayCard_Transpiler))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.Make)),
			postfix: new HarmonyMethod(GetType(), nameof(Combat_Make_Postfix))
		);
        ModEntry.Instance.Harmony.TryPatch(
            logger: ModEntry.Instance.Logger,
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DrainCardActions)),
        	transpiler: new HarmonyMethod(GetType(), nameof(Combat_DrainCardActions_Transpiler))
        );
        ModEntry.Instance.Harmony.TryPatch(
            logger: ModEntry.Instance.Logger,
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.MakeAllActionIcons)),
        	transpiler: new HarmonyMethod(GetType(), nameof(Card_MakeAllActionIcons_Transpiler))
        );
    }
    
    private static void Combat_TryPlayCard_Postfix(State s, Card card, Combat __instance, ref bool __result) {
        if (__result) {
            ModData.SetModData(card, LastGemmedTurnKey, __instance.turn);
        }
    }
    
    private static void Combat_Make_Postfix(State s) {
        foreach (Card card in s.deck) {
            ModData.RemoveModData(card, LastGemmedTurnKey);
        }
    }

    private static IEnumerable<CodeInstruction> Combat_TryPlayCard_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod, ILGenerator il) {
        List<ElementMatch<CodeInstruction>> sequence = [
            ILMatches.Ldloc(0).CreateLdlocInstruction(out var virtualLdLoc),
            ILMatches.Ldfld("card").Anchor(out var anchor),
            ILMatches.Ldarg(1),
            ILMatches.Ldarg(0),
            ILMatches.Call("GetActionsOverridden"),
            ILMatches.Stloc<List<CardAction>>(originalMethod).CreateLdlocInstruction(out var ldLoc)
        ];

        CodeInstruction instr = new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find(sequence).Anchors()
            .PointerMatcher(anchor).Element();

        return new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find(sequence)
			.PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                virtualLdLoc.Value,
                new CodeInstruction(instr.opcode, instr.operand),
                ldLoc.Value,
				new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(DisableGemsIfNeeded)))
			)
			.AllElements();
    }

    private static void DisableGemsIfNeeded(State s, Combat c, Card card, List<CardAction> actions) {
        if (ModData.TryGetModData(card, LastGemmedTurnKey, out int turn) && turn == c.turn) {
            foreach (CardAction action in actions) {
                ModData.SetModData(action, DontUseGemKey, true);
            }
        }
        bonusGems = GetGemHandCount(s, c);
        actions.Add(new AResetUsedGems {
            canRunAfterKill = true
        });
    }

    private static IEnumerable<CodeInstruction> Combat_DrainCardActions_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod, ILGenerator il) {
        LocalBuilder gemCount = il.DeclareLocal(typeof(int));
        var code = new SequenceBlockMatcher<CodeInstruction>(instructions)
			.Find(
				ILMatches.LdcI4((int)Status.shard),
				ILMatches.Call("Get"),
				ILMatches.Bgt
			)
			.PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, typeof(Combat).GetField("currentCardAction")),
				new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(GetGemHandCount_b))),
                new CodeInstruction(OpCodes.Stloc, gemCount.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, gemCount.LocalIndex),
                new CodeInstruction(OpCodes.Add)
			)
			.AllElements();
        
        return new SequenceBlockMatcher<CodeInstruction>(code)
			.Find(
				ILMatches.Ldfld("currentCardAction"),
				ILMatches.Ldflda("shardcost"),
				ILMatches.Call("get_Value"),
                ILMatches.Instruction(OpCodes.Sub)
			)
			.PointerMatcher(SequenceMatcherRelativeElement.Last)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                new CodeInstruction(OpCodes.Ldloc, gemCount.LocalIndex),
				new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(GetActualShardCost)))
			)
			.AllElements();
    }

    private static IEnumerable<CodeInstruction> Card_MakeAllActionIcons_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Ldarg(2),
                ILMatches.Ldfld("ship"),
                ILMatches.LdcI4((int)Status.shard),
                ILMatches.Call("Get")
            )
            .Insert(
                SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(GetGemHandCount_c))),
                new CodeInstruction(OpCodes.Add)
            )
            .AllElements();
	}

    private static int GetActualShardCost(int cost, int gemCount) {
        int res = Math.Max(0, cost - gemCount);
        usedGems += Math.Min(cost, gemCount);
        return res;
    }

    public static void SetGem(Card card, State s, bool? value, bool permanent = false) {
        CardsHelper.SetCardTraitOverride(s, card, GemTrait, value, permanent);
    }

    public static void SetPreciousGem(Card card, State s, bool? value, bool permanent = false) {
        CardsHelper.SetCardTraitOverride(s, card, PreciousGemTrait, value, permanent);
    }

    public static bool HasGem(Card card, State s)
    {
        return CardsHelper.IsCardTraitActive(s, card, GemTrait);
    }

    public static bool HasPreciousGem(Card card, State s)
    {
        return CardsHelper.IsCardTraitActive(s, card, PreciousGemTrait);
    }

    public static int GetGemHandCount(State s, Combat c) {
        int total = 0;
        foreach (Card card in c.hand) {
            if (CardsHelper.IsCardTraitActive(s, card, GemTrait)) total++;
            else if (CardsHelper.IsCardTraitActive(s, card, PreciousGemTrait)) total += 2;
        }
        return total;
    }

    private static int GetGemHandCount_b(G g, Combat c, CardAction action) {
        if (!ModData.TryGetModData(action, DontUseGemKey, out bool value) || !value)
            return bonusGems - usedGems;//GetGemHandCount(g.state, c) - usedGems;
        return 0;
    }

    private static int GetGemHandCount_c(G g, Card card) {
        if (g.state.route is Combat combat && !(ModData.TryGetModData(card, LastGemmedTurnKey, out int turn) && turn == combat.turn))
            return GetGemHandCount(g.state, combat);
        return 0;
    }
}