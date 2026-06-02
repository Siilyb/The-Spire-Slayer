# STS2 AI — Slay the Spire 2 全自动打牌 AI

基于游戏源码 `Slay The Spire 2/src/Core/` 完整复刻的战斗模拟器 + MCTS AI。

---

## 项目结构

```
sts2_ai/                          # .NET 8.0 类库（57 个源文件）
├── CombatSimulator/              # 战斗模拟引擎
│   ├── Enums.cs                  # 所有枚举定义
│   ├── ValuePropExtensions.cs    # IsPoweredAttack / IsPoweredCardOrMonsterMoveBlock
│   ├── IRandom.cs                # 随机数接口 + DefaultRandom + SeedRandom
│   ├── SimEngine.cs              # 战斗引擎核心（回合循环、伤害/格挡流水线）
│   ├── State/                    # 数据模型
│   │   ├── SimState.cs           # 战局状态
│   │   ├── SimCreature.cs        # 生物（HP/Block/Powers）
│   │   ├── SimCard.cs            # 卡牌数据
│   │   └── SimCardPile.cs        # 牌堆管理
│   ├── Effects/                  # 30+ 效果原语
│   │   ├── IEffect.cs            # 接口
│   │   ├── DealDamage.cs         # 伤害（多段/计算/条件）
│   │   ├── GainBlock.cs          # 格挡（支持计算格挡）
│   │   ├── DrawCards.cs          # 抽牌
│   │   ├── ApplyPower.cs         # 应用 Power（含 Artifact 拦截）
│   │   ├── GainEnergy.cs         # 能量
│   │   ├── Heal.cs / SelfDamage.cs / DiscardHand.cs
│   │   ├── GainMaxHp.cs / DamageWithBlock.cs
│   │   ├── ExhaustFromHand / AutoPlayFromDrawPile
│   │   ├── DuplicateCardInHand / ReturnFromExhaustToHand
│   │   ├── UpgradeFromHand / CreateCardInDiscard
│   │   └── CurseStatusEffects.cs / EventEffects.cs / AdvancedEffects.cs
│   └── Powers/                   # 34 个 Power
│       ├── SimPower.cs           # Power 基类（完整 Hook 系统）
│       ├── CorePowers.cs         # Strength/Dexterity/Vulnerable/Weak/Frail
│       ├── Batch2Powers.cs       # Plating/Regen/Buffer/Intangible/Barricade/Blur
│       ├── Batch3Powers.cs       # Poison/Thorns/Accuracy
│       ├── ArtifactPower.cs      # Artifact（Debuff 拦截）
│       ├── Batch4Powers.cs       # DemonForm/DarkEmbrace/FeelNoPain/FlameBarrier/...
│       ├── Batch5Powers.cs       # Rage/Inferno/CrimsonMantle/Cruelty/FreeAttack/...
│       └── CardPowers.cs         # SetupStrikePower / ColossusPower
│
├── Cards/                        # 卡牌数据库（225 张）
│   ├── CardRegistry.cs           # 注册入口
│   ├── Ironclad/                 # 87 张 Ironclad 卡
│   ├── Colorless/                # 64 张无色卡
│   ├── Curse/                    # 18 张诅咒卡
│   ├── Status/                   # 12 张状态卡
│   ├── Event/                    # 27 张 Event 卡
│   ├── Token/                    # 14 张 Token 卡
│   └── Quest/                    # 3 张任务卡
│
├── Enemies/                      # 怪物 AI
│   ├── IEnemyAi.cs               # IEnemyAi 接口 + EnemyTurnPlan
│   ├── EnemyDb.cs                # 怪物注册（80 个敌人）
│   ├── MonsterAis.cs             # 15 个核心 AI
│   └── MonsterAisExtended.cs     # 65 个扩展 AI
│
├── MCTS/                         # （待实现）
└── Evaluation/                   # （待实现）

sts2_ai_tests/                    # 测试项目（31 个测试）
└── Tests/
    ├── SimEngineTests.cs         # 引擎基础测试
    └── EffectAndPowerTests.cs    # Effect + Power 综合测试

sts2_ai_mod/                      # Mod 集成（待实现）
sts2_ai_visual/                   # 控制台可视化（待实现）
```

---

## 已完成模块

| 模块 | 状态 | 说明 |
|------|------|------|
| 引擎核心 | ✅ | SimEngine 回合循环、完整伤害/格挡流水线、Hook 系统 |
| Effect 原语 | ✅ | 30+ 个 Effect，覆盖全部基础效果 |
| Power系统 | ✅ | 34 个 Power，含完整 Hook 链 |
| Ironclad 卡牌 | ✅ 87/87 | 全部精确实现（含升级版） |
| 无色卡 | ✅ 64/64 | 含计算伤害/条件效果 |
| 诅咒卡 | ✅ 18/18 | 含回合结束效果 |
| 状态卡 | ✅ 12/12 | 含抽牌触发效果 |
| Event 卡 | ✅ 27/27 | 含特殊 Event 效果 |
| Token 卡 | ✅ 14/14 | 含 Shiv/Soul 等 |
| Quest 卡 | ✅ 3/3 | 任务物品卡 |
| 怪物 AI | ✅ 80/117 | 第一幕+多数精英 |
| 测试 | ✅ 31 个 | 引擎/Power/Effect 测试 |

---

## 设计原则

### 1. Hook 返回值约定（与游戏一致）
| Hook | 返回含义 | 默认值 |
|------|---------|--------|
| `ModifyDamageAdditive` | 要**添加**的伤害加值 | `0m` |
| `ModifyDamageMultiplicative` | 伤害乘数 | `1m` |
| `ModifyDamageCap` | 伤害上限 | `MaxValue` |
| `ModifyBlockAdditive` | 格挡加值 | `0m` |
| `ModifyBlockMultiplicative` | 格挡乘数 | `1m` |
| `ModifyHpLostAfterOsty` | HP 损失修改 | `amount` |

### 2. 伤害流水线
```
原始伤害 → 累加 Additive → 相乘 Multiplicative → 取 Cap 最小值
→ BeforeDamageReceived → 格挡扣除 → HP 损失修改（Buffer/Intangible）→ 扣血
```

### 3. 怪物 AI 状态机
每个怪物独立 AI 实例（`CustomState["__ai"]`），支持：
- 固定循环 / 交替循环 / 多段循环
- 加权随机分支
- 条件分支（HP 阈值等）
- 多阶段切换

---

## 开发计划

```
Phase 1 (完成) → Phase 2 (怪物, 80%完成) → Phase 3 (MCTS) → Phase 4 (抓牌) → Phase 5 (路线) → Phase 6 (Mod)
```
