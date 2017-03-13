using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public enum BodyStyleType
{
    HeadOnly,
    HandAndLegs,
    WithFins,
    Insectoid,
    Quadruped,
    MultiWinged,
    MultiBody,
    Tentacles,
    HeadAndBase,
    BipedalWithTail,
    BipedalTailless,
    SingleWinged,
    Serpentine,
    HeadAndArms,
}

[Serializable]
public class PokemonSpecies
{
    /// <summary>
    /// Used to determine which species it belongs to and which form it is.
    /// </summary>
    /// <example>
    /// Unown = letter of the alphabet.
    /// Deoxys = which of the four forms.
    /// Burmy/Wormadam = cloak type. Does not change for Wormadam.
    /// Shellos/Gastrodon = west/east alt colours.
    /// Rotom = different possesed appliance forms.
    /// Giratina = Origin/Altered form.
    /// Shaymin = Land/Sky form.
    /// Arceus = Type.
    /// Basculin = appearance.
    /// Deerling/Sawsbuck = appearance.
    /// Tornadus/Thundurus/Landorus = Incarnate/Therian forms.
    /// Kyurem = Normal/White/Black forms.
    /// Keldeo = Ordinary/Resolute forms.
    /// Meloetta = Aria/Pirouette forms.
    /// Genesect = different Drives.
    /// Vivillon = different Patterns.
    /// Flabebe/Floette/Florges = Flower colour.
    /// Furfrou = haircut.
    /// Pumpkaboo/Gourgeist = small/average/large/super sizes. 
    /// Hoopa = Confined/Unbound forms.
    /// </example>
    public int GameId { get; set; }

    public string Name { get; set; }

    /// <summary>
    /// -1 means it has no gender.
    /// </summary>
    public float MaleRatio { get; set; }

    public int CatchProbability { get; set; }

    public int HatchTime { get; set; }

    public int BaseFriendship { get; set; }

    #region Foreign Keys
    public BodyStyleType BodyStyle { get; set; }

    public PokemonLevelingRateType LevelingRate { get; set; }

    public object BattleReward { get; set; }
    #endregion

    #region External tables
    public PokemonStatList Stats { get; set; }

    // Ability ?

    // MoveSet ?
    #endregion
}
