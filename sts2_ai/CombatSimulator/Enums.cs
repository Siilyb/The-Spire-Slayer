namespace Sts2Ai.CombatSimulator;

public enum CombatSide
{
    Player,
    Enemy
}

public enum CardType
{
    None,
    Attack,
    Skill,
    Power,
    Status,
    Curse,
    Quest
}

public enum CardRarity
{
    None,
    Basic,
    Common,
    Uncommon,
    Rare,
    Ancient,
    Event,
    Token,
    Status,
    Curse,
    Quest
}

public enum TargetType
{
    None,
    Self,
    SingleEnemy,
    AllEnemies,
    RandomEnemy,
    AnyAlly,
    AllAllies
}

public enum PowerType
{
    Buff,
    Debuff
}

public enum PowerStackType
{
    Counter,
    Single
}

[Flags]
public enum ValueProp
{
    None = 0,
    Unblockable = 2,
    Unpowered = 4,
    Move = 8,
    SkipHurtAnim = 16
}
