# STS2 AI — 全自动打牌 AI 计划（修订版 v2）

> 基于游戏源码 `Slay The Spire 2/src/Core/` 完整复刻战斗系统。
> 本次对引擎架构进行全面修正，确保 Hook 系统与游戏一致。
> **核心原则**：每个实体独立文件；Hook 返回值遵守游戏约定（加值/乘数/上限）。

---

## 一、Phase 1 总目标

重建 `sts2_ai` 战斗模拟器，完整复刻以下内容：

| 模块 | 内容 | 数量 |
|------|------|------|
| 引擎核心 | SimState / SimEngine / Effects / Powers | ~55 文件 |
| Ironclad 卡牌 | 全部 87 张（含升级） | 87 文件 |
| 无色卡 (Colorless) | 全部 64 张（含升级） | 64 文件 |
| 诅咒卡 (Curse) | 全部 18 张（大多不可升级） | 18 文件 |
| 状态卡 (Status) | 全部 12 张（不可升级） | 12 文件 |
| 事件卡 (Event) | 全部 27 张（部分可升级） | 27 文件 |
| Token 卡 | 全部 14 张（部分可升级） | 14 文件 |
| 任务卡 (Quest) | 全部 3 张（不可升级） | 3 文件 |
| 怪物行为 | 全部 ~121 个敌人的 AI 状态机 | ~121 文件 |
| **文件总数** | | **~500 文件** |

---

## 二、架构核心（从游戏源码提取）

### 2.1 ValueProp 标志位

```csharp
[Flags] enum ValueProp {
    None          = 0,
    Unblockable   = 2,   // 伤害无视格挡
    Unpowered     = 4,   // 不受 Power 影响（力量/易伤/虚弱等）
    Move          = 8,   // 标记为"动作"（卡牌攻击/怪物移动）
    SkipHurtAnim  = 16   // 跳过受伤动画
}
```

### 2.2 "Powered" 判断扩展

```csharp
static bool IsPoweredAttack(ValueProp props)
    => props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);

static bool IsPoweredCardOrMonsterMoveBlock(ValueProp props)
    => props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
```

**规则**：只有同时带 `Move` 且不带 `Unpowered` 的效果才会受 Power 影响。

---

### 2.3 伤害计算流水线（完整）

这是引擎最核心的部分，直接对应游戏 `Hook.ModifyDamage()` + `CreatureCmd.Damage()`。

```
输入: baseDamage, source, target, strengthMultiplier, props

// === Phase 1: 伤害修改 ===
if (props.IsPoweredAttack()) {
    // 1a. 累加所有 ModifyDamageAdditive 返回值
    additiveBonus = 0
    for each power on source and target:
        additiveBonus += power.ModifyDamageAdditive(target, baseDamage, props, source, card)

    damage = baseDamage + additiveBonus

    // 1b. 相乘所有 ModifyDamageMultiplicative 返回值
    for each power on source and target:
        damage *= power.ModifyDamageMultiplicative(target, damage, props, source, card)

    // 1c. 取所有 ModifyDamageCap 的最小值
    cap = decimal.MaxValue
    for each power on source and target:
        cap = Min(cap, power.ModifyDamageCap(target, props, source, card))
    damage = Min(damage, cap)
} else {
    damage = baseDamage
}

// === Phase 2: 受伤前钩子 ===
for each power on target:
    power.BeforeDamageReceived(target, ref damage, source, props)

// === Phase 3: 格挡 ===
if (!props.HasFlag(ValueProp.Unblockable)) {
    blocked = Min(target.Block, damage)
    target.Block -= blocked
    damage -= blocked
}

// === Phase 4: HP 损失修改 ===
// ModifyHpLostBeforeOsty / ModifyHpLostAfterOsty / ModifyHpLostAfterOstyLate
for each power on target:
    damage = power.ModifyHpLostAfterOsty(target, damage, props, source)

for each power on target:
    damage = power.ModifyHpLostAfterOstyLate(target, damage, props, source)

// === Phase 5: 实际扣血 ===
target.CurrentHp -= damage
```

### 2.4 格挡计算流水线

```
输入: baseBlock, creature, props

if (props.IsPoweredCardOrMonsterMoveBlock()) {
    additiveBonus = 0
    for each power on creature:
        additiveBonus += power.ModifyBlockAdditive(creature, baseBlock, props)

    block = baseBlock + additiveBonus

    for each power on creature:
        block *= power.ModifyBlockMultiplicative(creature, block, props)

    block = Max(block, 0)
} else {
    block = baseBlock
}

creature.Block += block

for each power on creature:
    power.AfterBlockGained(creature, block)
```

### 2.5 Power Hook 返回值约定

| Hook | 返回含义 | 默认值 | 示例 |
|------|---------|--------|------|
| `ModifyDamageAdditive` | 要**添加**的伤害加值 | `0m` | StrengthPower → `Amount` |
| `ModifyDamageMultiplicative` | 伤害乘数 | `1m` | Vulnerable → `1.5`, Weak → `0.75` |
| `ModifyDamageCap` | 伤害上限 | `decimal.MaxValue` | Intangible → `1m` |
| `ModifyBlockAdditive` | 要**添加**的格挡加值 | `0m` | DexterityPower → `Amount` |
| `ModifyBlockMultiplicative` | 格挡乘数 | `1m` | Frail → `0.75` |
| `ModifyHpLostAfterOsty` | 修改后的 HP 损失 | `amount` | Intangible → `1m` |
| `ModifyHpLostAfterOstyLate` | 修改后的 HP 损失（晚） | `amount` | Buffer → `0m` |
| `TryModifyPowerAmountReceived` | 是否拦截 Debuff | `false` | Artifact → `true` |
| `ModifyEnergyGain` | 修改后的能量 | `amount` | NoEnergyGain → `0` |
| `ShouldDraw` | 是否允许抽牌 | `true` | NoDraw → `false` |
| `ShouldClearBlock` | 是否清除格挡 | `true` | Barricade/Blur → `false` |

---

## 三、项目结构

```
sts2_ai/                              # .NET 8.0 类库
├── sts2_ai.csproj
│
├── CombatSimulator/
│   ├── Enums.cs                      # CombatSide, CardType, CardRarity, TargetType, ValueProp
│   ├── ValuePropExtensions.cs        # IsPoweredAttack(), IsPoweredCardOrMonsterMoveBlock()
│   ├── IRandom.cs                    # IRandom + DefaultRandom + SeedRandom
│   ├── SimEngine.cs                  # 战斗引擎（回合循环 + PlayCard + 伤害/格挡流水线）
│   │
│   ├── State/
│   │   ├── SimState.cs               # 战局状态
│   │   ├── SimCreature.cs            # 生物（HP/Block/Powers/GainBlock/LoseHp）
│   │   ├── SimCard.cs                # 卡牌数据
│   │   └── SimCardPile.cs            # 牌堆（抽/弃/手/消耗）
│   │
│   ├── Effects/                      # 21 个 Effect（每个独立文件）
│   │   ├── IEffect.cs                # void Execute(SimEngine, SimCreature source, SimCreature? target)
│   │   ├── DealDamage.cs             # 完整伤害流水线
│   │   ├── GainBlock.cs              # 完整格挡流水线
│   │   ├── DrawCards.cs
│   │   ├── ApplyPower.cs             # 含 Artifact 检查
│   │   ├── GainEnergy.cs             # 含能量钩子
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
│   │
│   └── Powers/                       # 每个文件一组相关 Power
│       ├── SimPower.cs               # Power 基类 + 所有 Hook 虚方法
│       ├── CorePowers.cs             # Strength/Dexterity/Vulnerable/Weak/Frail
│       ├── Batch2Powers.cs           # Plating/Regen/Buffer/Intangible/Barricade/Blur
│       ├── Batch3Powers.cs           # Poison/Thorns/Accuracy
│       ├── ArtifactPower.cs          # Artifact（特殊拦截逻辑）
│       ├── Batch4Powers.cs           # DemonForm/DarkEmbrace/FeelNoPain/FlameBarrier/Juggernaut/NoDraw/Rupture/Corruption
│       ├── Batch5Powers.cs           # Rage/Inferno/CrimsonMantle/Cruelty/FreeAttack/Pyre/Vicious/Mangle/NoEnergyGain
│       └── PlaceholderPower.cs       # 无效果占位
│
├── Cards/                            # 卡牌定义
│   ├── CardRegistry.cs              # 注册入口（仅一行一个 Register 调用）
│   ├── Ironclad/                    # 87 张 Ironclad
│   ├── Colorless/                   # 64 张无色
│   ├── Curse/                       # 18 张诅咒
│   ├── Status/                      # 12 张状态
│   ├── Event/                       # 27 张事件
│   ├── Token/                       # 14 张 Token
│   └── Quest/                       # 3 张任务
│
├── Enemies/                          # 怪物 AI
│   ├── EnemyEnums.cs                 # IntentType, MoveRepeatType
│   ├── IEnemyAi.cs                   # 接口
│   ├── EnemyTurnPlan.cs              # 回合计划
│   ├── MonsterStateMachine.cs        # 状态机解释器
│   ├── MoveState.cs / ConditionalBranch.cs / RandomBranch.cs
│   ├── IntentFactory.cs
│   └── Act1/...（按家族分组）
│
├── MCTS/                             # MCTS 战斗 AI
└── Evaluation/                       # 评估模块

sts2_ai_tests/
├── sts2_ai_tests.csproj
├── SimEngineTests.cs
├── EffectTests.cs
├── PowerTests.cs
└── ...

sts2_ai_visual/
└── Program.cs

sts2_ai_mod/
└── ...
```

---

## 四、全部 Power 精确行为（从游戏源码提取）

### Core Powers

| Power | Type | Stack | AllowNeg | Hook | 条件 | 返回值 | TickDown |
|-------|------|-------|----------|------|------|--------|---------|
| StrengthPower | Buff | Counter | true | `ModifyDamageAdditive` | `dealer == Owner && props.IsPoweredAttack()` | `Amount` | 无 |
| DexterityPower | Buff | Counter | true | `ModifyBlockAdditive` | `(cardSource?.Owner.Creature ?? target) == Owner && props.IsPoweredCardOrMonsterMoveBlock()` | `Amount` | 无 |
| VulnerablePower | Debuff | Counter | false | `ModifyDamageMultiplicative` | `target == Owner && props.IsPoweredAttack()` | `1.5` | Enemy turn end |
| WeakPower | Debuff | Counter | false | `ModifyDamageMultiplicative` | `dealer == Owner && props.IsPoweredAttack()` | `0.75` | Enemy turn end |
| FrailPower | Debuff | Counter | false | `ModifyBlockMultiplicative` | `target == Owner && props.IsPoweredCardOrMonsterMoveBlock()` | `0.75` | Enemy turn end |

### Batch 2 Powers

| Power | Type | Stack | 行为 |
|-------|------|-------|------|
| PlatingPower | Buff | Counter | `BeforeSideTurnEndEarly`: 获得 `Amount` Unpowered 格挡；`AfterSideTurnStart`: 每回合减 `Decrement`（敌人按玩家数） |
| RegenPower | Buff | Counter | `AfterSideTurnEnd`: 治疗 `Amount` HP → `Decrement(this)` |
| BufferPower | Buff | Counter | `ModifyHpLostAfterOstyLate`: 返回 `0m` 完全吸收 → `AfterModifyingHpLostAfterOsty`: `Decrement(this)` |
| IntangiblePower | Buff | Counter | `ModifyHpLostAfterOsty`: 返回 `1m`（伤害上限 1）；`ModifyDamageCap`: 返回 `1m`；`AfterSideTurnEnd(Enemy)`: `Decrement(this)` |
| BarricadePower | Buff | Single | `ShouldClearBlock`: 返回 `false`（Owner 不清除格挡）|
| BlurPower | Buff | Counter | `ShouldClearBlock`: 返回 `false`（Owner 不清除）；`AfterSideTurnStart`: `Decrement(this)` |

### Batch 3 Powers

| Power | Type | Stack | 行为 |
|-------|------|-------|------|
| PoisonPower | Debuff | Counter | `AfterSideTurnStart`: 造成 `Amount` 伤害（Unblockable+Unpowered）`TriggerCount` 次，每次 `Decrement(this)` |
| ThornsPower | Buff | Counter | `BeforeDamageReceived`: 对攻击者造成 `Amount` 反伤（Unpowered+SkipHurtAnim） |
| AccuracyPower | Buff | Counter | `ModifyDamageAdditive`: 仅对 `CardTag.Shiv` 卡生效，返回 `Amount` |

### Special Powers

| Power | Type | Stack | 行为 |
|-------|------|-------|------|
| ArtifactPower | Buff | Counter | `TryModifyPowerAmountReceived`: 拦截 Debuff（`modifiedAmount=0`，返回 true）→ `Decrement(this)` |

### Batch 4 Powers

| Power | Type | Stack | 行为 |
|-------|------|-------|------|
| DemonFormPower | Buff | Counter | `AfterSideTurnStart`: 应用 `Amount` StrengthPower 给 Owner |
| DarkEmbracePower | Buff | Counter | `AfterCardExhausted`: 非 ethereal 抽 `Amount` 张；ethereal 计数；`AfterSideTurnEnd`: 抽 `Amount * etherealCount` 张 |
| FeelNoPainPower | Buff | Counter | `AfterCardExhausted`: 获得 `Amount` Unpowered 格挡 |
| FlameBarrierPower | Buff | Counter | `AfterDamageReceived`: 对攻击者造成 `Amount` 反伤；`AfterSideTurnEnd(Opponent)`: 移除自己 |
| JuggernautPower | Buff | Counter | `AfterBlockGained`: 对随机敌人造成 `Amount` 伤害 |
| NoDrawPower | Debuff | Single | `ShouldDraw`: 阻止非 `fromHandDraw` 的抽牌 |
| RupturePower | Buff | Counter | `BeforeDamageReceived`: 当 Owner 受伤时获得 `Amount` Strength |
| CorruptionPower | Buff | Single | 技能牌费用变 0 且打出即消耗 |

### Batch 5 Powers

| Power | Type | Stack | 行为 |
|-------|------|-------|------|
| RagePower | Buff | Counter(临时) | `AfterCardPlayed(Attack)`: 获得 `Amount` 格挡 |
| InfernoPower | Buff | Counter | `AfterCardPlayed(Attack)`: 对所有敌人造成 `Amount` 伤害 |
| CrimsonMantlePower | Buff | Counter | `AfterCardPlayed(Attack)`: 获得 `Amount` 格挡 |
| CrueltyPower | Buff | Counter | 打攻击牌给敌人上易伤（修改易伤乘数） |
| FreeAttackPower | Buff | Single(一次性) | 下张攻击牌 0 费，之后移除 |
| PyrePower | Buff | Counter | `AfterPlayerTurnStart`: 获得 `Amount` 能量 |
| ViciousPower | Buff | Counter | 上易伤时抽 `Amount` 张牌 |
| ManglePower | Debuff | Counter | 攻击 Owner 时减少攻击者力量 |
| NoEnergyGainPower | Debuff | Single | `ModifyEnergyGain`: 返回 0 |

---

## 五、21 个 Effect 规格

| Effect | 参数 | 执行行为 |
|--------|------|---------|
| DealDamage | BaseAmount, TargetType(Default=SingleEnemy), Props(=Move), HitCount(=1) | 经过完整伤害流水线 |
| GainBlock | BaseAmount, TargetType(=Self) | 经过完整格挡流水线 |
| DrawCards | Amount | 抽牌（检查 NoDraw）|
| ApplyPower | PowerFactory, Amount, TargetType | 应用 Power（检查 Artifact）|
| GainEnergy | Amount | 获得能量（检查 ModifyEnergyGain）|
| Heal | Amount, Target(=Self) | 治疗，不超过 MaxHp |
| SelfDamage | Amount | Unblockable+Unpowered 自伤 |
| DiscardHand | Amount(=0=全部) | 从手牌弃牌 |
| ExhaustThisCard | — | 由引擎 HandleCardDisposition 处理 |
| GainMaxHp | Amount | MaxHp += Amount + 等量治疗 |
| DamageWithBlock | BaseAmount, Target, Multiplier | damage = BaseAmount + Block * Multiplier |
| CreateCardInDiscard | — | 复制此牌到弃牌堆 |
| MoveCardFromDiscardToDrawTop | — | 弃牌堆随机牌→抽牌堆顶 |
| ExhaustFromHand | Amount(=0=全部), Filter | 手牌选牌消耗 |
| AutoPlayFromDrawPile | ExhaustAfterPlay | 自动打出牌堆顶 |
| PutCardOnDrawPile | CardFilter | 手牌→抽牌堆顶 |
| DuplicateCardInHand | CardFilter, UpgradeDuplicate | 复制手牌 |
| ReturnFromExhaustToHand | — | 消耗堆→手牌 |
| UpgradeFromHand | Amount(=0=全部), Filter | 升级手牌 |

---

## 六、实施方案（6 个 Sprint）

### Sprint 0：项目核心引擎

**文件清单**：
1. `sts2_ai.csproj` + `sts2_ai_tests.csproj`
2. `Enums.cs` — 所有枚举
3. `ValuePropExtensions.cs` — `IsPoweredAttack()` / `IsPoweredCardOrMonsterMoveBlock()`
4. `IRandom.cs` — 随机数接口
5. `SimCard.cs` — 卡牌数据
6. `SimCardPile.cs` — 牌堆
7. `SimCreature.cs` — 生物（`GainBlock`, `LoseHp`, 不含 Buffer/Intangible 特殊逻辑）
8. `SimState.cs` — 战局
9. `SimPower.cs` — Power 基类（**正确 Hook 签名**）
10. `SimEngine.cs` — **完整伤害/格挡/抽牌/能量流水线** + 回合循环
11. `IEffect.cs` — Effect 接口

**测试**：空战局 1 回合无崩溃

### Sprint 1：Effects + Powers

**Effects**（21 个文件）：
- `DealDamage.cs` / `GainBlock.cs` / `DrawCards.cs` / `ApplyPower.cs` / `GainEnergy.cs`
- `Heal.cs` / `SelfDamage.cs` / `DiscardHand.cs` / `ExhaustThisCard.cs` / `GainMaxHp.cs`
- `DamageWithBlock.cs` / `CreateCardInDiscard.cs` / `MoveCardFromDiscardToDrawTop.cs`
- `ExhaustFromHand.cs` / `AutoPlayFromDrawPile.cs` / `PutCardOnDrawPile.cs`
- `DuplicateCardInHand.cs` / `ReturnFromExhaustToHand.cs` / `UpgradeFromHand.cs`

**Powers**（8 个文件）：
- `CorePowers.cs` — Strength/Dexterity/Vulnerable/Weak/Frail
- `Batch2Powers.cs` — Plating/Regen/Buffer/Intangible/Barricade/Blur
- `Batch3Powers.cs` — Poison/Thorns/Accuracy
- `ArtifactPower.cs` — Artifact
- `Batch4Powers.cs` — DemonForm/DarkEmbrace/FeelNoPain/FlameBarrier/Juggernaut/NoDraw/Rupture/Corruption
- `Batch5Powers.cs` — Rage/Inferno/CrimsonMantle/Cruelty/FreeAttack/Pyre/Vicious/Mangle/NoEnergyGain
- `PlaceholderPower.cs`

**验证**：30+ 测试覆盖每个 Effect 和 Power

### Sprint 2-5：（同 Sprint 2-5 原始计划，无变化）

---

## 七、关键注意事项

### 7.1 Hook 签名必须一致

游戏中的 `ModifyDamageAdditive` 返回的是**加值**（不是修改后的总数），引擎会累加所有加值。例如：
- StrengthPower(3) + DexterityPower(0) + ... → additiveBonus = 3
- damage = baseDamage(6) + 3 = 9

### 7.2 IsPoweredAttack() 判断

所有 Power 效果必须检查 `props.IsPoweredAttack()`（攻击）或 `props.IsPoweredCardOrMonsterMoveBlock()`（格挡），否则返回默认值。

### 7.3 默认返回值

每个 Hook 必须有正确的默认返回值：
- `ModifyDamageAdditive` → `0m`（无加值）
- `ModifyDamageMultiplicative` → `1m`（无变化）
- `ModifyDamageCap` → `decimal.MaxValue`（无上限）
- `ModifyBlockAdditive` → `0m`
- `ModifyBlockMultiplicative` → `1m`
- `ModifyHpLostAfterOsty/OstyLate` → `amount`（无变化）

### 7.4 从游戏源码逐条验证

实现每个 Power 时必须对照 `Slay The Spire 2/src/Core/Models/Powers/<Name>.cs`，确保：
1. Hook 方法匹配
2. 条件判断匹配
3. 返回值匹配
4. TickDown 时机匹配

---

## 八、测试策略

| 测试类型 | 覆盖范围 |
|---------|---------|
| 引擎测试 | SimEngine 回合循环、PlayCard、伤害/格挡流水线 |
| Effect 测试 | 每个 Effect 独立执行结果 |
| Power 测试 | 每个 Power 的 Hook 返回值 + 条件 + TickDown |
| 卡牌测试 | 每张卡打出后 Effect 序列结果 |
| 怪物测试 | 每个怪物 5-10 回合行为序列 |
| 整合测试 | 真实卡组 vs 真实敌人 |

---

## 九、文件组织规范（防损坏）

1. **每张卡一个文件**
2. **每个怪物一个文件**
3. 每个 Effect 独立文件
4. Power 按批分组（每文件 2-8 个相关 Power）
5. **禁止大段替换**：任何编辑不超过 50 行
6. **Registry 只做注册行**，不包含任何逻辑
7. 每完成 5-10 张卡或 1-2 个怪物就 commit
