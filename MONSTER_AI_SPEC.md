# 怪物 AI 精确规范文档

> 来源：游戏源码 `src/Core/Models/Monsters/*.cs`
> 每个怪物定义 `GenerateMoveStateMachine()` 返回状态机

---

## 一、AI 系统架构

### 1.1 模拟器 IEnemyAi 接口

```csharp
interface IEnemyAi {
    string EnemyId { get; }
    int MinHp { get; }
    int MaxHp { get; }
    void OnCombatStart(SimCreature enemy);    // 设置初始 Buff
    EnemyTurnPlan PlanNextTurn(SimState, SimCreature enemy);
}

class EnemyTurnPlan {
    IntentType Intent;
    int Damage, Block, Hits(=1), BuffAmount, DebuffAmount;
    string? StatusCardId;   // 如 "DAZED", "SLIMED", "WOUND"
    int StatusCardCount;
    ValueProp DamageProps;  // 默认 ValueProp.Move
}
```

### 1.2 状态机翻译规则

| 游戏模式 | 模拟器实现 |
|---------|-----------|
| 固定自循环 `Move → FollowUp=self` | `PlanNextTurn` 始终返回同一动作 |
| 交替循环 `A→B→A→B` | 内部 `_toggle` 布尔切换 |
| 多段循环 `A→B→C→A` | 内部 `_phase` 计数器 |
| `ConditionalBranchState` | `if/switch` 条件分支 |
| `RandomBranchState` | `Rng` 加权随机 + 冷却追踪 |
| `MustPerformOnceBeforeTransitioning` | 布尔标记 |
| `MoveRepeatType` | 冷却/使用次数追踪 |
| `DeadState` | HP<=0 时的特殊动作 |
| 多段身体（Decimillipede） | HP 共享逻辑 |

---

## 二、全部怪物行为

### 2.1 简单固定循环

| ID | Name | HP | 初始 Buff | 行为 |
|----|------|----|----------|------|
| ZAPBOT | Zapbot | 18-23(19-24 asc) | HighVoltagePower 2 | 每回合 Zap: 14(15 asc) 单体伤害, 自循环 |
| GUARDBOT | Guardbot | — | — | 每回合 Guard: 格挡+攻击, 自循环 |
| NOISEBOT | Noisebot | — | — | 每回合 Noise: 伤害, 自循环 |
| PUNCH_CONSTRUCT | PunchConstruct | — | — | 每回合 Punch: 伤害, 自循环 |
| ROCKET | Rocket | — | — | 每回合 Rocket: AOE 伤害, 自循环 |
| STABBOT | Stabbot | — | — | 每回合 Stab: 伤害, 自循环 |
| CUBEX_CONSTRUCT | CubexConstruct | — | — | 每回合 Cubex: 伤害, 自循环 |

### 2.2 交替循环

| ID | Name | HP | 初始 Buff | 行为 |
|----|------|----|----------|------|
| CHOMPER | Chomper | 60-64(63-67 asc) | Artifact 2 | Clamp(8→9 dmg×2) ↔ Screech(3×Dazed), 可选 _screamFirst |
| LEAF_SLIME_M | LeafSlimeM | 32-35(33-36 asc) | — | ClumpShot(8→9 dmg) ↔ StickyShot(2×Slimed), 从 Sticky 开始 |
| LEAF_SLIME_S | LeafSlimeS | — | — | Attack(3) ↔ Slimed, 随机 |
| TWIG_SLIME_M | TwigSlimeM | — | — | Pounce(11) [cd2] ↔ Slimed |
| TWIG_SLIME_S | TwigSlimeS | — | — | Attack(4) 自循环 |
| SNEAKY_GREMLIN | SneakyGremlin | — | — | Attack(4) ↔ Attack(7) |
| FAT_GREMLIN | FatGremlin | — | — | Stun → Escape |

### 2.3 多段循环

| ID | Name | HP | 初始 Buff | 行为 |
|----|------|----|----------|------|
| NIBBIT | Nibbit | — | — | Attack12 → Attack6+Block5 → Strength2 |
| CORPSE_SLUG | CorpseSlug | — | Strength 4(5), 群组偏移 | 3×2 → Glomp 8 → Frail 2 |
| GREMLIN_MERC | GremlinMerc | — | — | 7×2 → 6×2+Weak → 8+Strength2 |
| WRIGGLER | Wriggler | — | — | Bite(dmg) ↔ Wriggle(Buff+Status), 按 SlotName 决定初始 |

### 2.4 随机分支

| ID | Name | HP | 初始 Buff | 行为 |
|----|------|----|----------|------|
| FLYCONID | Flyconid | — | — | Random: Vuln / Frail+8block / Smash 11 |
| SNAPPING_JAXFRUIT | SnappingJaxfruit | — | — | Attack 3 + Strength2 自循环 |
| SPECTRAL_KNIGHT | SpectralKnight | — | — | Hex(Debuff) → Random(soulSlash 冷却2 / soulFlame 只能用一次) |
| LOUSE_PROGENITOR | LouseProgenitor | — | Buffer 14(18) | Web 9+Frail → Curl 14block+5STR → Pounce 14 |
| FUZZY_WURM_CRAWLER | FuzzyWurmCrawler | — | — | Attack 4 ↔ Inhale 7STR |

### 2.5 精英

| ID | Name | HP | 初始 Buff | 行为 |
|----|------|----|----------|------|
| DECIMILLIPEDE | Decimillipede (每节) | 40-46(46-52 asc) | ReattachPower 25 | Writhe(5×2) → Bulk(6+STR2) → Constrict(8+Weak) → 死亡=Reattach → 复活后随机 |
| SOUL_NEXUS | SoulNexus | — | — | — |
| MECHA_KNIGHT | MechaKnight | — | — | — |

### 2.6 Boss

| ID | Name | HP | 阶段 | 行为 |
|----|------|----|------|------|
| QUEEN | Queen | — | 多阶段 | — |
| KNOWLEDGE_DEMON | KnowledgeDemon | — | 特殊 | 每回合出题 |
| ARCHITECT | Architect | — | — | — |
| WATERFALL_GIANT | WaterfallGiant | — | 多阶段 | 含 DeathBlow |
| THE_INSATIABLE | TheInsatiable | — | — | — |
| AEONGLASS | Aeonglass | — | — | — |

(完整文档因怪物数量巨大，实际实现时逐类对照源码添加)
