using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Enemies;

public static class EnemyDb
{
    public static Dictionary<string, IEnemyAi> All { get; } = new();

    public static void RegisterAll()
    {
        Register(new ZapbotAi());
        Register(new ChomperAi());
        Register(new LeafSlimeMAi());
        Register(new TwigSlimeSAi());
        Register(new CorpseSlugAi());
        Register(new GremlinMercAi());
        Register(new SneakyGremlinAi());
        Register(new FatGremlinAi());
        Register(new NibbitAi());
        Register(new FlyconidAi());
        Register(new SnappingJaxfruitAi());
        Register(new LouseProgenitorAi());
        Register(new FuzzyWurmCrawlerAi());
        Register(new WrigglerAi());
        Register(new SpectralKnightAi());
        Register(new AssassinRubyRaiderAi());
        Register(new AxeRubyRaiderAi());
        Register(new BruteRubyRaiderAi());
        Register(new CrossbowRubyRaiderAi());
        Register(new TrackerRubyRaiderAi());
        Register(new BowlbugEggAi());
        Register(new BowlbugNectarAi());
        Register(new BowlbugRockAi());
        Register(new BowlbugSilkAi());
        Register(new CalcifiedCultistAi());
        Register(new DampCultistAi());
        Register(new GuardbotAi());
        Register(new NoisebotAi());
        Register(new StabbotAi());
        Register(new ParafrightAi());
        Register(new PunchConstructAi());
        Register(new MyteAi());
        Register(new SewerClamAi());
        Register(new ShrinkerBeetleAi());
        Register(new SpinyToadAi());
        Register(new AxebotAi());
        Register(new CubexConstructAi());
        Register(new FogmogAi());
        Register(new HauntedShipAi());
        Register(new MawlerAi());
        Register(new FossilStalkerAi());
        Register(new SeapunkAi());
        Register(new EyeWithTeethAi());
        Register(new ScrollOfBitingAi());
        Register(new InkletAi());
        Register(new GlobeHeadAi());
        Register(new TwoTailedRatAi());
        Register(new SlitheringStranglerAi());
        Register(new SludgeSpinnerAi());
        Register(new SlimedBerserkerAi());
        Register(new SlumberingBeetleAi());
        Register(new TheLostAi());
        Register(new TheForgottenAi());
        Register(new VineShamblerAi());
        Register(new OvicopterAi());
        Register(new ToughEggAi());
        Register(new PhrogParasiteAi());
        Register(new TheInsatiableAi());
        Register(new SoulFyshAi());
        Register(new TerrorEelAi());
        Register(new SoulNexusAi());
        Register(new InfestedPrismAi());
        Register(new FlailKnightAi());
        Register(new KinFollowerAi());
        Register(new KinPriestAi());
        Register(new HunterKillerAi());
        Register(new MysteriousKnightAi());
        Register(new FrogKnightAi());
    }

    public static void Register(IEnemyAi ai) => All[ai.EnemyId] = ai;

    public static IEnemyAi? Get(string id) => All.TryGetValue(id, out var ai) ? ai : null;
}
