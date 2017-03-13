using System;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonStatModifierType
{
    IV,
    EV,

    LostHP,

    Battle,

    None,
}

[Serializable]
public class PokemonStatModifer
{
    public PokemonStatModifierType Type;

    private int _currentValue;
    public int CurrentValue
    {
        get { return _currentValue; }
        set
        {
            _currentValue = Mathf.Clamp(value, GetMinValue(), GetMaxValue());
        }
    }

    public PokemonStatModifer() : this(PokemonStatModifierType.None, 0) { }

    public PokemonStatModifer(PokemonStatModifierType pType, int pCurrentValue)
    {
        Type = pType;
        CurrentValue = pCurrentValue;
    }

    public int GetMinValue()
    {
        switch (Type)
        {
            case PokemonStatModifierType.Battle:
                return -6;

            case PokemonStatModifierType.IV:
            case PokemonStatModifierType.EV:
            case PokemonStatModifierType.LostHP:
            default:
                return 0;
        }
    }

    public int GetMaxValue()
    {
        switch (Type)
        {
            case PokemonStatModifierType.IV:
                return 31;
            case PokemonStatModifierType.EV:
                return 252;
            case PokemonStatModifierType.Battle:
                return 6;

            case PokemonStatModifierType.LostHP:
            default:
                return 1000;
        }
    }
}

[Serializable]
public class PokemonStatModifierList : List<PokemonStatModifer>
{
    public PokemonStatModifer this[PokemonStatModifierType pStatType]
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
