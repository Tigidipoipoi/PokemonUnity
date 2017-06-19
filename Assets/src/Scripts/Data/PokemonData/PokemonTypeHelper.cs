using System;

/// <summary>
/// Pokemon's types work as flags.
/// </summary>
[Flags]
public enum PokemonType
{
    // Used for struggle
    None        = 0,
    Normal      = 1,
    Fighting    = 2,
    Flying      = 4,
    Poison      = 8,
    Ground      = 16,
    Rock        = 32,
    Bug         = 64,
    Ghost       = 128,
    Steel       = 256,
    Fire        = 512,
    Water       = 1024,
    Grass       = 2048,
    Electric    = 4096,
    Psychic     = 8192,
    Ice         = 16384,
    Dragon      = 32768,
    Dark        = 65536,
    Fairy       = 131072,
};

public static class PokemonTypeHelper
{
    public static string GetName(PokemonType pType)
    {
        switch (pType)
        {
            case PokemonType.None:
                return "None";
            case PokemonType.Normal:
                return "Normal";
            case PokemonType.Fighting:
                return "Fighting";
            case PokemonType.Flying:
                return "Flying";
            case PokemonType.Poison:
                return "Poison";
            case PokemonType.Ground:
                return "Ground";
            case PokemonType.Rock:
                return "Rock";
            case PokemonType.Bug:
                return "Bug";
            case PokemonType.Ghost:
                return "Ghost";
            case PokemonType.Steel:
                return "Steel";
            case PokemonType.Fire:
                return "Fire";
            case PokemonType.Water:
                return "Water";
            case PokemonType.Grass:
                return "Grass";
            case PokemonType.Electric:
                return "Electric";
            case PokemonType.Psychic:
                return "Psychic";
            case PokemonType.Ice:
                return "Ice";
            case PokemonType.Dragon:
                return "Dragon";
            case PokemonType.Dark:
                return "Dark";
            case PokemonType.Fairy:
                return "Fairy";

            default:
                return null;
        }
    }
}
