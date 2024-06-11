using System.Collections.Generic;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class CardBrowseFilterManager
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
    static IModData ModData => Instance.Helper.ModData;

    internal const string FilterGemKey = "FilterGem";
    internal const string FilterPreciousGemKey = "FilterPreciousGem";
    internal const string FilterGemOrPreciousKey = "FilterGemOrPrecious";

    public CardBrowseFilterManager()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(ACardSelect), nameof(ACardSelect.BeginWithRoute)),
			transpiler: new HarmonyMethod(GetType(), nameof(ACardSelect_BeginWithRoute_Transpiler))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(CardBrowse), nameof(CardBrowse.GetCardList)),
			postfix: new HarmonyMethod(GetType(), nameof(CardBrowse_GetCardList_Postfix))
		);
    }

    private static IEnumerable<CodeInstruction> ACardSelect_BeginWithRoute_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Newobj(typeof(CardBrowse).GetConstructor([])!),
                ILMatches.Instruction(OpCodes.Dup)
            )
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Dup),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CardBrowseFilterManager), nameof(CopyDataToCardBrowse))),
            })
            .AllElements();
    }

    private static void CopyDataToCardBrowse(CardBrowse cardBrowse, ACardSelect cardSelect)
    {
        if (ModData.TryGetModData<bool>(cardSelect, FilterGemKey, out var filterGem))
            ModData.SetModData(cardBrowse, FilterGemKey, filterGem);

        if (ModData.TryGetModData<bool>(cardSelect, FilterPreciousGemKey, out var filterPreciousGem))
            ModData.SetModData(cardBrowse, FilterPreciousGemKey, filterPreciousGem);

        if (ModData.TryGetModData<bool>(cardSelect, FilterGemOrPreciousKey, out var filterGemOrPrecious))
            ModData.SetModData(cardBrowse, FilterGemOrPreciousKey, filterGemOrPrecious);
    }


    private static void CardBrowse_GetCardList_Postfix(CardBrowse __instance, ref List<Card> __result, G g)
    {
        bool doesFilterGem = ModData.TryGetModData<bool>(__instance, FilterGemKey, out var filterGem);
        bool doesFilterPreciousGem = ModData.TryGetModData<bool>(__instance, FilterPreciousGemKey, out var filterPreciousGem);
        bool doesFilterEither = ModData.TryGetModData<bool>(__instance, FilterGemOrPreciousKey, out var filterEither);
        Combat combat = g.state.route as Combat ?? DB.fakeCombat;
        if ((doesFilterGem || doesFilterPreciousGem || doesFilterEither) && __instance.browseSource != CardBrowse.Source.Codex) {
            __result.RemoveAll(delegate(Card c)
            {
                CardData data = c.GetDataWithOverrides(g.state);
                bool gem = GemManager.HasGem(c, g.state);
                bool preciousGem = GemManager.HasPreciousGem(c, g.state);

                if (doesFilterEither) {
                    if (gem != filterEither && preciousGem != filterEither)
                        return true;
                }

                if (doesFilterGem) {
                    if (gem != filterGem)
                        return true;
                }

                if (doesFilterPreciousGem) {
                    if (preciousGem != filterPreciousGem)
                        return true;
                }

                return false;
            });
        }
    }
}