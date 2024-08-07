using Nickel;
using HarmonyLib;
using System;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class CheapManager
{
    private static IModData ModData => ModEntry.Instance.Helper.ModData;

    internal static readonly string CheapKey = "Cheap";

    public CheapManager() {
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetDataWithOverrides)),
			postfix: new HarmonyMethod(GetType(), nameof(Card_GetDataWithOverrides_Postfix))
		);
    }

    private static void Card_GetDataWithOverrides_Postfix(State state, ref CardData __result, Card __instance) {
        if (ModData.TryGetModData(__instance, CheapKey, out int amount)) {
            __result.cost = Math.Max(0, __result.cost - amount);
        }
    }

    public static void MakeCheap(Card card, int amount) {
        ModData.SetModData(card, CheapKey, amount);
    }
}