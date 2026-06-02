# Mod 集成计划（最终版）

> Mod 只做**推荐**，不做自动出牌。
> 玩家手动出牌，Mod 在 CMD 窗口中显示 MCTS 的推荐和建议。
> 玩家可以看推荐来决定打哪张牌、打哪个敌人。

---

## 架构

```
游戏进程
├── sts2.dll
├── sts2_ai_mod.dll          # Mod DLL (Harmony 补丁)
│   ├── Sts2AiMod.cs         # Mod 入口 + Harmony 补丁
│   ├── StateConverter.cs    # 游戏 CombatState → SimState
│   ├── CardMapper.cs        # 游戏 CardModel ID → SimCard
│   └── ConsoleDisplay.cs    # CMD 窗口 + 实时显示
└── sts2_ai.dll              # AI 引擎库（被 mod 引用）
```

## Mod 工作流程

```
[玩家进入战斗]
  → CombatState.AfterPlayerTurnStart 事件
    → StateConverter 将游戏状态转为 SimState
    → MCTS 搜索最佳出牌
    → CMD 窗口显示:
        - 玩家/敌人状态
        - 手牌列表
        - AI 推荐: 打击 → 第一只史莱姆
        - AI 推荐: 痛击 → 第二只史莱姆
    → 玩家自行决定是否按推荐打牌

[玩家打出任意牌后]
  → CombatState.AfterCardPlayed 事件
    → 重新转换状态
    → 重新运行 MCTS（更新推荐）
    → CMD 窗口更新显示
```

## 实施步骤

| 步骤 | 内容 | 预计时间 |
|------|------|---------|
| 1 | 创建 mod 项目 + 引用游戏 DLL | 已完成 ✅ |
| 2 | CardMapper（225 张卡 ID 映射） | 已完成 ✅ |
| 3 | StateConverter（CombatState→SimState） | 已完成 ✅ |
| 4 | ConsoleDisplay（AllocConsole + 显示） | 已完成 ✅ |
| 5 | Sts2AiMod（Harmony 补丁 + 事件挂钩） | 已完成 ✅ |
| 6 | 部署到游戏 mods 目录 | 待完成 |
| 7 | 端到端测试 | 待完成 |

## 部署

编译后复制到游戏 mods 目录：
```
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\sts2_ai\
├── mod.json
├── sts2_ai_mod.dll         # Mod 主程序
└── sts2_ai.dll             # AI 引擎库
```

以 `--autoslay` 或 `-w` 模式启动游戏：
```
Slay the Spire 2.exe --autoslay
或
Steam 启动选项: --autoslay
```

游戏启动时會看到弹出的 CMD 窗口，进入战斗后自动显示 MCTS 推荐。
