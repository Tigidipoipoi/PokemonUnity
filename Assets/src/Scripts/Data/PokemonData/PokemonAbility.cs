using System;

[Flags]
public enum PokemonAbilityType
{
    None = 0,
    StatsUp = 1 << 0,
    //xxx = 1 << 1,
    //xxx = 1 << 2,
    //xxx = 1 << 3,
    //xxx = 1 << 4,
    //xxx = 1 << 5,
    //xxx = 1 << 6,
    //xxx = 1 << 7,
    //xxx = 1 << 8,
    //xxx = 1 << 9,
    //xxx = 1 << 10,
    //xxx = 1 << 11,
    //xxx = 1 << 12,
    //xxx = 1 << 13,
    //xxx = 1 << 14,
    //xxx = 1 << 15,
    //xxx = 1 << 16,
}

public class PokemonAbility
{
    public string Name { get; set; }

    public string Description { get; set; }

    public PokemonAbilityEffect EffectDuringBattle { get; set; }

    public PokemonAbilityEffect EffectOutOfBattle { get; set; }
}

public class PokemonAbilityEffect
{
    public PokemonAbilityType Type { get; set; }

    public Condition Condition { get; set; }
}
