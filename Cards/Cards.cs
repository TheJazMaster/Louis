using Nickel;
using TheJazMaster.Louis.Actions;
using System.Collections.Generic;
using System.Reflection;
using TheJazMaster.Louis.Features;

namespace TheJazMaster.Louis.Cards;



internal sealed class RubyCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Ruby", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Ruby.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Ruby", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => [GemManager.GemTrait, HeavyManager.HeavyTrait],
		_ => new HashSet<ICardTraitEntry>() { GemManager.GemTrait }
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1)
		}
	];
}

internal sealed class GlitzBlitzCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("GlitzBlitz", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/GlitzBlitz.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "GlitzBlitz", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade == Upgrade.A ? new HashSet<ICardTraitEntry>() { HeavyManager.HeavyTrait } : [];

	public override List<CardAction> GetActions(State s, Combat c) => [
		EnfeebleManager.MakeEnfeebleAttack(new AAttack {
			damage = GetDmg(s, 1)
		}, upgrade == Upgrade.B ? 2 : 1, this),
		EnfeebleManager.MakeEnfeebleAttack(new AAttack {
			damage = GetDmg(s, 1),
			shardcost = 1
		}, upgrade == Upgrade.B ? 2 : 1, this),
		EnfeebleManager.MakeEnfeebleAttack(new AAttack {
			damage = GetDmg(s, 1),
			shardcost = 1
		}, upgrade == Upgrade.B ? 2 : 1, this)
	];
}

internal sealed class DisplayCaseCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("DisplayCase", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/DisplayCase.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "DisplayCase", "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade == Upgrade.B ? new HashSet<ICardTraitEntry>() { HeavyManager.HeavyTrait } : [];

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade == Upgrade.None,
		retain = upgrade != Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", "DisplayCase", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAddCard {
			card = new FadingDiamondCard {
				upgrade = upgrade == Upgrade.B ? Upgrade.B : Upgrade.None
			},
			destination = CardDestination.Hand
		}
	];
}


internal sealed class FlashShotCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("FlashShot", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FlashShot.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FlashShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 3 : upgrade == Upgrade.A ? 2 : 1),
			shardcost = upgrade == Upgrade.B ? 2 : 1
		}.ApplyModData(EnfeebleManager.EnfeebleApplierKey, 1),
	];
}

internal sealed class PotatoStoneCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("PotatoStone", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/PotatoStone.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PotatoStone", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => [FleetingManager.FleetingTrait, GemManager.PreciousGemTrait, HeavyManager.HeavyTrait],
		_ => new HashSet<ICardTraitEntry>() { FleetingManager.FleetingTrait, GemManager.PreciousGemTrait }
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = Status.shield,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true
			}
		]
	};
}

internal sealed class PompAndFlairCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("PompAndFlair", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/PompAndFlair.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PompAndFlair", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => [],
		_ => new HashSet<ICardTraitEntry> { FleetingManager.FleetingTrait }
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, 4),
			shardcost = upgrade == Upgrade.B ? null : 1
		}
	];
}

internal sealed class EnlightenmentCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Enlightenment", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Enlightenment.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Enlightenment", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ADrawCard {
				count = 2,
				shardcost = 1
			}
		],
		_ => [
			new ADrawCard {
				count = upgrade == Upgrade.A ? 4 : 2
			},
			new ADrawCard {
				count = 2,
				shardcost = 1
			}
		]
	};
}

internal sealed class GlamourShotCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("GlamourShot", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/GlamourShot.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "GlamourShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			EnfeebleManager.MakeEnfeebleAttack(new AAttack {
				damage = GetDmg(s, 2)
			}, 3, this),
			new AEnergy {
				changeAmount = 1,
				shardcost = 1
			},
			new AEnergy {
				changeAmount = 1,
				shardcost = 1
			}
		],
		_ => [
			EnfeebleManager.MakeEnfeebleAttack(new AAttack {
				damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 2)
			}, upgrade == Upgrade.A ? 4 : 3, this),
			new AEnergy {
				changeAmount = 1,
				shardcost = 1
			}
		]
	};
}


internal sealed class GraceCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Grace", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Grace.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Grace", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => new HashSet<ICardTraitEntry> { HeavyManager.HeavyTrait },
		_ => []
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.evade,
			statusAmount = 1,
			targetPlayer = true,
			shardcost = upgrade == Upgrade.B ? 1 : null
		},
		new AStatus {
			status = Status.evade,
			statusAmount = 1,
			targetPlayer = true,
			shardcost = 1
		},
	];
}


internal sealed class SapphireCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Sapphire", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Sapphire.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Sapphire", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		infinite = true,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => [GemManager.GemTrait, HeavyManager.HeavyTrait],
		_ => new HashSet<ICardTraitEntry>() { GemManager.GemTrait }
	};
	
	public override List<CardAction> GetActions(State s, Combat c) => [
		new ADrawCard {
			count = upgrade == Upgrade.B ? 3 : 2
		}
	];
}


internal sealed class GemcutterCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Gemcutter", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Gemcutter.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Gemcutter", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		retain = upgrade == Upgrade.A,
		description = ModEntry.Instance.Localizations.Localize(["card", "Gemcutter", "description", upgrade.ToString()], new { Amount = 2 }),
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade == Upgrade.None ? new HashSet<ICardTraitEntry>() { HeavyManager.HeavyTrait } : [];
	
	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ACardSelectImproved
			{
				browseAction = new ATurnCardIntoShard {
					amount = 2
				},
				browseSource = CardBrowse.Source.Hand,
				filterUUID = uuid
			}
		],
		_ => [
			new ACardSelectImproved
			{
				browseAction = new ATurnCardIntoShard {
					amount = 2
				},
				browseSource = CardBrowse.Source.Hand,
				filterUUID = uuid
			}.ApplyModData(CardBrowseFilterManager.FilterGemOrPreciousKey, true)
		]
	};
}


internal sealed class MineralSiftingCard : Card, ILouisCard
{
	internal static Spr Art;
	public static void Register(IModHelper helper) {
		Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/MineralSifting.png")).Sprite;
		helper.Content.Cards.RegisterCard("MineralSifting", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = Art,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "MineralSifting", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		description = ModEntry.Instance.Localizations.Localize(["card", "MineralSifting", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new ADrawCard {
				count = 1,
				timer = -0.3
			},
			new AAddCard {
				card = new FadingDiamondCard(),
				destination = CardDestination.Deck,
				timer = -0.3
			},
			new AAddCard {
				card = new FadingDiamondCard(),
				destination = CardDestination.Discard,
				omitFromTooltips = true
			}
		],
		Upgrade.B => [
			new AAddCard {
				card = new FadingDiamondCard(),
				destination = CardDestination.Deck,
				timer = -0.3
			},
			new AAddCard {
				card = new FadingDiamondCard(),
				destination = CardDestination.Discard,
				timer = -0.3,
				omitFromTooltips = true
			},
			new AAddCard {
				card = new FadingDiamondCard(),
				destination = CardDestination.Hand,
				omitFromTooltips = true
			}
		],
		_ => [
			new AAddCard {
				card = new FadingDiamondCard(),
				destination = CardDestination.Deck,
				timer = -0.3
			},
			new AAddCard {
				card = new FadingDiamondCard(),
				destination = CardDestination.Discard,
				omitFromTooltips = true
			}
		]
	};
}


internal sealed class HeartBreakerCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("HeartBreaker", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/HeartBreaker.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HeartBreaker", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.B => [FleetingManager.FleetingTrait],
		_ => new HashSet<ICardTraitEntry>() { FleetingManager.FleetingTrait, HeavyManager.HeavyTrait }
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		EnfeebleManager.MakeEnfeebleAttack(new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 6 : (upgrade == Upgrade.A ? 5 : 4)),
		}, upgrade == Upgrade.B ? 4 : (upgrade == Upgrade.A ? 3 : 2), this)
	];
}


internal sealed class SweetNothingsCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("SweetNothings", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/SweetNothings.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SweetNothings", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true,
			},
			new AStatus {
				status = Status.shard,
				statusAmount = 1,
				targetPlayer = true,
				shardcost = 1
			},
			new AStatus {
				status = Status.shard,
				statusAmount = 1,
				targetPlayer = true,
				shardcost = 1
			}
		],
		_ => [
			new AStatus {
				status = upgrade == Upgrade.B ? Status.shield : Status.tempShield,
				statusAmount = 1,
				targetPlayer = true,
			},
			new AStatus {
				status = Status.shard,
				statusAmount = 1,
				targetPlayer = true,
				shardcost = 1
			}
		]
	};
}


internal sealed class ThousandBlowsCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("ThousandBlows", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/ThousandBlows.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ThousandBlows", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.OnslaughtStatus.Status,
			statusAmount = 1,
			targetPlayer = true,
			shardcost = upgrade == Upgrade.B ? 1 : 2
		},
	];
}


internal sealed class EnsnareCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Ensnare", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Ensnare.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Ensnare", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		EnfeebleManager.MakeEnfeebleAttack(new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1)
		}, upgrade == Upgrade.B ? 6 : 4, this)
	];
}


internal sealed class EmeraldCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Emerald", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Emerald.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Emerald", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		recycle = true,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => [GemManager.GemTrait, HeavyManager.HeavyTrait],
		_ => new HashSet<ICardTraitEntry>() { GemManager.GemTrait }
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.drawNextTurn,
			statusAmount = upgrade == Upgrade.B ? 4 : 3,
			targetPlayer = true
		}
	];
}


internal sealed class MagnumOpusCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("MagnumOpus", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/MagnumOpus.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "MagnumOpus", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 4 : (upgrade == Upgrade.A ? 2 : 3),
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "MagnumOpus", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAddCard {
			card = new UnobtainiumCard {
				upgrade = upgrade == Upgrade.B ? Upgrade.B : Upgrade.None
			},
			destination = CardDestination.Hand
		}
	];
}


internal sealed class FibonacciFlurryCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("FibonacciFlurry", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FibonacciFlurry.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FibonacciFlurry", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, 1),
			shardcost = upgrade == Upgrade.B ? null : 1
		},
		new AAttack {
			damage = GetDmg(s, 1),
			shardcost = 1
		},
		new AAttack {
			damage = GetDmg(s, 2),
			shardcost = 1
		},
		new AAttack {
			damage = GetDmg(s, 3),
			shardcost = 1
		},
		new AAttack {
			damage = GetDmg(s, 5),
			shardcost = 1
		}
	];
}


internal sealed class LoveAndWarCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("LoveAndWar", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/LoveAndWar.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "LoveAndWar", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.OnslaughtStatus.Status,
				statusAmount = 2,
				targetPlayer = true
			},
			new AAttack {
				damage = GetDmg(s, 4)
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.OnslaughtStatus.Status,
				statusAmount = upgrade == Upgrade.B ? 3 : 2,
				targetPlayer = true
			},
		]
	};
}


internal sealed class SteadfastCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Steadfast", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Steadfast.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Steadfast", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : (upgrade == Upgrade.A ? 1 : 2),
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.tempShield,
			statusAmount = 2,
			targetPlayer = true
		},
		new AStatus {
			status = ModEntry.Instance.SteadfastStatus.Status,
			statusAmount = 1,
			targetPlayer = true,
			shardcost = upgrade == Upgrade.B ? 1 : null
		}
	];
}


internal sealed class BurstShotCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("BurstShot", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/BurstShot.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BurstShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		temporary = true,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { FleetingManager.FleetingTrait };

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1)
		},
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1)
		},
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1),
			shardcost = 1
		}
	];
}


internal sealed class FadingDiamondCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("FadingDiamond", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/FadingDiamond.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FadingDiamond", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		unplayable = true,
		temporary = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "FadingDiamond", "description"]),
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => [GemManager.GemTrait, FleetingManager.FleetingTrait, HeavyManager.HeavyTrait],
		Upgrade.B => [GemManager.PreciousGemTrait, FleetingManager.FleetingTrait],
		_ => new HashSet<ICardTraitEntry>() { GemManager.GemTrait, FleetingManager.FleetingTrait }
	};

	public override List<CardAction> GetActions(State s, Combat c) => [];
}


internal sealed class UnobtainiumCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Unobtainium", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Unobtainium.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Unobtainium", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		unplayable = true,
		temporary = true,
		retain = true,
		buoyant = upgrade == Upgrade.A,
		description = ModEntry.Instance.Localizations.Localize(["card", "Unobtainium", "description"]),
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.B => [GemManager.PreciousGemTrait],
		_ => new HashSet<ICardTraitEntry>() { GemManager.GemTrait }
	};

	public override List<CardAction> GetActions(State s, Combat c) => [];
}


internal sealed class EncrustCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Encrust", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Encrust.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Encrust", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		retain = upgrade == Upgrade.B,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "Encrust", "description"]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AGemRightmost()
	];
}


internal sealed class PolishCard : Card, IHasCustomCardTraits, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Polish", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.LouisDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Polish.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Polish", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "Polish", "description"]),
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade == Upgrade.B ? new HashSet<ICardTraitEntry>() { HeavyManager.HeavyTrait } : [];

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ACardSelectImproved
		{
			browseAction = new AMakePreciousGem(),
			browseSource = CardBrowse.Source.Hand,
			filterUUID = uuid
		}.ApplyModData(CardBrowseFilterManager.FilterGemKey, true)
	];
}


internal sealed class LouisExeCard : Card, ILouisCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("LouisExe", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = Deck.colorless,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/LouisExe.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "LouisExe", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", "LouisExe", "description"], new { Amount = upgrade == Upgrade.B ? 3 : 2 }),
		artTint = "ffffff"
    };

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ACardOffering {
			amount = upgrade == Upgrade.B ? 5 : 3,
			limitDeck = ModEntry.Instance.LouisDeck.Deck,
			makeAllCardsTemporary = true,
			overrideUpgradeChances = false,
			canSkip = false,
			inCombat = true,
			discount = -1,
			dialogueSelector = ".summonLouis"
		},
		new AAddCard {
			card = new FadingDiamondCard {
				temporaryOverride = true
			},
			destination = CardDestination.Hand
		}
	];
}

