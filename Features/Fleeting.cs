using System;
using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using TheJazMaster.Louis.Actions;

namespace TheJazMaster.Louis.Features;
#nullable enable

public class FleetingManager
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
    private static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;

    internal static ICardTraitEntry FleetingTrait { get; private set; } = null!;

    public FleetingManager()
    {
        FleetingTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("Fleeting", new() {
            Icon = (_, _) => Instance.FleetingIcon.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "fleeting"]).Localize,
            Tooltips = (_, _) => [
                new GlossaryTooltip($"trait.{GetType().Namespace!}::Fleeting")
				{
					Icon = Instance.FleetingIcon.Sprite,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["trait", "fleeting", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["trait", "fleeting", "description"]),
				}
            ]
        });

        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AEndTurn), nameof(AEndTurn.Begin)),
			postfix: new HarmonyMethod(GetType(), nameof(AEndTurn_Begin_Postfix))
		);
    }

    private static void AEndTurn_Begin_Postfix(G g, State s, Combat c) {
        c.QueueImmediate(new AExhaustFleeting
		{
			ignoreHeavy = true
		});
    }
}