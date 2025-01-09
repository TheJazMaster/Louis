using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nickel;
using TheJazMaster.Louis.Actions;
using TheJazMaster.Louis.Cards;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis.Artifacts;

internal sealed class GrandEntranceArtifact : Artifact, ILouisArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("GrandEntrance", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.LouisDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/GrandEntrance.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GrandEntrance", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GrandEntrance", "description"]).Localize
		});
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		combat.QueueImmediate(new AAddCard
		{
			card = new BurstShotCard(),
			destination = CardDestination.Hand,
			amount = 3,
			artifactPulse = Key()
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => [new TTCard {
		card = new BurstShotCard()
	}];
}


internal sealed class JewelryKitArtifact : Artifact, ILouisArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("JewelryKit", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.LouisDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/JewelryKit.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "JewelryKit", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "JewelryKit", "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		(state.GetDialogue()?.actionQueue ?? state.GetCurrentQueue()).Queue(new AAddCard {
			card = new EncrustCard(),
			destination = CardDestination.Deck,
			callItTheDeckNotTheDrawPile = true
		});
		(state.GetDialogue()?.actionQueue ?? state.GetCurrentQueue()).Queue(new AAddCard {
			card = new PolishCard(),
			destination = CardDestination.Deck,
			callItTheDeckNotTheDrawPile = true
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTCard {
			card = new EncrustCard()
		},
		new TTCard {
			card = new PolishCard()
		}
	];
}


internal sealed class BlissfulIgnoranceArtifact : Artifact, ILouisArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("BlissfulIgnorance", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.LouisDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/BlissfulIgnorance.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BlissfulIgnorance", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BlissfulIgnorance", "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		for (int i = 0; i < 2; i++) {
			state.GetCurrentQueue().QueueImmediate(new ACardSelect {
				browseAction = new AMakeCheapAndExhaust {
					amount = 1
				},
				browseSource = CardBrowse.Source.Deck,
				filterExhaust = false,
				filterTemporary = false,
				filterMinCost = 1
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("cardtrait.exhaust")
	];
}


internal sealed class BlackRoseArtifact : Artifact, ILouisArtifact
{
	private static Spr SpriteActive;
	private static Spr SpriteInactive;
	public bool active = true;

	public static void Register(IModHelper helper)
	{
		SpriteActive = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/BlackRose.png")).Sprite;
		SpriteInactive = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/BlackRoseInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact("BlackRose", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.LouisDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = SpriteActive,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BlackRose", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BlackRose", "description"]).Localize
		});
	}

	public override Spr GetSprite()
	{
		return active ? SpriteActive : SpriteInactive;
	}

	public override void OnCombatEnd(State state)
	{
		active = true;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("cardtrait.exhaust")
	];
}


internal sealed class OsmiumRingArtifact : Artifact, ILouisArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("OsmiumRing", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.LouisDeck.Deck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/OsmiumRing.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "OsmiumRing", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "OsmiumRing", "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		state.ship.baseDraw -= 1;
	}

	public override void OnRemoveArtifact(State state)
	{
		state.ship.baseDraw += 1;
	}

	public override List<Tooltip>? GetExtraTooltips() => HeavyManager.HeavyTrait.Configuration.Tooltips!(DB.fakeState, null).ToList();
}
