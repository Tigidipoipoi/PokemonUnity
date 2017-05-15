public enum PokemonType
{
    // Used for struggle
    None,
    Normal,
    Fighting,
    Flying,
    Poison,
    Ground,
    Rock,
    Bug,
    Ghost,
    Steel,
    Fire,
    Water,
    Grass,
    Electric,
    Psychic,
    Ice,
    Dragon,
    Dark,
    Fairy,
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
