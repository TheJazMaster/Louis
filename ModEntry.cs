﻿using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using Shockah.Kokoro;
using TheJazMaster.Louis.Artifacts;
using TheJazMaster.Louis.Cards;
using TheJazMaster.Louis.Features;

#nullable enable
namespace TheJazMaster.Louis;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }
	internal IKokoroApi.IV2 KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IDuoArtifactsApi? DuoArtifactsApi { get; }

	internal FleetingManager FleetingManager { get; }
	internal GemManager GemManager { get; }
	internal HeavyManager HeavyManager { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal IPlayableCharacterEntryV2 LouisCharacter { get; }

    internal IDeckEntry LouisDeck { get; }

	internal IStatusEntry OnslaughtStatus { get; }
    internal IStatusEntry SteadfastStatus { get; }

    internal ISpriteEntry LouisPortraitMini { get; }
    internal ISpriteEntry LouisFrame { get; }
    internal ISpriteEntry LouisCardBorder { get; }

    internal ISpriteEntry GemIcon { get; }
    internal ISpriteEntry MakeGemIcon { get; }
    internal ISpriteEntry PreciousGemIcon { get; }
    internal ISpriteEntry UpgradeGemIcon { get; }
    internal ISpriteEntry FleetingIcon { get; }
    internal ISpriteEntry HeavyIcon { get; }
    internal ISpriteEntry HeavyUsedIcon { get; }
    internal ISpriteEntry EnfeebleIcon { get; }

	internal ISpriteEntry ExhaustCardIntoShard { get; }

    internal static IReadOnlyList<Type> StarterCardTypes { get; } = [
		typeof(RubyCard),
		typeof(GlitzBlitzCard),
	];

	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(DisplayCaseCard),
		typeof(FlashShotCard),
		typeof(PotatoStoneCard),
		typeof(PompAndFlairCard),
		typeof(EnlightenmentCard),
		typeof(GlamourShotCard),
		typeof(GraceCard),
		typeof(LouisExeCard),
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(SapphireCard),
		typeof(GemcutterCard),
		typeof(MineralSiftingCard),
		typeof(HeartBreakerCard),
		typeof(SweetNothingsCard),
		typeof(ThousandBlowsCard),
		typeof(EnsnareCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(EmeraldCard),
		typeof(MagnumOpusCard),
		typeof(FibonacciFlurryCard),
		typeof(LoveAndWarCard),
		typeof(SteadfastCard),
	];

	internal static IReadOnlyList<Type> SecretCardTypes { get; } = [
		typeof(BurstShotCard),
		typeof(FadingDiamondCard),
		typeof(UnobtainiumCard),
		typeof(EncrustCard),
		typeof(PolishCard),
	];

    internal static IEnumerable<Type> AllCardTypes
		=> StarterCardTypes
			.Concat(CommonCardTypes)
			.Concat(UncommonCardTypes)
			.Concat(RareCardTypes)
			.Concat(SecretCardTypes);

    internal static IReadOnlyList<Type> CommonArtifacts { get; } = [
		typeof(GrandEntranceArtifact),
		typeof(BlissfulIgnoranceArtifact),
		typeof(JewelryKitArtifact),
		typeof(BlackRoseArtifact),
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(OsmiumRingArtifact)
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;
		// DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

		DynamicWidthCardAction.ApplyPatches(Harmony);
		CombatPatches.ApplyPatches(Harmony);

        LouisPortraitMini = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Louis_mini.png"));
		LouisFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Louis_panel.png"));
        LouisCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Cards/Border.png"));

        SteadfastStatus = helper.Content.Statuses.RegisterStatus("Steadfast", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Steadfast.png")).Sprite,
				color = new("7F8E99"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Steadfast", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Steadfast", "description"]).Localize
		});

        OnslaughtStatus = helper.Content.Statuses.RegisterStatus("Onslaught", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Onslaught.png")).Sprite,
				color = new("FF3838"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Onslaught", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Onslaught", "description"]).Localize
		});

		EnfeebleIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Enfeeble.png"));
		HeavyIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Heavy.png"));
		HeavyUsedIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/HeavyUsed.png"));
		FleetingIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Fleeting.png"));
		GemIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Gem.png"));
		MakeGemIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/MakeGem.png"));
		PreciousGemIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/PreciousGem.png"));
		UpgradeGemIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/UpgradeGem.png"));
		ExhaustCardIntoShard = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/ExhaustCardIntoShard.png"));

		_ = new EnfeebleManager();
		FleetingManager = new FleetingManager();
		GemManager = new GemManager();
		HeavyManager = new HeavyManager();
		_ = new StatusManager();
		_ = new CheapManager();
		_ = new CardBrowseFilterManager();

		LouisDeck = helper.Content.Decks.RegisterDeck("Louis", new()
		{
			Definition = new() { color = new Color("A63447"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = LouisCardBorder.Sprite,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

        foreach (var cardType in AllCardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(ILouisCard.Register))?.Invoke(null, [helper]);
		foreach (var artifactType in AllArtifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(ILouisArtifact.Register))?.Invoke(null, [helper]);

		helper.Content.Cards.OnGetDynamicInnateCardTraitOverrides += (card, data) => {
			State state = data.State;
			if (state.route is Combat combat) {
				foreach (Artifact item in data.State.EnumerateAllArtifacts()) {
					if (item is OsmiumRingArtifact) {
						data.SetOverride(HeavyManager.HeavyTrait, true);
					}
				}
			}
		};

		MoreDifficultiesApi?.RegisterAltStarters(LouisDeck.Deck, new StarterDeck {
            cards = {
                new PotatoStoneCard(),
                new PompAndFlairCard()
            }
        });

        LouisCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("Louis", new()
		{
			Deck = LouisDeck.Deck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = LouisFrame.Sprite,
			Starters = new StarterDeck {
				cards = [ new RubyCard(), new GlitzBlitzCard() ],
			},
			ExeCardType = typeof(LouisExeCard),
			NeutralAnimation = RegisterTalkSprites("Neutral").Configuration,
			MiniAnimation = new()
			{
				CharacterType = LouisDeck.Deck.Key(),
				LoopTag = "mini",
				Frames = [
					LouisPortraitMini.Sprite
				]
			}
		});

		RegisterTalkSprites("Gameover");
		RegisterTalkSprites("Squint");
    }

	public override object? GetApi(IModManifest requestingMod)
		=> new ApiImplementation();

	private static ICharacterAnimationEntryV2 RegisterTalkSprites(string name)
    {
        return Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(name, new()
		{
			CharacterType = Instance.LouisDeck.Deck.Key(),
			LoopTag = name.ToLower(),
			Frames = Enumerable.Range(0, 100)
				.Select(i => Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Character/{name}/Louis_{name.ToLower()}_{i}.png"))
				.TakeWhile(f => f.Exists)
				.Select(f => Instance.Helper.Content.Sprites.RegisterSprite(f).Sprite)
				.ToList()
		});
    }
}