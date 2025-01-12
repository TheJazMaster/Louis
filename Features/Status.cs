using System.Collections.Generic;
using HarmonyLib;
using Shockah.Kokoro;
using TheJazMaster.Louis.Cards;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class StatusManager : IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    private static ModEntry Instance => ModEntry.Instance;

    private static Status Onslaught => Instance.OnslaughtStatus.Status;
    private static Status Steadfast => Instance.SteadfastStatus.Status;

    public StatusManager()
    {
        ModEntry.Instance.KokoroApi.StatusLogic.RegisterHook(this);
        ModEntry.Instance.KokoroApi.StatusRendering.RegisterHook(this);

        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.SendCardToExhaust)),
			postfix: new HarmonyMethod(GetType(), nameof(Combat_SendCardToExhaust_Postfix))
		);
    }
    
    private static void Combat_SendCardToExhaust_Postfix(Combat __instance, State s, Card card) {
        if (s.ship.Get(Steadfast) > 0) {
            int amt = s.ship.Get(Steadfast);
            if (!card.GetDataWithOverrides(s).temporary) amt *= 2;
            __instance.Queue(new AStatus {
                status = Status.tempShield,
                statusAmount = amt,
                targetPlayer = true,
                statusPulse = Steadfast
            });
        }
    }

    public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
	{
		if (args.Status != Onslaught)
			return false;
		if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
			return false;

        if (args.Amount > 0) {
            args.Combat.Queue(new AAddCard {
                card = new BurstShotCard(),
                amount = args.Amount,
                destination = CardDestination.Hand,
                statusPulse = Onslaught
            });
        }
		return false;
	}
    
    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args) {
		if (args.Status == Onslaught) return [
            ..args.Tooltips,
            new TTCard {
                card = new BurstShotCard()
            }
        ];
        if (args.Status == Steadfast) return [
            ..args.Tooltips,
            ..StatusMeta.GetTooltips(Status.tempShield, 1),
            new TTGlossary("cardtrait.exhaust")
        ];
        return args.Tooltips;
    }
}