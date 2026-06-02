# Mod 集成计划

## 目标

将 MCTS AI 接入真实游戏，替换 AutoSlay 的随机行为。

## 架构

```
sts2_ai_mod/                 # Mod 项目 (net9.0)
├── Sts2AiMod.cs             # Mod 入口 [ModInitializer] + Harmony 补丁
├── StateConverter.cs         # 游戏 CombatState → SimState 转换
├── CardMapper.cs             # 游戏 CardModel ID → SimCard ID 映射
├── mod.json                  # Mod 清单文件
└── sts2_ai_mod.csproj       # 项目文件（引用 sts2.dll + 0Harmony.dll + sts2_ai）

部署到: C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2_ai\
├── mod.json
├── sts2_ai_mod.dll           # Mod DLL
└── sts2_ai.dll               # 我们的 AI 库
```

## 实施步骤

### Step 1: 创建 sts2_ai_mod 项目
- .NET 9.0 类库
- 引用 `data_sts2_windows_x86_64/sts2.dll` + `0Harmony.dll` + `GodotSharp.dll`
- 引用 sts2_ai 项目

### Step 2: StateConverter
将游戏 `CombatState` → `SimState`：
- 遍历手牌 → 查找 CardDb → 构建 SimCard 列表
- 复制玩家 HP/Block/能量/Powers
- 复制敌人 HP/Block/Powers
- 复制抽牌堆/弃牌堆

### Step 3: Sts2AiMod
- [ModInitializer] → Harmony Patch
- Patch `AutoSlay.CombatRoomHandler.HandleAsync`
- 当战斗开始时，用 MCTS 替代随机出牌

### Step 4: 部署
- 编译 mod + 复制 sts2_ai.dll
- 创建 mod.json
- 放入游戏 mods 目录

## 挑战
1. CardModel → SimCard 映射（需要知道每张卡的 ID 对应关系）
2. PowerModel → SimPower 映射（游戏 Power 类 vs 我们的 SimPower 类）
3. Harmony 补丁的正确性
4. 游戏必须在 `--autoslay` 模式下启动才能启用 AI
