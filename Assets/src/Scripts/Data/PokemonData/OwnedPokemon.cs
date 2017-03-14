//Original Scripts by IIColour (IIColour_Spectrum)

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PokemonStatus
{
    NONE,
    BURNED,
    FROZEN,
    PARALYZED,
    PoisonED,
    ASLEEP,
    FAINTED
}

public enum PokemonGender
{
    NONE,
    MALE,
    FEMALE,
    CALCULATE
}

[Serializable]
public class OwnedPokemon
{
    #region Members
    public const int MAX_SUMMED_EV = 510;
    private const int MAX_EV = 252;

    /// <summary>
    /// All data linked to this pokemon's species.
    /// </summary>
    public PokemonSpecies Species { get; private set; }

    /// <summary>
    /// This pokemon's stats.
    /// </summary>
    public PokemonStatList Stats { get; set; }

    public PokemonNature Nature;

    /// <summary>
    /// This pokemon's nickname.
    /// </summary>
    public string Nickname;

    /// <summary>
    /// This pokemon's gender;
    /// </summary>
    public PokemonGender Gender;

    private int _currentLevel;
    /// <summary>
    /// Current level.
    /// </summary>
    /// <remarks>
    /// Ranges from 1 to 100.
    /// </remarks>
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = Mathf.Clamp(value, 1, 100); }
    }

    private int _currentExperience;
    /// <summary>
    /// The current experience owned by this pokemon.
    /// </summary>
    /// <remarks>
    /// Positive or null only.
    /// </remarks>
    public int CurrentExperience
    {
        get { return _currentExperience; }
        set
        {
            if (value < 0)
                _currentExperience = 0;
            else
                _currentExperience = value;
        }
    }

    /// <summary>
    /// The exeperience needed to level to the next level.
    /// Returns -1 if the max level is reached.
    /// </summary>
    public int NextLevelExp
    {
        get
        {
            if (CurrentLevel <= 99)
                return PokemonLevelingRateHelper.GetRequiredExperienceToTargetLevel(Species.LevelingRate, CurrentLevel + 1);
            // Level max reached.
            else
                return -1;
        }
    }

    private int _currentFriendship;
    /// <summary>
    /// The current frienship of this pokemon for its owner.
    /// </summary>
    /// <remarks>
    /// Ranges from 0 to 255.
    /// </remarks>
    public int CurrentFriendship
    {
        get { return _currentFriendship; }
        set { _currentFriendship = Mathf.Clamp(value, 0, 255); }
    }

    // ToDo: Implement pokerus => http://bulbapedia.bulbagarden.net/wiki/Pok%C3%A9rus
    //private bool pokerus;

    // ToDo: Finish shiny implementation => http://bulbapedia.bulbagarden.net/wiki/Shiny_Pok%C3%A9mon
    /// <summary>
    /// Used to determine shinyness and some other stuff.
    /// </summary>
    private int RareValue;

    /// <summary>
    /// See http://bulbapedia.bulbagarden.net/wiki/Shiny_Pok%C3%A9mon for more details on shinyness.
    /// </summary>
    public bool IsShiny { get { return RareValue < 16; } }

    private PokemonStatus _currentStatus;
    /// <summary>
    /// The current status of this pokemon.
    /// </summary>
    public PokemonStatus CurrentStatus
    {
        get { return _currentStatus; }
        set { TrySetStatus(value); }
    }

    // ToDo: implement sleep.
    //private int sleepTurns;

    // ToDo: implement items.
    //private string heldItem;

    private string caughtBall;

    private string metDate;
    private string metMap;
    private int metLevel;

    // ToDo: implement owner.
    ///// <summary>
    ///// if OT = null, pokemon may be caught.
    ///// </summary>
    //private string OT;
    //private int DONumber;

    private int _currentAbility; //(0/1/2(hiddenability)) if higher than number of abilites, rounds down to nearest ability.
    // if is 2, but pokemon only has 1 ability and no hidden, will use the one ability it does have.

    private string[] _currentMoveset;
    private string[] _moveHistory;
    private int[] PPups;
    private int[] maxPP;
    private int[] PP;
    #endregion

    #region Constructors
    public OwnedPokemon() : this("-1") { }

    public OwnedPokemon(string pGameId)
    {
        // ToDo: Find in Database.
        Species = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(pGameId);

        Stats = Species.BaseStats.GenerateStatList();
    }

    /// <summary>
    /// New Pokemon with: every specific detail.
    /// </summary>
    /// <param name="pPokemonID"></param>
    /// <param name="pNickname"></param>
    /// <param name="pGender"></param>
    /// <param name="pLevel"></param>
    /// <param name="pIsShiny"></param>
    /// <param name="pCaughtBall"></param>
    /// <param name="pHeldItem"></param>
    /// <param name="pOT"></param>
    /// <param name="pIV_HP"></param>
    /// <param name="pIV_ATK"></param>
    /// <param name="pIV_DEF"></param>
    /// <param name="pIV_SPA"></param>
    /// <param name="pIV_SPD"></param>
    /// <param name="pIV_SPE"></param>
    /// <param name="pEV_HP"></param>
    /// <param name="pEV_ATK"></param>
    /// <param name="pEV_DEF"></param>
    /// <param name="pEV_SPA"></param>
    /// <param name="pEV_SPD"></param>
    /// <param name="pEV_SPE"></param>
    /// <param name="pNature"></param>
    /// <param name="pAbility"></param>
    /// <param name="pMoveset"></param>
    /// <param name="pPPups"></param>
    public OwnedPokemon(string pPokemonID, string pNickname, PokemonGender pGender, int pLevel,
        bool pIsShiny, string pCaughtBall, string pHeldItem, string pOT,
        int pIV_HP, int pIV_ATK, int pIV_DEF, int pIV_SPA, int pIV_SPD, int pIV_SPE,
        int pEV_HP, int pEV_ATK, int pEV_DEF, int pEV_SPA, int pEV_SPD, int pEV_SPE,
        PokemonNature pNature, int pAbility, string[] pMoveset, int[] pPPups)
        : this(pPokemonID)
    {
        var thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(pPokemonID);

        Species.GameId = pPokemonID;
        Nickname = pNickname;
        //SET UP FORMS LATER #####################################################################################
        //_formId = 0;
        Gender = pGender;
        //if gender is CALCULATE, then calculate gender using maleRatio
        if (pGender == PokemonGender.CALCULATE)
        {
            if (thisPokemonData.MaleRatio < 0)
            {
                Gender = PokemonGender.NONE;
            }
            else if (Random.Range(0f, 100f) <= thisPokemonData.MaleRatio)
            {
                Gender = PokemonGender.MALE;
            }
            else
            {
                Gender = PokemonGender.FEMALE;
            }
        }
        CurrentLevel = pLevel;
        //Find exp for current level.
        CurrentExperience = PokemonLevelingRateHelper.GetRequiredExperienceToTargetLevel(thisPokemonData.LevelingRate, pLevel);
        CurrentFriendship = thisPokemonData.BaseFriendship;

        generateRareValue(pIsShiny);

        CurrentStatus = PokemonStatus.NONE;
        caughtBall = pCaughtBall;

        // ToDo: implement owner.
        //OT = (string.IsNullOrEmpty(pOT)) ? SaveData.currentSave.playerName : pOT;
        //if (OT != SaveData.currentSave.playerName)
        //{
        //    DONumber = Random.Range(0, 65536); //if owned by another trainer, assign a random number. 
        //} //this way if they trade it to you, it will have a different number to the player's.
        //else
        //{
        //    DONumber = SaveData.currentSave.playerID;
        //}

        metLevel = pLevel;
        if (PlayerMovement.player != null)
        {
            if (PlayerMovement.player.accessedMapSettings != null)
            {
                metMap = PlayerMovement.player.accessedMapSettings.mapName;
            }
            else
            {
                metMap = "Somewhere";
            }
        }
        else
        {
            metMap = "Somewhere";
        }
        metDate = DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + DateTime.Today.Year;

        // IVs.
        Stats[PokemonStatType.HP].IV = pIV_HP;
        Stats[PokemonStatType.Attack].IV = pIV_ATK;
        Stats[PokemonStatType.Defence].IV = pIV_DEF;
        Stats[PokemonStatType.SpecialAttack].IV = pIV_SPA;
        Stats[PokemonStatType.SpecialDefence].IV = pIV_SPD;
        Stats[PokemonStatType.Speed].IV = pIV_SPE;

        // EVs.
        Stats[PokemonStatType.HP].EV = pEV_HP;
        Stats[PokemonStatType.Attack].EV = pEV_ATK;
        Stats[PokemonStatType.Defence].EV = pEV_DEF;
        Stats[PokemonStatType.SpecialAttack].EV = pEV_SPA;
        Stats[PokemonStatType.SpecialDefence].EV = pEV_SPD;
        Stats[PokemonStatType.Speed].EV = pEV_SPE;

        //set nature
        Nature = pNature;

        _currentAbility = pAbility;

        _currentMoveset = pMoveset;
        _moveHistory = pMoveset;

        PPups = pPPups;
        //set maxPP and PP to be the regular PP defined by the move in the database.
        maxPP = new int[4];
        PP = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(pMoveset[i]))
            {
                maxPP[i] = Mathf.FloorToInt(MoveDatabase.getMove(pMoveset[i]).getPP() * ((PPups[i] * 0.2f) + 1));
                PP[i] = maxPP[i];
            }
        }
        packMoveset();
    }

    /// <summary>
    /// New Pokemon with: random IVS, Shininess, default moveset, and EVS (0)
    /// </summary>
    /// <param name="pPokemonID"></param>
    /// <param name="pGender"></param>
    /// <param name="pLevel"></param>
    /// <param name="pCaughtBall"></param>
    /// <param name="pHeldItem"></param>
    /// <param name="pOT"></param>
    /// <param name="pAbility"></param>
    public OwnedPokemon(string pPokemonID, PokemonGender pGender, int pLevel, string pCaughtBall, string pHeldItem, string pOT, int pAbility)
        : this(pPokemonID)
    {
        var thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(pPokemonID);

        Species.GameId = pPokemonID;

        //SET UP FORMS LATER #####################################################################################
        //_formId = 0;

        Gender = pGender;
        //if gender is CALCULATE, then calculate gender using maleRatio
        if (pGender == PokemonGender.CALCULATE)
        {
            if (thisPokemonData.MaleRatio < 0)
            {
                Gender = PokemonGender.NONE;
            }
            else if (Random.Range(0f, 100f) <= thisPokemonData.MaleRatio)
            {
                Gender = PokemonGender.MALE;
            }
            else
            {
                Gender = PokemonGender.FEMALE;
            }
        }

        CurrentLevel = pLevel;
        //Find exp for current level, and next level.
        CurrentExperience = PokemonLevelingRateHelper.GetRequiredExperienceToTargetLevel(thisPokemonData.LevelingRate, pLevel);

        CurrentFriendship = thisPokemonData.BaseFriendship;

        generateRareValue();

        CurrentStatus = PokemonStatus.NONE;

        caughtBall = pCaughtBall;

        metLevel = pLevel;
        if (PlayerMovement.player != null)
        {
            if (PlayerMovement.player.accessedMapSettings != null)
            {
                metMap = PlayerMovement.player.accessedMapSettings.mapName;
            }
            else
            {
                metMap = "Somewhere";
            }
        }
        else
        {
            metMap = "Somewhere";
        }
        metDate = DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + DateTime.Today.Year;

        // ToDo: implement owner.
        //OT = (string.IsNullOrEmpty(pOT)) ? SaveData.currentSave.playerName : pOT;
        //if (OT != SaveData.currentSave.playerName)
        //{
        //    DONumber = Random.Range(0, 65536); //if owned by another trainer, assign a random number. 
        //} //this way if they trade it to you, it will have a different number to the player's.
        //else
        //{
        //    DONumber = SaveData.currentSave.playerID;
        //}

        setNewPokemonIVsAndEVs();

        //Randomize nature
        Nature = PokemonNatureHelper.GetRandomNature();

        //set ability 
        if (pAbility < 0 || pAbility > 2)
        {
            //if ability out of range, randomize ability
            _currentAbility = Random.Range(0, 2);
        }
        else
        {
            _currentAbility = pAbility;
        }

        //Set moveset based off of the highest level moves possible.
        //_currentMoveset = thisPokemonData.GenerateMoveset(_currentLevel);
        _moveHistory = _currentMoveset;

        //set maxPP and PP to be the regular PP defined by the move in the database.
        PPups = new int[4];
        maxPP = new int[4];
        PP = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(_currentMoveset[i]))
            {
                maxPP[i] = MoveDatabase.getMove(_currentMoveset[i]).getPP();
                PP[i] = maxPP[i];
            }
        }
        packMoveset();
    }

    /// <summary>
    /// Adding a caught pokemon (only a few customizable details)
    /// </summary>
    /// <param name="pPokemon"></param>
    /// <param name="pNickname"></param>
    /// <param name="pCaughtBall"></param>
    public OwnedPokemon(OwnedPokemon pPokemon, string pNickname, string pCaughtBall)
        : this(pPokemon.Species.GameId)
    {
        //PokemonData thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(pokemon.Species.Id);

        Species.GameId = pPokemon.Species.GameId;
        Nickname = pNickname;
        //SET UP FORMS LATER #####################################################################################
        //_formId = 0;
        Gender = pPokemon.Gender;

        CurrentLevel = pPokemon.CurrentLevel;
        //Find exp for current level, and next level.
        CurrentExperience = pPokemon.CurrentExperience;
        CurrentFriendship = pPokemon.CurrentFriendship;

        RareValue = pPokemon.RareValue;

        CurrentStatus = pPokemon.CurrentStatus;
        caughtBall = pCaughtBall;

        // ToDo: implement owner.
        //OT = SaveData.currentSave.playerName;
        //DONumber = SaveData.currentSave.playerID;

        metLevel = CurrentLevel;
        if (PlayerMovement.player.accessedMapSettings != null)
        {
            metMap = PlayerMovement.player.accessedMapSettings.mapName;
        }
        else
        {
            metMap = "Somewhere";
        }
        metDate = DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + DateTime.Today.Year;

        // IVs.
        Stats[PokemonStatType.HP].IV = pPokemon.GetIV(PokemonStatType.HP);
        Stats[PokemonStatType.Attack].IV = pPokemon.GetIV(PokemonStatType.Attack);
        Stats[PokemonStatType.Defence].IV = pPokemon.GetIV(PokemonStatType.Defence);
        Stats[PokemonStatType.SpecialAttack].IV = pPokemon.GetIV(PokemonStatType.SpecialAttack);
        Stats[PokemonStatType.SpecialDefence].IV = pPokemon.GetIV(PokemonStatType.SpecialDefence);
        Stats[PokemonStatType.Speed].IV = pPokemon.GetIV(PokemonStatType.Speed);

        // EVs.
        Stats[PokemonStatType.HP].EV = pPokemon.GetEV(PokemonStatType.HP);
        Stats[PokemonStatType.Attack].EV = pPokemon.GetEV(PokemonStatType.Attack);
        Stats[PokemonStatType.Defence].EV = pPokemon.GetEV(PokemonStatType.Defence);
        Stats[PokemonStatType.SpecialAttack].EV = pPokemon.GetEV(PokemonStatType.SpecialAttack);
        Stats[PokemonStatType.SpecialDefence].EV = pPokemon.GetEV(PokemonStatType.SpecialDefence);
        Stats[PokemonStatType.Speed].EV = pPokemon.GetEV(PokemonStatType.Speed);

        //set nature
        Nature = pPokemon.Nature;

        Stats = pPokemon.Stats;

        _currentAbility = pPokemon._currentAbility;

        _currentMoveset = pPokemon._currentMoveset;
        _moveHistory = pPokemon._moveHistory;

        PPups = pPokemon.PPups;
        //set maxPP and PP to be the regular PP defined by the move in the database.
        maxPP = new int[4];
        PP = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(_currentMoveset[i]))
            {
                maxPP[i] = Mathf.FloorToInt(MoveDatabase.getMove(_currentMoveset[i]).getPP() * ((PPups[i] * 0.2f) + 1));
                PP[i] = maxPP[i];
            }
        }
        packMoveset();
    }
    #endregion

    private void generateRareValue()
    {
        RareValue = Random.Range(0, 65536);
    }

    private void generateRareValue(bool pIsShiny)
    {
        if (pIsShiny)
        {
            RareValue = Random.Range(0, 16);
        }
        else
        {
            RareValue = Random.Range(16, 65536);
        }
    }

    private void setNewPokemonIVsAndEVs()
    {
        // IVs.
        Stats[PokemonStatType.HP].IV = Random.Range(0, 32);
        Stats[PokemonStatType.Attack].IV = Random.Range(0, 32);
        Stats[PokemonStatType.Defence].IV = Random.Range(0, 32);
        Stats[PokemonStatType.SpecialAttack].IV = Random.Range(0, 32);
        Stats[PokemonStatType.SpecialDefence].IV = Random.Range(0, 32);
        Stats[PokemonStatType.Speed].IV = Random.Range(0, 32);

        // EVs.
        Stats[PokemonStatType.HP].EV = 0;
        Stats[PokemonStatType.Attack].EV = 0;
        Stats[PokemonStatType.Defence].EV = 0;
        Stats[PokemonStatType.SpecialAttack].EV = 0;
        Stats[PokemonStatType.SpecialDefence].EV = 0;
        Stats[PokemonStatType.Speed].EV = 0;
    }

    public int GetCurrentLevelStatValue(PokemonStatType pStatType)
    {
        return Stats[pStatType].GetCurrentLevelValue(CurrentLevel, Nature);
    }

    public int GetCurrentStatValue(PokemonStatType pStatType)
    {
        return Stats[pStatType].GetCurrentValue(CurrentLevel, Nature);
    }

    public string swapHeldItem(string newItem)
    {
        // ToDo: Implement items.
        return "";
    }

    public void addExp(int expAdded)
    {
        if (CurrentLevel < 100)
        {
            CurrentExperience += expAdded;
            while (CurrentExperience >= NextLevelExp)
            {
                int currentBaseHP = GetCurrentLevelStatValue(PokemonStatType.HP);

                CurrentLevel += 1;

                // We heal the amount of gained HP by leveling.
                Stats[PokemonStatType.HP].LoseHP(currentBaseHP - GetCurrentLevelStatValue(PokemonStatType.HP));

                // We remove the death flag if needed.
                if (CurrentStatus == PokemonStatus.FAINTED
                    && GetCurrentStatValue(PokemonStatType.HP) > 0)
                    CurrentStatus = PokemonStatus.NONE;
            }
        }
    }

    public bool TryAddEV(PokemonStatType pStatType, int pAmount)
    {
        int hpEV = GetEV(PokemonStatType.HP);
        int atkEV = GetEV(PokemonStatType.Attack);
        int defEV = GetEV(PokemonStatType.Defence);
        int sAtkEV = GetEV(PokemonStatType.SpecialAttack);
        int sDefEV = GetEV(PokemonStatType.SpecialDefence);
        int spdEV = GetEV(PokemonStatType.Speed);

        int currentEV = 0;
        switch (pStatType)
        {
            case PokemonStatType.HP:
                currentEV = hpEV;
                break;
            case PokemonStatType.Attack:
                currentEV = atkEV;
                break;
            case PokemonStatType.Defence:
                currentEV = defEV;
                break;
            case PokemonStatType.Speed:
                currentEV = sAtkEV;
                break;
            case PokemonStatType.SpecialAttack:
                currentEV = sDefEV;
                break;
            case PokemonStatType.SpecialDefence:
                currentEV = spdEV;
                break;

            default:
                // Non EV stat.
                return false;
        }

        int evTotal = hpEV + atkEV + defEV + sAtkEV + sDefEV + spdEV;

        if (evTotal >= MAX_SUMMED_EV
            || currentEV >= MAX_EV)
            // EV cap reached.
            return false;

        // We check if we will pass the max total amount.
        if (evTotal + pAmount > MAX_SUMMED_EV)
            // If we do, we clamp.
            pAmount = MAX_SUMMED_EV - evTotal;

        // We check if we will pass the max single amount.
        if (currentEV + pAmount > MAX_EV)
            // If we do, we clamp.
            pAmount = MAX_EV - currentEV;

        // We add the EV.
        Stats[pStatType].EV += pAmount;

        return true;
    }

    /*/
        public int getEvolutionID(string currentMethod){
            PokemonData thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.Id);
            int[] evolutions = thisPokemonData.getEvolutions();
            string[] evolutionRequirements = thisPokemonData.getEvolutionRequirements();
            string[] evolutionRequirementsSplit = new string[evolutionRequirements.Length];
    
            string[] methods = new string[evolutionRequirements.Length];
            string[] parameters = new string[evolutionRequirements.Length];
            for(int i = 0; i < evolutionRequirements.Length; i++){
                evolutionRequirementsSplit = evolutionRequirements[i].Split(',');
                methods[i] = evolutionRequirementsSplit[0];
                if(evolutionRequirementsSplit.Length > 1){
                    parameters[i] = evolutionRequirementsSplit[1];}
            }
    
            string[] currentMethodSplit = currentMethod.Split(','); //if currentMethod needs a parameter attached, it will be separated by a ' , '
    
            for(int i = 0; i < methods.Length; i++){ //check every method
                if(methods[0] == currentMethodSplit[0]){ //if method name equals current method name...
                    if (checkEvolutionMethods(currentMethod, evolutionRequirements[i]) == true){ //if an evolution method was satisfied
                        return evolutions[i]; //do not check any others. (This prioritises the evolutions in order first to last)
                    }
                }
            }
            return -1;
        }
    //*/

    public int getEvolutionID(string currentMethod)
    {
        // ToDo: Implement evolutions
        //var thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.GameId);
        //int[] evolutions = thisPokemonData.getEvolutions();
        //string[] evolutionRequirements = thisPokemonData.getEvolutionRequirements();

        //for (int i = 0; i < evolutions.Length; i++)
        //{
        //    //if an evolution method was satisfied, return true
        //    if (checkEvolutionMethods(currentMethod, evolutionRequirements[i]))
        //    {
        //        Debug.Log("Relevant ID[" + i + "] = " + evolutions[i]);
        //        return evolutions[i];
        //    }
        //}
        return -1;
    }

    //Check PokemonData.cs for list of evolution method names.
    public bool canEvolve(string currentMethod)
    {
        // ToDo: Implement evolutions
        //var thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.GameId);
        //int[] evolutions = thisPokemonData.getEvolutions();
        //string[] evolutionRequirements = thisPokemonData.getEvolutionRequirements();

        //for (int i = 0; i < evolutions.Length; i++)
        //{
        //    //if an evolution method was satisfied, return true
        //    if (checkEvolutionMethods(currentMethod, evolutionRequirements[i]))
        //    {
        //        //		Debug.Log("All Checks Passed");
        //        return true;
        //    }
        //}

        return false;
    }

    //check that the evolution can be 
    public bool checkEvolutionMethods(string currentMethod, string evolutionRequirements)
    {
        // ToDo: Implement evolutions
        //string[] evolutionSplit = evolutionRequirements.Split(',');
        //string[] methods = evolutionSplit[0].Split('\\');
        //string[] currentMethodSplit = currentMethod.Split(',');
        ////if currentMethod needs a parameter attached, it will be separated by a ' , '
        //string[] parameters = new string[] { };
        //if (evolutionSplit.Length > 0)
        //{
        //    //if true, there is a parameter attached
        //    parameters = evolutionSplit[1].Split('\\');
        //}
        //for (int i = 0; i < methods.Length; i++)
        //{
        //    //for every method for the currently checked evolution
        //    //Debug.Log(evolutionRequirements +" | "+ currentMethodSplit[0] +" "+ methods[i] +" "+ parameters[i]);
        //    if (methods[i] == "Level")
        //    {
        //        //if method contains a Level requirement
        //        if (currentMethodSplit[0] != "Level")
        //        {
        //            //and system is not checking for a level evolution
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //        else
        //        {
        //            if (_currentLevel < int.Parse(parameters[i]))
        //            {
        //                //and pokemon's level is not high enough to evolve,
        //                return false; //cannot evolve. return false stop checking.
        //            }
        //        }
        //    }
        //    else if (methods[i] == "Stone")
        //    {
        //        //if method contains a Stone requirement
        //        if (currentMethodSplit[0] != "Stone")
        //        {
        //            //and system is not checking for a stone evolution
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //        else
        //        {
        //            //if it is checking for a stone evolution,
        //            if (currentMethodSplit[1] != parameters[i])
        //            {
        //                //and parameter being checked does not match the required one
        //                return false; //cannot evolve. return false and stop checking.
        //            }
        //        }
        //    }
        //    else if (methods[i] == "Trade")
        //    {
        //        //if method contains a Trade requirement
        //        if (currentMethodSplit[0] != "Trade")
        //        {
        //            //and system is not checking for a trade evolution
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //    }
        //    else if (methods[i] == "Friendship")
        //    {
        //        //if method contains a Friendship requirement
        //        if (friendship < 220)
        //        {
        //            //and pokemon's friendship is less than 220
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //    }
        //    else if (methods[i] == "Item")
        //    {
        //        //if method contains an Item requirement
        //        if (heldItem == parameters[i])
        //        {
        //            //and pokemon's Held Item is not the specified Item
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //    }
        //    else if (methods[i] == "Gender")
        //    {
        //        //if method contains a Gender requirement
        //        if (_gender.ToString() != parameters[i])
        //        {
        //            //and pokemon's gender is not the required gender to evolve,
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //    }
        //    else if (methods[i] == "Move")
        //    {
        //        //if method contains a Move requirement
        //        if (!HasMove(parameters[i]))
        //        {
        //            //and pokemon does not have the specified move
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //    }
        //    else if (methods[i] == "Map")
        //    {
        //        //if method contains a Map requirement
        //        string mapName = PlayerMovement.player.currentMap.name;
        //        if (mapName != parameters[i])
        //        {
        //            //and current map is not the required map to evolve,
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //    }
        //    else if (methods[i] == "Time")
        //    {
        //        //if method contains a Time requirement
        //        string dayNight = "Day";
        //        if (DateTime.Now.Hour >= 21 || DateTime.Now.Hour < 4)
        //        {
        //            //if time is night time
        //            dayNight = "Night"; //set dayNight to be "Night"
        //        }
        //        if (dayNight != parameters[i])
        //        {
        //            //if time is not what the evolution requires (Day/Night)
        //            return false; //cannot evolve. return false and stop checking.
        //        }
        //    }
        //    else
        //    {
        //        //if methods[i] did not equal to anything above, methods[i] is an invalid method.
        //        return false;
        //    }
        //}
        ////if the code did not return false once, then the evolution requirements must have been met.
        //return true;
        return false;
    }

    public bool evolve(string currentMethod)
    {
        // ToDo: Implement evolutions
        //int[] evolutions = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.GameId).getEvolutions();
        //string[] evolutionRequirements = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.GameId).getEvolutionRequirements();
        //for (int i = 0; i < evolutions.Length; i++)
        //{
        //    if (checkEvolutionMethods(currentMethod, evolutionRequirements[i]))
        //    {
        //        // ToDo: Handle evolution.
        //        // We want to keep the same HP rate before and after evolution.
        //        int currentHP = GetCurrentStatValue(PokemonStatType.HP);
        //        int maxHP = GetCurrentLevelStatValue(PokemonStatType.HP);
        //        float hpPercent = (float)currentHP / maxHP;

        //        Species.GameId = evolutions[i];

        //        maxHP = GetCurrentLevelStatValue(PokemonStatType.HP);
        //        currentHP = Mathf.RoundToInt(maxHP * hpPercent);

        //        return true;
        //    }
        //}
        return false;
    }

    //public override string ToString()
    //{
    //    string result = string.Format("{0}: {1}({2}), {3}, Level {4}, EXP: {5}, To next: {6}, Friendship: {7}, RareValue={8}, Pokerus={9}, Shiny={10}, Status: {11}, Ball: {12}, Item: {13}, met at Level {14} on {15} at {16}, OT: {17}, IVs: {18}, {19}, {20}, {21}, {22}, {23}, EVs: {24}, {25}, {26}, {27}, {28}, {29}, Stats: {30}/{31}, {32}, {33}, {34}, {35}, {36}, Nature: {37}", //, Ability: {38}
    //        Species.GameId,
    //        GetName(),
    //        Species.Name,
    //        Gender.ToString(),
    //        CurrentLevel,
    //        CurrentExperience,
    //        (NextLevelExp - CurrentExperience),
    //        CurrentFriendship,
    //        RareValue,
    //        IsShiny.ToString(),
    //        _currentStatus,
    //        caughtBall,
    //        heldItem,
    //        metLevel,
    //        metDate,
    //        metMap,
    //        OT,
    //        IDno,
    //        Stats[PokemonStatType.HP].IV,
    //        Stats[PokemonStatType.Attack].IV,
    //        Stats[PokemonStatType.Defence].IV,
    //        Stats[PokemonStatType.SpecialAttack].IV,
    //        Stats[PokemonStatType.SpecialDefence].IV,
    //        Stats[PokemonStatType.Speed].IV,
    //        Stats[PokemonStatType.HP].EV,
    //        Stats[PokemonStatType.Attack].EV,
    //        Stats[PokemonStatType.Defence].EV,
    //        Stats[PokemonStatType.SpecialAttack].EV,
    //        Stats[PokemonStatType.SpecialDefence].EV,
    //        Stats[PokemonStatType.Speed].EV,
    //        GetCurrentStatValue(PokemonStatType.HP),
    //        GetCurrentLevelStatValue(PokemonStatType.HP),
    //        GetCurrentLevelStatValue(PokemonStatType.Attack),
    //        GetCurrentLevelStatValue(PokemonStatType.Defence),
    //        GetCurrentLevelStatValue(PokemonStatType.SpecialAttack),
    //        GetCurrentLevelStatValue(PokemonStatType.SpecialDefence),
    //        GetCurrentLevelStatValue(PokemonStatType.Speed),
    //        _nature
    //        //PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.GameId).getAbility(_currentAbility)
    //        );

    //    result += ", [";
    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (!string.IsNullOrEmpty(_currentMoveset[i]))
    //        {
    //            result += _currentMoveset[i] + ": " + PP[i] + "/" + maxPP[i] + ", ";
    //        }
    //    }
    //    result = result.Remove(result.Length - 2, 2);
    //    result += "]";

    //    return result;
    //}

    public void healFull()
    {
        var lostHPMod = Stats[PokemonStatType.HP].Modifiers[PokemonStatModifierType.LostHP];
        if (lostHPMod != null)
            healHP(lostHPMod.CurrentValue);

        PP[0] = maxPP[0];
        PP[1] = maxPP[1];
        PP[2] = maxPP[2];
        PP[3] = maxPP[3];
        CurrentStatus = PokemonStatus.NONE;
    }

    /// <summary>
    /// </summary>
    /// <param name="pAmount"></param>
    /// <returns>Returns the actually healed HP.</returns>
    public int healHP(int pAmount)
    {
        // We use absolute value first to ease comparisions.
        pAmount = Mathf.Abs(pAmount);

        int healedHP = 0;

        var lostHPMod = Stats[PokemonStatType.HP].Modifiers[PokemonStatModifierType.LostHP];
        if (lostHPMod != null)
        {
            if (lostHPMod.CurrentValue > pAmount)
                healedHP = pAmount;
            else
                healedHP = lostHPMod.CurrentValue;

            // We don't forget to negate our heal value.
            Stats[PokemonStatType.HP].LoseHP(-pAmount);
        }

        return healedHP;
    }

    public int healPP(int move, float amount)
    {
        int excess = 0;
        int intAmount = Mathf.RoundToInt(amount);
        PP[move] += intAmount;
        if (PP[move] > maxPP[move])
        {
            excess = PP[move] - maxPP[move];
            PP[move] = maxPP[move];
        }
        return intAmount - excess;
    }

    public void healStatus()
    {
        CurrentStatus = PokemonStatus.NONE;
    }

    public void removeHP(int pAmount)
    {
        // We use absolute value.
        pAmount = Mathf.Abs(pAmount);

        Stats[PokemonStatType.HP].LoseHP(pAmount);

        if (GetCurrentStatValue(PokemonStatType.HP) <= 0)
        {
            CurrentStatus = PokemonStatus.FAINTED;
        }
    }


    public void removePP(string move, float amount)
    {
        removePP(getMoveIndex(move), amount);
    }

    public void removePP(int move, float amount)
    {
        if (move >= 0)
        {
            int intAmount = Mathf.RoundToInt(amount);
            PP[move] -= intAmount;
            if (PP[move] < 0)
            {
                PP[move] = 0;
            }
        }
    }


    public bool TrySetStatus(PokemonStatus pNewStatus)
    {
        if (pNewStatus == CurrentStatus)
            return true;

        if (CurrentStatus == PokemonStatus.NONE
            || pNewStatus == PokemonStatus.NONE
            || pNewStatus == PokemonStatus.FAINTED)
        {
            _currentStatus = pNewStatus;

            return true;
        }

        return false;
    }

    public string getFirstFEInstance(string moveName)
    {
        for (int i = 0; i < _currentMoveset.Length; i++)
        {
            if (MoveDatabase.getMove(_currentMoveset[i]).getFieldEffect() == moveName)
            {
                return _currentMoveset[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Get the pokemon's nickname, or regular name if it has none.
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        if (string.IsNullOrEmpty(Nickname))
        {
            return Species.Name;
        }
        else
        {
            return Nickname;
        }
    }

    public string getCaughtBall()
    {
        return caughtBall;
    }

    public string getHeldItem()
    {
        // ToDo: implement items.
        return "";
    }

    public string getMetDate()
    {
        return metDate;
    }

    public string getMetMap()
    {
        return metMap;
    }

    public int getMetLevel()
    {
        return metLevel;
    }

    public int GetIV(PokemonStatType pStatType)
    {
        switch (pStatType)
        {
            case PokemonStatType.HP:
            case PokemonStatType.Attack:
            case PokemonStatType.Defence:
            case PokemonStatType.Speed:
            case PokemonStatType.SpecialAttack:
            case PokemonStatType.SpecialDefence:
                return Stats[pStatType].IV;

            default:
                return -1;
        }
    }

    public PokemonStatType GetHighestIV()
    {
        var highestIV = PokemonStatType.None;
        int highestValue = -1;

        foreach (var stat in Stats)
        {
            switch (stat.Type)
            {
                case PokemonStatType.HP:
                case PokemonStatType.Attack:
                case PokemonStatType.Defence:
                case PokemonStatType.Speed:
                case PokemonStatType.SpecialAttack:
                case PokemonStatType.SpecialDefence:
                    if (stat.IV > highestValue)
                    {
                        highestValue = stat.IV;
                        highestIV = stat.Type;
                    }
                    break;
            }
        }

        return highestIV;
    }

    public int GetEV(PokemonStatType pStatType)
    {
        switch (pStatType)
        {
            case PokemonStatType.HP:
            case PokemonStatType.Attack:
            case PokemonStatType.Defence:
            case PokemonStatType.Speed:
            case PokemonStatType.SpecialAttack:
            case PokemonStatType.SpecialDefence:
                return Stats[pStatType].EV;

            default:
                return -1;
        }
    }

    public float getPercentHP()
    {
        return (float)GetCurrentStatValue(PokemonStatType.HP) / GetCurrentLevelStatValue(PokemonStatType.HP);
    }

    public int getAbility()
    {
        return _currentAbility;
    }

    public int getMoveIndex(string move)
    {
        for (int i = 0; i < _currentMoveset.Length; i++)
        {
            if (!string.IsNullOrEmpty(_currentMoveset[i]))
            {
                if (_currentMoveset[i] == move)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public string[] getMoveset()
    {
        string[] result = new string[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = _currentMoveset[i];
        }
        return result;
    }

    public void swapMoves(int target1, int target2)
    {
        string temp = _currentMoveset[target1];
        _currentMoveset[target1] = _currentMoveset[target2];
        _currentMoveset[target2] = temp;
    }


    private void ResetPP(int index)
    {
        PPups[index] = 0;
        maxPP[index] = Mathf.FloorToInt(MoveDatabase.getMove(_currentMoveset[index]).getPP() * ((PPups[index] * 0.2f) + 1));
        PP[index] = maxPP[index];
    }


    /// Returns false if no room to add the new move OR move already is learned.
    public bool addMove(string newMove)
    {
        if (!HasMove(newMove) && string.IsNullOrEmpty(_currentMoveset[3]))
        {
            _currentMoveset[3] = newMove;
            ResetPP(3);
            packMoveset();
            return true;
        }
        return false;
    }

    public void replaceMove(int index, string newMove)
    {
        if (index >= 0 && index < 4)
        {
            _currentMoveset[index] = newMove;
            addMoveToHistory(newMove);
            ResetPP(index);
        }
    }

    /// Returns false if only one move is left in the moveset.
    public bool forgetMove(int index)
    {
        if (getMoveCount() > 1)
        {
            _currentMoveset[index] = null;
            packMoveset();
            return true;
        }
        return false;
    }

    public int getMoveCount()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(_currentMoveset[i]))
            {
                count += 1;
            }
        }
        return count;
    }

    private void packMoveset()
    {
        string[] packedMoveset = new string[4];
        int[] packedPP = new int[4];
        int[] packedMaxPP = new int[4];
        int[] packedPPups = new int[4];

        int i2 = 0; //counter for packed array
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(_currentMoveset[i]))
            {
                //if next move in moveset is not null
                packedMoveset[i2] = _currentMoveset[i]; //add to packed moveset
                packedPP[i2] = PP[i];
                packedMaxPP[i2] = maxPP[i];
                packedPPups[i2] = PPups[i];
                i2 += 1;
            } //ready packed moveset's next position
        }
        _currentMoveset = packedMoveset;
        PP = packedPP;
        maxPP = packedMaxPP;
        PPups = packedPPups;
    }

    private void addMoveToHistory(string move)
    {
        if (!HasMoveInHistory(move))
        {
            string[] newHistory = new string[_moveHistory.Length + 1];
            for (int i = 0; i < _moveHistory.Length; i++)
            {
                newHistory[i] = _moveHistory[i];
            }
            newHistory[_moveHistory.Length] = move;
            _moveHistory = newHistory;
        }
    }

    public bool HasMove(string move)
    {
        if (string.IsNullOrEmpty(move))
        {
            return false;
        }
        for (int i = 0; i < _currentMoveset.Length; i++)
        {
            if (_currentMoveset[i] == move)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasMoveInHistory(string move)
    {
        for (int i = 0; i < _currentMoveset.Length; i++)
        {
            if (_currentMoveset[i] == move)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanLearnMove(string move)
    {
        // ToDo: Implement moves.
        //var thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.GameId);

        //string[] moves = thisPokemonData.getMovesetMoves();
        //for (int i = 0; i < moves.Length; i++)
        //{
        //    if (moves[i] == move)
        //    {
        //        return true;
        //    }
        //}
        //moves = thisPokemonData.getTmList();
        //for (int i = 0; i < moves.Length; i++)
        //{
        //    if (moves[i] == move)
        //    {
        //        return true;
        //    }
        //}
        return false;
    }

    public string MoveLearnedAtLevel(int level)
    {
        // ToDo: Implement moves.
        //var thisPokemonData = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(Species.GameId);

        //int[] movesetLevels = thisPokemonData.getMovesetLevels();
        //for (int i = 0; i < movesetLevels.Length; i++)
        //{
        //    if (movesetLevels[i] == level)
        //    {
        //        return thisPokemonData.getMovesetMoves()[i];
        //    }
        //}
        return null;
    }


    public int[] getPPups()
    {
        return PPups;
    }

    public int[] getMaxPP()
    {
        return maxPP;
    }

    public int[] getPP()
    {
        return PP;
    }

    public int getPP(int index)
    {
        return PP[index];
    }


    public Sprite[] GetFrontAnim_()
    {
        return GetAnimFromID_("PokemonSprites", Species.GameId, Gender, IsShiny);
    }

    public Sprite[] GetBackAnim_()
    {
        return GetAnimFromID_("PokemonBackSprites", Species.GameId, Gender, IsShiny);
    }


    public static Sprite[] GetFrontAnimFromID_(string ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID_("PokemonSprites", ID, gender, isShiny);
    }

    public static Sprite[] GetBackAnimFromID_(string ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID_("PokemonBackSprites", ID, gender, isShiny);
    }

    private static Sprite[] GetAnimFromID_(string folder, string ID, PokemonGender gender, bool isShiny)
    {
        Sprite[] animation = new Sprite[0];
        // ToDo: implement anim search pattern (with nomeclature).
        //string shiny = (isShiny) ? "s" : "";
        //if (gender == PokemonGender.FEMALE)
        //{
        //    //Attempt to load Female Variant
        //    animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + "f" + shiny + "/");
        //    if (animation.Length == 0)
        //    {
        //        Debug.LogWarning("Female Variant NOT Found");
        //        //Attempt to load Base Variant (possibly Shiny)
        //        animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + shiny + "/");
        //    }
        //    //	else{ Debug.Log("Female Variant Found"); }
        //}
        //else
        //{
        //    //Attempt to load Base Variant (possibly Shiny)
        //    animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + shiny + "/");
        //}
        //if (animation.Length == 0 && isShiny)
        //{
        //    Debug.LogWarning("Shiny Variant NOT Found");
        //    //No Shiny Variant exists, Attempt to load Regular Variant
        //    animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + "/");
        //}
        return animation;
    }


    public Sprite[] GetIcons_()
    {
        return GetIconsFromID_(Species.GameId, IsShiny);
    }

    public static Sprite[] GetIconsFromID_(string ID, bool isShiny)
    {
        // ToDo: implement icon search pattern (with nomeclature).
        //string shiny = (isShiny) ? "s" : "";
        Sprite[] icons = new Sprite[0];
        //Resources.LoadAll<Sprite>("PokemonIcons/icon" + convertLongID(ID) + shiny);
        //if (icons == null)
        //{
        //    Debug.LogWarning("Shiny Variant NOT Found");
        //    icons = Resources.LoadAll<Sprite>("PokemonIcons/icon" + convertLongID(ID));
        //}
        return icons;
    }

    public float GetCryPitch()
    {
        return (CurrentStatus == PokemonStatus.FAINTED) ? 0.9f : 1f - (0.06f * (1 - getPercentHP()));
    }

    public AudioClip GetCry()
    {
        return GetCryFromID(Species.GameId);
    }

    public static AudioClip GetCryFromID(string ID)
    {
        // ToDo: implement cry search pattern (with nomeclature).
        //return Resources.Load<AudioClip>("Audio/cry/" + convertLongID(ID));
        return null;
    }


    public Texture[] GetFrontAnim()
    {
        return GetAnimFromID("PokemonSprites", Species.GameId, Gender, IsShiny);
    }

    public Texture[] GetBackAnim()
    {
        return GetAnimFromID("PokemonBackSprites", Species.GameId, Gender, IsShiny);
    }

    public Texture GetIcons()
    {
        return GetIconsFromID(Species.GameId, IsShiny);
    }

    public Sprite[] GetSprite(bool getLight)
    {
        return GetSpriteFromID(Species.GameId, IsShiny, getLight);
    }


    public static Texture[] GetFrontAnimFromID(string ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID("PokemonSprites", ID, gender, isShiny);
    }

    public static Texture[] GetBackAnimFromID(string ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID("PokemonBackSprites", ID, gender, isShiny);
    }

    private static Texture[] GetAnimFromID(string folder, string ID, PokemonGender gender, bool isShiny)
    {
        Texture[] animation = new Texture[0];
        // ToDo: implement anim search pattern (with nomeclature).
        //string shiny = (isShiny) ? "s" : "";
        //if (gender == PokemonGender.FEMALE)
        //{
        //    //Attempt to load Female Variant
        //    animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + "f" + shiny + "/");
        //    if (animation.Length == 0)
        //    {
        //        Debug.LogWarning("Female Variant NOT Found (may not be required)");
        //        //Attempt to load Base Variant (possibly Shiny)
        //        animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + shiny + "/");
        //    }
        //    //	else{ Debug.Log("Female Variant Found");}
        //}
        //else
        //{
        //    //Attempt to load Base Variant (possibly Shiny)
        //    animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + shiny + "/");
        //}
        //if (animation.Length == 0 && isShiny)
        //{
        //    Debug.LogWarning("Shiny Variant NOT Found");
        //    //No Shiny Variant exists, Attempt to load Regular Variant
        //    animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + "/");
        //}
        return animation;
    }

    public static Texture GetIconsFromID(string ID, bool isShiny)
    {
        // ToDo: implement icon search pattern (with nomeclature).
        //string shiny = (isShiny) ? "s" : "";
        //Texture icons = Resources.Load<Texture>("PokemonIcons/icon" + convertLongID(ID) + shiny);
        //if (icons == null)
        //{
        //    Debug.LogWarning("Shiny Variant NOT Found");
        //    icons = Resources.Load<Texture>("PokemonIcons/icon" + convertLongID(ID));
        //}
        //return icons;

        return null;
    }


    public static Sprite[] GetSpriteFromID(string ID, bool isShiny, bool getLight)
    {
        // ToDo: implement sprite search pattern (with nomeclature).
        //string shiny = (isShiny) ? "s" : "";
        //string light = (getLight) ? "Lights/" : "";
        //Sprite[] spriteSheet = Resources.LoadAll<Sprite>("OverworldPokemonSprites/" + light + convertLongID(ID) + shiny);
        //if (spriteSheet.Length == 0)
        //{
        //    //No Light found AND/OR No Shiny found, load non-shiny
        //    if (isShiny)
        //    {
        //        if (getLight)
        //        {
        //            Debug.LogWarning("Shiny Light NOT Found (may not be required)");
        //        }
        //        else
        //        {
        //            Debug.LogWarning("Shiny Variant NOT Found");
        //        }
        //    }
        //    spriteSheet = Resources.LoadAll<Sprite>("OverworldPokemonSprites/" + light + convertLongID(ID));
        //}
        //if (spriteSheet.Length == 0)
        //{
        //    //No Light found OR No Sprite found, return 8 blank sprites
        //    if (!getLight)
        //    {
        //        Debug.LogWarning("Sprite NOT Found");
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Light NOT Found (may not be required)");
        //    }
        //    return new Sprite[8];
        //}
        //return spriteSheet;

        return new Sprite[0];
    }
}
