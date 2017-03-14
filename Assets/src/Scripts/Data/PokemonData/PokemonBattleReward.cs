using System;
using SQLite4Unity3d;

[Serializable]
[Table("PokemonBattleReward")]
public class PokemonBattleReward
{
    [Column("ExperienceYield")]
    public int ExperienceYield { get; set; }
    [Column("HPYield")]
    public int HPYield { get; set; }
    [Column("AtkYield")]
    public int AtkYield { get; set; }
    [Column("DefYield")]
    public int DefYield { get; set; }
    [Column("SpaYield")]
    public int SpaYield { get; set; }
    [Column("SpdYield")]
    public int SpdYield { get; set; }
    [Column("SpeYield")]
    public int SpeYield { get; set; }

    #region Foreign key
    [Column("ItemFK")]
    public int ItemFK { get; set; }
    [Ignore]
    public object Item { get; set; }
    #endregion
}
