using Sts2Ai.CombatSimulator.State;
using Sts2Ai.Cards.Ironclad;

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
    }

    private static void Register(Func<SimCard> factory, string id) => All[id] = factory;
}
