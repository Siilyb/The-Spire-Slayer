using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Sts2Ai.CombatSimulator;
using Sts2Ai.CombatSimulator.Powers;
using Sts2Ai.CombatSimulator.State;
using SimCombatSide = Sts2Ai.CombatSimulator.CombatSide;
// Game power types via fully qualified name to avoid conflicts
using GStrength = MegaCrit.Sts2.Core.Models.Powers.StrengthPower;
using GDexterity = MegaCrit.Sts2.Core.Models.Powers.DexterityPower;
using GVulnerable = MegaCrit.Sts2.Core.Models.Powers.VulnerablePower;
using GWeak = MegaCrit.Sts2.Core.Models.Powers.WeakPower;
using GFrail = MegaCrit.Sts2.Core.Models.Powers.FrailPower;
using GPoison = MegaCrit.Sts2.Core.Models.Powers.PoisonPower;
using GThorns = MegaCrit.Sts2.Core.Models.Powers.ThornsPower;
using GArtifact = MegaCrit.Sts2.Core.Models.Powers.ArtifactPower;
using GBuffer = MegaCrit.Sts2.Core.Models.Powers.BufferPower;
using GIntangible = MegaCrit.Sts2.Core.Models.Powers.IntangiblePower;
using GPlating = MegaCrit.Sts2.Core.Models.Powers.PlatingPower;
using GBarricade = MegaCrit.Sts2.Core.Models.Powers.BarricadePower;

namespace Sts2AiMod;

public static class StateConverter
{
    public static SimState Convert(CombatState combatState)
    {
        var state = new SimState();
        state.Rng = new GameRng();

        foreach (var player in combatState.Players)
        {
            var sim = ConvertCreature(player.Creature, SimCombatSide.Player);
            state.Players.Add(sim);
            state.Energy = player.PlayerCombatState.Energy;
            state.MaxEnergy = player.PlayerCombatState.MaxEnergy;
        }

        foreach (var enemy in combatState.Enemies)
        {
            var sim = ConvertCreature(enemy, SimCombatSide.Enemy);
            sim.Name = enemy.Monster?.Id.Entry ?? enemy.Name;
            state.Enemies.Add(sim);
        }

        foreach (var player in combatState.Players)
        {
            var pc = player.PlayerCombatState;
            ConvertPile(pc.Hand.Cards, state.Hand);
            ConvertPile(pc.DrawPile.Cards, state.DrawPile);
            ConvertPile(pc.DiscardPile.Cards, state.DiscardPile);
            ConvertPile(pc.ExhaustPile.Cards, state.ExhaustPile);
        }

        state.TurnNumber = combatState.RoundNumber;
        return state;
    }

    private static SimCreature ConvertCreature(Creature creature, SimCombatSide side)
    {
        var sim = new SimCreature
        {
            Name = creature.Name,
            Side = side,
            CurrentHp = creature.CurrentHp,
            MaxHp = creature.MaxHp,
            Block = creature.Block,
        };

        foreach (var power in creature.Powers)
        {
            var simPower = MapPower(power);
            if (simPower != null)
                sim.ApplyPower(simPower);
        }

        return sim;
    }

    private static SimPower? MapPower(MegaCrit.Sts2.Core.Models.PowerModel gamePower)
    {
        return gamePower switch
        {
            GStrength => new StrengthPower { Amount = gamePower.Amount },
            GDexterity => new DexterityPower { Amount = gamePower.Amount },
            GVulnerable => new VulnerablePower { Amount = gamePower.Amount },
            GWeak => new WeakPower { Amount = gamePower.Amount },
            GFrail => new FrailPower { Amount = gamePower.Amount },
            GPoison => new PoisonPower { Amount = gamePower.Amount },
            GThorns => new ThornsPower { Amount = gamePower.Amount },
            GArtifact => new ArtifactPower { Amount = gamePower.Amount },
            GBuffer => new BufferPower { Amount = gamePower.Amount },
            GIntangible => new IntangiblePower { Amount = gamePower.Amount },
            GPlating => new PlatingPower { Amount = gamePower.Amount },
            GBarricade => new BarricadePower { Amount = gamePower.Amount },
            _ => null // Unknown powers ignored
        };
    }

    private static void ConvertPile(IEnumerable<CardModel> gameCards, SimCardPile targetPile)
    {
        foreach (var gameCard in gameCards)
        {
            var simCard = CardMapper.CreateSimCard(gameCard.Id.Entry);
            if (simCard != null)
            {
                simCard.IsUpgraded = gameCard.IsUpgraded;
                targetPile.Add(simCard);
            }
        }
    }
}
