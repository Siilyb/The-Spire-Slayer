# Phase 1 完整参考文档（推倒重做用）

> 本文档记录 sts2_ai 项目中 Phase 1（战斗模拟器）的全部设计、代码结构、已实现内容及技术债务，供重新实现时参考。

---

## 一、项目结构

```
sts2_ai/                          # .NET 8.0 类库
├── CombatSimulator/
│   ├── Enums.cs                  # 基础枚举
│   ├── IRandom.cs                # 随机数接口 + DefaultRandom + SeedRandom
│   ├── SimEngine.cs              # 战斗引擎核心
│   ├── CardDb.cs                 # 卡牌数据库（手写）
│   ├── State/
│   │   ├── SimState.cs           # 战局状态
│   │   ├── SimCreature.cs        # 生物（HP/Block/Powers）
│   │   ├── SimCard.cs            # 卡牌
│   │   └── SimCardPile.cs        # 牌堆管理
│   ├── Effects/                  # 效果原语（19+ 个）
│   │   ├── IEffect.cs            # 接口
│   │   ├── DealDamage.cs
│   │   ├── GainBlock.cs
│   │   ├── DrawCards.cs
│   │   ├── ApplyPower.cs
│   │   ├── GainEnergy.cs
│   │   ├── Heal.cs
│   │   ├── SelfDamage.cs
│   │   ├── DiscardHand.cs
│   │   ├── ExhaustThisCard.cs
│   │   ├── GainMaxHp.cs
│   │   ├── DamageWithBlock.cs
│   │   ├── CreateCardInDiscard.cs
│   │   ├── MoveCardFromDiscardToDrawTop.cs
│   │   ├── ExhaustFromHand.cs
│   │   ├── AutoPlayFromDrawPile.cs
│   │   ├── PutCardOnDrawPile.cs
│   │   ├── DuplicateCardInHand.cs
│   │   ├── ReturnFromExhaustToHand.cs
│   │   └── UpgradeFromHand.cs
│   └── Powers/                   # Power 系统（24+ 个）
│       ├── SimPower.cs           # Power 基类（含 Hook 方法）
│       ├── CorePowers.cs         # Strength/Dexterity/Vulnerable/Weak/Frail
│       ├── Batch2Powers.cs       # Plating/Regen/Buffer/Intangible/Barricade
│       ├── Batch3Powers.cs       # Focus/Poison/Thorns/Blur/Accuracy
│       ├── ArtifactPower.cs      # Artifact
│       ├── Batch4Powers.cs       # DemonForm/DarkEmbrace/FeelNoPain/FlameBarrier/Juggernaut/NoDraw/Rupture/Corruption
│       └── Batch5Powers.cs       # Rage/Inferno/CrimsonMantle/Cruelty/FreeAttack/Pyre/Vicious/Mangle/NoEnergyGain
├── MCTS/
│   ├── MctsNode.cs               # MCTS 树节点 + UCB1
│   ├── MctsEngine.cs             # 搜索引擎（Selection/Expansion/Rollout/Backprop）
│   ├── RolloutPolicy.cs          # 启发式 Rollout 策略
│   └── CombatAi.cs               # ICombatDecisionMaker 实现
├── BattleScenarios.cs             # 预置战斗场景（不含手牌预设，抽牌堆初始）
├── sts2_ai.csproj                 # 项目文件

sts2_ai_tests/                    # .NET 8.0 测试项目
├── Tests/
│   ├── SimEngineTests.cs
│   ├── BatchPowerTests.cs
│   ├── Batch4PowerTests.cs
│   ├── CardDbTests.cs
│   ├── EnemyBehaviorTests.cs
│   └── MctsTests.cs
└── sts2_ai_tests.csproj

sts2_ai_mod/                      # .NET 9.0 Mod 项目
├── Sts2AiMod.cs                  # [ModInitializer] + Harmony Patch
├── ModuleInit.cs                 # [ModuleInitializer] 提前注册 AssemblyResolve
├── StateConverter.cs             # 游戏 CombatState → SimState
├── CardMapper.cs                 # 游戏卡牌 ID → SimCard ID 映射
├── ConsoleDisplay.cs             # AllocConsole + 状态打印 + MCTS 建议
└── mod.json

sts2_ai_visual/                   # .NET 8.0 控制台可视化项目
└── Program.cs                    # 菜单选择战斗场景 + 战斗日志输出
```

---

## 二、核心数据模型

### SimState

```csharp
class SimState {
    List<SimCreature> Players;
    List<SimCreature> Enemies;
    SimCardPile DrawPile, Hand, DiscardPile, ExhaustPile;
    int Energy, MaxEnergy = 3, TurnNumber;
    CombatSide CurrentSide = Player;
    IRandom Rng;
    Clone() → 深拷贝全部
}
```

### SimCreature

```csharp
class SimCreature {
    string Name;
    CombatSide Side;
    int CurrentHp, MaxHp, Block;
    bool IsAlive => CurrentHp > 0;
    List<SimPower> Powers;
    Dictionary<string, object> CustomState;  // 敌人状态追踪
    
    void GainBlock(decimal);
    DamageResult LoseHp(decimal, ValueProp);
    void ApplyPower(SimPower);       // 同类 Power 自动合并数值
    void RemovePower(SimPower);
    T? GetPower<T>();
    int GetPowerAmount<T>();
    void TickDownPowers();           // Counter 类型递减
}
```

### SimCard

```csharp
class SimCard {
    string Id, Name;
    CardType Type;              // Attack/Skill/Power/Status/Curse
    CardRarity Rarity;          // Basic/Common/Uncommon/Rare/Status/Curse
    int EnergyCost;             // -1 = X cost (不支持)
    TargetType DefaultTargetType;
    List<IEffect> Effects;
    List<string> Keywords;      // "Exhaust", "Unplayable", "Strike"...
    List<string> Tags;
    bool IsUpgraded;
    SimCreature Owner, Target;
    Clone() → 浅拷贝 Effects, 深拷贝 Keywords/Tags
}
```

### SimCardPile

```csharp
class SimCardPile {
    IReadOnlyList<SimCard> Cards;
    void Add(SimCard), Remove(SimCard), Clear();
    SimCard? Draw();
    List<SimCard> DrawMultiple(int);
    void Shuffle(IRandom);
    void InsertAt(int, SimCard);
    void MoveTo(SimCardPile, Func<SimCard,bool>?);
}
```

---

## 三、枚举定义

```csharp
enum CombatSide { Player, Enemy }
enum CardType { None, Attack, Skill, Power, Status, Curse, Quest }
enum CardRarity { Basic, Common, Uncommon, Rare, Curse, Status, Event, Quest, Ancient }
enum TargetType { Self, SingleEnemy, AllEnemies, RandomEnemy, AnyAlly }
enum PowerType { Buff, Debuff }
enum PowerStackType { Counter, Single }  // Counter=可叠加数值, Single=唯一实例

[Flags]
enum ValueProp {
    None = 0,
    Unblockable = 1 << 0,   // 不被格挡
    Unpowered = 1 << 1,     // 不受力量影响
    Move = 1 << 2,         // 基础攻击（非技能生成）
}
```

---

## 四、效果原语 Effect（共 19 个）

| 原语 | 文件 | 参数 | 说明 |
|------|------|------|------|
| `IEffect` | IEffect.cs | `Execute(engine, source, target)` | 接口 |
| `DealDamage` | DealDamage.cs | `BaseAmount, Target, Props, HitCount, StrengthMultiplier, CalculatedDamageFn` | 伤害（多段/动态/倍率） |
| `GainBlock` | GainBlock.cs | `BaseAmount, Target, CalculatedBlockFn` | 格挡 |
| `DrawCards` | DrawCards.cs | `Amount` | 抽牌（受 NoDrawPower 限制） |
| `ApplyPower` | ApplyPower.cs | `PowerFactory` 或 `(Type, Amount, Target)` | 施加 Power |
| `GainEnergy` | GainEnergy.cs | `Amount` | 获得能量（受 ModifyEnergyGain 限制） |
| `Heal` | Heal.cs | `Amount, Target` | 治疗 |
| `SelfDamage` | SelfDamage.cs | `Amount` | 自伤（Unblockable+Unpowered） |
| `DiscardHand` | DiscardHand.cs | `Amount(0=全部)` | 丢弃手牌 |
| `ExhaustThisCard` | ExhaustThisCard.cs | — | 消耗此牌 |
| `GainMaxHp` | GainMaxHp.cs | `Amount` | 增加最大 HP + 等量治疗 |
| `DamageWithBlock` | DamageWithBlock.cs | `BaseAmount, Target, Multiplier` | 伤害=格挡×倍率+基础 |
| `CreateCardInDiscard` | CreateCardInDiscard.cs | — | 复制此牌到弃牌堆 |
| `MoveCardFromDiscardToDrawTop` | MoveCardFromDiscardToDrawTop.cs | — | 弃牌堆随机牌→抽牌堆顶 |
| `ExhaustFromHand` | ExhaustFromHand.cs | `Amount, Filter` | 从手牌消耗 |
| `AutoPlayFromDrawPile` | AutoPlayFromDrawPile.cs | `ExhaustAfterPlay` | 自动打出牌堆顶 |
| `PutCardOnDrawPile` | PutCardOnDrawPile.cs | `CardFilter` | 手牌→抽牌堆顶 |
| `DuplicateCardInHand` | DuplicateCardInHand.cs | `CardFilter, UpgradeDuplicate` | 复制手牌 |
| `ReturnFromExhaustToHand` | ReturnFromExhaustToHand.cs | — | 消耗堆→手牌 |
| `UpgradeFromHand` | UpgradeFromHand.cs | `Amount, Filter` | 升级手牌 |

### 需要新增的 Effect

| 原语 | 用途 |
|------|------|
| `GainEnergyOnExhaust` | DrumOfBattle：消耗此牌时获得能量 |
| `GainEnergyPerAttackInHand` | ExpectAFight：能量=手牌中攻击牌数 |
| `GenerateRandomAttack` | InfernalBlade：生成随机攻击牌到手牌 |
| `SendToExhaustPileOnDrawn` | Dazed、Burn 等状态卡：抽到时自动消耗 |

---

## 五、Power 系统（共 34 个）

### Power 基类 Hook 方法

```csharp
abstract class SimPower {
    int Amount;
    PowerType Type;              // Buff / Debuff
    PowerStackType StackType;     // Counter / Single
    SimCreature? Owner;
    bool AllowNegative;

    // === Hook 方法 ===
    virtual decimal ModifyDamageAdditive(damage, attacker, target, strengthMultiplier);
    virtual decimal ModifyDamageMultiplicative(damage, attacker, target);
    virtual decimal ModifyBlockAdditive(block, creature);
    virtual bool ShouldClearBlock(creature);
    virtual void AfterSideTurnStart(engine, owner);
    virtual void AfterSideTurnEnd(engine, owner);
    virtual void BeforeCardPlayed(engine, owner, card);
    virtual void AfterCardPlayed(engine, owner, card);
    virtual void AfterCardExhausted(engine, owner, card);
    virtual void AfterBlockGained(engine, owner, amount);
    virtual void BeforeDamageReceived(engine, target, attacker, damage);
    virtual int ModifyCardCost(originalCost, card);
    virtual bool ShouldCardExhaust(card);
    virtual int ModifyEnergyGain(amount);
    virtual void TickDown();
    virtual SimPower Clone();
}
```

### 已实现的 Power（34 个）

| 批次 | Power | Hook | 效果 |
|------|-------|------|------|
| Core | StrengthPower | ModifyDamageAdditive | 攻击伤害 +Amount×倍率 |
| Core | DexterityPower | ModifyBlockAdditive | 格挡 +Amount |
| Core | VulnerablePower | ModifyDamageMultiplicative | 受到伤害 ×1.5 |
| Core | WeakPower | ModifyDamageMultiplicative | 造成伤害 ×0.75 |
| Core | FrailPower | ModifyBlockAdditive | 获得格挡 ×0.75 |
| Batch2 | PlatingPower | AfterSideTurnStart | 回合开始得 Amount 格挡，-1 |
| Batch2 | RegenPower | AfterSideTurnEnd | 回合结束回 Amount HP，-1 |
| Batch2 | BufferPower | AbsorbHit() | 吸收致命伤害 |
| Batch2 | IntangiblePower | ModifyDamageMultiplicative | 伤害上限 1 |
| Batch2 | BarricadePower | ShouldClearBlock | 格挡不清除 |
| Batch3 | FocusPower | ModifyOrbValue | 球体效果 |
| Batch3 | PoisonPower | AfterSideTurnStart | 回合开始毒伤 |
| Batch3 | ThornsPower | TriggerThorns | 反伤 |
| Batch3 | BlurPower | ShouldClearBlock + AfterSideTurnStart | 保护格挡 |
| Batch3 | AccuracyPower | ModifyShivDamage | Shiv 伤害 |
| Special | ArtifactPower | (ApplyPower 检查) | 抵消 Debuff |
| Batch4 | DemonFormPower | AfterSideTurnStart | 每回合 +Amount 力量 |
| Batch4 | DarkEmbracePower | AfterCardExhausted | 消耗时抽 Amount 牌 |
| Batch4 | FeelNoPainPower | AfterCardExhausted | 消耗时得 Amount 格挡 |
| Batch4 | FlameBarrierPower | BeforeDamageReceived | 受击反伤 Amount |
| Batch4 | JuggernautPower | AfterBlockGained | 得格挡时对敌伤害 |
| Batch4 | NoDrawPower | (DrawCards 检查) | 禁抽牌 |
| Batch4 | RupturePower | BeforeDamageReceived | 受伤时 +Amount 力量 |
| Batch4 | CorruptionPower | ModifyCardCost + ShouldCardExhaust | 技能 0 费 + 消耗 |
| Batch5 | RagePower | AfterCardPlayed | 打攻击牌得 Amount 格挡 |
| Batch5 | InfernoPower | AfterCardPlayed | 打攻击牌对全体 Amount 伤害 |
| Batch5 | CrimsonMantlePower | AfterCardPlayed | 打攻击牌得 Amount 格挡 |
| Batch5 | CrueltyPower | BeforeCardPlayed | 打攻击牌上 Amount 易伤 |
| Batch5 | FreeAttackPower | ModifyCardCost | 下张攻击牌 0 费 |
| Batch5 | PyrePower | AfterSideTurnStart | 回合开始 +Amount 能量 |
| Batch5 | ViciousPower | AfterCardPlayed | 上易伤时抽 Amount 牌 |
| Batch5 | ManglePower | ModifyDamageAdditive | 目标减 Amount 力量 |
| Batch5 | NoEnergyGainPower | ModifyEnergyGain → 0 | 禁能量获取 |
| Batch5 | PlaceholderPower | — | 无效果占位 |

### 仍需实现的 Power

| Power | 效果 | 用途 |
|-------|------|------|
| HellraiserPower | 未知（游戏源码未找到定义） | Hellraiser 卡 |
| UnmovablePower | 未知 | Unmovable 卡 |
| JugglingPower | 未知 | Juggling 卡 |
| StampedePower | 未知 | Stampede 卡 |

---

## 六、SimEngine 战斗循环

```
RunFullCombat(ai, maxTurns):
  InitializeEnemies()    // 调用 OnCombatStart + CorpseSlug 偏移分配
  loop turn=0..maxTurns:
    TurnNumber++
    CheckCombatEnd
    
    // === 玩家回合 ===
    CurrentSide = Player
    BeginPlayerTurn()    // 能量重置 + 触发 AfterSideTurnStart
    DrawCardsForPlayer()  // 抽 5 张（受 NoDrawPower 限制）
    cardsToPlay = ai.ChooseCards(State)
    foreach card in cardsToPlay:
      PlayCard(card)
      CheckCombatEnd
    
    EndPlayerTurn()      // 触发 AfterSideTurnEnd + 弃手牌
    
    // === 敌人回合 ===
    CurrentSide = Enemy
    BeginEnemyTurn()
    foreach enemy:
      EnemyTakeTurn()    // 查 EnemyDb → PlanNextTurn → 执行
    EndEnemyTurn()       // TickDownPowers
```

### PlayCard 流程

```
PlayCard(card):
  1. if !Hand.Contains(card) → return    // 已消耗的牌
  2. if Keywords contains "Unplayable" → return  // 状态/诅咒牌
  3. modifiedCost = 应用 ModifyCardCost Hook
  4. X 费卡 (EnergyCost=-1) → modifiedCost = State.Energy
  5. if State.Energy < modifiedCost → return
  6. Auto-selected target (if null + not Self)
  7. State.Energy -= modifiedCost
  8. BeforeCardPlayed Hook
  9. 执行所有 Effects
  10. AfterCardPlayed Hook
  11. HandleCardDisposition(card)     // Exhaust 关键词/Corruption → 消耗; 否则弃牌
```

### DealDamage 流水线

```
DealDamage(attacker, target, baseDamage, props, strengthMultiplier):
  1. damage = baseDamage
  2. Attacker ModifyDamageAdditive (力量 × strengthMultiplier)
  3. Target ModifyDamageMultiplicative (易伤 ×1.5)
  4. Attacker ModifyDamageMultiplicative (虚弱 ×0.75)
  5. if Unpowered: damage = baseDamage, re-apply multiplicative
  6. BeforeDamageReceived Hook (Rupture, FlameBarrier)
  7. BufferPower 吸收 (if damage >= target.HP)
  8. IntangiblePower → damage = 1 (if ≥ 1)
  9. target.LoseHp(damage, props)  // 格挡处理 + HP 减少
  10. TriggerThorns (if not Unpowered)
```

---

## 七、卡牌数据库 CardDb

### 数据来源

手写，从游戏源码 `src/Core/Models/Cards/*.cs` 逐张提取。每张卡定义为 `private static SimCard CardName() => new() { ... }`。

### 卡牌 ID 映射规则

游戏内部使用 `ModelId.Entry` 格式：类名经 `Slugify()` 处理 → 大写蛇形。
- `StrikeIronclad` → `STRIKE_IRONCLAD`
- `DefendIronclad` → `DEFEND_IRONCLAD`
- `PommelStrike` → `POMMEL_STRIKE`
- `BodySlam` → `BODY_SLAM`

**重要**：注册 `RegisterAll()` 中两张卡之间不能有缺失。如果方法定义被删除，注册会报错。

### 完整 Ironclad 卡牌清单（87 张）

每条都应有 base 和 upgraded 两个版本。

**Basic (3)**：
```
StrikeIronclad   Attack  1费  6(9)伤  Tag:Strike
DefendIronclad   Skill   1费  5(8)格挡  Tag:Defend
Bash             Attack  2费  8(10)伤 + 2(3)易伤
```

**Common (26)**：
```
Anger            Attack  0费  6(8)伤 + 复制到弃牌堆
Armaments        Skill   1费  5格挡 + 升级1(全部)手牌
BloodWall        Skill   2费  自伤2 + 16(20)格挡
Bloodletting     Skill   0费  自伤3 + 2(3)能量
BodySlam         Attack  1(0)费  伤害 = 当前格挡值
Breakthrough     Attack  1费  自伤1 + 9(13)全体伤害
Cinder           Attack  2费  18(24)伤 + 消耗1手牌  Exhaust
Cleave           Attack  1费  8(11)全体伤害
HeavyBlade       Attack  1费  7伤 × 5力倍率
Headbutt         Attack  1费  9(12)伤 + 弃牌堆→抽牌堆顶
IronWave         Attack  1费  5(7)格挡 + 5(7)伤
MoltenFist       Attack  1费  10(14)伤  Exhaust
PerfectedStrike  Attack  2费  6 + 2×Strike标签数  Tag:Strike
PommelStrike     Attack  1费  9(10)伤 + 抽1(2)  Tag:Strike
SetupStrike      Attack  1费  7(9)伤 + 2(3)力量  Tag:Strike
ShrugItOff       Skill   1费  8(11)格挡 + 抽1
SwordBoomerang   Attack  1费  3伤 × 3(4)次随机
Thunderclap      Attack  1费  4(7)全体 + 1全体易伤
Tremble          Skill   1费  3(4)易伤  Exhaust
TrueGrit         Skill   1费  7(9)格挡 + 消耗1手牌
TwinStrike       Attack  1费  5(7)×2  Tag:Strike
Havoc            Skill   1(0)费  自动打出牌堆顶后消耗
Warcry           Skill   0费  抽1(2) + 手牌→抽牌堆顶  Exhaust
Intimidate       Skill   0费  1(2)全体虚弱  Exhaust
```

**Uncommon (33)**：
```
AshenStrike      Attack  1费  6 + 3×消耗堆卡数  Tag:Strike
BattleTrance     Skill   0费  抽3(4) + NoDrawPower
Bludgeon         Attack  3费  32(42)伤
Bully            Attack  0费  4 + 2(3)×目标易伤层数
BurningPact      Skill   1费  消耗1手牌 + 抽2(3)
Colossus         Skill   1费  5(8)格挡 + ColossusPower
DemonicShield    Skill   0费  自伤1 + 格挡=当前格挡  Exhaust (多人)
Dismantle        Attack  1费  8(10)伤 × 1(2)(有易伤时×2)
Dominate         Skill   1费  1(2)易伤 + 力量=目标易伤层数  Exhaust
DrumOfBattle     Skill   1费  抽2 + 消耗时得2(3)能量  Exhaust
EvilEye          Skill   1费  8(11)格挡(本回合有消耗则×2)
ExpectAFight     Skill   2(1)费  能量=手牌攻击牌数 + NoEnergyGain
FightMe          Attack  2费  5(6)×2 + 自3(4)力量 + 敌1力量
FlameBarrier     Skill   2费  12(16)格挡 + 4(6)FlameBarrierPower
ForgottenRitual  Skill   1费  本回合有消耗则得3(4)能量  Exhaust
Hemokinesis      Attack  1费  自伤2 + 15(20)伤
HowlFromBeyond   Attack  3费  16(21)全体  Exhaust + 消耗后自动打出
InfernalBlade    Skill   1(0)费  生成随机攻击牌0费到手牌  Exhaust
Inferno          Power   1费  InfernoPower 6(9)
Juggling         Power   1费  JugglingPower(未知) 升级:Innate
Metallicize      Power   1费  3(4)PlatingPower
Pillage          Attack  1费  6(9)伤 + 抽到非攻击为止
Rage             Skill   0费  RagePower 3(5)
Rampage          Attack  1费  8(+5/次) 自增伤害
RecklessCharge   Attack  0费  7(10)伤  Exhaust
Rupture          Power   1费  RupturePower 1(2)
SearingBlow      Attack  2费  12(16)伤(每升1级+4)
SecondWind       Skill   1费  5(7)格挡 + 消耗全部非攻击手牌
SeeingRed        Skill   1(0)费  2能量
Sentinel         Skill   1费  12(16)格挡(消耗时得3(5)能量)
Stampede         Power   2(1)费  StampedePower(未知)
StoneArmor       Power   1费  4(6)PlatingPower
Stomp            Attack  3费  12(15)全体(每打1攻击减1费)
Taunt            Skill   1费  7(8)格挡 + 1(2)易伤
Unrelenting      Attack  2费  14(20)伤 + FreeAttackPower
Uppercut         Attack  2费  13伤 + 1(2)虚弱 + 1(2)易伤
Vicious          Power   1费  1(2)ViciousPower
Whirlwind        Attack   -1费(X)  5(8)全体×X次
StoneArmor       Power   1费  PlatingPower 4(6)
FeelNoPain       Power   1费  FeelNoPainPower 3(4)
DualWield        Skill   1费  复制攻击/力量牌到手中(升级复制品升级)
Inflame          Power   1费  StrengthPower 2(3)
Rupture          Power   1费  RupturePower 1(2)
```

**Rare (22)**：
```
Aggression       Power   1费  AggressionPower(自动出牌)
Barricade        Power   2(3)费  BarricadePower
Brand            Skill   0费  自伤1 + 1(2)力量 + 消耗1手牌
Cascade          Skill   X费  自动打出X张牌堆顶
Conflagration    Attack  1费  2伤害×4(5)全体
Corruption       Power   3(2)费  CorruptionPower
CrimsonMantle    Power   1费  CrimsonMantlePower 8(10)
Cruelty          Power   1费  CrueltyPower 25(50)
DarkEmbrace      Power   2(1)费  DarkEmbracePower 1
DemonForm        Power   3费  DemonFormPower 2(3)
Exhume           Skill   1(0)费  消耗堆→手牌  Exhaust
Feed             Attack  1费  10(12)伤 + 击杀+3MaxHP  Exhaust
FiendFire        Attack  2费  7(10)×手牌数 + 烧全手
FireBreathing    Attack  1费  4(6)全体(每弃1张+1伤)
Hellraiser       Power   2(1)费  HellraiserPower(未知)
Impervious       Skill   2费  30(40)格挡  Exhaust
Juggernaut       Power   2费  JuggernautPower 5(7)
LimitBreak       Power   1费  力量×2
Mangle           Attack  3费  15(20)伤 + 10(15)ManglePower(减力)
NotYet           Skill   2费  治疗10(13)  Exhaust
Offering         Skill   0费  自伤6 + 2能量 + 抽3(5)  Exhaust
OneTwoPunch      Skill   1费  OneTwoPunchPower(额外攻击)
PactsEnd         Attack  0费  17(23)全体(需消耗堆≥3张)
PrimalForce      Skill   0费  手牌攻击牌变巨岩
Pyre             Power   2费  PyrePower 1(2)
Stoke            Skill   1费  消耗全手 + 随机抽等量(升级版升级)
TearAsunder      Attack  2费  5(7)×(1+受未格挡伤害次数)
Thrash           Attack  1费  4(6)×2 + 消耗1攻击牌永久加其伤害  Exhaust
Unmovable        Power   2(1)费  UnmovablePower(未知)
Unrelenting      Attack  2费  14(20)伤 + FreeAttackPower
```

**Ancient (2)**：
```
Break            Attack  1费  20(30)伤 + 5(7)易伤
Corruption       Power   3(2)费  CorruptionPower
```

### Status 卡（12 张）

```
Dazed        Status  0费  不可打出, 抽到时自动消耗  Unplayable+Exhaust
Slimed       Status  1费  消耗  Exhaust
Wound        Status  0费  不可打出  Unplayable
Burn         Status  0费  自伤2  Unplayable+Exhaust
Void         Status  0费  不可打出, 抽到时消耗  Unplayable+Exhaust
Infection    Status  0费  不可打出  Unplayable
Beckon       Status  1费  回合结束扣6血  Exhaust
Debris       Status  1费  无效果  Exhaust
Toxic        Status  1费  自伤5  Exhaust
Soot         Status -1费  不可打出  Unplayable
Wither       Status -1费  不可打出  Unplayable
FranticEscape Status 1费  Exhaust
```

### Curse 卡（18 张）

```
Injury, Clumsy, Doubt, Shame, Decay, Regret — 全部 Unplayable
AscendersBane  Curse -1费  Unplayable+Ethereal
CurseOfTheBell Curse -1费  Unplayable
Debt           Curse -1费  Unplayable
Enthralled     Curse  2费  Unplayable
Folly          Curse -1费  Unplayable
Greed          Curse -1费  Unplayable
Guilty         Curse -1费  Unplayable
Normality      Curse -1费  Unplayable
PoorSleep      Curse -1费  Unplayable
SporeMind      Curse  1费  Exhaust
Writhe         Curse -1费  Unplayable
```

---

## 八、敌人系统 EnemyDb

### 接口

```csharp
interface IEnemyAi {
    string EnemyId;
    EnemyTurnPlan PlanNextTurn(state, enemy);
    void OnCombatStart(enemy) { }  // 初始 buff
}

class EnemyTurnPlan {
    IntentType Intent;      // Attack/Buff/Debuff/AttackDebuff/Block/Status
    int Damage, Block, Hits, BuffAmount, DebuffAmount;
    Type? DebuffPowerType;
    ValueProp DamageProps;
}
```

### 已实现的 14 个 Act1 敌人

| 敌人 | 行为 | 初始 Buff |
|------|------|-----------|
| Chomper | Attack 8×2 ↔ Status(3×Dazed) | Artifact 2 |
| LeafSlimeM | Status(2×Slimed) ↔ Attack 8 | — |
| LeafSlimeS | Random(Attack 3 / Status Slimed) | — |
| TwigSlimeM | Pounce 11[cd2] / Status Slimed | — |
| TwigSlimeS | Attack 4 (自循环) | — |
| Flyconid | Random(Vuln / Frail+8 / Smash 11) | — |
| SnappingJaxfruit | Attack 3 + 2STR (自循环) | — |
| Nibbit | 3 循环: Attack 12 → Attack 6+5block → 2STR | — |
| LouseProgenitor | 3 循环: Web 9+Frail → Curl 14block+5STR → Pounce 14 | Buffer 14/18 |
| FuzzyWurmCrawler | Attack 4 ↔ Inhale 7STR | — |
| CorpseSlug | 3 循环: 3×2 → Glomp 8 → Frail 2 | Strength 4(5), 群组偏移 |
| GremlinMerc | 3 循环: 7×2 → 6×2+Weak → 8+2STR | — |
| SneakyGremlin | Attack 4 ↔ Attack 7 | — |
| FatGremlin | Stun → Escape | — |

---

## 九、MCTS 战斗 AI

### 算法

```
每回合每张牌：MCTS 搜索 → 返回最佳卡牌 ID
循环：找到最佳 ID → 扣除模拟能量 → 移除模拟手牌 → 继续搜索
直到 MCTS 返回 null（结束回合）
```

### MctsEngine 参数
- `iterations`: 300-800（越多越准确，越慢）
- `explorationC`: 1.414（√2，UCB1 标准参数）
- `determinize`: true（ISMCTS，每次迭代打乱抽牌堆）

### 奖励函数
```
胜利: 10 + HP/10
阵亡: -100
未终局: (玩家HP - 敌人HP) / 50
```

### 选择策略（FindBestAction）
```
root.Children
  .Where(c => c.Visits > 5)
  .OrderByDescending(c => c.WinRate)
  .ThenByDescending(c => c.Visits)
  .FirstOrDefault() ?? root.MostVisitedChild
```

---

## 十、Mod 集成

### 文件
- `sts2_ai_mod.dll` — Harmony Patch 主程序（.NET 9.0）
- `sts2_ai.dll` — AI 引擎库（.NET 8.0，被 mod 引用）
- `mod.json` — Mod 清单

### 核心 Patch
- `[ModuleInitializer]` → `AppDomain.CurrentDomain.AssemblyResolve`（加载 sts2_ai.dll）
- `[HarmonyPrefix]` on `CardCmd.AutoPlay` → 替换 AutoSlay 随机出牌为 MCTS
- `[HarmonyPostfix]` on `CardModel.TryManualPlay` → 手动出牌时刷新控制台
- `[HarmonyPostfix]` on `CardCmd.AutoPlay` → 自动出牌时刷新控制台

### StateConverter
```
游戏 CombatState → SimState:
  Players → SimCreature (HP/Block/Powers)
  PlayerCombatState → Energy/MaxEnergy + 手牌/抽牌堆/弃牌堆/消耗堆
  Enemies → SimCreature (HP/Block/Powers + Monster.Id.Entry 作为敌人名)
  RoundNumber → TurnNumber
```

### CardMapper
```
游戏 ID → SimCard ID:
  "STRIKE_IRONCLAD" → "Strike_Ironclad"
  "DEFEND_IRONCLAD" → "Defend_Ironclad"
  不匹配时返回 null → 卡牌不加入 SimState → MCTS 看不到它
```

### ConsoleDisplay
- `AllocConsole()` → 弹出 CMD 窗口
- 每秒定时器刷新 + 手动/自动出牌时刷新
- 显示：玩家HP/格挡/能量/Powers + 手牌 + 抽牌堆/弃牌堆 + 敌人HP/意图/Powers + AI 建议

---

## 十一、已知问题与教训

### 文件损坏
CardDb.cs 在反复大段编辑后严重损坏。**教训**：避免大段替换编辑。每次编辑应尽量小粒度、精准匹配。

### X 费卡不支持
EnergyCost = -1 的卡（Whirlwind、Cascade）在 PlayCard 中被特殊处理为 `modifiedCost = State.Energy`，但 MCTS 的 Expand 会跳过所有 `EnergyCost < 0` 的卡。

### 测试分离
测试代码必须放在独立项目 `sts2_ai_tests` 中，否则测试依赖的 xUnit/FluentAssertions 会被打包到 sts2_ai.dll，导致 ModManager 加载时报 `FileNotFoundException`。

### .NET 版本
- `sts2_ai` 用 `net8.0`（开发机有 8.0 runtime）
- `sts2_ai_mod` 用 `net9.0`（游戏用 9.0，ModManager 加载 mod DLL）
- 跨版本加载：.NET 9.0 可以加载 8.0 的 DLL

### Mod 加载流程
1. 游戏 `ModManager` 扫描 `mods/<mod_id>/` 目录
2. 加载 `<mod_id>.dll`（所以文件名必须和 json 中的 `id` 一致）
3. 执行 `[ModuleInitializer]`（加载 `sts2_ai.dll` 依赖）
4. 查找 `[ModInitializer("Init")]` 并调用

---

## 十二、技术债务汇总

| 类别 | 项 | 说明 |
|------|-----|------|
| X 费卡 | Whirlwind, Cascade | 不支持，MCTS 跳过 |
| Power 缺失 | HellraiserPower, UnmovablePower, JugglingPower, StampedePower | 游戏中使用了但源码未找到定义 |
| 丢失卡定义 | FiendFire, FireBreathing, Offering, Impervious, Brand, Aggression, Juggernaut, Corruption, Exhume, LimitBreak, Feed | 文件损坏导致方法体丢失 |
| 未注册卡 | DrumOfBattle, EvilEye, ExpectAFight, ForgottenRitual, InfernalBlade, Juggling, Stampede, Vicious | 8 张 Uncommon 未注册 |
| 简化实现 | Spite, Dominate, Bully, Stomp, TearAsunder, Thrash, HowlFromBeyond, Pillage | 见 4.1 节 |
| 空实现 | Cascade, PrimalForce, Stoke, Hellraiser, Unmovable | 注册但无效果 |
| 状态卡自动消耗 | Dazed, Void, Burn | 需要在抽到时自动触发，目前仅依赖 Exhaust 关键词（必须打出才消耗） |
| 缺失角色 | Silent(88), Defect(87), Necrobinder(88), Regent(88) | 仅实现了 Ironclad |
| 精英/Boss | Decimillipede, Queen, TheInsatiable 等 | 未实现 |
| 遗物/药水 | — | 未实现 |
