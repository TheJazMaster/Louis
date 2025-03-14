using System;
using System.Collections.Generic;
using Nickel;
using HarmonyLib;
using static TheJazMaster.Louis.ILouisApi;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Shockah.Kokoro;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class EnfeebleManager
{
    private static IModData ModData => ModEntry.Instance.Helper.ModData;
	private static IKokoroApi.IV2 KokoroApi => ModEntry.Instance.KokoroApi;

    internal static readonly string EnfeebleApplierKey = "EnfeebleApplier";

	private static AAttack? AttackContext;

	private static readonly Pool<BeforeEnfeebleArgs> BeforeEnfeebleArgsPool = new(() => new());
	private static readonly Pool<AdjustEnfeebleArgs> AdjustEnfeebleArgsPool = new(() => new());

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
			if (part is null)
				return;

			(int amount, Card? fromCard) = GetEnfeeble(state, attack);
			if (amount == 0)
				return;

			EnfeeblePart(state, combat, part, amount, fromCard);
		}, 0);
    }

	public static bool EnfeeblePart(State state, Combat combat, Part part, int amount, Card? fromCard) {
			if (part.intent is not IntentAttack intent) return false;
			
			int worldX = combat.otherShip.parts.FindIndex(p => p == part) + combat.otherShip.x;
			BeforeEnfeebleArgsPool.Do(args => {
				args.State = state;
				args.Combat = combat;
				args.Part = part;
				args.Amount = amount;
				args.WorldX = worldX;
				args.FromCard = fromCard;
				
				foreach (IHook hook in ModEntry.Instance.HookManager.GetHooksWithProxies(ModEntry.Instance.Helper.Utilities.ProxyManager, state.EnumerateAllArtifacts())) {
					if (!hook.BeforeEnfeeble(args)) break;
				}

				intent.damage -= args.Amount;
			});
			return true;
	}

    public static AAttack MakeEnfeebleAttack(AAttack attack, int strength) {
        return attack.ApplyModData(EnfeebleApplierKey, strength);
    }

    public static (int amount, Card? fromCard) GetEnfeeble(State s, AAttack attack) {
		// if (!IsEnfeeble(attack)) return (0, null);

		(int amount, Card? fromCard) = (ModData.GetModDataOrDefault(attack, EnfeebleApplierKey, 0), ModEntry.Instance.KokoroApi.ActionInfo.GetSourceCard(s, attack));
		return AdjustEnfeebleArgsPool.Do(args => {
			args.State = s;
			args.Amount = amount;
			args.FromCard = fromCard;
			args.Attack = attack;
			
			foreach (IHook hook in ModEntry.Instance.HookManager.GetHooksWithProxies(ModEntry.Instance.Helper.Utilities.ProxyManager, s.EnumerateAllArtifacts())) {
				args.Amount += hook.AdjustEnfeeble(args);
			}

        	return (args.Amount, args.FromCard);
		});
    }

	private static void AAttack_Begin_Prefix(AAttack __instance)
		=> AttackContext = __instance;

	private static void AAttack_Begin_Finalizer()
		=> AttackContext = null;

    private static void AAttack_GetTooltips_Postfix(AAttack __instance, State s, ref List<Tooltip> __result)
	{
		int amount = GetEnfeeble(s, __instance).amount;
		if (amount == 0)
			return;

		__result.Add(ModEntry.Instance.Api.GetEnfeebleGlossary(amount));
	}

	private static bool Card_RenderAction_Prefix(G g, State state, CardAction action, bool dontDraw, int shardAvailable, int stunChargeAvailable, int bubbleJuiceAvailable, ref int __result)
	{
		if (action is not AAttack attack)
			return true;

		int amount = GetEnfeeble(state, attack).amount;
		if (amount == 0)
			return true;

		ModEntry.Instance.Helper.ModData.RemoveModData(attack, EnfeebleApplierKey);

		var position = g.Push(rect: new()).rect.xy;
		int initialX = (int)position.x;

		position.x += Card.RenderAction(g, state, attack, dontDraw, shardAvailable, stunChargeAvailable, bubbleJuiceAvailable);
		g.Pop();

		__result = (int)position.x - initialX;
		__result += 2;

		if (!dontDraw)
		{
			Draw.Sprite(ModEntry.Instance.EnfeebleIcon.Sprite, initialX + __result, position.y, color: action.disabled ? Colors.disabledIconTint : Colors.white);
		}
		__result += 10;
		if (!dontDraw) {
			BigNumbers.Render(amount, initialX + __result, position.y, action.disabled ? Colors.disabledText : Colors.redd);
		}
		__result += amount.ToString().Length * 6;

		ModEntry.Instance.Helper.ModData.SetModData(attack, EnfeebleApplierKey, amount);
		return false;
	}

	private sealed class BeforeEnfeebleArgs : IHook.IBeforeEnfeebleArgs {
		public State State { get; set; } = null!;
		public Combat Combat { get; set; } = null!;
		public Part Part { get; set; } = null!;
		public int Amount { get; set; }
		public int WorldX { get; set; }
		public Card? FromCard { get; set; }
	}

	private sealed class AdjustEnfeebleArgs : IHook.IAdjustEnfeebleArgs {
		public State State { get; set; } = null!;
		public int Amount { get; set; }
		public Card? FromCard { get; set; }
		public AAttack Attack { get; set; } = null!;
	}
}