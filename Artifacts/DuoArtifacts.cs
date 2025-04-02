using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nickel;
using Shockah.Kokoro;
using TheJazMaster.Louis.Actions;
using TheJazMaster.Louis.Features;
using static Shockah.Kokoro.IKokoroApi.IV2.IActionCostsApi.IHook;
using static Shockah.Kokoro.IKokoroApi.IV2.IRedrawStatusApi.IHook;

namespace TheJazMaster.Louis.Artifacts;

internal sealed class FreeSpiritedArtifact : Artifact, ILouisArtifact, IOnExhaustArtifact, ILouisApi.IHook
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<FreeSpiritedArtifact>([Deck.riggs, ModEntry.Instance.LouisDeck.Deck]);
	}

	public void OnExhaustCard(State s, Combat c, Card card)
	{
		if (!FleetingManager.IsFleetingHappening) {
			c.QueueImmediate(new ADrawCard {
				count = 1,
				artifactPulse = Key()
			});
		}
	}

	public void OnFleetingExhaust(ILouisApi.IHook.IOnFleetingExhaustArgs args) {
		if (FleetingManager.IsFleetingHappening) {
			args.Combat.QueueImmediate(new AStatus {
				status = Status.drawNextTurn,
				statusAmount = args.Cards.Count,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [.. FleetingManager.FleetingTrait.Configuration.Tooltips!(DB.fakeState, null) ];
}

internal sealed class ParalyzerArtifact : Artifact, ILouisArtifact, ILouisApi.IHook, IHookPriority
{
	public bool active = true;
	public int lastUuid = -1;

	private static string ArtifactName = null!;

	public double HookPriority => -9;

	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<ParalyzerArtifact>([Deck.dizzy, ModEntry.Instance.LouisDeck.Deck]);
	}

	public override void OnTurnStart(State state, Combat combat) {
		active = true;
	}

	public bool BeforeEnfeeble(ILouisApi.IHook.IBeforeEnfeebleArgs args) {
		if (active) {
			active = false;
			args.Combat.QueueImmediate(new AStunPart {
				worldX = args.WorldX,
				artifactPulse = Key()
			});
		}
		return true;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		ModEntry.Instance.Api.GetEnfeebleGlossary(1),
		new TTGlossary("action.stun")
	];
}

internal sealed class DiamondSwordArtifact : Artifact, ILouisArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<DiamondSwordArtifact>([Deck.peri, ModEntry.Instance.LouisDeck.Deck]);
	}

	public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
	{
		if (combat != null && card != null && card.GetMeta().deck == Deck.peri && ModEntry.Instance.Api.GemHandCount(state, combat) > 0) {
			return 1;
		}
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips() => [ .. ModEntry.Instance.Api.GemTrait.Configuration.Tooltips!(DB.fakeState, null) ];
}

internal sealed class CrystalOscillatorArtifact : Artifact, ILouisArtifact
{
	public bool active = true;
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<CrystalOscillatorArtifact>([Deck.goat, ModEntry.Instance.LouisDeck.Deck]);
	}

	public override void OnTurnStart(State state, Combat combat) {
		active = true;
	}

	public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
	{
		if (active && ModEntry.Instance.Api.GemHandCount(state, combat) > 0) {
			active = false;
			combat.QueueImmediate(new ATriggerMidrowTurn {
				thing = thing,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [ .. ModEntry.Instance.Api.GemTrait.Configuration.Tooltips!(DB.fakeState, null) ];
}

internal sealed class FireStoneArtifact : Artifact, ILouisArtifact, ILouisApi.IHook
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<FireStoneArtifact>([Deck.eunice, ModEntry.Instance.LouisDeck.Deck]);
	}

	public int AdjustEnfeeble(ILouisApi.IHook.IAdjustEnfeebleArgs args) {
		if (args.State.ship.Get(Status.heat) >= 3) {
			return 2;
		}
		return 0;
	}

	// public override void OnEnemyGetHit(State state, Combat combat, Part? part)
	// {
	// 	if (part != null && state.ship.Get(Status.heat) >= 3) {
	// 		ModEntry.Instance.Api.EnfeeblePart(state, combat, part, 2);
	// 		Pulse();
	// 	}
	// }

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.heat, MG.inst.g.state.ship.heatTrigger),
		ModEntry.Instance.Api.GetEnfeebleGlossary(2)
	];
}

internal sealed class RGBLEDArtifact : Artifact, ILouisArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<RGBLEDArtifact>([Deck.hacker, ModEntry.Instance.LouisDeck.Deck]);
	}

	public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
	{
		if (!fromPlayer || card == null || combat == null) {
			return 0;
		}
		int pos = combat.hand.FindIndex(crd => crd == card);
		if (pos == -1) return 0;
		for (int i = pos + 1; i < combat.hand.Count; i++) {
			Card crd = combat.hand[i];
			if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(state, crd, ModEntry.Instance.Api.GemTrait)) return 1;
			if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(state, crd, ModEntry.Instance.Api.PreciousGemTrait)) return 1;
		}
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. ModEntry.Instance.Api.GemTrait.Configuration.Tooltips!(DB.fakeState, null)
	];
}

internal sealed class InfinityGemArtifact : Artifact, ILouisArtifact, IKokoroApi.IV2.IActionCostsApi.IHook
{
	public int cardJustPlayedUuid = -1;
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<InfinityGemArtifact>([Deck.shard, ModEntry.Instance.LouisDeck.Deck]);
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		ModEntry.Instance.KokoroApi.ActionCosts.MakeStatusResource(Status.shard, true);
		cardJustPlayedUuid = -1;
	}

	public void OnActionCostsTransactionFinished(IOnActionCostsTransactionFinishedArgs args) {
		if (args.Card != null && args.Card.uuid != cardJustPlayedUuid
			&& args.TransactionPaymentResult.UnpaidResources.ContainsKey(ModEntry.Instance.KokoroApi.ActionCosts.MakeStatusResource(Status.shard, true))
		) {
			cardJustPlayedUuid = args.Card.uuid;
			args.Combat.Queue(new AStatus {
				status = Status.shard,
				statusAmount = 1,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.shard, 3)
	];
}

internal sealed class DiamondTierArtifact : Artifact, ILouisArtifact
{
	private static IModHelper Helper => ModEntry.Instance.Helper;
	private static readonly HashSet<Type> ExeTypes = [];

	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<DiamondTierArtifact>([Deck.catartifact, ModEntry.Instance.LouisDeck.Deck]);

		ModEntry.Instance.Harmony.PatchVirtual(
			original: AccessTools.DeclaredMethod(typeof(ACardOffering), nameof(ACardOffering.BeginWithRoute)),
			prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ACardOffering_BeginWithRoute_Prefix))
		);
	}

	private static void ACardOffering_BeginWithRoute_Prefix(G g, State s, Combat c, ACardOffering __instance) {
		Artifact? artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is DiamondTierArtifact, null);
		if (__instance.inCombat && __instance.amount < 7 && artifact != null && ModEntry.Instance.Api.GemHandCount(s, c) > 0) {
			__instance.amount++;
			__instance.artifactPulse = artifact.Key();
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. ModEntry.Instance.Api.GemTrait.Configuration.Tooltips!(DB.fakeState, null)
	];
}

// internal sealed class TigersEyeArtifact : Artifact, ILouisArtifact
// {
// 	private static string ArtifactName = null!;
// 	public static void Register(IModHelper helper)
// 	{
// 		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.TyAndSashaApi == null) return;
// 		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
// 		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
// 		{
// 			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
// 			Meta = new()
// 			{
// 				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
// 			},
// 			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
// 			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
// 			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
// 		});
// 		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<TigersEyeArtifact>([ModEntry.Instance.TyAndSashaApi.TyDeck, ModEntry.Instance.LouisDeck.Deck]);
// 	}

// 	public override List<Tooltip>? GetExtraTooltips() => [
// 		.. ModEntry.Instance.Api.GemTrait.Configuration.Tooltips!(DB.fakeState, null),
// 		.. ModEntry.Instance.TyAndSashaApi!.WildTrait.Configuration.Tooltips!(DB.fakeState, null)
// 	];
// }

internal sealed class DiamondHandsArtifact : Artifact, ILouisArtifact, ILouisApi.IHook
{
	public readonly HashSet<int> strenghtenedThisCombat = [];

	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.JohnsonApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<DiamondHandsArtifact>([ModEntry.Instance.JohnsonApi.JohnsonDeck.Deck, ModEntry.Instance.LouisDeck.Deck]);
	}

	public int AdjustEnfeeble(ILouisApi.IHook.IAdjustEnfeebleArgs args) {
		if (args.FromCard == null) return 0;
		if (args.Amount > 0) return ModEntry.Instance.JohnsonApi!.GetStrengthen(args.FromCard);
		return 0;
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (card.GetMeta().deck != ModEntry.Instance.LouisDeck.Deck || strenghtenedThisCombat.Contains(card.uuid)) return;

		var a = ModEntry.Instance.JohnsonApi!.MakeStrengthenAction(card.uuid, 1);
		a.artifactPulse = Key();
		combat.Queue(a);
		strenghtenedThisCombat.Add(card.uuid);
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		strenghtenedThisCombat.Clear();
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		ModEntry.Instance.Api.GetEnfeebleGlossary(1),
		ModEntry.Instance.JohnsonApi!.GetStrengthenTooltip(1)
	];
}

internal sealed class PositiveReinforcementArtifact : Artifact, ILouisArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.BucketApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<PositiveReinforcementArtifact>([ModEntry.Instance.BucketApi.BucketDeck, ModEntry.Instance.LouisDeck.Deck]);
		ModEntry.Instance.KokoroApi.RedrawStatus.RegisterHook(new PositiveReinforcementHook(), 0);
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. ModEntry.Instance.Api.FleetingTrait.Configuration.Tooltips!(DB.fakeState, null),
		.. StatusMeta.GetTooltips(ModEntry.Instance.KokoroApi.RedrawStatus.Status, 1),
		.. StatusMeta.GetTooltips(Status.shield, 1)
	];

	private sealed class PositiveReinforcementHook : IKokoroApi.IV2.IRedrawStatusApi.IHook {

		public void AfterRedraw(IAfterRedrawArgs args) {
			var a = args.State.EnumerateAllArtifacts().FirstOrDefault(a => a is PositiveReinforcementArtifact);
			if (a != null) {
				if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(args.State, args.Card, FleetingManager.FleetingTrait)) {
					args.Combat.QueueImmediate(new AStatus {
						status = Status.shield,
						statusAmount = 1,
						targetPlayer = true,
						artifactPulse = a.Key()
					});
				}
			}
		}
	}
}

internal sealed class RockCrusherArtifact : Artifact, ILouisArtifact, ILouisApi.IHook
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.TuckerApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<RockCrusherArtifact>([ModEntry.Instance.TuckerApi.TuckerDeck, ModEntry.Instance.LouisDeck.Deck]);
	}

	public int AdjustEnfeeble(ILouisApi.IHook.IAdjustEnfeebleArgs args) {
		if (ModEntry.Instance.TuckerApi!.IsBluntAttack(args.Attack)) return 2;
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. ModEntry.Instance.TuckerApi!.MakeNewBluntAttack().GetTooltips(DB.fakeState),
		ModEntry.Instance.Api.GetEnfeebleGlossary(2),
	];
}

internal sealed class TransienceArtifact : Artifact, ILouisArtifact, ILouisApi.IHook
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || !ModEntry.Instance.Helper.ModRegistry.LoadedMods.ContainsKey("rtf.Marielle")) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<TransienceArtifact>([ModEntry.Instance.Helper.Content.Decks.LookupByUniqueName("rtf.Marielle::Marielle")!.Deck, ModEntry.Instance.LouisDeck.Deck]);
	}

	public void OnFleetingExhaust(ILouisApi.IHook.IOnFleetingExhaustArgs args) {
		args.Combat.QueueImmediate(new AStatus {
			status = Status.tempShield,
			statusAmount = args.Cards.Count,
			targetPlayer = true
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. ModEntry.Instance.Api.FleetingTrait.Configuration.Tooltips!(DB.fakeState, null),
		.. StatusMeta.GetTooltips(Status.tempShield, 1)
	];
}

internal sealed class QuartzResonatorArtifact : Artifact, ILouisArtifact, ILouisApi.IHook
{
	private static string ArtifactName = null!;
	private static Status PlusChargeStatus;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.TH34Api == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		PlusChargeStatus = ModEntry.Instance.TH34Api!.PlusChargeStatus.Status;
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<QuartzResonatorArtifact>([ModEntry.Instance.TH34Api.TH34_Deck.Deck, ModEntry.Instance.LouisDeck.Deck]);
	}

	public int AddToPassiveShardPay(ILouisApi.IHook.IAddToPassiveShardPayArgs args) {
		if (args.State.ship.Get(PlusChargeStatus) > 0) return 1;
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(PlusChargeStatus, 1),
		.. StatusMeta.GetTooltips(Status.shard, 3)
	];
}

internal sealed class GlitterBombArtifact : Artifact, ILouisArtifact, IDynaHook
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null || ModEntry.Instance.DynaApi == null) return;
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/Duos/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", ArtifactName, "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<GlitterBombArtifact>([ModEntry.Instance.DynaApi.DynaDeck.Deck, ModEntry.Instance.LouisDeck.Deck]);
	}

	public void OnBlastwaveHit(State state, Combat combat, Ship ship, int originWorldX, int waveWorldX, bool hitMidrow) {
		if (ship.isPlayerShip) return;
		int ind = waveWorldX - ship.x;
		if (ind >= ship.parts.Count || ind < 0) return;
		Part part = ship.parts[ind];

		
		if (ModEntry.Instance.Api.EnfeeblePart(state, combat, part, 1)) Pulse();
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. ModEntry.Instance.DynaApi!.SetBlastwave(new AAttack(), 1).GetTooltips(DB.fakeState),
		ModEntry.Instance.Api.GetEnfeebleGlossary(1)
	];
}