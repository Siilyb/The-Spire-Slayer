using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;

namespace Sts2Ai.Enemies;

public static class EnemyDb
{
    private static Dictionary<string, Func<IEnemyAi>> _factories = new();

    public static void RegisterAll()
    {
        Register(() => new ZapbotAi());
        Register(() => new ChomperAi());
        Register(() => new LeafSlimeMAi());
        Register(() => new TwigSlimeSAi());
        Register(() => new CorpseSlugAi());
        Register(() => new GremlinMercAi());
        Register(() => new SneakyGremlinAi());
        Register(() => new FatGremlinAi());
        Register(() => new NibbitAi());
        Register(() => new FlyconidAi());
        Register(() => new SnappingJaxfruitAi());
        Register(() => new LouseProgenitorAi());
        Register(() => new FuzzyWurmCrawlerAi());
        Register(() => new WrigglerAi());
        Register(() => new SpectralKnightAi());
        Register(() => new AssassinRubyRaiderAi());
        Register(() => new AxeRubyRaiderAi());
        Register(() => new BruteRubyRaiderAi());
        Register(() => new CrossbowRubyRaiderAi());
        Register(() => new TrackerRubyRaiderAi());
        Register(() => new BowlbugEggAi());
        Register(() => new BowlbugNectarAi());
        Register(() => new BowlbugRockAi());
        Register(() => new BowlbugSilkAi());
        Register(() => new CalcifiedCultistAi());
        Register(() => new DampCultistAi());
        Register(() => new GuardbotAi());
        Register(() => new NoisebotAi());
        Register(() => new StabbotAi());
        Register(() => new ParafrightAi());
        Register(() => new PunchConstructAi());
        Register(() => new MyteAi());
        Register(() => new SewerClamAi());
        Register(() => new ShrinkerBeetleAi());
        Register(() => new SpinyToadAi());
        Register(() => new AxebotAi());
        Register(() => new CubexConstructAi());
        Register(() => new FogmogAi());
        Register(() => new HauntedShipAi());
        Register(() => new MawlerAi());
        Register(() => new FossilStalkerAi());
        Register(() => new SeapunkAi());
        Register(() => new EyeWithTeethAi());
        Register(() => new ScrollOfBitingAi());
        Register(() => new InkletAi());
        Register(() => new GlobeHeadAi());
        Register(() => new TwoTailedRatAi());
        Register(() => new SlitheringStranglerAi());
        Register(() => new SludgeSpinnerAi());
        Register(() => new SlimedBerserkerAi());
        Register(() => new SlumberingBeetleAi());
        Register(() => new TheLostAi());
        Register(() => new TheForgottenAi());
        Register(() => new VineShamblerAi());
        Register(() => new OvicopterAi());
        Register(() => new ToughEggAi());
        Register(() => new PhrogParasiteAi());
        Register(() => new TheInsatiableAi());
        Register(() => new SoulFyshAi());
        Register(() => new TerrorEelAi());
        Register(() => new SoulNexusAi());
        Register(() => new InfestedPrismAi());
        Register(() => new FlailKnightAi());
        Register(() => new KinFollowerAi());
        Register(() => new KinPriestAi());
        Register(() => new HunterKillerAi());
        Register(() => new MysteriousKnightAi());
        Register(() => new FrogKnightAi());
        Register(() => new BigDummyAi());
        Register(() => new ArchitectAi());
        Register(() => new OneHpMonsterAi());
        Register(() => new TenHpMonsterAi());
        Register(() => new DeprecatedMonsterAi());
        Register(() => new BattleFriendV1Ai());
        Register(() => new BattleFriendV2Ai());
        Register(() => new BattleFriendV3Ai());
        Register(() => new OstyAi());
        Register(() => new ByrdpipAi());
        Register(() => new PaelsLegionAi());
        Register(() => new LeafSlimeSAi());
        Register(() => new TwigSlimeMAi());
        Register(() => new SingleAttackMoveMonsterAi());
        Register(() => new MultiAttackMoveMonsterAi());
        Register(() => new ExoskeletonAi());
        Register(() => new LivingShieldAi());
        Register(() => new LivingFogAi());
        Register(() => new TunnelerAi());
        Register(() => new ToadpoleAi());
        Register(() => new TurretOperatorAi());
        Register(() => new GasBombAi());
        Register(() => new VantomAi());
        Register(() => new SkulkingColonyAi());
        Register(() => new ThievingHopperAi());
        Register(() => new EntomancerAi());
        Register(() => new MagiKnightAi());
        Register(() => new DevotedSculptorAi());
        Register(() => new TheObscuraAi());
        Register(() => new OwlMagistrateAi());
        Register(() => new BygoneEffigyAi());
        Register(() => new ByrdonisAi());
        Register(() => new MechaKnightAi());
        Register(() => new FabricatorAi());
        Register(() => new DecimillipedeSegmentAi());
        Register(() => new DecimillipedeSegmentFrontAi());
        Register(() => new DecimillipedeSegmentMiddleAi());
        Register(() => new DecimillipedeSegmentBackAi());
        Register(() => new LagavulinMatriarchAi());
        Register(() => new CeremonialBeastAi());
        Register(() => new WaterfallGiantAi());
        Register(() => new QueenAi());
        Register(() => new KnowledgeDemonAi());
        Register(() => new AeonglassAi());
        Register(() => new TestSubjectAi());
        Register(() => new TheAdversaryMkOneAi());
        Register(() => new TheAdversaryMkTwoAi());
        Register(() => new TheAdversaryMkThreeAi());
        Register(() => new FakeMerchantMonsterAi());
        Register(() => new CrusherAi());
        Register(() => new RocketAi());
    }

    public static void Register(Func<IEnemyAi> factory)
    {
        var ai = factory();
        _factories[ai.EnemyId] = factory;
    }

    public static IEnemyAi? Create(string id)
    {
        return _factories.TryGetValue(id, out var factory) ? factory() : null;
    }
}
