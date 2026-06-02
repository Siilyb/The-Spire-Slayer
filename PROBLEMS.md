# 问题记录

> 记录开发过程中遇到的所有问题、解决方案和尚未解决的问题。

| 日期 | 问题 | 状态 | 解决方案 |
|------|------|------|---------|
| 2026-06-02 | Hook 返回值约定错误（Additive 返回总伤害而非加值） | 已修复 | 改为返回加值，引擎累加 |
| 2026-06-02 | 缺失 ModifyDamageCap / ModifyHpLostAfterOsty 钩子 | 已修复 | 新增虚方法 |
| 2026-06-02 | 大部分 Power 条件判断/触发时机有偏差 | 已修复 | 逐条对照游戏源码重写 |
| 2026-06-02 | 87 张 Ironclad 卡全部实现（Basic/Common/Uncommon/Rare/Ancient） | ✅ 已完成 | 1586 行卡牌定义 |
| 2026-06-02 | All 87 Ironclad cards compile and pass basic tests. 31 + 23 tests | ✅ 已完成 | New unique effects added for complex cards |

**仍需实现（后续 Sprint）：**
- 无色卡 64 张
- 诅咒卡 18 张
- 状态卡 12 张
- Event 卡 27 张
- Token 卡 14 张
- Quest 卡 3 张
- 怪物 AI ~121 个
- MCTS 战斗 AI
- 抓牌/路线 AI
- Mod 集成

**部分卡牌为简化实现（需后续精确化）：**
- Spite（条件多段依赖本回合受伤历史，暂用固定 1 段）
- PrimalForce（变换攻击牌为 GiantRock — 需要 CardFactory 机制）
- Stoke（消耗全手并生成随机牌 — 需要 CardFactory）
- Feed（击杀+MaxHP 依赖 Fatal 判断）
- FiendFire（总伤害 = 手牌数×单发，已实现但需验证）
- Thrash（永久增加伤害需要持久化追踪）
- Rampage（跨战斗追踪已用 static 字典）
- Brand（手牌选择消耗 — 简化版）
- BurningPact（手牌选择 — 简化版）
