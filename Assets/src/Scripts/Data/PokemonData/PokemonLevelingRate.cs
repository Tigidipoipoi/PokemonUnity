using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonLevelingRateType
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuating,
}

/// <summary>
/// See http://bulbapedia.bulbagarden.net/wiki/Experience
/// </summary>
public static class PokemonLevelingRateHelper
{
    public static int GetRequiredExperienceToTargetLevel(PokemonLevelingRateType pType, int pTargetLevel)
    {
        if (pTargetLevel >= 100
            || pTargetLevel <= 1)
            return 0;

        int requiredExperience = 0;

        switch (pType)
        {
            case PokemonLevelingRateType.Erratic:
                requiredExperience = (int)Mathf.Pow(pTargetLevel, 3);
                // 1 -> 50
                if (pTargetLevel <= 50)
                {
                    requiredExperience *= 100 - pTargetLevel;
                    requiredExperience /= 50;
                }
                // 50 -> 68
                else if (pTargetLevel <= 68)
                {
                    requiredExperience *= 150 - pTargetLevel;
                    requiredExperience /= 100;
                }
                // 68 -> 98
                else if (pTargetLevel <= 98)
                {
                    requiredExperience *= (1911 - 10 * pTargetLevel) / 3;
                    requiredExperience /= 500;
                }
                // 98 -> 100
                else
                {
                    requiredExperience *= 160 - pTargetLevel;
                    requiredExperience /= 100;
                }
                break;
            case PokemonLevelingRateType.Fast:
                requiredExperience = 4 * (int)Mathf.Pow(pTargetLevel, 3) / 5;
                break;
            case PokemonLevelingRateType.MediumFast:
                requiredExperience = (int)Mathf.Pow(pTargetLevel, 3);
                break;
            case PokemonLevelingRateType.MediumSlow:
                requiredExperience = 6 * (int)Mathf.Pow(pTargetLevel, 3) / 5
                    - 15 * (int)Mathf.Pow(pTargetLevel, 2)
                    + 100 * pTargetLevel
                    - 140;
                break;
            case PokemonLevelingRateType.Slow:
                requiredExperience = 5 * (int)Mathf.Pow(pTargetLevel, 3) / 4;
                break;
            case PokemonLevelingRateType.Fluctuating:
                if (pTargetLevel <= 15)
                {
                    requiredExperience = Mathf.FloorToInt(pTargetLevel + 1 / 3) + 24;
                }
                else if (pTargetLevel <= 36)
                {
                    requiredExperience = pTargetLevel + 14;
                }
                else
                {
                    requiredExperience = pTargetLevel / 2 + 32;
                }

                requiredExperience /= 50;
                requiredExperience *= (int)Mathf.Pow(pTargetLevel, 3);
                break;
        }

        return requiredExperience;
    }

    /// <summary>
    /// </summary>
    /// <param name="pWildernessFactor">1 for wild; 1.5 for trainer owned. (or always 1 since gen VII)</param>
    /// <param name="pDefeatedPokemonExpYield"></param>
    /// <param name="pDefeatedPokemonLevel"></param>
    /// <param name="pSharedExpFactor">1 if participated to the battle; 2 if multi exp active and didn't participate.</param>
    /// <param name="pWinnerPokemonLevel"></param>
    /// <param name="pOriginalOwnerFactor">1 if original is the same as the current owner; 1.5 if original != current but same country; 1.7 if original != current and different country.</param>
    /// <param name="pLuckyEggFactor">1 if no lucky egg; 1.5 otherwise.</param>
    /// <param name="pAuraFactor">0.5; 0.66; 0.8; 1; 1.2; 1.5; 2;</param>
    /// <returns></returns>
    public static int GetEarnedExperience(float pWildernessFactor, int pDefeatedPokemonExpYield, int pDefeatedPokemonLevel, int pSharedExpFactor, int pWinnerPokemonLevel,
        float pOriginalOwnerFactor, float pLuckyEggFactor, float pAuraFactor)
    {
        int earnedExperience = Mathf.FloorToInt(pWildernessFactor * pDefeatedPokemonExpYield * pDefeatedPokemonLevel)
            / 5 * pSharedExpFactor;

        earnedExperience *= (int)Mathf.Pow(2 * pDefeatedPokemonLevel + 10, 2.5f)
            / (int)Mathf.Pow(pWinnerPokemonLevel * pDefeatedPokemonLevel + 10, 2.5f);

        earnedExperience = Mathf.FloorToInt((earnedExperience + 1) * pOriginalOwnerFactor * pLuckyEggFactor * pAuraFactor);

        return earnedExperience;
    }
}
