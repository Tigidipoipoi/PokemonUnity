using System;

[Serializable]
public class PokemonBattleReward
{
    public int ExperienceYield { get; set; }
    public int HPYield { get; set; }
    public int AtkYield { get; set; }
    public int DefYield { get; set; }
    public int SpaYield { get; set; }
    public int SpdYield { get; set; }
    public int SpeYield { get; set; }

    #region Foreign key
    public object Item { get; set; }
    #endregion
}
