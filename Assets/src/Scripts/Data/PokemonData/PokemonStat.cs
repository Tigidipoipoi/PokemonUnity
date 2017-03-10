using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonStatType
{
    HP,
    Attack,
    Defense,
    SpecialAttack,
    SpecialDefense,
    Speed,

    Evasion,
    Accuracy,
}

// ToDo: Add modifiers (during battle only !)
public class PokemonStat
{
    public PokemonStatType Type;

    public int BaseValue;
    /// <summary>
    /// Individual Value.
    /// </summary>
    public int IV;
    /// <summary>
    /// Effort Value.
    /// </summary>
    public int EV;

    /// <summary>
    /// See http://bulbapedia.bulbagarden.net/wiki/Statistic for formula.
    /// </summary>
    /// <param name="pPokemonLevel"></param>
    /// <returns></returns>
    public int GetCurrentLevelValue(int pPokemonLevel, int pNature)
    {
        int currentValue = 0;

        switch (Type)
        {
            case PokemonStatType.HP:
            case PokemonStatType.Attack:
            case PokemonStatType.Defense:
            case PokemonStatType.SpecialAttack:
            case PokemonStatType.SpecialDefense:
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

        return currentValue;
    }
}
