using System;
using SQLite4Unity3d;

[Serializable]
[Table("PokemonSpecies")]
public class PokemonSpecies
{
    [Column("GameId")]
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
    public string GameId { get; set; }

    [Column("Name")]
    public string Name { get; set; }

    [Column("MaleRatio")]
    /// <summary>
    /// -1 means it has no gender.
    /// </summary>
    public float MaleRatio { get; set; }

    [Column("CatchProbability")]
    public int CatchProbability { get; set; }

    [Column("HatchTime")]
    public int HatchTime { get; set; }

    [Column("BaseFriendship")]
    public int BaseFriendship { get; set; }

    #region Foreign Keys
    [Column("LevelingRate")]
    public PokemonLevelingRateType LevelingRate { get; set; }

    [Column("BodyStyle")]
    public PokemonBodyStyle BodyStyle { get; set; }

    [Column("BattleRewardFK")]
    public int BattleRewardFK { get; set; }
    [Ignore]
    public PokemonBattleReward BattleReward { get; set; }

    [Column("BaseStatsFK")]
    public int BaseStatsFK { get; set; }
    [Ignore]
    public PokemonStatList Stats { get; set; }
    #endregion

    #region External tables
    // Types

    // Abilities

    // MoveSet

    // Evolutions
    //Level,int level
    //	if pokemon's level is greater or equal to int level
    //Stone,string itemName
    //	if name of stone is equal to string itemName
    //Trade
    //	if currently trading pokemon
    //Friendship
    //	if pokemon's friendship is greater or equal to 220
    //Item,string itemName
    //	if pokemon's heldItem is equal to string itemName
    //Gender,Pokemon.Gender
    //  if pokemon's gender is equal to Pokemon.Gender
    //Move,string moveName
    //	if pokemon has a move thats name or typing is equal to string moveName
    //Map,string mapName
    //  if currentMap is equal to string mapName
    //Time,string dayNight
    //	if time is between 9PM and 4AM time is "Night". else time is "Day".
    //	if time is equal to string dayNight (either Day, or Night)
    //
    //		Unique evolution methods:
    //Mantine
    //	if party contains a Remoraid
    //Pangoro
    //	if party contains a dark pokemon
    //Goodra
    //	if currentMap's weather is rain
    //Hitmonlee
    //	if pokemon's ATK is greater than DEF
    //Hitmonchan
    //	if pokemon's ATK is lower than DEF
    //Hitmontop
    //  if pokemon's ATK is equal to DEF
    //Silcoon
    //  if pokemon's shinyValue divided by 2's remainder is equal to 0
    //Cascoon
    //	if pokemon's shinyValue divided by 2's remainder is equal to 1
    #endregion

    /// <summary>
    /// Loads the data using foreign keys and n to n tables.
    /// </summary>
    /// <param name="pDBConnection"></param>
    public void LoadForeignData(SQLiteConnection pDBConnection)
    {
        if (pDBConnection == null)
            return;

        BattleReward = pDBConnection.Find<PokemonBattleReward>(BattleRewardFK);
        Stats = pDBConnection.Find<PokemonBaseStat>(BaseStatsFK).GenerateStatList();
    }
}
