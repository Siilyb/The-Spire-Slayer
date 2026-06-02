# STS2 遗物完整规范

> 来源：SharedRelicPool.cs（118 种共享遗物）
> 按战斗影响优先级分组

---

## P0: 直接影响战斗（~30 种）

| Id | Name | 效果 | 模拟方式 |
|---|------|------|---------|
| VAJRA | 金刚杵 | +1 力量 | StrengthPower(1) |
| ODD_SMOOTH_STONE | 滑石 | +1 敏捷 | DexterityPower(1) |
| DU_VU_DOLL | 杜瓦娃娃 | 每诅咒+1力量 | StrengthPower(每诅咒数) |
| RED_SKULL | 红骷髅 | HP≤50%时+3力量 | ConditionalStrength(3) |
| GIRYA | 杠铃 | +1力量(可升级) | StrengthPower(1) |
| STRIKE_DUMMY | 打击木桩 | Strike+3伤害 | DamageBuff("Strike",3) |
| BURNING_BLOOD | 燃烧之血 | 战后回6HP | 非战斗效果 |
| ORICHALCUM | 山铜 | 无格挡时得6 | EndOfTurnBlock(6) |
| THREAD_AND_NEEDLE | 针与线 | 开局+4层甲 | PlatingPower(4) |
| BAG_OF_PREPARATION | 备战袋 | 首回合+2抽 | FirstTurnDraw(2) |
| ANCIENT_TEA_SET | 古茶具 | 首回合+2能量 | FirstTurnEnergy(2) |
| HAPPY_FLOWER | 欢乐之花 | 3回合+1能量 | PeriodicEnergy(3,1) |
| CENTENNIAL_PUZZLE | 百年谜题 | 每回合首次受伤抽1 | HurtDraw |
| TUNGSTEN_ROD | 钨棒 | 受伤-1 | DamageReduction(1) |
| TOOTH_BRUSH | 牙刷 | 每回合+1敏捷? | PerTurnBuff |
| BOOT_SEQUENCE | 自检程序 | 开局+10格挡 | TempBlock(10) |
| CAPTAIN_WHEEL | 船长轮 | HP≤50%+3敏捷 | ConditionalDex(3) |
| SHURIKEN | 手里剑 | 每3张攻击+1力量 | PerNAttacks(3,Strength) |
| KUNAL | 苦无 | 每3张攻击+1敏捷 | PerNAttacks(3,Dexterity) |
| POCKETWATCH | 怀表 | ≤3张牌时抽牌保留 | HandSizeBonus |
| NUNCHAKU | 双节棍 | 每2张攻击+1能量 | PerNAttacks(2,Energy) |

## P1: 间接影响战斗（~30 种）

药水相关、遗物触发、回合开始/结束效果等...

## P2: 非战斗（~60 种）

地图、商店、休息处、金币、事件等...
