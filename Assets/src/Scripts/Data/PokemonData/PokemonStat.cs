using SQLite4Unity3d;
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

// ToDo: -Clear battle modifiers on end fight event.
//      -Decide struct or class
[Serializable]
public class PokemonStat
{
    #region Members
    private PokemonStatType _type;
    public PokemonStatType Type
    {
        get { return _type; }
        set { _type = value; }
    }

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

    private PokemonStatModifierList _modifiers;
    public PokemonStatModifierList Modifiers
    {
        get { return _modifiers; }
        set { _modifiers = value; }
    }
    #endregion

    public int IV
    {
        get
        {
            if (Modifiers == null)
                return 0;

            switch (Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.Speed:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                    break;

                default:
                    return 0;
            }

            var mod = Modifiers[PokemonStatModifierType.IV];
            if (mod == null)
                return 0;

            return mod.CurrentValue;
        }
        set
        {
            if (Modifiers == null)
                return;

            switch (Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.Speed:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                    break;

                default:
                    return;
            }

            var mod = Modifiers[PokemonStatModifierType.IV];
            // If the modifer doesn't exist we create and set it.
            if (mod == null)
                Modifiers.Add(new PokemonStatModifer(PokemonStatModifierType.IV, value));
            // If it already exist, we just set it.
            else
                mod.CurrentValue = value;
        }
    }

    public int EV
    {
        get
        {
            if (Modifiers == null)
                return 0;

            switch (Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.Speed:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                    break;

                default:
                    return 0;
            }

            var mod = Modifiers[PokemonStatModifierType.EV];
            if (mod == null)
                return 0;

            return mod.CurrentValue;
        }
        set
        {
            if (Modifiers == null)
                return;

            switch (Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.Speed:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                    break;

                default:
                    return;
            }

            var mod = Modifiers[PokemonStatModifierType.EV];
            // If the modifer doesn't exist we create and set it.
            if (mod == null)
                Modifiers.Add(new PokemonStatModifer(PokemonStatModifierType.EV, value));
            // If it already exist, we just set it.
            else
                mod.CurrentValue = value;
        }
    }

    #region Constructors
    public PokemonStat() : this(PokemonStatType.None, 0) { }

    public PokemonStat(PokemonStatType pStatType, int pBaseValue)
    {
        Type = pStatType;
        BaseValue = pBaseValue;

        Modifiers = new PokemonStatModifierList();
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

        foreach (var mod in Modifiers)
        {
            switch (mod.Type)
            {
                case PokemonStatModifierType.LostHP:
                    // Current HP
                    currentLevelValue -= mod.CurrentValue;
                    break;
                case PokemonStatModifierType.Battle:
                    // Battle modifiers for non HP stats.
                    switch (Type)
                    {
                        case PokemonStatType.Attack:
                        case PokemonStatType.Defence:
                        case PokemonStatType.Speed:
                        case PokemonStatType.SpecialAttack:
                        case PokemonStatType.SpecialDefence:
                            if (mod.CurrentValue < 0)
                                currentLevelValue *= 2 / 2 - mod.CurrentValue;
                            else
                                currentLevelValue *= (2 + mod.CurrentValue) / 2;
                            break;

                        case PokemonStatType.Evasion:
                        case PokemonStatType.Accuracy:
                            if (mod.CurrentValue < 0)
                                currentLevelValue *= 3 / 3 - mod.CurrentValue;
                            else
                                currentLevelValue *= (3 + mod.CurrentValue) / 3;
                            break;
                    }
                    break;

                default:
                    continue;
            }
        }

        return currentLevelValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pCurrentLevel"></param>
    /// <param name="pDamageHeal">Positive for damages, negative for heal.</param>
    public void LoseHP(int pDamageHeal)
    {
        if (Type != PokemonStatType.HP)
            return;

        var lostHPMod = Modifiers[PokemonStatModifierType.LostHP];

        // We update lost HP modifier.
        if (lostHPMod != null)
            lostHPMod.CurrentValue += pDamageHeal;
        // If it doesn't exist, we create it if needed.
        else if (pDamageHeal > 0)
            Modifiers.Add(new PokemonStatModifer(PokemonStatModifierType.LostHP, pDamageHeal));

        // We remove the modifier if it became useless.
        if (lostHPMod != null
            && lostHPMod.CurrentValue <= 0)
        {
            Modifiers.RemoveAll(mod => mod.Type == PokemonStatModifierType.LostHP);
        }
    }

    public void ClearBattleModifiers()
    {
        Modifiers.RemoveAll(mod => mod.Type == PokemonStatModifierType.Battle);
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

    public PokemonStatList() : base((int)PokemonStatType.None) { }
    public PokemonStatList(int pCapacity) : base(pCapacity) { }
    public PokemonStatList(IEnumerable<PokemonStat> pCollection) : base(pCollection) { }
}

[Serializable]
[Table("PokemonBaseStat")]
public class PokemonBaseStat
{
    [Column("HP")]
    public int HP { get; set; }
    [Column("Attack")]
    public int Attack { get; set; }
    [Column("Defence")]
    public int Defence { get; set; }
    [Column("SpecialAttack")]
    public int SpecialAttack { get; set; }
    [Column("SpecialDefence")]
    public int SpecialDefence { get; set; }
    [Column("Speed")]
    public int Speed { get; set; }

    public PokemonStatList GenerateStatList()
    {
        PokemonStatList stats = new PokemonStatList();

        stats.Add(new PokemonStat(PokemonStatType.HP, HP));
        stats.Add(new PokemonStat(PokemonStatType.Attack, Attack));
        stats.Add(new PokemonStat(PokemonStatType.Defence, Defence));
        stats.Add(new PokemonStat(PokemonStatType.SpecialAttack, SpecialAttack));
        stats.Add(new PokemonStat(PokemonStatType.SpecialDefence, SpecialDefence));
        stats.Add(new PokemonStat(PokemonStatType.Speed, Speed));

        return stats;
    }
}