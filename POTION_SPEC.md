# STS2 药水完整规范

> 来源：SharedPotionPool.cs（45 种共享药水）+ 各药水源码

---

## 使用现有 Effect 可直接实现的（30 种）

| # | Id | Rarity | 效果 | 模拟器实现 |
|---|-----|--------|------|-----------|
| 1 | BLOCK_POTION | Common | 获得 12 格挡 | GainBlock(12, Move) |
| 2 | DEXTERITY_POTION | Common | +2 敏捷 | ApplyPower(Dexterity, 2) |
| 3 | ENERGY_POTION | Common | 获得 2 能量 | GainEnergy(2) |
| 4 | EXPLOSIVE_AMPOULE | Common | 全体 10 伤害(Unpowered) | DealDamage(10, AOE, Unpowered) |
| 5 | FIRE_POTION | Common | 单体 20 伤害(Unpowered) | DealDamage(20, single, Unpowered) |
| 6 | STRENGTH_POTION | Common | +2 力量 | ApplyPower(Strength, 2) |
| 7 | SWIFT_POTION | Common | 抽 3 张 | DrawCards(3) |
| 8 | VULNERABLE_POTION | Common | 单体 3 易伤 | ApplyPower(Vulnerable, 3, single) |
| 9 | WEAK_POTION | Common | 单体 3 虚弱 | ApplyPower(Weak, 3, single) |
| 10 | REGEN_POTION | Uncommon | +5 回春 | ApplyPower(RegenPower, 5) |
| 11 | HEART_OF_IRON | Uncommon | +7 层甲 | ApplyPower(PlatingPower, 7) |
| 12 | LIQUID_BRONZE | Uncommon | +3 荆棘 | ApplyPower(ThornsPower, 3) |
| 13 | LUCKY_TONIC | Rare | +1 缓冲 | ApplyPower(BufferPower, 1) |
| 14 | CURE_ALL | Uncommon | +1 能量 + 抽 2 | GainEnergy(1) + DrawCards(2) |
| 15 | FYSH_OIL | Uncommon | +1 力量 +1 敏捷 | ApplyPower(Strength,1) + ApplyPower(Dexterity,1) |
| 16 | POTION_OF_BINDING | Uncommon | 全体 1 虚弱 + 1 易伤 | ApplyPower(Weak,1,AOE) + ApplyPower(Vulnerable,1,AOE) |
| 17 | FRUIT_JUICE | Rare | +5 最大 HP | GainMaxHp(5) |
| 18 | FAIRY_IN_A_BOTTLE | Rare | 30% 最大 HP 治疗 | Heal(30%) |
| 19 | DUPLICATOR | Uncommon | 下张牌复制一次 | ApplyPower(Placeholder,1) |
| 20 | SHIP_IN_A_BOTTLE | Rare | 10 格挡 + 下回合 10 格挡 | GainBlock(10) |
| 21 | STABLE_SERUM | Uncommon | 保留手牌 2 回合 | ApplyPower(Placeholder,2) |
| 22 | MAZALETHS_GIFT | Rare | +1 仪式(每回合+力量) | ApplyPower(Placeholder,1) |
| 23 | FORTIFIER | Uncommon | 格挡翻倍 | GainBlock(当前格挡×2) |
| 24 | BLESSING_OF_THE_FORGE | Uncommon | 升级所有手牌 | UpgradeAllInHand |
| 25 | DROPLET_OF_PRECOGNITION | Rare | 从抽牌堆选 1 张到手牌 | SecretTechniqueEffect(any) |
| 26 | LIQUID_MEMORIES | Rare | 从弃牌堆选 1 张, 本回合免费 | SecretTechniqueEffect(any from discard) |
| 27 | CLARITY | Uncommon | 抽 1 + ClarityPower 3 | DrawCards(1) |
| 28 | RADIANT_TINCTURE | Uncommon | 1 能量 + RadiancePower 3 | GainEnergy(1) |
| 29 | BEETLE_JUICE | Rare | ShrinkPower 4 | ApplyPower(Placeholder,4) |
| 30 | GIGANTIFICATION_POTION | Rare | GigantificationPower | ApplyPower(Placeholder,1) |

## 需要特殊处理的（15 种）

| # | Id | 说明 |
|---|-----|------|
| 31 | SPEED_POTION | 临时 5 敏捷(回合末消失) |
| 32 | FLEX_POTION | 临时 5 力量(回合末消失) |
| 33 | SHACKLING_POTION | 全体 7 临时减力量 |
| 34 | POWDERED_DEMISE | 击杀时造成 9 伤害 |
| 35 | SNECKO_OIL | 抽 7 张, 随机化费用(0-3) |
| 36 | DISTILLED_CHAOS | 自动打出牌堆顶 3 张 |
| 37 | GAMBLERS_BREW | 弃全部手牌, 抽等量 |
| 38 | TOUCH_OF_INSANITY | 选 1 张手牌本战斗免费 |
| 39 | BOTTLED_POTENTIAL | 手牌放回抽牌堆, 洗牌, 抽 5 |
| 40 | ENTROPIC_BREW | 填满药水栏位(非战斗) |
| 41 | ATTACK_POTION | 从 3 张攻击牌中选 1 张获得 |
| 42 | SKILL_POTION | 从 3 张技能牌中选 1 张获得 |
| 43 | POWER_POTION | 从 3 张能力牌中选 1 张获得 |
| 44 | COLORLESS_POTION | 从 3 张无色牌中选 1 张获得 |
| 45 | OROBIC_ACID | 获得 1 攻击+1 技能+1 能力, 免费 |
