using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonStatType
{
    HP,
    Attack,
    Defence,
    Speed,
    SpecialAttack,
    SpecialDefence,

    Evasion,
    Accuracy,

    // Default and serves as count to.
    None,
}

// ToDo: Add modifiers (during battle only !); Idea: clear modifiers on end fight event except for HP.
// Decide struct or class
[Serializable]
public class PokemonStat
{
    #region Members
    public PokemonStatType Type;

    private int _baseValue;
    public int BaseValue
    {
        get { return _baseValue; }
        set
        {
            switch (Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                case PokemonStatType.Speed:
                    _baseValue = value;
                    break;

                case PokemonStatType.Evasion:
                case PokemonStatType.Accuracy:
                    if (_baseValue != 100)
                        _baseValue = 100;
                    break;

                case PokemonStatType.None:
                default:
                    break;
            }
        }
    }

    private int _iv;
    /// <summary>
    /// Individual Value.
    /// </summary>
    public int IV
    {
        get { return _iv; }
        set
        {
            switch (Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.Speed:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                    int clampedValue = Mathf.Clamp(value, 0, 31);
                    _iv = clampedValue;
                    break;

                case PokemonStatType.Evasion:
                case PokemonStatType.Accuracy:
                case PokemonStatType.None:
                default:
                    _iv = 0;
                    break;
            }
        }
    }

    private int _ev;
    /// <summary>
    /// Effort Value.
    /// </summary>
    public int EV
    {
        get { return _ev; }
        set
        {
            switch (Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                case PokemonStatType.Speed:
                    int clampedValue = Mathf.Clamp(value, 0, 252);
                    _ev = clampedValue;
                    break;

                case PokemonStatType.Evasion:
                case PokemonStatType.Accuracy:
                case PokemonStatType.None:
                default:
                    if (_ev != 0)
                        _ev = 0;
                    break;
            }
        }
    }
    #endregion

    #region Constructors
    public PokemonStat() : this(PokemonStatType.None, 0) { }

    public PokemonStat(PokemonStatType pStatType, int pBaseValue)
    {
        Type = pStatType;
        BaseValue = pBaseValue;
        IV = 0;
        EV = 0;
    }
    #endregion

    /// <summary>
    /// Gets the stat's value according to the current level and nature.
    /// 
    /// See http://bulbapedia.bulbagarden.net/wiki/Statistic for formula.
    /// </summary>
    /// <param name="pPokemonLevel"></param>
    /// <returns></returns>
    public int GetCurrentLevelValue(int pPokemonLevel, PokemonNature pNature)
    {
        if (BaseValue == 1)
            return 1;

        int currentValue = 1;

        switch (Type)
        {
            case PokemonStatType.HP:
            case PokemonStatType.Attack:
            case PokemonStatType.Defence:
            case PokemonStatType.SpecialAttack:
            case PokemonStatType.SpecialDefence:
            case PokemonStatType.Speed:
                currentValue = Mathf.CeilToInt(Mathf.Sqrt(EV));
                currentValue = Mathf.FloorToInt(currentValue * 0.25f);
                currentValue += (BaseValue + IV) * 2;
                currentValue *= pPokemonLevel;
                currentValue = Mathf.FloorToInt(currentValue * 0.1f);
                currentValue += 5;

                if (Type == PokemonStatType.HP)
                    currentValue += pPokemonLevel + 5;
                else
                    currentValue = Mathf.FloorToInt(currentValue * PokemonNatureHelper.GetStatFactorFromNature(Type, pNature));

                break;

            case PokemonStatType.Evasion:
            case PokemonStatType.Accuracy:
                // Should be 100.
                currentValue = BaseValue;
                break;
        }

        if (currentValue < 1)
            currentValue = 1;

        return currentValue;
    }

    /// <summary>
    /// Gets the stat's value according to the current level and nature with the current modifiers.
    /// </summary>
    /// <param name="pPokemonLevel"></param>
    /// <param name="pNature"></param>
    /// <returns></returns>
    public int GetCurrentValue(int pPokemonLevel, PokemonNature pNature)
    {
        int currentLevelValue = GetCurrentLevelValue(pPokemonLevel, pNature);

        return currentLevelValue;
    }
}

[Serializable]
public class PokemonStatList : List<PokemonStat>
{
    public PokemonStat this[PokemonStatType pStatType]
    {
        get
        {
            // We get the 1st occurence of the desired PokemonStatType.
            foreach (var item in this)
            {
                if (item.Type == pStatType)
                    return item;
            }

            // No occurence found.
            return null;
        }
        set
        {
            var statToSet = this[pStatType];

            if (statToSet != null)
                statToSet = value;
        }
    }
}
