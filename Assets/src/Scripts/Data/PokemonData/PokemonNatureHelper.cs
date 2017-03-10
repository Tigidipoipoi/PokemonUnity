using System;
using System.Collections.Generic;
using UnityEngine;

public enum Flavor
{
    Spicy,
    Sour,
    Sweet,
    Dry,
    Bitter,

    None
}

public static class PokemonNatureHelper
{
    /// <summary>
    /// DO NOT REORDER !
    /// See http://bulbapedia.bulbagarden.net/wiki/Nature
    /// </summary>
    private static readonly string[] POKEMON_NATURE = new string[25]
    {
        "Hardy",
        "Lonely",
        "Brave",
        "Adamant",
        "Naughty",
        "Bold",
        "Docile",
        "Relaxed",
        "Impish",
        "Lax",
        "Timid",
        "Hasty",
        "Serious",
        "Jolly",
        "Naive",
        "Modest",
        "Mild",
        "Quiet",
        "Bashful",
        "Rash",
        "Calm",
        "Gentle",
        "Sassy",
        "Careful",
        "Quirky"
    };

    public static float GetStatFactorFromNature(PokemonStatType pStatType, int pNature)
    {
        float factor = 1.0f;

        if (pNature % 6 == 0)
            return factor;

        switch (pStatType)
        {
            case PokemonStatType.Attack:
                if (Mathf.FloorToInt(pNature / 5) == 0)
                    factor = 1.1f;
                else if (pNature % 5 == 0)
                    factor = 0.9f;
                break;
            case PokemonStatType.Defense:
                if (Mathf.FloorToInt(pNature / 5) == 1)
                    factor = 1.1f;
                else if (pNature % 5 == 1)
                    factor = 0.9f;
                break;
            case PokemonStatType.Speed:
                if (Mathf.FloorToInt(pNature / 5) == 2)
                    factor = 1.1f;
                else if (pNature % 5 == 2)
                    factor = 0.9f;
                break;
            case PokemonStatType.SpecialAttack:
                if (Mathf.FloorToInt(pNature / 5) == 3)
                    factor = 1.1f;
                else if (pNature % 5 == 3)
                    factor = 0.9f;
                break;
            case PokemonStatType.SpecialDefense:
                if (Mathf.FloorToInt(pNature / 5) == 4)
                    factor = 1.1f;
                else if (pNature % 5 == 4)
                    factor = 0.9f;
                break;
        }

        return factor;
    }

    public static string GetNatureName(int pNature)
    {
        if (pNature < 0
            || pNature > 24)
            throw new Exception("Invalid nature providen.");

        return POKEMON_NATURE[pNature];
    }

    public static Flavor GetFavoriteFlavor(int pNature)
    {
        if (pNature % 6 == 0)
            return Flavor.None;

        int onFive = Mathf.FloorToInt(pNature / 5);

        switch (onFive)
        {
            case 0:
                return Flavor.Spicy;
            case 1:
                return Flavor.Sour;
            case 2:
                return Flavor.Sweet;
            case 3:
                return Flavor.Dry;
            case 4:
                return Flavor.Bitter;
        }

        return Flavor.None;
    }

    public static Flavor GetDislikedFlavor(int pNature)
    {
        if (pNature % 6 == 0)
            return Flavor.None;

        int modFive = pNature % 5;

        switch (modFive)
        {
            case 0:
                return Flavor.Spicy;
            case 1:
                return Flavor.Sour;
            case 2:
                return Flavor.Sweet;
            case 3:
                return Flavor.Dry;
            case 4:
                return Flavor.Bitter;
        }

        return Flavor.None;
    }
}
