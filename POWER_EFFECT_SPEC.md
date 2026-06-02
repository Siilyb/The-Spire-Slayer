# Power & Effect 精确规范文档

> 基于 STS2 游戏源码 `src/Core/Models/Powers/*.cs` 和命令系统逐条提取。
> 本规范用于指导模拟器的 Power 和 Effect 实现。

---

## 一、架构核心概念

### 1.1 ValueProp 标志位

```csharp
[Flags] enum ValueProp {
    None = 0,
    Unblockable = 2,    // 伤害无视格挡
    Unpowered = 4,      // 不受 Power 影响（力量/易伤/虚弱等）
    Move = 8,           // 标记为"动作"（卡牌攻击/怪物移动）
    SkipHurtAnim = 16   // 跳过受伤动画
}
```

### 1.2 "Powered" 判断

```csharp
// 攻击是否受 Power 影响
bool IsPoweredAttack(ValueProp props) 
    => props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);

// 格挡是否受 Power 影响 
bool IsPoweredCardOrMonsterMoveBlock(ValueProp props)
    => props.HasFlag(ValueProp.Move) && !props.HasFlag(ValueProp.Unpowered);
```

**规则**：只有带 `Move` 标志且不带 `Unpowered` 的效果才会受 Power 影响。

### 1.3 伤害计算流水线

```
原始伤害
  → 所有 Hook.ModifyDamageAdditive 返回值的总和
  → damage += 总和
  → 所有 Hook.ModifyDamageMultiplicative 返回值相乘
  → damage *= 每个乘数
  → 所有 Hook.ModifyDamageCap 返回值取最小值
  → damage = Min(damage, 每个上限)
  → Hook.BeforeDamageReceived
  → 格挡扣除 (除非 Unblockable)
  → Hook.ModifyHpLostBeforeOsty
  → Hook.ModifyHpLostAfterOsty
  → Hook.ModifyHpLostAfterOstyLate
  → target.LoseHpInternal(finalDamage)
  → Hook.AfterDamageReceived
  → Hook.AfterDamageGiven
```

### 1.4 格挡计算流水线

```
原始格挡
  → 所有 Hook.ModifyBlockAdditive 返回值的总和
  → block += 总和
  → 所有 Hook.ModifyBlockMultiplicative 返回值相乘
  → block *= 每个乘数
  → block = Max(block, 0)
  → creature.GainBlockInternal(block)
  → Hook.AfterBlockGained
```

### 1.5 Hook 返回值约定

- `ModifyDamageAdditive` → 返回要 **添加** 的数值（不是修改后的总数）
- `ModifyDamageMultiplicative` → 返回乘数（1.5, 0.75, 1.0）
- `ModifyDamageCap` → 返回上限（默认 decimal.MaxValue）
- `ModifyBlockAdditive` → 返回要添加的数值
- `ModifyBlockMultiplicative` → 返回乘数（0.75, 1.0）
- `ModifyHpLostAfterOsty` → 返回修改后的伤害值
- `TryModifyPowerAmountReceived` → 返回 true=拦截, false=放行

---

## 二、全部 Power 精确行为

### 2.1 StrengthPower
- **类型**: Buff, Counter, AllowNegative=true
- **Hook**: `ModifyDamageAdditive`
  - 条件: `dealer == Owner && props.IsPoweredAttack()`
  - 返回: `Amount`（直接加到伤害上）
- **不实现**: TickDown

### 2.2 DexterityPower
- **类型**: Buff, Counter, AllowNegative=true
- **Hook**: `ModifyBlockAdditive`
  - 条件: 如果 `cardSource != null` 则要求 `cardSource.Owner.Creature == Owner`
    否则要求 `target == Owner`
    并且 `props.IsPoweredCardOrMonsterMoveBlock()`
  - 返回: `Amount`

### 2.3 VulnerablePower
- **类型**: Debuff, Counter
- **Hook**: `ModifyDamageMultiplicative`
  - 条件: `target == Owner && props.IsPoweredAttack()`
  - 返回: 1.5（受 PaperPhrog/CrueltyPower/DebilitatePower 影响）
- **TickDown**: `AfterSideTurnEnd` when `side == Enemy` → `PowerCmd.TickDownDuration(this)`

### 2.4 WeakPower
- **类型**: Debuff, Counter
- **Hook**: `ModifyDamageMultiplicative`
  - 条件: `dealer == Owner && props.IsPoweredAttack()`
  - 返回: 0.75（受 PaperKrane/DebilitatePower 影响）
- **TickDown**: `AfterSideTurnEnd` when `side == Enemy` → `PowerCmd.TickDownDuration(this)`

### 2.5 FrailPower
- **类型**: Debuff, Counter
- **Hook**: `ModifyBlockMultiplicative`
  - 条件: `target == Owner && props.IsPoweredCardOrMonsterMoveBlock()`
  - 返回: 0.75
- **TickDown**: `AfterSideTurnEnd` when `side == Enemy` → `PowerCmd.TickDownDuration(this)`

### 2.6 PlatingPower
- **类型**: Buff, Counter
- **行为**:
  - `BeforeSideTurnEndEarly` → 如果 `participants.Contains(Owner)`, 获得 `Amount` 格挡（Unpowered）
  - `AfterSideTurnStart` → 如果不是第 1 回合, 减少 Amount（敌人侧减 `Decrement` 值, 玩家侧减 1）
- **注意**: 格挡在回合结束前获得（不是开始时）

### 2.7 RegenPower
- **类型**: Buff, Counter
- **Hook**: `AfterSideTurnEnd`
  - 条件: `participants.Contains(Owner) && !Owner.IsDead`
  - 行为: 治疗 `Amount` HP → 然后 `Decrement(this)`

### 2.8 BufferPower
- **类型**: Buff, Counter
- **Hook**: `ModifyHpLostAfterOstyLate`
  - 条件: `target == Owner`
  - 返回: `0m`（完全吸收伤害）
  - 然后 `AfterModifyingHpLostAfterOsty` → `Decrement(this)`

### 2.9 IntangiblePower
- **类型**: Buff, Counter
- **Hook**: `ModifyHpLostAfterOsty`
  - 条件: `CombatManager.Instance.IsInProgress && target == Owner && amount >= 1`
  - 返回: `1m`（伤害上限为 1）
- **Hook**: `ModifyDamageCap`
  - 条件: `target == Owner`
  - 返回: `1m`
- **TickDown**: `AfterSideTurnEnd` when `side == Enemy` → `Decrement(this)`

### 2.10 BarricadePower
- **类型**: Buff, Single
- **Hook**: `ShouldClearBlock`
  - 返回 `false` 当 `creature == Owner`, 否则 `true`

### 2.11 PoisonPower
- **类型**: Debuff（但使用 cream 颜色标签）, Counter
- **行为**: 每次 Owner 回合开始, 造成 `Amount` 伤害（Unblockable+Unpowered）
  - 触发次数 = `Min(Amount, 1 + 敌人身上的 AccelerantPower 层数总和)`
  - 每次触发后, 如果 Owner 存活则 `Decrement(this)`
- **TickDown**: 每次伤害后减 1

### 2.12 ThornsPower
- **类型**: Buff, Counter
- **Hook**: `BeforeDamageReceived`
  - 条件: `target == Owner && dealer != null && (props.IsPoweredAttack() || cardSource is Omnislice)`
  - 行为: 对 `dealer` 造成 `Amount` 伤害（Unpowered + SkipHurtAnim）

### 2.13 BlurPower
- **类型**: Buff, Counter
- **Hook**: `ShouldClearBlock`
  - 返回 `false` 当 `creature == Owner`
- **Hook**: `AfterSideTurnStart`
  - 条件: `participants.Contains(Owner)` → `Decrement(this)`
- **注意**: 每回合开始时减少 1 层

### 2.14 AccuracyPower
- **类型**: Buff, Counter
- **Hook**: `ModifyDamageAdditive`
  - 条件: `dealer == Owner && props.IsPoweredAttack() && card != null && card.Tags.Contains(CardTag.Shiv)`
  - 返回: `Amount`（仅对 Shiv 标签的卡生效）

### 2.15 ArtifactPower
- **类型**: Buff, Counter
- **Hook**: `TryModifyPowerAmountReceived`
  - 条件: `target == Owner && incomingPower.TypeForCurrentAmount == Debuff && incomingPower.IsVisible`
  - 返回: `true`（拦截）, 设置 `modifiedAmount = 0`
  - 然后 `AfterModifyingPowerAmountReceived` → `Decrement(this)`

### 2.16 DemonFormPower
- **类型**: Buff, Counter
- **Hook**: `AfterSideTurnStart`
  - 条件: `participants.Contains(Owner)`
  - 行为: 应用 `Amount` 层 StrengthPower 给自己 → Flash()

### 2.17 DarkEmbracePower
- **类型**: Buff, Counter
- **行为**: 追踪 ethereal 计数
  - `AfterCardExhausted` → 如果 `card.Owner.Creature == Owner`:
    - 如果 `causedByEthereal` → etherealCount++
    - 否则 → Draw(Amount)
  - `AfterSideTurnEnd` → 如果 `participants.Contains(Owner)`:
    - Draw(Amount * etherealCount), 然后重置 etherealCount = 0

### 2.18 FeelNoPainPower
- **类型**: Buff, Counter
- **Hook**: `AfterCardExhausted`
  - 条件: `card.Owner.Creature == Owner`
  - 行为: 获得 `Amount` 格挡（Unpowered）

### 2.19 FlameBarrierPower
- **类型**: Buff, Counter
- **Hook**: `AfterDamageReceived`
  - 条件: `target == Owner && dealer != null && props.IsPoweredAttack()`
  - 行为: 对 `dealer` 造成 `Amount` 伤害（Unpowered）
- **移除**: `AfterSideTurnEnd` 当 `Owner.Side != side` → `PowerCmd.Remove(this)`

### 2.20 JuggernautPower
- **类型**: Buff, Counter
- **Hook**: `AfterBlockGained`
  - 条件: `amount > 0 && creature == Owner`
  - 行为: 从 `hittableEnemies` 中**随机选一个**, 造成 `Amount` 伤害（Unpowered）

### 2.21 NoDrawPower
- **类型**: Debuff, Single
- **Hook**: `ShouldDraw`
  - 条件: 如果 `fromHandDraw` → 放行（`true`）
  - 如果 `player != Owner.Player` → 放行
  - 否则 → 阻止（`false`）, Flash()

### 2.22 RupturePower
- **类型**: Buff, Counter
- **Hook**: `BeforeDamageReceived`
  - 行为: 当 Owner 受到伤害时, 获得 `Amount` 层 StrengthPower（需要查源码确认精确条件）

### 2.23 CorruptionPower
- **类型**: Buff, Single
- **Hook**: `TryModifyEnergyCostInCombatLate`
  - 条件: `card.Type == CardType.Skill`
  - 设置: `modifiedCost = 0`, 返回 `true`
- **Hook**: `ModifyCardPlayResultPileTypeAndPosition`
  - 条件: `card.Type == CardType.Skill`
  - 设置: `pileType = PileType.Exhaust`, 覆盖默认行为

### 2.24 RagePower
- **类型**: Buff, Counter（临时）
- **Hook**: `AfterCardPlayed`
  - 条件: `card.Type == CardType.Attack`
  - 行为: 获得 `Amount` 格挡

### 2.25 InfernoPower
- **类型**: Buff, Counter
- **Hook**: `AfterCardPlayed`
  - 条件: 打出攻击牌
  - 行为: 对所有敌人造成 `Amount` 伤害

### 2.26 CrimsonMantlePower
- **类型**: Buff, Counter
- **Hook**: `AfterCardPlayed`
  - 条件: 打出攻击牌
  - 行为: 获得 `Amount` 格挡

### 2.27 CrueltyPower
- **类型**: Buff, Counter
- **行为**: 打攻击牌前给目标上易伤（影响 VulnerablePower 的乘数）
  - `ModifyVulnerableMultiplier` 方法修改易伤乘数

### 2.28 FreeAttackPower
- **类型**: Buff, Single（一次性）
- **Hook**: `TryModifyEnergyCostInCombatLate`
  - 条件: `card.Type == CardType.Attack`
  - 设置: `modifiedCost = 0`, 返回 `true`
  - 然后移除自己

### 2.29 PyrePower
- **类型**: Buff, Counter
- **Hook**: `AfterPlayerTurnStart`
  - 行为: 每回合开始获得 `Amount` 能量

### 2.30 ViciousPower
- **类型**: Buff, Counter
- **行为**: 当给敌人上易伤时, 抽 `Amount` 张牌

### 2.31 ManglePower
- **类型**: Debuff, Counter
- **Hook**: `ModifyDamageAdditive`
  - 行为: 攻击 Owner 时减少攻击者的力量

### 2.32 NoEnergyGainPower
- **类型**: Debuff, Single
- **Hook**: `ModifyEnergyGain`
  - 返回: `0`

---

## 三、全部 Effect 精确行为

### 3.1 DealDamage
- 参数: BaseAmount, TargetType, Props(=Move), HitCount(=1), StrengthMultiplier(=1)
- 执行: 对目标造成伤害, 经过完整伤害流水线

### 3.2 GainBlock
- 参数: BaseAmount, TargetType(=Self)
- 执行: 获得格挡, 经过格挡流水线

### 3.3 DrawCards
- 参数: Amount
- 执行: 抽牌（受 NoDraw 限制, `fromHandDraw=false`）

### 3.4 ApplyPower
- 参数: PowerFactory, Amount, TargetType
- 执行: 应用 Power 到目标, 经过 Artifact 检查

### 3.5 GainEnergy
- 参数: Amount
- 执行: 获得能量（受 ModifyEnergyGain 钩子影响）

### 3.6 Heal
- 参数: Amount, Target(=Self)
- 执行: 治疗目标, 不超过 MaxHp

### 3.7 SelfDamage
- 参数: Amount
- 执行: 对自己造成 Unblockable+Unpowered 伤害

### 3.8 DiscardHand
- 参数: Amount(=0=全部)
- 执行: 从手牌弃牌

### 3.9 ExhaustThisCard
- 标记: 由引擎的 HandleCardDisposition 处理

### 3.10 GainMaxHp
- 参数: Amount
- 执行: MaxHp += Amount, 然后等量治疗

### 3.11 DamageWithBlock
- 参数: BaseAmount, Target, Multiplier
- 执行: damage = BaseAmount + Block * Multiplier

### 3.12 CreateCardInDiscard
- 执行: 复制当前牌到弃牌堆

### 3.13 MoveCardFromDiscardToDrawTop
- 执行: 弃牌堆随机一张牌放到抽牌堆顶

### 3.14 ExhaustFromHand
- 参数: Amount(=0=全部), Filter
- 执行: 从手牌消耗选中的牌

### 3.15 AutoPlayFromDrawPile
- 参数: ExhaustAfterPlay
- 执行: 自动打出牌堆顶

### 3.16 PutCardOnDrawPile
- 参数: CardFilter
- 执行: 手牌中选一张放到抽牌堆顶

### 3.17 DuplicateCardInHand
- 参数: CardFilter, UpgradeDuplicate
- 执行: 复制手牌中符合条件的牌

### 3.18 ReturnFromExhaustToHand
- 执行: 消耗堆最新一张牌回到手牌

### 3.19 UpgradeFromHand
- 参数: Amount(=0=全部), Filter
- 执行: 升级手牌中符合条件的牌

---

## 四、引擎关键修改

### 4.1 伤害计算引擎

```csharp
decimal CalculateDamage(source, target, baseDamage, strengthMultiplier, props):
    if props.IsPoweredAttack():
        additiveSum = sum of all powers' ModifyDamageAdditive(source, target, baseDamage, strengthMultiplier, props)
        damage = baseDamage + additiveSum
        for each power:
            damage *= power.ModifyDamageMultiplicative(target, damage, props, source, cardSource)
        cap = min of all powers' ModifyDamageCap(target, props, source, cardSource)
        damage = Min(damage, cap)
    else:
        damage = baseDamage
    
    // BeforeDamageReceived hook
    for each power on target:
        power.BeforeDamageReceived(target, source, ref damage)
    
    // Block
    if !props.IsUnblockable:
        blocked = Min(target.Block, damage)
        target.Block -= blocked
        damage -= blocked
    
    // HP loss hooks
    if target.HasPower<BufferPower>():
        damage = 0 // ModifyHpLostAfterOstyLate
    
    if target.HasPower<IntangiblePower>() && damage >= 1:
        damage = 1 // ModifyHpLostAfterOsty
    
    target.CurrentHp -= damage
    return damage
```

### 4.2 格挡计算引擎

```csharp
decimal CalculateBlock(creature, baseBlock, props):
    if props.IsPoweredCardOrMonsterMoveBlock():
        additiveSum = sum of all powers' ModifyBlockAdditive(creature, baseBlock, props)
        block = baseBlock + additiveSum
        for each power:
            block *= power.ModifyBlockMultiplicative(creature, block, props)
        block = Max(block, 0)
    else:
        block = baseBlock
    creature.GainBlock(block)
    for each power:
        power.AfterBlockGained(creature, block)
    return block
```

### 4.3 Power 应用引擎

```csharp
void ApplyPower(target, powerTemplate, amount, applier):
    // Artifact check
    if target.HasPower<ArtifactPower>():
        if artifact.TryBlockDebuff(powerTemplate):
            return // debuff完全拦截
    
    // Stack
    existing = target.GetPower(powerTemplate.GetType())
    if existing != null:
        existing.Amount += amount
    else:
        power = powerTemplate.Clone()
        power.Amount = amount
        power.Owner = target
        target.AddPower(power)
```

### 4.4 抽牌引擎

```csharp
int DrawCards(count, fromHandDraw=false):
    for i in 0..count:
        if NoDraw exists and !fromHandDraw: break
        if draw pile empty: shuffle discard to draw
        card = draw pile draw
        card to hand
```
