# Ironclad 87 张卡牌精确规范

> 来源：`IroncladCardPool.cs`（87 张）+ 每张卡源码 `src/Core/Models/Cards/<Name>.cs`
> **注意**：Cleave, HeavyBlade, Warcry, Intimidate, Fisticuffs 不在 STS2 Ironclad 卡池中

---

## 一、基础结构定义

### 1.1 模拟器卡牌字段

```csharp
class SimCard {
    Id (string),          // 如 "STRIKE_IRONCLAD", "DEFEND_IRONCLAD"
    Name (string),        // 如 "Strike", "Defend"
    Cost (int),           // 基础费用，-1 = X费
    UpgradedCost (int),   // 升级后费用，-1 = 不变
    Type (CardType),      // Attack/Skill/Power/Status/Curse/Quest
    Rarity (CardRarity),  // Basic/Common/Uncommon/Rare/Ancient
    DefaultTargetType,    // Self/SingleEnemy/AllEnemies/RandomEnemy
    HasXCost (bool),
    Keywords (HashSet<string>),     // "Exhaust", "Ethereal", "Innate", "Unplayable", "Retain"
    UpgradedKeywords (HashSet<string>),
    Tags (HashSet<string>),         // "Strike", "Defend", "Shiv"
    Effects (List<IEffect>),
    UpgradedEffects (List<IEffect>),
}
```

### 1.2 升级规则

- `MaxUpgradeLevel: 1` = 可升级（大多数卡）
- `MaxUpgradeLevel: 0` = 不可升级（代码中无 OnUpgrade 或明确返回 0）
- 升级变化类型：
  - `base.DynamicVars.X.UpgradeValueBy(N)` → `UpgradedEffects` 中数值 +N
  - `base.EnergyCost.UpgradeBy(-1)` → `UpgradedCost = Cost - 1`
  - `AddKeyword(CardKeyword.Innate)` → `UpgradedKeywords` 增加 "Innate"
  - `RemoveKeyword(CardKeyword.Exhaust)` → `UpgradedKeywords` 移除 "Exhaust"
  - `RemoveKeyword(CardKeyword.Ethereal)` → `UpgradedKeywords` 移除 "Ethereal"
  - 代码中 `if (base.IsUpgraded)` 分支 → 在 OnPlay 中判断的特殊逻辑（如 Armaments）

---

## 二、全部 87 张卡规范

### Basic (3)

| # | Id | Name | Cost | UpgCost | Type | Rarity | Target | Keywords | Tags | Effects(Base) | Effects(Upgraded) |
|---|-----|------|------|---------|------|--------|--------|----------|------|---------------|-------------------|
| 1 | STRIKE_IRONCLAD | Strike | 1 | - | Attack | Basic | SingleEnemy | — | Strike | DealDamage(6, Move) | DealDamage(9, Move) |
| 2 | DEFEND_IRONCLAD | Defend | 1 | - | Skill | Basic | Self | — | Defend | GainBlock(5, Move) | GainBlock(8, Move) |
| 3 | BASH | Bash | 2 | - | Attack | Basic | SingleEnemy | — | — | DealDamage(8, Move) + ApplyPower(Vulnerable,2) | DealDamage(10, Move) + ApplyPower(Vulnerable,3) |

### Common (20)

| # | Id | Cost | Up | Type | Target | Keywords | Tags | Effects Base | Effects Upgraded | 特殊说明 |
|---|-----|------|----|------|--------|----------|------|-------------|-----------------|---------|
| 4 | ANGER | 0 | - | Attack | SingleEnemy | 无 | — | DealDamage(6, Move) + CreateCardInDiscard | DealDamage(8, Move) + CreateCardInDiscard | 打出后复制一张到弃牌堆 |
| 5 | ARMAMENTS | 1 | - | Skill | Self | — | — | GainBlock(5, Move) + 升级1张手牌 | GainBlock(5, Move) + 升级全部手牌 | 升级版升级全部可升级手牌 |
| 6 | BLOOD_WALL | 2 | - | Skill | Self | — | — | SelfDamage(2) + GainBlock(16, Move) | SelfDamage(2) + GainBlock(20, Move) | |
| 7 | BLOODLETTING | 0 | - | Skill | Self | — | — | SelfDamage(3) + GainEnergy(2) | SelfDamage(3) + GainEnergy(3) | |
| 8 | BODY_SLAM | 1 | 0 | Attack | SingleEnemy | — | — | DealDamage(Block, Move) | DealDamage(Block, Move) | 伤害=当前格挡值; 升级费用变0 |
| 9 | BREAKTHROUGH | 1 | - | Attack | AllEnemies | — | — | SelfDamage(1) + DealDamage(9, Move) | SelfDamage(1) + DealDamage(13, Move) | AOE自伤 |
| 10 | CINDER | 2 | - | Attack | SingleEnemy | Exhaust | — | DealDamage(18, Move) + ExhaustRandomHand | DealDamage(24, Move) + ExhaustRandomHand | 随机消耗1张手牌 |
| 11 | HAVOC | 1 | 0 | Skill | Self | — | — | AutoPlayFromDrawPile(exhaust:true) | AutoPlayFromDrawPile(exhaust:true) | 升级费用变0 |
| 12 | HEADBUTT | 1 | - | Attack | SingleEnemy | — | — | DealDamage(9, Move) + DiscardToDrawTop | DealDamage(12, Move) + DiscardToDrawTop | 弃牌堆选1张放抽牌堆顶 |
| 13 | IRON_WAVE | 1 | - | Attack | SingleEnemy | — | — | GainBlock(5, Move) + DealDamage(5, Move) | GainBlock(7, Move) + DealDamage(7, Move) | |
| 14 | MOLTEN_FIST | 1 | - | Attack | SingleEnemy | Exhaust | — | DealDamage(10, Move) + 复制目标易伤 | DealDamage(14, Move) + 复制目标易伤 | 如果目标有易伤, 复制等层数易伤 |
| 15 | PERFECTED_STRIKE | 2 | - | Attack | SingleEnemy | — | Strike | DealDamage(6+2×Strike总数, Move) | DealDamage(6+3×Strike总数, Move) | 每张Strike标签牌+2(3)伤害 |
| 16 | POMMEL_STRIKE | 1 | - | Attack | SingleEnemy | — | Strike | DealDamage(9, Move) + DrawCards(1) | DealDamage(10, Move) + DrawCards(2) | |
| 17 | SETUP_STRIKE | 1 | - | Attack | SingleEnemy | — | Strike | DealDamage(7, Move) + ApplyPower(SetupStrikePower,2) | DealDamage(9, Move) + ApplyPower(SetupStrikePower,3) | 获得临时力量 |
| 18 | SHRUG_IT_OFF | 1 | - | Skill | Self | — | — | GainBlock(8, Move) + DrawCards(1) | GainBlock(11, Move) + DrawCards(1) | |
| 19 | SWORD_BOOMERANG | 1 | - | Attack | RandomEnemy | — | — | DealDamage(3, Move) × 3 Hits | DealDamage(3, Move) × 4 Hits | 随机目标 |
| 20 | THUNDERCLAP | 1 | - | Attack | AllEnemies | — | — | DealDamage(4, Move) + ApplyPower(Vulnerable,1全体) | DealDamage(7, Move) + ApplyPower(Vulnerable,1全体) | AOE+全体易伤 |
| 21 | TREMBLE | 1 | - | Skill | SingleEnemy | Exhaust | — | ApplyPower(Vulnerable,3) | ApplyPower(Vulnerable,4) | |
| 22 | TRUE_GRIT | 1 | - | Skill | Self | — | — | GainBlock(7, Move) + ExhaustRandomHand | GainBlock(9, Move) + 选择消耗1张手牌 | 升级版可選消耗 |
| 23 | TWIN_STRIKE | 1 | - | Attack | SingleEnemy | — | Strike | DealDamage(5, Move) × 2 Hits | DealDamage(7, Move) × 2 Hits | |

### Uncommon (26)

| # | Id | Cost | Up | Type | Target | Keywords | Tags | Effects Base | Effects Upgraded | 特殊说明 |
|---|-----|------|----|------|--------|----------|------|-------------|-----------------|---------|
| 24 | ASHEN_STRIKE | 1 | - | Attack | SingleEnemy | — | Strike | DealDamage(6+3×消耗堆大小, Move) | DealDamage(6+4×消耗堆大小, Move) | |
| 25 | BATTLE_TRANCE | 0 | - | Skill | Self | — | — | DrawCards(3) + ApplyPower(NoDraw,1) | DrawCards(4) + ApplyPower(NoDraw,1) | |
| 26 | BLUDGEON | 3 | - | Attack | SingleEnemy | — | — | DealDamage(32, Move) | DealDamage(42, Move) | |
| 27 | BULLY | 0 | - | Attack | SingleEnemy | — | — | DealDamage(4+2×目标易伤层数, Move) | DealDamage(4+3×目标易伤层数, Move) | |
| 28 | BURNING_PACT | 1 | - | Skill | Self | — | — | ExhaustFromHand(1) + DrawCards(2) | ExhaustFromHand(1) + DrawCards(3) | 選1张消耗 |
| 29 | COLOSSUS | 1 | - | Skill | Self | — | — | GainBlock(5, Move) + ApplyPower(ColossusPower,1) | GainBlock(8, Move) + ApplyPower(ColossusPower,1) | |
| 30 | DEMONIC_SHIELD | 0 | - | Skill | AnyAlly | Exhaust(升级移除) | — | SelfDamage(1) + GainBlock(等于施放者格挡, Move) | SelfDamage(1) + GainBlock(等于施放者格挡, Move) | 多人模式 |
| 31 | DISMANTLE | 1 | - | Attack | SingleEnemy | — | — | DealDamage(8, Move) × (1或2)Hits | DealDamage(10, Move) × (1或2)Hits | 目标有易伤时打2下 |
| 32 | DOMINATE | 1 | - | Skill | SingleEnemy | Exhaust | — | ApplyPower(Vulnerable,1) + ApplyPower(Strength,=目标易伤层数) | ApplyPower(Vulnerable,2) + ApplyPower(Strength,=目标易伤层数) | |
| 33 | DRUM_OF_BATTLE | 1 | - | Skill | Self | Exhaust | — | DrawCards(2) | DrawCards(2) + 消耗时获得2(3)能量 | 消耗时额外效果 |
| 34 | EVIL_EYE | 1 | - | Skill | Self | — | — | GainBlock(8, Move) 或 ×2(本回合有消耗时) | GainBlock(11, Move) 或 ×2(本回合有消耗时) | |
| 35 | EXPECT_A_FIGHT | 2 | 1 | Skill | Self | — | — | GainEnergy(=手牌攻击牌数) + ApplyPower(NoEnergyGain,1) | GainEnergy(=手牌攻击牌数) + ApplyPower(NoEnergyGain,1) | 升级费用变1 |
| 36 | FEEL_NO_PAIN | 1 | - | Power | Self | — | — | ApplyPower(FeelNoPainPower,3) | ApplyPower(FeelNoPainPower,4) | |
| 37 | FLAME_BARRIER | 2 | - | Skill | Self | — | — | GainBlock(12, Move) + ApplyPower(FlameBarrierPower,4) | GainBlock(16, Move) + ApplyPower(FlameBarrierPower,6) | |
| 38 | FORGOTTEN_RITUAL | 1 | - | Skill | Self | Exhaust | — | 本回有消耗则 GainEnergy(3) | 本回有消耗则 GainEnergy(4) | |
| 39 | HEMOKINESIS | 1 | - | Attack | SingleEnemy | — | — | SelfDamage(2) + DealDamage(15, Move) | SelfDamage(2) + DealDamage(20, Move) | |
| 40 | HOWL_FROM_BEYOND | 3 | - | Attack | AllEnemies | Exhaust | — | DealDamage(16, Move) AOE + 消耗后自动复打 | DealDamage(21, Move) AOE + 消耗后自动复打 | |
| 41 | INFERNAL_BLADE | 1 | 0 | Skill | Self | Exhaust | — | CreateRandomAttackInHand(0费) | CreateRandomAttackInHand(0费) | |
| 42 | INFERNO | 1 | - | Power | Self | — | — | ApplyPower(InfernoPower,6) | ApplyPower(InfernoPower,9) | |
| 43 | INFLAME | 1 | - | Power | Self | — | — | ApplyPower(Strength,2) | ApplyPower(Strength,3) | |
| 44 | JUGGLING | 1 | - | Power | Self | — | — | ApplyPower(JugglingPower,1) | ApplyPower(JugglingPower,1) + Innate | 升级带固有 |
| 45 | PILLAGE | 1 | - | Attack | SingleEnemy | — | — | DealDamage(6, Move) + 抽到非攻击牌为止 | DealDamage(9, Move) + 抽到非攻击牌为止 | |
| 46 | RAGE | 0 | - | Skill | Self | — | — | ApplyPower(RagePower,3) | ApplyPower(RagePower,5) | |
| 47 | RAMPAGE | 1 | - | Attack | SingleEnemy | — | — | DealDamage(9, Move), 永久+5 | DealDamage(9, Move), 永久+9(升级增额) | 每打一次永久+5(+9) |
| 48 | RUPTURE | 1 | - | Power | Self | — | — | ApplyPower(RupturePower,1) | ApplyPower(RupturePower,2) | |
| 49 | SECOND_WIND | 1 | - | Skill | Self | — | — | GainBlock(5, Move)×非攻击手牌数, 消耗他们 | GainBlock(7, Move)×非攻击手牌数, 消耗他们 | 消耗全部非攻击手牌 |
| 50 | SPITE | 0 | - | Attack | SingleEnemy | — | — | DealDamage(5, Move) × (1或2)Hits | DealDamage(5, Move) × (1或3)Hits | 本回合受伤则多段 |
| 51 | STAMPEDE | 2 | 1 | Power | Self | — | — | ApplyPower(StampedePower,1) | ApplyPower(StampedePower,1) | |
| 52 | STOMP | 3 | - | Attack | AllEnemies | — | — | DealDamage(12, Move) AOE, 每打攻击-1费 | DealDamage(15, Move) AOE, 每打攻击-1费 | 动态减费 |
| 53 | STONE_ARMOR | 1 | - | Power | Self | — | — | ApplyPower(PlatingPower,4) | ApplyPower(PlatingPower,6) | |
| 54 | TAUNT | 1 | - | Skill | SingleEnemy | — | — | GainBlock(7, Move) + ApplyPower(Vulnerable,1) | GainBlock(8, Move) + ApplyPower(Vulnerable,2) | |
| 55 | UNRELENTING | 2 | - | Attack | SingleEnemy | — | — | DealDamage(14, Move) + ApplyPower(FreeAttackPower,1) | DealDamage(20, Move) + ApplyPower(FreeAttackPower,1) | 下张攻击免费 |
| 56 | UPPERCUT | 2 | - | Attack | SingleEnemy | — | — | DealDamage(13, Move) + ApplyPower(Weak,1) + ApplyPower(Vulnerable,1) | DealDamage(13, Move) + ApplyPower(Weak,2) + ApplyPower(Vulnerable,2) | |
| 57 | VICIOUS | 1 | - | Power | Self | — | — | ApplyPower(ViciousPower,1) | ApplyPower(ViciousPower,2) | |
| 58 | WHIRLWIND | 0(X) | - | Attack | AllEnemies | — | — | DealDamage(5, Move) × X Hits AOE | DealDamage(8, Move) × X Hits AOE | X费 |

### Rare (21)

| # | Id | Cost | Up | Type | Target | Keywords | Tags | Effects Base | Effects Upgraded | 特殊说明 |
|---|-----|------|----|------|--------|----------|------|-------------|-----------------|---------|
| 59 | AGGRESSION | 1 | - | Power | Self | — | — | ApplyPower(AggressionPower,1) | ApplyPower(AggressionPower,1) + Innate | |
| 60 | BARRICADE | 3 | 2 | Power | Self | — | — | ApplyPower(BarricadePower,1) | ApplyPower(BarricadePower,1) | |
| 61 | BRAND | 0 | - | Skill | Self | — | — | SelfDamage(1) + ExhaustFromHand(1) + ApplyPower(Strength,1) | SelfDamage(1) + ExhaustFromHand(1) + ApplyPower(Strength,2) | 消耗手牌+力量 |
| 62 | CASCADE | -1(X) | - | Skill | Self | — | — | AutoPlayFromDrawPile(X张) | AutoPlayFromDrawPile(X+1张) | X费 |
| 63 | CONFLAGRATION | 1 | - | Attack | AllEnemies | — | — | DealDamage(2, Move) × 4 Hits AOE | DealDamage(2, Move) × 5 Hits AOE | 多段AOE |
| 64 | CRIMSON_MANTLE | 1 | - | Power | Self | — | — | ApplyPower(CrimsonMantlePower,8) | ApplyPower(CrimsonMantlePower,10) | |
| 65 | CRUELTY | 1 | - | Power | Self | — | — | ApplyPower(CrueltyPower,25) | ApplyPower(CrueltyPower,50) | |
| 66 | DARK_EMBRACE | 2 | 1 | Power | Self | — | — | ApplyPower(DarkEmbracePower,1) | ApplyPower(DarkEmbracePower,1) | |
| 67 | DEMON_FORM | 3 | - | Power | Self | — | — | ApplyPower(DemonFormPower,2) | ApplyPower(DemonFormPower,3) | |
| 68 | FEED | 1 | - | Attack | SingleEnemy | Exhaust | — | DealDamage(10, Move) + 击杀+3MaxHP | DealDamage(12, Move) + 击杀+4MaxHP | 战斗中不可生成 |
| 69 | FIEND_FIRE | 2 | - | Attack | SingleEnemy | Exhaust | — | ExhaustAllHand + DealDamage(7×手牌数, Move) | ExhaustAllHand + DealDamage(10×手牌数, Move) | 烧全手 |
| 70 | HELLRAISER | 2 | 1 | Power | Self | — | — | ApplyPower(HellraiserPower,1) | ApplyPower(HellraiserPower,1) | |
| 71 | IMPERVIOUS | 2 | - | Skill | Self | Exhaust | — | GainBlock(30, Move) | GainBlock(40, Move) | |
| 72 | JUGGERNAUT | 2 | - | Power | Self | — | — | ApplyPower(JuggernautPower,5) | ApplyPower(JuggernautPower,7) | |
| 73 | NOT_YET | 2 | - | Skill | Self | Exhaust | — | Heal(10) | Heal(13) | 战斗中不可生成 |
| 74 | OFFERING | 0 | - | Skill | Self | Exhaust | — | SelfDamage(6) + GainEnergy(2) + DrawCards(3) | SelfDamage(6) + GainEnergy(2) + DrawCards(5) | |
| 75 | ONE_TWO_PUNCH | 1 | - | Skill | Self | — | — | ApplyPower(OneTwoPunchPower,1) | ApplyPower(OneTwoPunchPower,2) | |
| 76 | PACTS_END | 0 | - | Attack | AllEnemies | — | — | 消耗堆≥3时 DealDamage(17, Move) AOE | 消耗堆≥3时 DealDamage(23, Move) AOE | 条件触发 |
| 77 | PRIMAL_FORCE | 0 | - | Skill | Self | — | — | 手牌攻击牌→GiantRock | 手牌攻击牌→GiantRock(升级) | 变换 |
| 78 | PYRE | 2 | - | Power | Self | — | — | ApplyPower(PyrePower,1) | ApplyPower(PyrePower,2) | |
| 79 | STOKE | 1 | - | Skill | Self | — | — | 消耗全手+随机抽等量 | 消耗全手+随机抽等量(升级) | |
| 80 | TANK | 1 | 0 | Power | Self | — | — | ApplyPower(TankPower,1) | ApplyPower(TankPower,1) | 多人模式 |
| 81 | TEAR_ASUNDER | 2 | - | Attack | SingleEnemy | — | — | DealDamage(5, Move) × (1+未格挡次数) Hits | DealDamage(7, Move) × (1+未格挡次数) Hits | |
| 82 | THRASH | 1 | - | Attack | SingleEnemy | Exhaust | — | DealDamage(4, Move) × 2 Hits + 消耗1攻击牌永久加其伤害 | DealDamage(6, Move) × 2 Hits + 消耗1攻击牌永久加其伤害 | |
| 83 | UNMOVABLE | 2 | 1 | Power | Self | — | — | ApplyPower(UnmovablePower,1) | ApplyPower(UnmovablePower,1) | |

### Ancient (2)

| # | Id | Cost | Up | Type | Target | Keywords | Tags | Effects Base | Effects Upgraded |
|---|-----|------|----|------|--------|----------|------|-------------|-----------------|
| 84 | BREAK | 1 | - | Attack | SingleEnemy | — | — | DealDamage(20, Move) + ApplyPower(Vulnerable,5) | DealDamage(30, Move) + ApplyPower(Vulnerable,7) |
| 85 | CORRUPTION | 3 | 2 | Power | Self | — | — | ApplyPower(CorruptionPower,1) | ApplyPower(CorruptionPower,1) |

### 特殊卡片（需单独处理）

| # | Id | 特殊逻辑说明 |
|---|-----|-------------|
| 4 | ANGER | 打出后 CreateCardInDiscard: 克隆自己到弃牌堆 |
| 5 | ARMAMENTS | 未升级: 选择1张手牌升级; 升级: 升级全部手牌 |
| 8 | BODY_SLAM | 伤害 = 施放者当前格挡值 (CalculatedDamage) |
| 10 | CINDER | 造成伤害后随机消耗1张手牌 |
| 15 | PERFECTED_STRIKE | 伤害 = 6 + 2(或3) × 全卡组中 Strike 标签卡数量 |
| 20 | THUNDERCLAP | Vulnerable 施加给全体敌人 |
| 22 | TRUE_GRIT | 未升级: 随机消耗1张手牌; 升级: 选择消耗 |
| 24 | ASHEN_STRIKE | 伤害 = 6 + 3(或4) × 消耗堆中卡牌数量 |
| 25 | BATTLE_TRANCE | 抽牌后施加 NoDrawPower |
| 27 | BULLY | 伤害 = 4 + 2(或3) × 目标易伤层数 |
| 31 | DISMANTLE | 目标有易伤时打2下，否则1下 |
| 32 | DOMINATE | 获得力量 = 目标当前易伤层数 |
| 35 | EXPECT_A_FIGHT | 获得能量 = 手牌中攻击牌数量 |
| 38 | FORGOTTEN_RITUAL | 本回合有消耗过才获得能量 |
| 47 | RAMPAGE | 每次打出永久+5(+9)伤害(跨战斗保存) |
| 49 | SECOND_WIND | 消耗全部非攻击牌, 每张得5(7)格挡 |
| 50 | SPITE | 本回合受伤过则多段 |
| 52 | STOMP | 费用随本回合攻击牌打出次数减少 |
| 62 | CASCADE | X费: 自动打出X张牌堆顶 |
| 63 | CONFLAGRATION | 多段 (4→5) × 固定伤害AOE |
| 69 | FIEND_FIRE | 消耗全部手牌, 伤害 × 手牌数 |
| 75 | ONE_TWO_PUNCH | 应用 OneTwoPunchPower, 不是攻击牌 |
| 76 | PACTS_END | 消耗堆 ≥ 3 才造成伤害 |
| 77 | PRIMAL_FORCE | 手牌中攻击牌变成 GiantRock |
| 79 | STOKE | 消耗全部手牌, 随机创造等量新牌 |
| 81 | TEAR_ASUNDER | 多段数 = 1 + 本回合未格挡伤害次数 |
| 82 | THRASH | 消耗1张攻击牌, 永久增加其伤害 |

---

## 三、实现注意事项

### 3.1 模拟器 Effect 映射

| 游戏命令 | 模拟器 Effect | 参数说明 |
|---------|-------------|---------|
| `DamageCmd.Attack(X)` | `DealDamage { BaseAmount=X, Props=ValueProp.Move }` | |
| `WithHitCount(N)` | `HitCount = N` | 多段 |
| `TargetingAllOpponents` | `TargetType = TargetType.AllEnemies` | AOE |
| `TargetingRandomOpponents` | `TargetType = TargetType.RandomEnemy` | 随机 |
| `CreatureCmd.GainBlock(X)` | `GainBlock { BaseAmount=X }` | |
| `CreatureCmd.Damage(self, X, Unblockable\|Unpowered)` | `SelfDamage { Amount=X }` | 自伤 |
| `PlayerCmd.GainEnergy(X)` | `GainEnergy { Amount=X }` | |
| `CardPileCmd.Draw(X)` | `DrawCards { Amount=X }` | |
| `PowerCmd.Apply<T>(amount)` | `ApplyPower { PowerFactory=()=>new T(), Amount=amount }` | |
| `CreatureCmd.Heal(X)` | `Heal { Amount=X }` | |
| `CreatureCmd.GainMaxHp(X)` | `GainMaxHp { Amount=X }` | |

### 3.2 条件效果

部分卡片需要条件判断（如 Bully、Dismantle、Spite、PactsEnd 等），这些在模拟器中通过以下方式实现：
- **条件分支**: 在 CardDb 的卡片工厂中根据参数创建不同的 Effect 列表
- **计算伤害**: 使用 `DealDamage` 的 `CalculatedDamageFn` 回调参数（如果添加支持）
- **简化处理**: 对于 MCTS 模拟，可以先用简化版（如 Bully 总是取平均易伤层数）

### 3.3 X 费卡

Whirlwind 和 Cascade 是 X 费卡 (`HasXCost = true`, `Cost = 0`)。
- PlayCard 时: `if (card.HasXCost) modifiedCost = State.Energy`
- 伤害: Whirlwind 的 HitCount = X, Cascade 的 AutoPlay 数量 = X

### 3.4 特殊 Power 依赖

| 卡片 | 依赖 Power |
|------|-----------|
| BattleTrance | NoDrawPower |
| Colossus | ColossusPower |
| SetupStrike | SetupStrikePower |
| Rage | RagePower |
| Inferno | InfernoPower |
| CrimsonMantle | CrimsonMantlePower |
| Cruelty | CrueltyPower |
| Unrelenting | FreeAttackPower |
| Juggling | JugglingPower |
| Stampede | StampedePower |
| Aggression | AggressionPower |
| OneTwoPunch | OneTwoPunchPower |
| Hellraiser | HellraiserPower |
| Unmovable | UnmovablePower |
| Tank | TankPower |
