using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Cards.Ironclad;
using Sts2Ai.Cards.Colorless;
using Sts2Ai.Cards.Curse;
using Sts2Ai.Cards.Status;
using Sts2Ai.Cards.Event;
using Sts2Ai.Cards.Token;
using Sts2Ai.Cards.Quest;

namespace Sts2Ai.Cards;

public static class CardRegistry
{
    public static Dictionary<string, Func<SimCard>> All { get; } = new();

    public static void RegisterAll()
    {
        // Basic
        Register(IroncladCards.StrikeIronclad, "STRIKE_IRONCLAD");
        Register(IroncladCards.DefendIronclad, "DEFEND_IRONCLAD");
        Register(IroncladCards.Bash, "BASH");

        // Common
        Register(IroncladCards.Anger, "ANGER");
        Register(IroncladCards.Armaments, "ARMAMENTS");
        Register(IroncladCards.BloodWall, "BLOOD_WALL");
        Register(IroncladCards.Bloodletting, "BLOODLETTING");
        Register(IroncladCards.BodySlam, "BODY_SLAM");
        Register(IroncladCards.Breakthrough, "BREAKTHROUGH");
        Register(IroncladCards.Cinder, "CINDER");
        Register(IroncladCards.Havoc, "HAVOC");
        Register(IroncladCards.Headbutt, "HEADBUTT");
        Register(IroncladCards.IronWave, "IRON_WAVE");
        Register(IroncladCards.MoltenFist, "MOLTEN_FIST");
        Register(IroncladCards.PerfectedStrike, "PERFECTED_STRIKE");
        Register(IroncladCards.PommelStrike, "POMMEL_STRIKE");
        Register(IroncladCards.SetupStrike, "SETUP_STRIKE");
        Register(IroncladCards.ShrugItOff, "SHRUG_IT_OFF");
        Register(IroncladCards.SwordBoomerang, "SWORD_BOOMERANG");
        Register(IroncladCards.Thunderclap, "THUNDERCLAP");
        Register(IroncladCards.Tremble, "TREMBLE");
        Register(IroncladCards.TrueGrit, "TRUE_GRIT");
        Register(IroncladCards.TwinStrike, "TWIN_STRIKE");

        // Uncommon
        Register(IroncladCards.AshenStrike, "ASHEN_STRIKE");
        Register(IroncladCards.BattleTrance, "BATTLE_TRANCE");
        Register(IroncladCards.Bludgeon, "BLUDGEON");
        Register(IroncladCards.Bully, "BULLY");
        Register(IroncladCards.BurningPact, "BURNING_PACT");
        Register(IroncladCards.Colossus, "COLOSSUS");
        Register(IroncladCards.DemonicShield, "DEMONIC_SHIELD");
        Register(IroncladCards.Dismantle, "DISMANTLE");
        Register(IroncladCards.Dominate, "DOMINATE");
        Register(IroncladCards.DrumOfBattle, "DRUM_OF_BATTLE");
        Register(IroncladCards.EvilEye, "EVIL_EYE");
        Register(IroncladCards.ExpectAFight, "EXPECT_A_FIGHT");
        Register(IroncladCards.FeelNoPain, "FEEL_NO_PAIN");
        Register(IroncladCards.FlameBarrier, "FLAME_BARRIER");
        Register(IroncladCards.ForgottenRitual, "FORGOTTEN_RITUAL");
        Register(IroncladCards.Hemokinesis, "HEMOKINESIS");
        Register(IroncladCards.HowlFromBeyond, "HOWL_FROM_BEYOND");
        Register(IroncladCards.InfernalBlade, "INFERNAL_BLADE");
        Register(IroncladCards.Inferno, "INFERNO");
        Register(IroncladCards.Inflame, "INFLAME");
        Register(IroncladCards.Juggling, "JUGGLING");
        Register(IroncladCards.Pillage, "PILLAGE");
        Register(IroncladCards.Rage, "RAGE");
        Register(IroncladCards.Rampage, "RAMPAGE");
        Register(IroncladCards.Rupture, "RUPTURE");
        Register(IroncladCards.SecondWind, "SECOND_WIND");
        Register(IroncladCards.Spite, "SPITE");
        Register(IroncladCards.Stampede, "STAMPEDE");
        Register(IroncladCards.Stomp, "STOMP");
        Register(IroncladCards.StoneArmor, "STONE_ARMOR");
        Register(IroncladCards.Taunt, "TAUNT");
        Register(IroncladCards.Unrelenting, "UNRELENTING");
        Register(IroncladCards.Uppercut, "UPPERCUT");
        Register(IroncladCards.Vicious, "VICIOUS");
        Register(IroncladCards.Whirlwind, "WHIRLWIND");

        // Rare
        Register(IroncladCards.Aggression, "AGGRESSION");
        Register(IroncladCards.Barricade, "BARRICADE");
        Register(IroncladCards.Brand, "BRAND");
        Register(IroncladCards.Cascade, "CASCADE");
        Register(IroncladCards.Conflagration, "CONFLAGRATION");
        Register(IroncladCards.CrimsonMantle, "CRIMSON_MANTLE");
        Register(IroncladCards.Cruelty, "CRUELTY");
        Register(IroncladCards.DarkEmbrace, "DARK_EMBRACE");
        Register(IroncladCards.DemonForm, "DEMON_FORM");
        Register(IroncladCards.Feed, "FEED");
        Register(IroncladCards.FiendFire, "FIEND_FIRE");
        Register(IroncladCards.Hellraiser, "HELLRAISER");
        Register(IroncladCards.Impervious, "IMPERVIOUS");
        Register(IroncladCards.Juggernaut, "JUGGERNAUT");
        Register(IroncladCards.NotYet, "NOT_YET");
        Register(IroncladCards.Offering, "OFFERING");
        Register(IroncladCards.OneTwoPunch, "ONE_TWO_PUNCH");
        Register(IroncladCards.PactsEnd, "PACTS_END");
        Register(IroncladCards.PrimalForce, "PRIMAL_FORCE");
        Register(IroncladCards.Pyre, "PYRE");
        Register(IroncladCards.Stoke, "STOKE");
        Register(IroncladCards.Tank, "TANK");
        Register(IroncladCards.TearAsunder, "TEAR_ASUNDER");
        Register(IroncladCards.Thrash, "THRASH");
        Register(IroncladCards.Unmovable, "UNMOVABLE");

        // Ancient
        Register(IroncladCards.Break, "BREAK");
        Register(IroncladCards.Corruption, "CORRUPTION");

        // === Colorless ===
        Register(ColorlessCards.UltimateStrike, "ULTIMATE_STRIKE");
        Register(ColorlessCards.UltimateDefend, "ULTIMATE_DEFEND");
        Register(ColorlessCards.Finesse, "FINESSE");
        Register(ColorlessCards.FlashOfSteel, "FLASH_OF_STEEL");
        Register(ColorlessCards.MasterOfStrategy, "MASTER_OF_STRATEGY");
        Register(ColorlessCards.Production, "PRODUCTION");
        Register(ColorlessCards.Impatience, "IMPATIENCE");
        Register(ColorlessCards.PanicButton, "PANIC_BUTTON");
        Register(ColorlessCards.Prowess, "PROWESS");
        Register(ColorlessCards.Equilibrium, "EQUILIBRIUM");
        Register(ColorlessCards.EternalArmor, "ETERNAL_ARMOR");
        Register(ColorlessCards.Automation, "AUTOMATION");
        Register(ColorlessCards.Mayhem, "MAYHEM");
        Register(ColorlessCards.Nostalgia, "NOSTALGIA");
        Register(ColorlessCards.Stratagem, "STRATAGEM");
        Register(ColorlessCards.Panache, "PANACHE");
        Register(ColorlessCards.PrepTime, "PREP_TIME");
        Register(ColorlessCards.Fasten, "FASTEN");
        Register(ColorlessCards.RollingBoulder, "ROLLING_BOULDER");
        Register(ColorlessCards.Calamity, "CALAMITY");
        Register(ColorlessCards.Entropy, "ENTROPY");
        Register(ColorlessCards.MindBlast, "MIND_BLAST");
        Register(ColorlessCards.GoldAxe, "GOLD_AXE");
        Register(ColorlessCards.Omnislice, "OMNISLICE");
        Register(ColorlessCards.DramaticEntrance, "DRAMATIC_ENTRANCE");
        Register(ColorlessCards.Fisticuffs, "FISTICUFFS");
        Register(ColorlessCards.SeekerStrike, "SEEKER_STRIKE");
        Register(ColorlessCards.Rend, "REND");
        Register(ColorlessCards.Salvo, "SALVO");
        Register(ColorlessCards.TheGambit, "THE_GAMBIT");
        Register(ColorlessCards.Prolong, "PROLONG");
        Register(ColorlessCards.ThinkingAhead, "THINKING_AHEAD");
        Register(ColorlessCards.Scrawl, "SCRAWL");
        Register(ColorlessCards.Restlessness, "RESTLESSNESS");
        Register(ColorlessCards.Purity, "PURITY");
        Register(ColorlessCards.Bolas, "BOLAS");
        Register(ColorlessCards.ThrummingHatchet, "THRUMMING_HATCHET");
        Register(ColorlessCards.Volley, "VOLLEY");
        Register(ColorlessCards.Lift, "LIFT");
        Register(ColorlessCards.Intercept, "INTERCEPT");
        Register(ColorlessCards.Rally, "RALLY");
        Register(ColorlessCards.BelieveInYou, "BELIEVE_IN_YOU");
        Register(ColorlessCards.Coordinate, "COORDINATE");
        Register(ColorlessCards.TagTeam, "TAG_TEAM");
        Register(ColorlessCards.Knockdown, "KNOCKDOWN");
        Register(ColorlessCards.GangUp, "GANG_UP");
        Register(ColorlessCards.HuddleUp, "HUDDLE_UP");
        Register(ColorlessCards.BeaconOfHope, "BEACON_OF_HOPE");
        Register(ColorlessCards.JackOfAllTrades, "JACK_OF_ALL_TRADES");
        Register(ColorlessCards.Discovery, "DISCOVERY");
        Register(ColorlessCards.Splash, "SPLASH");
        Register(ColorlessCards.Jackpot, "JACKPOT");
        Register(ColorlessCards.Alchemize, "ALCHEMIZE");
        Register(ColorlessCards.Anointed, "ANOINTED");
        Register(ColorlessCards.SecretTechnique, "SECRET_TECHNIQUE");
        Register(ColorlessCards.SecretWeapon, "SECRET_WEAPON");
        Register(ColorlessCards.DarkShackles, "DARK_SHACKLES");
        Register(ColorlessCards.HandOfGreed, "HAND_OF_GREED");
        Register(ColorlessCards.HiddenGem, "HIDDEN_GEM");
        Register(ColorlessCards.Mimic, "MIMIC");
        Register(ColorlessCards.BeatDown, "BEAT_DOWN");
        Register(ColorlessCards.Catastrophe, "CATASTROPHE");
        Register(ColorlessCards.TheBomb, "THE_BOMB");

        // === Curse (18) ===
        Register(CurseCards.AscendersBane, "ASCENDERS_BANE");
        Register(CurseCards.BadLuck, "BAD_LUCK");
        Register(CurseCards.Clumsy, "CLUMSY");
        Register(CurseCards.CurseOfTheBell, "CURSE_OF_THE_BELL");
        Register(CurseCards.Debt, "DEBT");
        Register(CurseCards.Decay, "DECAY");
        Register(CurseCards.Doubt, "DOUBT");
        Register(CurseCards.Enthralled, "ENTHRALLED");
        Register(CurseCards.Folly, "FOLLY");
        Register(CurseCards.Greed, "GREED");
        Register(CurseCards.Guilty, "GUILTY");
        Register(CurseCards.Injury, "INJURY");
        Register(CurseCards.Normality, "NORMALITY");
        Register(CurseCards.PoorSleep, "POOR_SLEEP");
        Register(CurseCards.Regret, "REGRET");
        Register(CurseCards.Shame, "SHAME");
        Register(CurseCards.SporeMind, "SPORE_MIND");
        Register(CurseCards.Writhe, "WRITHE");

        // === Status (12) ===
        Register(StatusCards.Beckon, "BECKON");
        Register(StatusCards.Burn, "BURN");
        Register(StatusCards.Dazed, "DAZED");
        Register(StatusCards.Debris, "DEBRIS");
        Register(StatusCards.FranticEscape, "FRANTIC_ESCAPE");
        Register(StatusCards.Infection, "INFECTION");
        Register(StatusCards.Wither, "WITHER");
        Register(StatusCards.Slimed, "SLIMED");
        Register(StatusCards.Soot, "SOOT");
        Register(StatusCards.Toxic, "TOXIC");
        Register(StatusCards.Void, "VOID");
        Register(StatusCards.Wound, "WOUND");

        // === Event (27) ===
        Register(EventCards.Apotheosis, "APOTHEOSIS");
        Register(EventCards.Apparition, "APPARITION");
        Register(EventCards.BrightestFlame, "BRIGHTEST_FLAME");
        Register(EventCards.ByrdSwoop, "BYRD_SWOOP");
        Register(EventCards.Caltrops, "CALTROPS");
        Register(EventCards.Clash, "CLASH");
        Register(EventCards.Distraction, "DISTRACTION");
        Register(EventCards.DualWield, "DUAL_WIELD");
        Register(EventCards.Enlightenment, "ENLIGHTENMENT");
        Register(EventCards.Entrench, "ENTRENCH");
        Register(EventCards.Exterminate, "EXTERMINATE");
        Register(EventCards.FeedingFrenzy, "FEEDING_FRENZY");
        Register(EventCards.HelloWorld, "HELLO_WORLD");
        Register(EventCards.MadScience, "MAD_SCIENCE");
        Register(EventCards.Maul, "MAUL");
        Register(EventCards.Metamorphosis, "METAMORPHOSIS");
        Register(EventCards.NeowsFury, "NEOWS_FURY");
        Register(EventCards.Outmaneuver, "OUTMANEUVER");
        Register(EventCards.Peck, "PECK");
        Register(EventCards.Rebound, "REBOUND");
        Register(EventCards.Relax, "RELAX");
        Register(EventCards.RipAndTear, "RIP_AND_TEAR");
        Register(EventCards.Squash, "SQUASH");
        Register(EventCards.Stack, "STACK");
        Register(EventCards.ToricToughness, "TORIC_TOUGHNESS");
        Register(EventCards.Wish, "WISH");
        Register(EventCards.Whistle, "WHISTLE");

        // === Token (14) ===
        Register(TokenCards.Disintegration, "DISINTEGRATION");
        Register(TokenCards.Fuel, "FUEL");
        Register(TokenCards.GiantRock, "GIANT_ROCK");
        Register(TokenCards.Luminesce, "LUMINESCE");
        Register(TokenCards.MindRot, "MIND_ROT");
        Register(TokenCards.MinionDiveBomb, "MINION_DIVE_BOMB");
        Register(TokenCards.MinionSacrifice, "MINION_SACRIFICE");
        Register(TokenCards.MinionStrike, "MINION_STRIKE");
        Register(TokenCards.Shiv, "SHIV");
        Register(TokenCards.Sloth, "SLOTH");
        Register(TokenCards.Soul, "SOUL");
        Register(TokenCards.SovereignBlade, "SOVEREIGN_BLADE");
        Register(TokenCards.SweepingGaze, "SWEEPING_GAZE");
        Register(TokenCards.WasteAway, "WASTE_AWAY");

        // === Quest (3) ===
        Register(QuestCards.ByrdonisEgg, "BYRDONIS_EGG");
        Register(QuestCards.LanternKey, "LANTERN_KEY");
        Register(QuestCards.SpoilsMap, "SPOILS_MAP");
    }

    private static void Register(Func<SimCard> factory, string id) => All[id] = factory;
}
