# 问题记录

> 记录开发过程中遇到的所有问题、解决方案和尚未解决的问题。

| 日期 | 问题 | 状态 | 解决方案 |
|------|------|------|---------|
| 2026-06-02 | Hook 返回值约定错误 | 已修复 | 改为返回加值而非总伤害 |
| 2026-06-02 | 缺失 ModifyDamageCap 等钩子 | 已修复 | 新增虚方法 |
| 2026-06-02 | Power 行为偏差 | 已修复 | 逐条对照游戏源码重写 |
| 2026-06-02 | Ironclad 87 张卡全部实现 | ✅ | 3 文件 ~2000 行 |
| 2026-06-02 | 简化卡牌全部精确化 | ✅ | Spite/Feed/EvilEye/Thrash/PrimalForce/Stoke/TearAsunder/Rampage |
| 2026-06-02 | SimEngine 追踪状态 | ✅ | CardsExhaustedThisTurn / UnblockedDamageReceivedThisTurn / LostHpThisTurn |

**已修复（2026-06-02）：**
| 卡牌 | 修复 |
|------|------|
| HowlFromBeyond | HandleCardDisposition 中检测 HowlFromBeyondEffect → 从消耗堆移除并 ExecuteCard |
| DrumOfBattle | HandleCardDisposition 中检测 "DRUM_OF_BATTLE" → 消耗时获得 2(3) 能量 |
| Stoke 升级版 | StokeEffect 中检测 IsUpgraded → 创造牌也标记升级并增加伤害 |

**后续 Sprint：**
- 无色卡 64 张
- 诅咒卡 18 张
- 状态卡 12 张
- Event/Token/Quest 44 张
- 怪物 AI ~121 个
- MCTS / 抓牌 / 路线 AI / Mod
