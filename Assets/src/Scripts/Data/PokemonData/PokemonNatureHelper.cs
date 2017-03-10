using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Flavor
{
    Spicy,
    Sour,
    Sweet,
    Dry,
    Bitter,

    None
}

public enum PokemonNature
{
    Hardy,
    Lonely,
    Brave,
    Adamant,
    Naughty,
    Bold,
    Docile,
    Relaxed,
    Impish,
    Lax,
    Timid,
    Hasty,
    Serious,
    Jolly,
    Naive,
    Modest,
    Mild,
    Quiet,
    Bashful,
    Rash,
    Calm,
    Gentle,
    Sassy,
    Careful,
    Quirky
}

public static class PokemonNatureHelper
{
    /// <summary>
    /// DO NOT REORDER !
    /// See http://bulbapedia.bulbagarden.net/wiki/Nature
    /// </summary>
    private static readonly string[] POKEMON_NATURE_NAMES = new string[25]
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

    public static string[] GetAllNatureNames()
    {
        string[] allNatureNamesCopy = new string[POKEMON_NATURE_NAMES.Length];
        for (int i = POKEMON_NATURE_NAMES.Length - 1; i != 0; --i)
        {
            allNatureNamesCopy[i] = POKEMON_NATURE_NAMES[i];
        }

        return allNatureNamesCopy;
    }

    public static float GetStatFactorFromNature(PokemonStatType pStatType, PokemonNature pNature)
    {
        float factor = 1.0f;

        int natureToInt = (int)pNature;
        if (natureToInt % 6 == 0)
            return factor;

        switch (pStatType)
        {
            case PokemonStatType.Attack:
                if (Mathf.FloorToInt(natureToInt / 5) == 0)
                    factor = 1.1f;
                else if (natureToInt % 5 == 0)
                    factor = 0.9f;
                break;
            case PokemonStatType.Defence:
                if (Mathf.FloorToInt(natureToInt / 5) == 1)
                    factor = 1.1f;
                else if (natureToInt % 5 == 1)
                    factor = 0.9f;
                break;
            case PokemonStatType.Speed:
                if (Mathf.FloorToInt(natureToInt / 5) == 2)
                    factor = 1.1f;
                else if (natureToInt % 5 == 2)
                    factor = 0.9f;
                break;
            case PokemonStatType.SpecialAttack:
                if (Mathf.FloorToInt(natureToInt / 5) == 3)
                    factor = 1.1f;
                else if (natureToInt % 5 == 3)
                    factor = 0.9f;
                break;
            case PokemonStatType.SpecialDefence:
                if (Mathf.FloorToInt(natureToInt / 5) == 4)
                    factor = 1.1f;
                else if (natureToInt % 5 == 4)
                    factor = 0.9f;
                break;
        }

        return factor;
    }

    public static PokemonStatType GetUpgradedStat(PokemonNature pNature)
    {
        int natureToInt = (int)pNature;

        return GetUpgradedStat(natureToInt);
    }
    public static PokemonStatType GetUpgradedStat(int pNature)
    {
        if (pNature % 6 == 0)
            return PokemonStatType.None;

        int onFive = Mathf.FloorToInt(pNature / 5);

        switch (onFive)
        {
            case 0:
                return PokemonStatType.Attack;
            case 1:
                return PokemonStatType.Defence;
            case 2:
                return PokemonStatType.Speed;
            case 3:
                return PokemonStatType.SpecialAttack;
            case 4:
                return PokemonStatType.SpecialDefence;
        }

        return PokemonStatType.None;
    }

    public static PokemonStatType GetDowngradedStat(PokemonNature pNature)
    {
        int natureToInt = (int)pNature;

        return GetDowngradedStat(natureToInt);
    }
    public static PokemonStatType GetDowngradedStat(int pNature)
    {
        if (pNature % 6 == 0)
            return PokemonStatType.None;

        int modFive = pNature % 5;

        switch (modFive)
        {
            case 0:
                return PokemonStatType.Attack;
            case 1:
                return PokemonStatType.Defence;
            case 2:
                return PokemonStatType.Speed;
            case 3:
                return PokemonStatType.SpecialAttack;
            case 4:
                return PokemonStatType.SpecialDefence;
        }

        return PokemonStatType.None;
    }

    public static string GetNatureName(int pNature)
    {
        if (pNature < 0
            || pNature > 24)
            throw new Exception("Invalid nature providen.");

        return POKEMON_NATURE_NAMES[pNature];
    }
    public static string GetNatureName(PokemonNature pNature)
    {
        int natureToInt = (int)pNature;
        if (natureToInt < 0
            || natureToInt > 24)
            throw new Exception("Invalid nature providen.");

        return POKEMON_NATURE_NAMES[natureToInt];
    }

    public static Flavor GetFavoriteFlavor(PokemonNature pNature)
    {
        int natureToInt = (int)pNature;
        if (natureToInt % 6 == 0)
            return Flavor.None;

        int onFive = Mathf.FloorToInt(natureToInt / 5);

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
        int natureToInt = (int)pNature;
        if (natureToInt % 6 == 0)
            return Flavor.None;

        int modFive = natureToInt % 5;

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

    public static PokemonNature GetRandomNature()
    {
        return (PokemonNature)Random.Range(0, POKEMON_NATURE_NAMES.Length);
    }
}
