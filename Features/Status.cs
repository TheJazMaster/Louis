using HarmonyLib;
using TheJazMaster.Louis.Cards;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class StatusManager : IStatusLogicHook
{
    private static ModEntry Instance => ModEntry.Instance;

    private static Status Onslaught => Instance.OnslaughtStatus.Status;
    private static Status Steadfast => Instance.SteadfastStatus.Status;

    public StatusManager()
    {
        ModEntry.Instance.KokoroApi.RegisterStatusLogicHook(this, 0);

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

    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
	{
		if (status != Onslaught)
			return false;
		if (timing != StatusTurnTriggerTiming.TurnStart)
			return false;

        if (amount > 0) {
            combat.Queue(new AAddCard {
                card = new BurstShotCard(),
                amount = amount,
                destination = CardDestination.Hand,
                statusPulse = Onslaught
            });
        }
		return false;
	}
}