# 无色卡 64 张精确规范

> 来源：`ColorlessCardPool.cs`（64 张）+ 每张卡源码

---

## 实现分类

### Group A: 简单 Attack/Skill/Power（可用现有 Effect 直接实现）
| # | Id | Cost | Upg | Type | Target | Keywords | Effects Base | Upgraded |
|---|-----|------|------|------|--------|----------|-------------|----------|
| 1 | ULTIMATE_STRIKE | 1 | - | Attack | SingleEnemy | — | DealDamage(14, Move) | DealDamage(20, Move) |
| 2 | ULTIMATE_DEFEND | 1 | - | Skill | Self | — | GainBlock(11, Move) | GainBlock(15, Move) |
| 3 | FINESSE | 0 | - | Skill | Self | — | GainBlock(4, Move) + DrawCards(1) | GainBlock(7, Move) + DrawCards(1) |
| 4 | FLASH_OF_STEEL | 0 | - | Attack | SingleEnemy | — | DealDamage(5, Move) + DrawCards(1) | DealDamage(8, Move) + DrawCards(1) |
| 5 | MASTER_OF_STRATEGY | 0 | - | Skill | Self | Exhaust | DrawCards(3) | DrawCards(4) |
| 6 | PRODUCTION | 0 | - | Skill | Self | Exhaust | GainEnergy(2) | GainEnergy(3) |
| 7 | IMPATIENCE | 0 | - | Skill | Self | — | if no Attack in hand: DrawCards(2) | if no Attack in hand: DrawCards(3) |
| 8 | PANIC_BUTTON | 0 | - | Skill | Self | Exhaust | GainBlock(30, Move) + ApplyPower(NoBlockPower,2) | GainBlock(40, Move) + ApplyPower(NoBlockPower,2) |
| 9 | PROWESS | 1 | - | Power | Self | — | ApplyPower(Strength,1) + ApplyPower(Dexterity,1) | ApplyPower(Strength,2) + ApplyPower(Dexterity,2) |
| 10 | LIFT | 1 | - | Skill | AnyAlly(MP) | — | GainBlock(11, Move) to ally | GainBlock(16, Move) to ally |
| 11 | INTERCEPT | 1 | - | Skill | AnyAlly(MP) | — | GainBlock(9, Move) self + ApplyPower(CoveredPower,1) to ally | GainBlock(13, Move) self + ApplyPower(CoveredPower,1) to ally |
| 12 | RALLY | 2 | - | Skill | AllAllies(MP) | — | GainBlock(12) to all allies | GainBlock(17) to all allies |
| 13 | BELIEVE_IN_YOU | 0 | - | Skill | AnyAlly(MP) | — | GainEnergy(2) to ally | GainEnergy(3) to ally |
| 14 | COORDINATE | 1 | - | Skill | AnyAlly(MP) | — | ApplyPower(CoordinatePower,5) to ally | ApplyPower(CoordinatePower,8) to ally |
| 15 | TAG_TEAM | 2 | - | Attack | SingleEnemy(MP) | — | DealDamage(11, Move) + ApplyPower(TagTeamPower,1) to target | DealDamage(15, Move) + ApplyPower(TagTeamPower,1) to target |
| 16 | KNOCKDOWN | 3 | - | Attack | SingleEnemy(MP) | — | DealDamage(10, Move) + ApplyPower(KnockdownPower,2) | DealDamage(14, Move) + ApplyPower(KnockdownPower,3) |
| 17 | GANG_UP | 1 | - | Attack | SingleEnemy(MP) | — | DealDamage(5+5×allyHits, Move) | DealDamage(5+7×allyHits, Move) |
| 18 | HUDDLE_UP | 1 | - | Skill | AllAllies(MP) | Exhaust | DrawCards(2) for each ally | DrawCards(3) for each ally |
| 19 | BOLAS | 0 | - | Attack | SingleEnemy | — | DealDamage(3, Move); returns to hand next turn | DealDamage(4, Move); returns to hand next turn |
| 20 | THRUMMING_HATCHET | 1 | - | Attack | SingleEnemy | — | DealDamage(11, Move); returns to hand next turn | DealDamage(14, Move); returns to hand next turn |
| 21 | VOLVOLLEY | 0(X) | - | Attack | RandomEnemy | — | DealDamage(10, Move) × X hits | DealDamage(13, Move) × X hits |

### Group B: 需要计算伤害/条件（已有 Effect 支持）
| # | Id | 说明 |
|---|-----|------|
| 22 | MIND_BLAST | 伤害 = 抽牌堆卡牌数, 1费→0费升级, Innate |
| 23 | GOLD_AXE | 伤害 = 本回合已打出卡牌数, 升级版 Retain |
| 24 | OMNISLICE | 0费, 对主目标造成 8(11)伤害, 同时对其他敌人造成等量 Unpowered 伤害 |
| 25 | DRAMATIC_ENTRANCE | 0费, Innate+Exhaust, 全体 11(15)伤害 |
| 26 | FISTICUFFS | 1费, 造成 7(9)伤害, 获得等于总伤害的格挡 |
| 27 | SEEKER_STRIKE | Strike标签, 1费, 9(12)伤害, 抽牌堆选1张入手 |
| 28 | RENDING | 2费, 伤害=15(18)+5(8)×目标非临时Debuff数量 |
| 29 | SALVOSALVO | 1费, 12(16)伤害, 附加 RetainHandPower |
| 30 | THE_GAMBIT | 0费, 获得 50(75)格挡, 附加 TheGambitPower |
| 31 | PROLONG | 0费, Exhaust→升级移除, 将当前格挡值转为 BlockNextTurnPower |
| 32 | THINKING_AHEAD | 0费, Exhaust→升级移除, 抽2(2)张, 1张放回牌堆顶 |
| 33 | SCRAWL | 1费, Exhaust, 抽到满手(10), 升级 Retain |
| 34 | RESLESSNESS | 0费, Retain, 如果唯一手牌: 抽2(3)张+获得2(3)能量 |
| 35 | PURITY | 0费, Retain+Exhaust, 消耗3(5)张手牌 |
| 36 | ETERNAL_ARMOR | 3费, Power, ApplyPower(PlatingPower, 9→12) |
| 37 | AUTOMATION | 1费→0, Power, ApplyPower(AutomationPower, 1) |
| 38 | MAYHEM | 2费→1, Power, ApplyPower(MayhemPower, 1) |
| 39 | NOSTALGIA | 1费→0, Power, ApplyPower(NostalgiaPower, 1) |
| 40 | STRATAGEM | 1费→0, Power, ApplyPower(StratagemPower, 1) |
| 41 | PANACHE | 0费, Power, ApplyPower(PanachePower, 10→14) |
| 42 | PREP_TIME | 1费, Power, ApplyPower(PrepTimePower, 4→6) |
| 43 | FASTEN | 1费, Power, ApplyPower(FastenPower, 4→6) |
| 44 | ROLLING_BOULDER | 3费, Power, ApplyPower(RollingBoulderPower, 5→10) |
| 45 | EQUILIBRIUM | 2费, GainBlock(13→16), ApplyPower(RetainHandPower, 1) |
| 46 | CALAMITY | 3费→2, Power, ApplyPower(CalamityPower, 1) |
| 47 | ENTROPY | 1费, Power, ApplyPower(EntropyPower, 1), 升级 Innate |

### Group C: 需要特殊 Effect
| # | Id | 说明 |
|---|-----|------|
| 48 | JACK_OF_ALL_TRADES | 0费, Exhaust, 从无色池随机抽1(2)张 |
| 49 | DISCOVERY | 1费, Exhaust→升级移除, 选1张牌获得(0费本回合) |
| 50 | SPLASH | 1费, 从其他角色池中选1张攻击牌获得(0费本回合) |
| 51 | JACKPOT | 3费, 25(30)伤害, 获得3张0费牌 |
| 52 | ALCHEMIZE | 1费→0, Exhaust, 随机获得1瓶药水 |
| 53 | ANOINTED | 1费, Exhaust, 抽稀有牌到满手, 升级 Retain |
| 54 | SECRET_TECHNIQUE | 0费, Exhaust→升级移除, 从抽牌堆选1张 Skill 入手 |
| 55 | SECRET_WEAPON | 0费, Exhaust→升级移除, 从抽牌堆选1张 Attack 入手 |
| 56 | DARK_SHACKLES | 0费, Exhaust, 减目标 9(15)力量至回合末 |
| 57 | BEACON_OF_HOPE | 1费(MP), Power, ApplyPower(BeaconOfHopePower, 1), 升级 Innate |
| 58 | HAND_OF_GREED | 2费, 20(25)伤害, 击杀得 20(25)金币 |
| 59 | HIDDEN_GEM | 1费(不可战斗中生成), 给抽牌堆一张牌附加 Replay(2→3) |
| 60 | MIMIC | 1费(MP), Exhaust→升级移除, 获得等于目标格挡的格挡 |
| 61 | BEAT_DOWN | 3费, 从弃牌堆自动打出 3(4)张攻击牌 |
| 62 | CATASTROPHE | 2费, 自动打出牌堆顶 2(3)张牌 |
| 63 | THE_BOMB | 2费, Power, 3回合后对所有敌人造成 40(50)伤害 |
| 64 | DEMONIC_SHIELD | 0费, Exhaust→升级移除(MP), SelfDamage(1)+格挡=自己格挡 |

---

## 实现计划

1. 先实现 Group A（21 张简单卡）
2. 再实现 Group B（27 张需要计算/条件的卡）
3. 最后 Group C（16 张需要特殊 Effect 的卡）

Group A 全部可以使用现有 Effect 系统直接实现。
Group B 大部分使用现有 Effect，少数需要 `CalculatedAmount` 或条件逻辑。
Group C 需要新增特殊 Effect 类。
