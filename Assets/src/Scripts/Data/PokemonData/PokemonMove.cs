using System;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonContestCategory
{
    Coolness,
    Beauty,
    Cuteness,
    Cleverness,
    Toughness,

    Unknown,
}

public enum PokemonMoveCategory
{
    Physical,
    Special,
    Status,
}

[Flags]
public enum PokemonMoveTargetFlags
{
    Self = 1,
    Ally = 1 << 1,
    Foe = 1 << 2,
    // Single if not multiple
    Multiple = 1 << 3,
    // Close if not range
    Range = 1 << 4,
    MakesContact = 1 << 5,
}

public enum PokemonMoveEffectType
{
    // Heal are negative values.
    Damage,
    // Positive or negative.
    Buff,
    Ailment,
    Weather,
    Flinch,
    Charging,
    Reloading,
    Field,
}

public class PokemonMove
{
    public string Name { get; set; }

    // ToDo: Add localization.
    //public string Description { get { return Name + "description"; } }

    public PokemonType BaseType { get; set; }

    public PokemonType CurrentType
    {
        get
        {
            // ToDo: handle type changing moves.
            return BaseType;
        }
    }

    public PokemonMoveCategory BattleCategory { get; set; }

    /// <summary>
    /// -1 is used for unmissable moves.
    /// </summary>
    public int Accuracy { get; set; }

    public List<PokemonMoveEffect> Effects { get; set; }

    public int BasePP { get; set; }

    public int MaxPP { get { return Mathf.FloorToInt(BasePP * 1.6f); } }
}

public class OwnedPokemonMove
{
    public PokemonMove Move { get; set; }

    public int CurrentPP { get; set; }

    public int CurrentPPUps { get; set; }

    public int CurrentMaxPP
    {
        get
        {
            float maxPP = Mathf.Clamp(Move.BasePP * (1 + CurrentPPUps * 0.2f), Move.BasePP, Move.MaxPP);

            return Mathf.FloorToInt(maxPP);
        }
    }

    public OwnedPokemonMove() { }

    public OwnedPokemonMove(PokemonMove pMove)
    {
        Move = pMove;
        CurrentPP = Move.BasePP;
    }

    public void ResetPP()
    {
        CurrentPP = CurrentMaxPP;
    }
}

/// <summary>
/// Note that damages are considered as effect.
/// </summary>
public class PokemonMoveEffect
{
    public PokemonMoveEffectType Type { get; set; }

    public PokemonMoveTargetFlags Target { get; set; }

    /// <summary>
    /// </summary>
    /// <example>
    /// -The damage/heal value 
    /// -The status applied
    /// -The weather
    /// -Preparation turn
    /// -Cooldown turn
    /// ...
    /// </example>
    public string Args { get; set; }
}
