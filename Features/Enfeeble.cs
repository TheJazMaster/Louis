using System;
using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System.Text.RegularExpressions;
using Nickel;
using HarmonyLib;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class EnfeebleManager
{
    private static IModData ModData => ModEntry.Instance.Helper.ModData;

    internal static readonly string EnfeebleApplierKey = "EnfeebleApplier";
    internal static readonly string EnfeebleedPartKey = "EnfeebleedPart";

	private static AAttack? AttackContext;

    public EnfeebleManager()
    {
		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetTooltips)),
			postfix: new HarmonyMethod(GetType(), nameof(AAttack_GetTooltips_Postfix))
		);
		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.RenderAction)),
			prefix: new HarmonyMethod(GetType(), nameof(Card_RenderAction_Prefix))
		);
		ModEntry.Instance.Harmony.TryPatch(
			logger: ModEntry.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
			prefix: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Prefix)),
			finalizer: new HarmonyMethod(GetType(), nameof(AAttack_Begin_Finalizer))
		);

		ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnEnemyGetHit), (State state, Combat combat, Part? part) =>
		{
			if (AttackContext is not { } attack)
				return;
			if (!IsEnfeeble(attack))
				return;
			if (part is null || part.intent is not IntentAttack intent)
				return;
			intent.damage = Math.Max(0, intent.damage - GetEnfeeble(attack));
		}, 0);
    }

    public static AAttack MakeEnfeebleAttack(AAttack attack, int strength) {
        return attack.ApplyModData(EnfeebleApplierKey, strength);
    }

    public static bool IsEnfeeble(AAttack self) {
        return ModData.TryGetModData(self, EnfeebleApplierKey, out int _);
    }

    public static int GetEnfeeble(AAttack attack) {
        return ModData.TryGetModData(attack, EnfeebleApplierKey, out int amount) ? amount : 0;
    }

    // public static bool IsEnfeebleed(Part part) {
    //     return ModData.TryGetModData(part, EnfeebleedPartKey, out int _);
    // }

    // public static int GetEnfeebleed(Part part) {
    //     return ModData.TryGetModData(part, EnfeebleedPartKey, out int amount) ? amount : 0;
    // }

	private static void AAttack_Begin_Prefix(AAttack __instance)
		=> AttackContext = __instance;

	private static void AAttack_Begin_Finalizer()
		=> AttackContext = null;

    private static void AAttack_GetTooltips_Postfix(AAttack __instance, State s, ref List<Tooltip> __result)
	{
		if (!IsEnfeeble(__instance))
			return;

		__result.Add(new GlossaryTooltip(
            $"action.{typeof(EnfeebleManager).Namespace!}::Enfeeble") {
            Icon = ModEntry.Instance.EnfeebleIcon.Sprite,
            TitleColor = Colors.action,
            Title = ModEntry.Instance.Localizations.Localize(["action", "enfeeble", "name"]),
            Description = ModEntry.Instance.Localizations.Localize(["action", "enfeeble", "description"], new { Amount = GetEnfeeble(__instance) })
        });
	}

	private static bool Card_RenderAction_Prefix(G g, State state, CardAction action, bool dontDraw, int shardAvailable, int stunChargeAvailable, int bubbleJuiceAvailable, ref int __result)
	{
		if (action is not AAttack attack)
			return true;
		if (!IsEnfeeble(attack))
			return true;

		var copy = Mutil.DeepCopy(attack);
		ModEntry.Instance.Helper.ModData.RemoveModData(copy, EnfeebleApplierKey);

		var position = g.Push(rect: new()).rect.xy;
		int initialX = (int)position.x;

		position.x += Card.RenderAction(g, state, copy, dontDraw, shardAvailable, stunChargeAvailable, bubbleJuiceAvailable);
		g.Pop();

		__result = (int)position.x - initialX;
		__result += 2;

		int amount = GetEnfeeble(attack);
		if (!dontDraw)
		{
			Draw.Sprite(ModEntry.Instance.EnfeebleIcon.Sprite, initialX + __result, position.y, color: action.disabled ? Colors.disabledIconTint : Colors.white);
		}
		__result += 10;
		if (!dontDraw) {
			BigNumbers.Render(amount, initialX + __result, position.y, action.disabled ? Colors.disabledText : Colors.redd);
		}
		__result += amount.ToString().Length * 6;

		return false;
	}
}