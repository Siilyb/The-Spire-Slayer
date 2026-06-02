# 杀戮尖塔 2 敌人完整列表

> 来源：游戏反编译源码 `src/Core/Models/Monsters/` + `src/Core/Models/Encounters/`
> 说明：之前写的 JawWorm / RedLouse / BlueSlimeSmall AI **是我凭 STS1 记忆写的占位**，未经游戏源码验证。真实 AI 行为需要通过阅读源码中的 `GenerateMoveStateMachine()` 方法逐敌人提取。

---

## 一、敌人 AI 系统架构（游戏实际结构）

游戏使用 **状态机** 管理敌人行为，而非简单的 if/else。

### 状态机组件

| 组件 | 作用 |
|------|------|
| `MonsterMoveStateMachine` | 状态机驱动器，持有所有状态节点 |
| `MoveState` | **叶子节点**，表示一个实际动作（带 Intent + 数值回调） |
| `ConditionalBranchState` | **分支节点**，根据布尔条件选择下一状态 |
| `RandomBranchState` | **分支节点**，加权随机选择（有冷却/重复限制） |
| `FollowUpState` | 每个 `MoveState` 的链式指针，指向下一个状态 |
| `MoveRepeatType` | 重复策略：`CanRepeatForever` / `CannotRepeat` / `UseOnlyOnce` |
| `MustPerformOnceBeforeTransitioning` | 强制至少执行一次再转移 |

### Intent 类型（19 种）

```
Attack          攻击
    ├ SingleAttack    单段攻击
    ├ MultiAttack     多段攻击
    └ DeathBlow       致命攻击
Buff            给自己上增益
Debuff          给玩家上减益
DebuffStrong    强力减益
Defend          格挡
Escape          逃跑
Heal            治疗
Hidden          隐藏意图
Sleep           睡眠
Stun            眩晕
StatusCard      往牌堆塞状态牌
CardDebuff      减益手牌
Summon          召唤爪牙
Unknown         未知
```

---

## 二、完整敌人列表（按类别分组，121 个敌人）

### 第一幕敌人（Starter Act — 根据 Encounter 命名推测）

#### 普通敌人

| 敌人 | 英文 ID | 家族 |
|------|---------|------|
| 大嘴花 | Chomper | Chomper |
| 史莱姆(大) | LeafSlimeM / TwigSlimeM | Slimes |
| 史莱姆(小) | LeafSlimeS / TwigSlimeS | Slimes |
| 虫蛋 | BowlbugEgg | Workers |
| 虫蜜 | BowlbugNectar | Workers |
| 虫石 | BowlbugRock | Workers |
| 虫丝 | BowlbugSilk | Workers |
| 甲虫 | SlumberingBeetle | Workers |
| 真菌 | Flyconid | Mushroom |
| 蛇果 | SnappingJaxfruit | Mushroom |
| 夜袭 | Nibbit | Nibbit |
| 藤蔓 | VineShambler | — |
| 活藤 | TheLost | — |
| 遗忘者 | TheForgotten | — |
| 雾霾 | Fogmog | — |
| 陆行鲨 | Mawler | — |
| 寄居蟹 | SewerClam | — |
| 眼珠 | EyeWithTeeth | — |
| 虫子 | Wriggler | — |
| 蟾蜍 | SpinyToad | — |
| 甲壳虫 | ShrinkerBeetle | Shrinker |
| 软泥 | SludgeSpinner | — |
| 史莱姆狂战士 | SlimedBerserker | — |
| 水蛭 | CorpseSlug | Slugs |
| 潮虫 | Ovicopter | — |
| 刺猬 | Exoskeleton | Exoskeletons |
| 枯木 | FossilStalker | — |
| 飞蛾 | Ovicopter? | — |
| 蛞蝓 | CorpseSlug | Slugs |
| 螳螂 | TheObscura | — |
| 猎手 | HunterKiller | — |
| 海朋克 | Seapunk | Seapunk |
| 咬卷 | ScrollOfBiting | Scrolls |
| 甲壳 | ToughEgg | — |
| 船员 | PaelsLegion | — |
| 夜惊 | Parafright | — |
| 刺客 | Inklet | — |
| 鬼船 | HauntedShip | — |
| 灯泡 | GlobeHead | — |
| 猫头鹰法官 | OwlMagistrate | — |
| 蟹 | WaterfallGiant(Boss) | — |
| 双尾鼠 | TwoTailedRat | — |
| 石匠 | DevotedSculptor | — |
| 掘洞 | Tunneler | Burrower |
| 海鳗 | TerrorEel (Elite) | — |
| 水母 | LivingFog | — |
| 葡萄球虫 | LouseProgenitor | — |
| 半鱼人 | FrogKnight | Knights |
| 被遗忘者 | TheForgotten | — |
| 蚊子 | Myte | — |
| 苔藓 | FuzzyWurmCrawler | Crawler |
| 潜行偷 | ThievingHopper | Thieves |
| 刺虫 | SlitheringStrangler | — |
| 吸血鬼 | Seapunk | Seapunk |
| 石像 | DevotedSculptor | — |
| 地精 | GremlinMerc | Gremlins |
| 胖地精 | FatGremlin | Gremlins |
| 鬼祟地精 | SneakyGremlin | Gremlins |

#### 精英敌人

| 敌人 | 英文 ID | 说明 |
|------|---------|------|
| 千足虫后 | Decimillipede (4 segments) | 多段身体，可重生 |
| 机械骑士 | MechaKnight | 重甲骑士 |
| 灵卫 | SoulNexus | 灵魂系 |
| 寄生蛙 | PhrogParasite | — |
| 虚灵园丁 | PhantasmalGardener | — |
| 虫巫师 | Entomancer | — |
| 掠食者 | SkulkingColony | — |
| 远古雕像 | BygoneEffigy | — |
| 窃贼鸟 | Byrdonis + Byrdpip | — |
| 棱镜 | InfestedPrism | — |
| 骑士团 | KnightsElite (4 骑士) | Flail/Magi/Spectral/Frog Knight |

#### BOSS

| 敌人 | 英文 ID | 说明 |
|------|---------|------|
| 女王 | Queen | — |
| K 博士 | KnowledgeDemon | — |
| 建筑家 | Architect | — |
| 噬神者 | TheInsatiable | — |
| 拉伽沃琳女族长 | LagavulinMatriarch | — |
| 邪恶野兽 | CeremonialBeast | — |
| 玻璃龙 | Aeonglass | — |
| 虚空 | Vantom | — |
| 瀑布巨人 | WaterfallGiant | — |
| 祭品 | TestSubject | — |
| 灵鱼 | SoulFysh | — |
| 血亲 | KinPriest + KinFollower | 带爪牙的 BOSS |

---

### 构造体（Constructs — 多幕跨场景）

| 敌人 | 英文 ID | 说明 |
|------|---------|------|
| 斧头机器人 | Axebot | — |
| 毒气弹 | GasBomb | 带自爆？ |
| 护卫机器人 | Guardbot | — |
| 噪音机器人 | Noisebot | — |
| 拳击手 | PunchConstruct | — |
| 火箭 | Rocket | — |
| 刺刀 | Stabbot | — |
| 充电宝 | Zapbot | 固定循环 |
| 工匠 | Fabricator | — |
| 立方体 | CubexConstruct | — |

### 邪教徒（Cultists）

| 敌人 | 英文 ID | 说明 |
|------|---------|------|
| 钙化邪教徒 | CalcifiedCultist | — |
| 潮湿邪教徒 | DampCultist | — |

### 红宝石掠夺者（Ruby Raiders — 同族多兵种）

| 敌人 | 英文 ID | 说明 |
|------|---------|------|
| 刺客 | AssassinRubyRaider | — |
| 斧兵 | AxeRubyRaider | — |
| 野蛮人 | BruteRubyRaider | — |
| 弩手 | CrossbowRubyRaider | — |
| 追踪者 | TrackerRubyRaider | — |

---

## 三、遭遇（Encounter）分组

### 按难度

| 分类 | 命名规则 | 示例 |
|------|---------|------|
| **普通** | `*Normal` | SlimesNormal, ChompersNormal |
| **较弱** | `*Weak` | SlimesWeak, CorpseSlugsWeak |
| **精英** | `*Elite` | KnightsElite, MechaKnightElite |
| **BOSS** | `*Boss` | QueenBoss, KnowledgeDemonBoss |
| **事件** | `*EventEncounter` | FakeMerchantEventEncounter |

### 第 1 幕已知遭遇（通过 Normal/Weak 文件名推测）

| 遭遇 | 敌人组成 |
|------|---------|
| ChompersNormal | 大嘴花 ×2 |
| BowlbugsNormal | Bowlbug ×2 |
| BowlbugsWeak | Bowlbug ×1（较弱） |
| SlimesNormal | 史莱姆 ×2 |
| SlimesWeak | 史莱姆 ×1（较弱） |
| CorpseSlugsNormal | 蛞蝓 ×2 |
| CorpseSlugsWeak | 蛞蝓 ×1 |
| NibbitsNormal | 夜袭 ×2 |
| NibbitsWeak | 夜袭 ×1 |
| CultistsNormal | 邪教徒 ×2 |
| ExoskeletonsNormal | 刺猬 ×2 |
| ExoskeletonsWeak | 刺猬 ×1 |
| FrogKnightNormal | 半鱼人骑士 + ? |
| MytesNormal | 蚊子群 |
| LouseProgenitorNormal | 葡萄球虫 |
| GremlinMercNormal | 地精细 ×2 |
| SeapunkNormal | 海朋克 |
| SeapunkWeak | 海朋克（较弱） |
| ScrollsOfBitingNormal | 咬卷 |
| ScrollsOfBitingWeak | 咬卷（较弱） |
| TwoTailedRatsNormal | 双尾鼠 |
| ThievingHopperWeak | 偷窃虫（较弱） |

---

## 四、敌人行为状态机模式总结

从源码分析，敌人行为主要有以下几种模式：

### 模式 1：固定循环（如 Zapbot）
```
MoveA → MoveA → MoveA → ...（自指 FollowUpState，永远同一招）
```

### 模式 2：交替循环（如 Wriggler）
```
MoveA → MoveB → MoveA → MoveB → ...（链式 FollowUpState）
```

### 模式 3：多段循环（如 Decimillipede）
```
MoveA → MoveB → MoveC → MoveA → ...（3+ 链，含 ConditionalBranch）
```

### 模式 4：随机选择（如 Decimillipede 重生后）
```
RandomBranch
├── MoveA (weight, cooldown)
├── MoveB (weight, cooldown)
└── MoveC (weight, cooldown)
```

### 模式 5：条件分支（如 Wriggler 初始动作）
```
ConditionalBranch
├── slot == "wriggler1" → MoveA
├── slot == "wriggler2" → MoveB
└── else → MoveC
```

### 模式 6：多阶段 BOSS（如 WaterfallGiant）
```
Phase 1: MoveA → MoveB → MoveC → ...
  ↓ (HP 阈值触发外部 ForceCurrentState)
Phase 2: MoveD → MoveE → ...
  ↓
Phase 3: DeathBlow
```

---

## 五、当前模拟器 AI vs 真实 AI 差距

| 方面 | 当前模拟器（占位） | 游戏真实实现 |
|------|-------------------|-------------|
| **AI 系统** | 每回合 `PlanNextTurn()` 硬编码 | 状态机（MoveState + BranchState） |
| **Intent 数值** | 凭 STS1 记忆硬编码 | 从 `CanonicalVars` 或回调函数计算 |
| **随机分支** | 无 | 加权随机 + 冷却计时 |
| **条件分支** | 无 | 基于 `SlotName`、HP 阈值等 |
| **多段身体** | 不支持 | Decimillipede 等分身前中后 |
| **召唤爪牙** | 无 | SummonIntent |
| **眩晕/睡眠** | 无 | StunIntent / SleepIntent |
| **多阶段 BOSS** | 无 | HP 阈值触发 phase 切换 |
| **卡牌减益** | 无 | CardDebuffIntent |
| **逃跑** | 无 | EscapeIntent |

**要精确模拟敌人，需要从每个 `MonsterModel` 的 `GenerateMoveStateMachine()` 源码中提取状态机，转换为模拟器的 `IEnemyAi` 实现。**
