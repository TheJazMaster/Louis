using HarmonyLib;
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
using System.Threading;

namespace TheJazMaster.Louis;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

	internal readonly HookManager<ILouisApi.IHook> HookManager;
    internal Harmony Harmony { get; }
    internal ApiImplementation Api { get; }

	internal IEssentialsApi? EssentialsApi { get; private set; }
	internal IKokoroApi.IV2 KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IDuoArtifactsApi? DuoArtifactsApi { get; }
	// internal ITyAndSashaApi? TyAndSashaApi { get; }
	internal IJohnsonApi? JohnsonApi { get; }
	internal IBucketApi? BucketApi { get; }
	internal ITuckerApi? TuckerApi { get; }
	internal ITH34Api? TH34Api { get; }
	internal IDynaApi? DynaApi { get; }

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

	internal static IReadOnlyList<Type> DuoArtifacts { get; } = [
		typeof(FreeSpiritedArtifact),
		typeof(ParalyzerArtifact),
		typeof(CrystalOscillatorArtifact),
		typeof(FireStoneArtifact),
		typeof(DiamondSwordArtifact),
		typeof(RGBLEDArtifact),
		typeof(DiamondTierArtifact),
		typeof(InfinityGemArtifact),
		typeof(DiamondHandsArtifact),
		typeof(PositiveReinforcementArtifact),
		typeof(RockCrusherArtifact),
		// typeof(LapisLazuliArtifact),
		typeof(TransienceArtifact),
		typeof(QuartzResonatorArtifact),
		typeof(GlitterBombArtifact),
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts).Concat(DuoArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		HookManager = new(package.Manifest.UniqueName);
		Api = new();

		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");
		EssentialsApi = helper.ModRegistry.GetApi<IEssentialsApi>("Nickel.Essentials");
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;
		DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");
		// TyAndSashaApi = helper.ModRegistry.GetApi<ITyAndSashaApi>("TheJazMaster.TyAndSasha");
		JohnsonApi = helper.ModRegistry.GetApi<IJohnsonApi>("Shockah.Johnson");
		BucketApi = helper.ModRegistry.GetApi<IBucketApi>("TheJazMaster.Bucket");
		TuckerApi = helper.ModRegistry.GetApi<ITuckerApi>("TuckerTheSaboteur");
		// DestinyApi = helper.ModRegistry.GetApi<IDestinyApi>("Shockah.Destiny");
		TH34Api = helper.ModRegistry.GetApi<ITH34Api>("Fred.TH34");
		DynaApi = helper.ModRegistry.GetApi<IDynaApi>("Shockah.Dyna");

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
		// if (TyAndSashaApi != null) {
		// 	helper.Content.Cards.OnGetFinalDynamicCardTraitOverrides += (card, data) => {
		// 		State state = data.State;
		// 		if (state.route is Combat combat && data.TraitStates[GemManager.GemTrait].IsActive) {
		// 			foreach (Artifact item in data.State.EnumerateAllArtifacts()) {
		// 				if (item is TigersEyeArtifact) {
		// 					data.SetOverride(TyAndSashaApi.WildTrait, true);
		// 				}
		// 			}
		// 		}
		// 	};
		// }

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