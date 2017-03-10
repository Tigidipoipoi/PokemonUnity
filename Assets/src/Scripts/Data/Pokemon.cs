//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public enum PokemonStatus
{
    NONE,
    BURNED,
    FROZEN,
    PARALYZED,
    POISONED,
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
    private int pokemonID;
    private string nickname;

    ///// <summary>
    ///// Used only for a few pokemon to specify what form is in.
    ///// Default value is 0.
    ///// </summary>
    ///// <example>
    ///// Unown = letter of the alphabet.
    ///// Deoxys = which of the four forms.
    ///// Burmy/Wormadam = cloak type. Does not change for Wormadam.
    ///// Shellos/Gastrodon = west/east alt colours.
    ///// Rotom = different possesed appliance forms.
    ///// Giratina = Origin/Altered form.
    ///// Shaymin = Land/Sky form.
    ///// Arceus = Type.
    ///// Basculin = appearance.
    ///// Deerling/Sawsbuck = appearance.
    ///// Tornadus/Thundurus/Landorus = Incarnate/Therian forms.
    ///// Kyurem = Normal/White/Black forms.
    ///// Keldeo = Ordinary/Resolute forms.
    ///// Meloetta = Aria/Pirouette forms.
    ///// Genesect = different Drives.
    ///// Vivillon = different Patterns.
    ///// Flabebe/Floette/Florges = Flower colour.
    ///// Furfrou = haircut.
    ///// Pumpkaboo/Gourgeist = small/average/large/super sizes. 
    ///// Hoopa = Confined/Unbound forms.
    ///// </example>
    //private int _formId;

    private PokemonGender gender;
    private int _currentLevel;
    private int exp;
    private int nextLevelExp;

    private int friendship;

    private bool pokerus;
    private int rareValue;
    private bool isShiny;

    private PokemonStatus status;
    private int sleepTurns;

    private string caughtBall;
    private string heldItem;

    private string metDate;
    private string metMap;
    private int metLevel;

    //if OT = null, pokemon may be caught.
    private string OT;
    private int IDno;

    private int IV_HP;
    private int IV_ATK;
    private int IV_DEF;
    private int IV_SPA;
    private int IV_SPD;
    private int IV_SPE;

    private int EV_HP;
    private int EV_ATK;
    private int EV_DEF;
    private int EV_SPA;
    private int EV_SPD;
    private int EV_SPE;

    private PokemonNature _nature;

    private PokemonStatList _stats;

    private int ability; //(0/1/2(hiddenability)) if higher than number of abilites, rounds down to nearest ability.
    // if is 2, but pokemon only has 1 ability and no hidden, will use the one ability it does have.

    private string[] moveset;
    private string[] moveHistory;
    private int[] PPups;
    private int[] maxPP;
    private int[] PP;


    #region Constructors
    public OwnedPokemon()
    {
        _stats = new PokemonStatList();
        _stats.Add(new PokemonStat(PokemonStatType.HP, 1));
        _stats.Add(new PokemonStat(PokemonStatType.Attack, 1));
        _stats.Add(new PokemonStat(PokemonStatType.Defence, 1));
        _stats.Add(new PokemonStat(PokemonStatType.SpecialAttack, 1));
        _stats.Add(new PokemonStat(PokemonStatType.SpecialDefence, 1));
        _stats.Add(new PokemonStat(PokemonStatType.Speed, 1));

        _stats.Add(new PokemonStat(PokemonStatType.Accuracy, 100));
        _stats.Add(new PokemonStat(PokemonStatType.Evasion, 100));
    }

    //New Pokemon with: every specific detail
    public OwnedPokemon(int pPokemonID, string pNickname, PokemonGender pGender, int pLevel,
        bool pIsShiny, string pCaughtBall, string pHeldItem, string pOT,
        int pIV_HP, int pIV_ATK, int pIV_DEF, int pIV_SPA, int pIV_SPD, int pIV_SPE,
        int pEV_HP, int pEV_ATK, int pEV_DEF, int pEV_SPA, int pEV_SPD, int pEV_SPE,
        PokemonNature pNature, int pAbility, string[] pMoveset, int[] pPPups)
        : this()
    {
        PokemonData thisPokemonData = PokemonDatabase.getPokemon(pPokemonID);

        pokemonID = pPokemonID;
        nickname = pNickname;
        //SET UP FORMS LATER #####################################################################################
        //_formId = 0;
        gender = pGender;
        //if gender is CALCULATE, then calculate gender using maleRatio
        if (pGender == PokemonGender.CALCULATE)
        {
            if (thisPokemonData.getMaleRatio() < 0)
            {
                gender = PokemonGender.NONE;
            }
            else if (Random.Range(0f, 100f) <= thisPokemonData.getMaleRatio())
            {
                gender = PokemonGender.MALE;
            }
            else
            {
                gender = PokemonGender.FEMALE;
            }
        }
        _currentLevel = pLevel;
        //Find exp for current level, and next level.
        exp = PokemonDatabase.getLevelExp(thisPokemonData.getLevelingRate(), pLevel);
        nextLevelExp = PokemonDatabase.getLevelExp(thisPokemonData.getLevelingRate(), pLevel + 1);
        friendship = thisPokemonData.getBaseFriendship();

        isShiny = pIsShiny;
        if (pIsShiny)
        {
            rareValue = Random.Range(0, 16);
        }
        else
        {
            rareValue = Random.Range(16, 65536);
            if (rareValue < 19)
            {
                pokerus = true;
            }
        }

        status = PokemonStatus.NONE;
        sleepTurns = 0;
        caughtBall = pCaughtBall;
        heldItem = pHeldItem;

        OT = (string.IsNullOrEmpty(pOT)) ? SaveData.currentSave.playerName : pOT;
        if (OT != SaveData.currentSave.playerName)
        {
            IDno = Random.Range(0, 65536); //if owned by another trainer, assign a random number. 
        } //this way if they trade it to you, it will have a different number to the player's.
        else
        {
            IDno = SaveData.currentSave.playerID;
        }

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
        metDate = System.DateTime.Today.Day + "/" + System.DateTime.Today.Month + "/" + System.DateTime.Today.Year;

        //Set IVs 
        IV_HP = pIV_HP;
        IV_ATK = pIV_ATK;
        IV_DEF = pIV_DEF;
        IV_SPA = pIV_SPA;
        IV_SPD = pIV_SPD;
        IV_SPE = pIV_SPE;
        //set EVs
        EV_HP = pEV_HP;
        EV_ATK = pEV_ATK;
        EV_DEF = pEV_DEF;
        EV_SPA = pEV_SPA;
        EV_SPD = pEV_SPD;
        EV_SPE = pEV_SPE;
        //set nature
        _nature = pNature;
        //calculate stats
        calculateStats();

        ability = pAbility;

        moveset = pMoveset;
        moveHistory = pMoveset;

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


    //New Pokemon with: random IVS, and Shininess 
    //					default moveset, and EVS (0)
    public OwnedPokemon(int pPokemonID, PokemonGender pGender, int pLevel, string pCaughtBall, string pHeldItem, string pOT, int pAbility)
        : this()
    {
        PokemonData thisPokemonData = PokemonDatabase.getPokemon(pPokemonID);

        pokemonID = pPokemonID;

        //SET UP FORMS LATER #####################################################################################
        //_formId = 0;

        gender = pGender;
        //if gender is CALCULATE, then calculate gender using maleRatio
        if (pGender == PokemonGender.CALCULATE)
        {
            if (thisPokemonData.getMaleRatio() < 0)
            {
                gender = PokemonGender.NONE;
            }
            else if (Random.Range(0f, 100f) <= thisPokemonData.getMaleRatio())
            {
                gender = PokemonGender.MALE;
            }
            else
            {
                gender = PokemonGender.FEMALE;
            }
        }

        _currentLevel = pLevel;
        //Find exp for current level, and next level.
        exp = PokemonDatabase.getLevelExp(thisPokemonData.getLevelingRate(), pLevel);
        nextLevelExp = PokemonDatabase.getLevelExp(thisPokemonData.getLevelingRate(), pLevel + 1);

        friendship = thisPokemonData.getBaseFriendship();

        //Set Shininess randomly. 16/65535 if not shiny, then pokerus. 3/65535
        rareValue = Random.Range(0, 65536);
        if (rareValue < 16)
        {
            isShiny = true;
        }
        else if (rareValue < 19)
        {
            pokerus = true;
        }

        status = PokemonStatus.NONE;
        sleepTurns = 0;

        caughtBall = pCaughtBall;
        heldItem = pHeldItem;

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
        metDate = System.DateTime.Today.Day + "/" + System.DateTime.Today.Month + "/" + System.DateTime.Today.Year;

        OT = (string.IsNullOrEmpty(pOT)) ? SaveData.currentSave.playerName : pOT;
        if (OT != SaveData.currentSave.playerName)
        {
            IDno = Random.Range(0, 65536); //if owned by another trainer, assign a random number. 
        } //this way if they trade it to you, it will have a different number to the player's.
        else
        {
            IDno = SaveData.currentSave.playerID;
        }

        //Set IVs randomly between 0 and 32 (32 is exlcuded)
        IV_HP = Random.Range(0, 32);
        IV_ATK = Random.Range(0, 32);
        IV_DEF = Random.Range(0, 32);
        IV_SPA = Random.Range(0, 32);
        IV_SPD = Random.Range(0, 32);
        IV_SPE = Random.Range(0, 32);

        //unless specified with a full new Pokemon constructor, set EVs to 0.
        EV_HP = 0;
        EV_ATK = 0;
        EV_DEF = 0;
        EV_SPA = 0;
        EV_SPD = 0;
        EV_SPE = 0;

        //Randomize nature
        _nature = PokemonNatureHelper.GetRandomNature();

        //calculate stats
        calculateStats();

        //set ability 
        if (pAbility < 0 || pAbility > 2)
        {
            //if ability out of range, randomize ability
            ability = Random.Range(0, 2);
        }
        else
        {
            ability = pAbility;
        }

        //Set moveset based off of the highest level moves possible.
        moveset = thisPokemonData.GenerateMoveset(_currentLevel);
        moveHistory = moveset;

        //set maxPP and PP to be the regular PP defined by the move in the database.
        PPups = new int[4];
        maxPP = new int[4];
        PP = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(moveset[i]))
            {
                maxPP[i] = MoveDatabase.getMove(moveset[i]).getPP();
                PP[i] = maxPP[i];
            }
        }
        packMoveset();
    }

    //adding a caught pokemon (only a few customizable details)
    public OwnedPokemon(OwnedPokemon pPokemon, string pNickname, string pCaughtBall)
        : this()
    {
        //PokemonData thisPokemonData = PokemonDatabase.getPokemon(pokemon.pokemonID);

        pokemonID = pPokemon.pokemonID;
        nickname = pNickname;
        //SET UP FORMS LATER #####################################################################################
        //_formId = 0;
        gender = pPokemon.gender;

        _currentLevel = pPokemon._currentLevel;
        //Find exp for current level, and next level.
        exp = pPokemon.exp;
        nextLevelExp = pPokemon.nextLevelExp;
        friendship = pPokemon.friendship;

        isShiny = pPokemon.isShiny;
        rareValue = pPokemon.rareValue;
        pokerus = pPokemon.pokerus;

        status = pPokemon.status;
        sleepTurns = pPokemon.sleepTurns;
        caughtBall = pCaughtBall;
        heldItem = pPokemon.heldItem;

        OT = SaveData.currentSave.playerName;
        IDno = SaveData.currentSave.playerID;

        metLevel = _currentLevel;
        if (PlayerMovement.player.accessedMapSettings != null)
        {
            metMap = PlayerMovement.player.accessedMapSettings.mapName;
        }
        else
        {
            metMap = "Somewhere";
        }
        metDate = System.DateTime.Today.Day + "/" + System.DateTime.Today.Month + "/" + System.DateTime.Today.Year;

        //Set IVs 
        IV_HP = pPokemon.IV_HP;
        IV_ATK = pPokemon.IV_ATK;
        IV_DEF = pPokemon.IV_DEF;
        IV_SPA = pPokemon.IV_SPA;
        IV_SPD = pPokemon.IV_SPD;
        IV_SPE = pPokemon.IV_SPE;
        //set EVs
        EV_HP = pPokemon.EV_HP;
        EV_ATK = pPokemon.EV_ATK;
        EV_DEF = pPokemon.EV_DEF;
        EV_SPA = pPokemon.EV_SPA;
        EV_SPD = pPokemon.EV_SPD;
        EV_SPE = pPokemon.EV_SPE;
        //set nature
        _nature = pPokemon._nature;

        _stats = pPokemon._stats;

        ability = pPokemon.ability;

        moveset = pPokemon.moveset;
        moveHistory = pPokemon.moveHistory;

        PPups = pPokemon.PPups;
        //set maxPP and PP to be the regular PP defined by the move in the database.
        maxPP = new int[4];
        PP = new int[4];
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(moveset[i]))
            {
                maxPP[i] = Mathf.FloorToInt(MoveDatabase.getMove(moveset[i]).getPP() * ((PPups[i] * 0.2f) + 1));
                PP[i] = maxPP[i];
            }
        }
        packMoveset();
    }
    #endregion

    public int GetCurrentLevelStatValue(PokemonStatType pStatType)
    {
        return _stats[pStatType].GetCurrentLevelValue(_currentLevel, _nature);
    }

    public int GetCurrentStatValue(PokemonStatType pStatType)
    {
        return _stats[pStatType].GetCurrentValue(_currentLevel, _nature);
    }

    //Recalculate the pokemon's Stats.
    public void calculateStats() { }

    //set Nickname
    public void setNickname(string pNickname)
    {
        nickname = pNickname;
    }

    public string swapHeldItem(string newItem)
    {
        string oldItem = heldItem;
        heldItem = newItem;
        return oldItem;
    }

    public void addExp(int expAdded)
    {
        if (_currentLevel < 100)
        {
            exp += expAdded;
            while (exp >= nextLevelExp)
            {
                _currentLevel += 1;
                nextLevelExp = PokemonDatabase.getLevelExp(
                    PokemonDatabase.getPokemon(pokemonID).getLevelingRate(), _currentLevel + 1);

                // ToDo: Update current HP !
                //if (HP > 0 && status == PokemonStatus.FAINTED)
                //    status = PokemonStatus.NONE;
            }
        }
    }

    public bool addEVs(string stat, float amount)
    {
        int intAmount = Mathf.FloorToInt(amount);
        int evTotal = EV_HP + EV_ATK + EV_DEF + EV_SPA + EV_SPD + EV_SPE;
        if (evTotal < 510)
        {
            //if total EV cap is already reached.
            if (evTotal + intAmount > 510)
            {
                //if this addition will pass the total EV cap.
                intAmount = 510 - evTotal; //set intAmount to be the remaining points before cap is reached.
            }
            if (stat == "HP")
            {
                //if adding to HP.
                if (EV_HP < 252)
                {
                    //if HP is not full.
                    EV_HP += intAmount;
                    if (EV_HP > 252)
                    {
                        //if single stat EV cap is passed.
                        EV_HP = 252;
                    } //set stat back to the cap.
                    return true;
                }
            }
            else if (stat == "ATK")
            {
                //if adding to ATK.
                if (EV_ATK < 252)
                {
                    //if ATK is not full.
                    EV_ATK += intAmount;
                    if (EV_ATK > 252)
                    {
                        //if single stat EV cap is passed.
                        EV_ATK = 252;
                    } //set stat back to the cap.
                    return true;
                }
            }
            else if (stat == "DEF")
            {
                //if adding to DEF.
                if (EV_DEF < 252)
                {
                    //if DEF is not full.
                    EV_DEF += intAmount;
                    if (EV_DEF > 252)
                    {
                        //if single stat EV cap is passed.
                        EV_DEF = 252;
                    } //set stat back to the cap.
                    return true;
                }
            }
            else if (stat == "SPA")
            {
                //if adding to SPA.
                if (EV_SPA < 252)
                {
                    //if SPA is not full.
                    EV_SPA += intAmount;
                    if (EV_SPA > 252)
                    {
                        //if single stat EV cap is passed.
                        EV_SPA = 252;
                    } //set stat back to the cap.
                    return true;
                }
            }
            else if (stat == "SPD")
            {
                //if adding to SPD.
                if (EV_SPD < 252)
                {
                    //if SPD is not full.
                    EV_SPD += intAmount;
                    if (EV_SPD > 252)
                    {
                        //if single stat EV cap is passed.
                        EV_SPD = 252;
                    } //set stat back to the cap.
                    return true;
                }
            }
            else if (stat == "SPE")
            {
                //if adding to SPE.
                if (EV_SPE < 252)
                {
                    //if SPE is not full.
                    EV_SPE += intAmount;
                    if (EV_SPE > 252)
                    {
                        //if single stat EV cap is passed.
                        EV_SPE = 252;
                    } //set stat back to the cap.
                    return true;
                }
            }
        }
        return false; //returns false if total or relevant EV cap was reached before running.
    }

    /*/
        public int getEvolutionID(string currentMethod){
            PokemonData thisPokemonData = PokemonDatabase.getPokemon(pokemonID);
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
        PokemonData thisPokemonData = PokemonDatabase.getPokemon(pokemonID);
        int[] evolutions = thisPokemonData.getEvolutions();
        string[] evolutionRequirements = thisPokemonData.getEvolutionRequirements();

        for (int i = 0; i < evolutions.Length; i++)
        {
            //if an evolution method was satisfied, return true
            if (checkEvolutionMethods(currentMethod, evolutionRequirements[i]))
            {
                Debug.Log("Relevant ID[" + i + "] = " + evolutions[i]);
                return evolutions[i];
            }
        }
        return -1;
    }

    //Check PokemonData.cs for list of evolution method names.
    public bool canEvolve(string currentMethod)
    {
        PokemonData thisPokemonData = PokemonDatabase.getPokemon(pokemonID);
        int[] evolutions = thisPokemonData.getEvolutions();
        string[] evolutionRequirements = thisPokemonData.getEvolutionRequirements();

        for (int i = 0; i < evolutions.Length; i++)
        {
            //if an evolution method was satisfied, return true
            if (checkEvolutionMethods(currentMethod, evolutionRequirements[i]))
            {
                //		Debug.Log("All Checks Passed");
                return true;
            }
        }
        //Debug.Log("Check Failed");
        return false;
    }

    //check that the evolution can be 
    public bool checkEvolutionMethods(string currentMethod, string evolutionRequirements)
    {
        string[] evolutionSplit = evolutionRequirements.Split(',');
        string[] methods = evolutionSplit[0].Split('\\');
        string[] currentMethodSplit = currentMethod.Split(',');
        //if currentMethod needs a parameter attached, it will be separated by a ' , '
        string[] parameters = new string[] { };
        if (evolutionSplit.Length > 0)
        {
            //if true, there is a parameter attached
            parameters = evolutionSplit[1].Split('\\');
        }
        for (int i = 0; i < methods.Length; i++)
        {
            //for every method for the currently checked evolution
            //Debug.Log(evolutionRequirements +" | "+ currentMethodSplit[0] +" "+ methods[i] +" "+ parameters[i]);
            if (methods[i] == "Level")
            {
                //if method contains a Level requirement
                if (currentMethodSplit[0] != "Level")
                {
                    //and system is not checking for a level evolution
                    return false; //cannot evolve. return false and stop checking.
                }
                else
                {
                    if (_currentLevel < int.Parse(parameters[i]))
                    {
                        //and pokemon's level is not high enough to evolve,
                        return false; //cannot evolve. return false stop checking.
                    }
                }
            }
            else if (methods[i] == "Stone")
            {
                //if method contains a Stone requirement
                if (currentMethodSplit[0] != "Stone")
                {
                    //and system is not checking for a stone evolution
                    return false; //cannot evolve. return false and stop checking.
                }
                else
                {
                    //if it is checking for a stone evolution,
                    if (currentMethodSplit[1] != parameters[i])
                    {
                        //and parameter being checked does not match the required one
                        return false; //cannot evolve. return false and stop checking.
                    }
                }
            }
            else if (methods[i] == "Trade")
            {
                //if method contains a Trade requirement
                if (currentMethodSplit[0] != "Trade")
                {
                    //and system is not checking for a trade evolution
                    return false; //cannot evolve. return false and stop checking.
                }
            }
            else if (methods[i] == "Friendship")
            {
                //if method contains a Friendship requirement
                if (friendship < 220)
                {
                    //and pokemon's friendship is less than 220
                    return false; //cannot evolve. return false and stop checking.
                }
            }
            else if (methods[i] == "Item")
            {
                //if method contains an Item requirement
                if (heldItem == parameters[i])
                {
                    //and pokemon's Held Item is not the specified Item
                    return false; //cannot evolve. return false and stop checking.
                }
            }
            else if (methods[i] == "Gender")
            {
                //if method contains a Gender requirement
                if (gender.ToString() != parameters[i])
                {
                    //and pokemon's gender is not the required gender to evolve,
                    return false; //cannot evolve. return false and stop checking.
                }
            }
            else if (methods[i] == "Move")
            {
                //if method contains a Move requirement
                if (!HasMove(parameters[i]))
                {
                    //and pokemon does not have the specified move
                    return false; //cannot evolve. return false and stop checking.
                }
            }
            else if (methods[i] == "Map")
            {
                //if method contains a Map requirement
                string mapName = PlayerMovement.player.currentMap.name;
                if (mapName != parameters[i])
                {
                    //and current map is not the required map to evolve,
                    return false; //cannot evolve. return false and stop checking.
                }
            }
            else if (methods[i] == "Time")
            {
                //if method contains a Time requirement
                string dayNight = "Day";
                if (System.DateTime.Now.Hour >= 21 || System.DateTime.Now.Hour < 4)
                {
                    //if time is night time
                    dayNight = "Night"; //set dayNight to be "Night"
                }
                if (dayNight != parameters[i])
                {
                    //if time is not what the evolution requires (Day/Night)
                    return false; //cannot evolve. return false and stop checking.
                }
            }
            else
            {
                //if methods[i] did not equal to anything above, methods[i] is an invalid method.
                return false;
            }
        }
        //if the code did not return false once, then the evolution requirements must have been met.
        return true;
    }

    public bool evolve(string currentMethod)
    {
        int[] evolutions = PokemonDatabase.getPokemon(pokemonID).getEvolutions();
        string[] evolutionRequirements = PokemonDatabase.getPokemon(pokemonID).getEvolutionRequirements();
        for (int i = 0; i < evolutions.Length; i++)
        {
            if (checkEvolutionMethods(currentMethod, evolutionRequirements[i]))
            {
                int currentHP = GetCurrentStatValue(PokemonStatType.HP);
                int maxHP = GetCurrentLevelStatValue(PokemonStatType.HP);
                float hpPercent = (float)currentHP / maxHP;
                pokemonID = evolutions[i];
                calculateStats();
                i = evolutions.Length;
                // ToDo: Update modifier instead
                currentHP = Mathf.RoundToInt(maxHP * hpPercent);

                return true;
            }
        }
        return false;
    }

    //return a string that contains all of this pokemon's data
    public override string ToString()
    {
        string result = string.Format("{0}: {1}({2}), {3}, Level {4}, EXP: {5}, To next: {6}, Friendship: {7}, RareValue={8}, Pokerus={9}, Shiny={10}, Status: {11}, Ball: {12}, Item: {13}, met at Level {14} on {15} at {16}, OT: {17}, IVs: {18}, {19}, {20}, {21}, {22}, {23}, EVs: {24}, {25}, {26}, {27}, {28}, {29}, Stats: {30}/{31}, {32}, {33}, {34}, {35}, {36}, Nature: {37}, Ability: {38}",
            pokemonID,
            getName(),
            PokemonDatabase.getPokemon(pokemonID).getName(),
            gender.ToString(),
            _currentLevel,
            exp,
            (nextLevelExp - exp),
            friendship,
            rareValue,
            pokerus.ToString(),
            isShiny.ToString(),
            status,
            caughtBall,
            heldItem,
            metLevel,
            metDate,
            metMap,
            OT,
            IDno,
            _stats[PokemonStatType.HP].IV,
            _stats[PokemonStatType.Attack].IV,
            _stats[PokemonStatType.Defence].IV,
            _stats[PokemonStatType.SpecialAttack].IV,
            _stats[PokemonStatType.SpecialDefence].IV,
            _stats[PokemonStatType.Speed].IV,
            _stats[PokemonStatType.HP].EV,
            _stats[PokemonStatType.Attack].EV,
            _stats[PokemonStatType.Defence].EV,
            _stats[PokemonStatType.SpecialAttack].EV,
            _stats[PokemonStatType.SpecialDefence].EV,
            _stats[PokemonStatType.Speed].EV,
            GetCurrentStatValue(PokemonStatType.HP),
            GetCurrentLevelStatValue(PokemonStatType.HP),
            GetCurrentLevelStatValue(PokemonStatType.Attack),
            GetCurrentLevelStatValue(PokemonStatType.Defence),
            GetCurrentLevelStatValue(PokemonStatType.SpecialAttack),
            GetCurrentLevelStatValue(PokemonStatType.SpecialDefence),
            GetCurrentLevelStatValue(PokemonStatType.Speed),
            _nature,
            PokemonDatabase.getPokemon(pokemonID).getAbility(ability));

        result += ", [";
        for (int i = 0; i < 4; i++)
        {
            if (!string.IsNullOrEmpty(moveset[i]))
            {
                result += moveset[i] + ": " + PP[i] + "/" + maxPP[i] + ", ";
            }
        }
        result = result.Remove(result.Length - 2, 2);
        result += "]";

        return result;
    }

    //Heal the pokemon
    public void healFull()
    {
        // ToDo: Update modifers.
        //currentHP = HP;
        PP[0] = maxPP[0];
        PP[1] = maxPP[1];
        PP[2] = maxPP[2];
        PP[3] = maxPP[3];
        status = PokemonStatus.NONE;
    }

    ///returns the excess hp
    public int healHP(float amount)
    {
        int excess = 0;
        int intAmount = Mathf.RoundToInt(amount);

        // ToDo: Update modifers.
        //currentHP = HP;
        //currentHP += intAmount;
        //if (currentHP > HP)
        //{
        //    excess = currentHP - HP;
        //    currentHP = HP;
        //}
        //if (status == PokemonStatus.FAINTED && currentHP > 0)
        //{
        //    status = PokemonStatus.NONE;
        //}
        return intAmount - excess;
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
        status = PokemonStatus.NONE;
    }

    public void removeHP(float amount)
    {
        int intAmount = Mathf.RoundToInt(amount);
        // ToDo: Update modifers.
        //currentHP -= intAmount;
        //if (currentHP <= 0)
        //{
        //    currentHP = 0;
        //    status = PokemonStatus.FAINTED;
        //}
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


    public bool setStatus(PokemonStatus pStatus)
    {
        if (status == PokemonStatus.NONE)
        {
            status = pStatus;
            if (pStatus == PokemonStatus.ASLEEP)
            {
                //if applying sleep, set sleeping 
                sleepTurns = Random.Range(1, 4);
            } //turns to 1, 2, or 3
            return true;
        }
        else
        {
            if (pStatus == PokemonStatus.NONE || pStatus == PokemonStatus.FAINTED)
            {
                status = pStatus;
                sleepTurns = 0;
                return true;
            }
        }
        return false;
    }


    public void removeSleepTurn()
    {
        if (status == PokemonStatus.ASLEEP)
        {
            sleepTurns -= 1;
            if (sleepTurns <= 0)
            {
                setStatus(PokemonStatus.NONE);
            }
        }
    }

    public int getSleepTurns()
    {
        return sleepTurns;
    }


    public string getFirstFEInstance(string moveName)
    {
        for (int i = 0; i < moveset.Length; i++)
        {
            if (MoveDatabase.getMove(moveset[i]).getFieldEffect() == moveName)
            {
                return moveset[i];
            }
        }
        return null;
    }

    public int getID()
    {
        return pokemonID;
    }

    public string getLongID()
    {
        string result = pokemonID.ToString();
        while (result.Length < 3)
        {
            result = "0" + result;
        }
        return result;
    }

    public static string convertLongID(int ID)
    {
        string result = ID.ToString();
        while (result.Length < 3)
        {
            result = "0" + result;
        }
        return result;
    }

    //Get the pokemon's nickname, or regular name if it has none.
    public string getName()
    {
        if (string.IsNullOrEmpty(nickname))
        {
            return PokemonDatabase.getPokemon(pokemonID).getName();
        }
        else
        {
            return nickname;
        }
    }

    public PokemonGender getGender()
    {
        return gender;
    }

    public int getLevel()
    {
        return _currentLevel;
    }

    public int getExp()
    {
        return exp;
    }

    public int getExpNext()
    {
        return nextLevelExp;
    }

    public int getFriendship()
    {
        return friendship;
    }

    public bool getPokerus()
    {
        return pokerus;
    }

    public int getRareValue()
    {
        return rareValue;
    }

    public bool getIsShiny()
    {
        return isShiny;
    }

    public PokemonStatus getStatus()
    {
        return status;
    }

    public string getCaughtBall()
    {
        return caughtBall;
    }

    public string getHeldItem()
    {
        return heldItem;
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

    public string getOT()
    {
        return OT;
    }

    public int getIDno()
    {
        return IDno;
    }

    public int GetIV(int index)
    {
        if (index == 0)
        {
            return IV_HP;
        }
        else if (index == 1)
        {
            return IV_ATK;
        }
        else if (index == 2)
        {
            return IV_DEF;
        }
        else if (index == 3)
        {
            return IV_SPA;
        }
        else if (index == 4)
        {
            return IV_SPD;
        }
        else if (index == 5)
        {
            return IV_SPE;
        }
        return -1;
    }

    public int getIV_HP()
    {
        return IV_HP;
    }

    public int getIV_ATK()
    {
        return IV_ATK;
    }

    public int getIV_DEF()
    {
        return IV_DEF;
    }

    public int getIV_SPA()
    {
        return IV_SPA;
    }

    public int getIV_SPD()
    {
        return IV_SPD;
    }

    public int getIV_SPE()
    {
        return IV_SPE;
    }

    public int GetHighestIV()
    {
        int highestIVIndex = 0;
        int highestIV = IV_HP;
        //by default HP is highest. Check if others are higher. Use RareValue to consistantly break a tie
        if (IV_ATK > highestIV || (IV_ATK == highestIV && rareValue > 10922))
        {
            highestIVIndex = 1;
            highestIV = IV_ATK;
        }
        if (IV_DEF > highestIV || (IV_DEF == highestIV && rareValue > 21844))
        {
            highestIVIndex = 2;
            highestIV = IV_DEF;
        }
        if (IV_SPA > highestIV || (IV_SPA == highestIV && rareValue > 32766))
        {
            highestIVIndex = 3;
            highestIV = IV_SPA;
        }
        if (IV_SPD > highestIV || (IV_SPD == highestIV && rareValue > 43688))
        {
            highestIVIndex = 4;
            highestIV = IV_SPD;
        }
        if (IV_SPE > highestIV || (IV_SPE == highestIV && rareValue > 54610))
        {
            highestIVIndex = 5;
            highestIV = IV_SPE;
        }
        return highestIVIndex;
    }

    public int getEV_HP()
    {
        return EV_HP;
    }

    public int getEV_ATK()
    {
        return EV_ATK;
    }

    public int getEV_DEF()
    {
        return EV_DEF;
    }

    public int getEV_SPA()
    {
        return EV_SPA;
    }

    public int getEV_SPD()
    {
        return EV_SPD;
    }

    public int getEV_SPE()
    {
        return EV_SPE;
    }

    public PokemonNature getNature()
    {
        return _nature;
    }

    public float getPercentHP()
    {
        return (float)GetCurrentStatValue(PokemonStatType.HP) / GetCurrentLevelStatValue(PokemonStatType.HP);
    }

    public int getAbility()
    {
        return ability;
    }


    public int getMoveIndex(string move)
    {
        for (int i = 0; i < moveset.Length; i++)
        {
            if (!string.IsNullOrEmpty(moveset[i]))
            {
                if (moveset[i] == move)
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
            result[i] = moveset[i];
        }
        return result;
    }

    public void swapMoves(int target1, int target2)
    {
        string temp = moveset[target1];
        moveset[target1] = moveset[target2];
        moveset[target2] = temp;
    }


    private void ResetPP(int index)
    {
        PPups[index] = 0;
        maxPP[index] = Mathf.FloorToInt(MoveDatabase.getMove(moveset[index]).getPP() * ((PPups[index] * 0.2f) + 1));
        PP[index] = maxPP[index];
    }


    /// Returns false if no room to add the new move OR move already is learned.
    public bool addMove(string newMove)
    {
        if (!HasMove(newMove) && string.IsNullOrEmpty(moveset[3]))
        {
            moveset[3] = newMove;
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
            moveset[index] = newMove;
            addMoveToHistory(newMove);
            ResetPP(index);
        }
    }

    /// Returns false if only one move is left in the moveset.
    public bool forgetMove(int index)
    {
        if (getMoveCount() > 1)
        {
            moveset[index] = null;
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
            if (!string.IsNullOrEmpty(moveset[i]))
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
            if (!string.IsNullOrEmpty(moveset[i]))
            {
                //if next move in moveset is not null
                packedMoveset[i2] = moveset[i]; //add to packed moveset
                packedPP[i2] = PP[i];
                packedMaxPP[i2] = maxPP[i];
                packedPPups[i2] = PPups[i];
                i2 += 1;
            } //ready packed moveset's next position
        }
        moveset = packedMoveset;
        PP = packedPP;
        maxPP = packedMaxPP;
        PPups = packedPPups;
    }

    private void addMoveToHistory(string move)
    {
        if (!HasMoveInHistory(move))
        {
            string[] newHistory = new string[moveHistory.Length + 1];
            for (int i = 0; i < moveHistory.Length; i++)
            {
                newHistory[i] = moveHistory[i];
            }
            newHistory[moveHistory.Length] = move;
            moveHistory = newHistory;
        }
    }

    public bool HasMove(string move)
    {
        if (string.IsNullOrEmpty(move))
        {
            return false;
        }
        for (int i = 0; i < moveset.Length; i++)
        {
            if (moveset[i] == move)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasMoveInHistory(string move)
    {
        for (int i = 0; i < moveset.Length; i++)
        {
            if (moveset[i] == move)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanLearnMove(string move)
    {
        PokemonData thisPokemonData = PokemonDatabase.getPokemon(pokemonID);

        string[] moves = thisPokemonData.getMovesetMoves();
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] == move)
            {
                return true;
            }
        }
        moves = thisPokemonData.getTmList();
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] == move)
            {
                return true;
            }
        }
        return false;
    }

    public string MoveLearnedAtLevel(int level)
    {
        PokemonData thisPokemonData = PokemonDatabase.getPokemon(pokemonID);

        int[] movesetLevels = thisPokemonData.getMovesetLevels();
        for (int i = 0; i < movesetLevels.Length; i++)
        {
            if (movesetLevels[i] == level)
            {
                return thisPokemonData.getMovesetMoves()[i];
            }
        }
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
        return GetAnimFromID_("PokemonSprites", pokemonID, gender, isShiny);
    }

    public Sprite[] GetBackAnim_()
    {
        return GetAnimFromID_("PokemonBackSprites", pokemonID, gender, isShiny);
    }


    public static Sprite[] GetFrontAnimFromID_(int ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID_("PokemonSprites", ID, gender, isShiny);
    }

    public static Sprite[] GetBackAnimFromID_(int ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID_("PokemonBackSprites", ID, gender, isShiny);
    }

    private static Sprite[] GetAnimFromID_(string folder, int ID, PokemonGender gender, bool isShiny)
    {
        Sprite[] animation;
        string shiny = (isShiny) ? "s" : "";
        if (gender == PokemonGender.FEMALE)
        {
            //Attempt to load Female Variant
            animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + "f" + shiny + "/");
            if (animation.Length == 0)
            {
                Debug.LogWarning("Female Variant NOT Found");
                //Attempt to load Base Variant (possibly Shiny)
                animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + shiny + "/");
            }
            //	else{ Debug.Log("Female Variant Found"); }
        }
        else
        {
            //Attempt to load Base Variant (possibly Shiny)
            animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + shiny + "/");
        }
        if (animation.Length == 0 && isShiny)
        {
            Debug.LogWarning("Shiny Variant NOT Found");
            //No Shiny Variant exists, Attempt to load Regular Variant
            animation = Resources.LoadAll<Sprite>(folder + "/" + convertLongID(ID) + "/");
        }
        return animation;
    }


    public Sprite[] GetIcons_()
    {
        return GetIconsFromID_(pokemonID, isShiny);
    }

    public static Sprite[] GetIconsFromID_(int ID, bool isShiny)
    {
        string shiny = (isShiny) ? "s" : "";
        Sprite[] icons = Resources.LoadAll<Sprite>("PokemonIcons/icon" + convertLongID(ID) + shiny);
        if (icons == null)
        {
            Debug.LogWarning("Shiny Variant NOT Found");
            icons = Resources.LoadAll<Sprite>("PokemonIcons/icon" + convertLongID(ID));
        }
        return icons;
    }

    public float GetCryPitch()
    {
        return (status == PokemonStatus.FAINTED) ? 0.9f : 1f - (0.06f * (1 - getPercentHP()));
    }

    public AudioClip GetCry()
    {
        return GetCryFromID(pokemonID);
    }

    public static AudioClip GetCryFromID(int ID)
    {
        return Resources.Load<AudioClip>("Audio/cry/" + convertLongID(ID));
    }


    public Texture[] GetFrontAnim()
    {
        return GetAnimFromID("PokemonSprites", pokemonID, gender, isShiny);
    }

    public Texture[] GetBackAnim()
    {
        return GetAnimFromID("PokemonBackSprites", pokemonID, gender, isShiny);
    }

    public Texture GetIcons()
    {
        return GetIconsFromID(pokemonID, isShiny);
    }

    public Sprite[] GetSprite(bool getLight)
    {
        return GetSpriteFromID(pokemonID, isShiny, getLight);
    }


    public static Texture[] GetFrontAnimFromID(int ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID("PokemonSprites", ID, gender, isShiny);
    }

    public static Texture[] GetBackAnimFromID(int ID, PokemonGender gender, bool isShiny)
    {
        return GetAnimFromID("PokemonBackSprites", ID, gender, isShiny);
    }

    private static Texture[] GetAnimFromID(string folder, int ID, PokemonGender gender, bool isShiny)
    {
        Texture[] animation;
        string shiny = (isShiny) ? "s" : "";
        if (gender == PokemonGender.FEMALE)
        {
            //Attempt to load Female Variant
            animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + "f" + shiny + "/");
            if (animation.Length == 0)
            {
                Debug.LogWarning("Female Variant NOT Found (may not be required)");
                //Attempt to load Base Variant (possibly Shiny)
                animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + shiny + "/");
            }
            //	else{ Debug.Log("Female Variant Found");}
        }
        else
        {
            //Attempt to load Base Variant (possibly Shiny)
            animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + shiny + "/");
        }
        if (animation.Length == 0 && isShiny)
        {
            Debug.LogWarning("Shiny Variant NOT Found");
            //No Shiny Variant exists, Attempt to load Regular Variant
            animation = Resources.LoadAll<Texture>(folder + "/" + convertLongID(ID) + "/");
        }
        return animation;
    }

    public static Texture GetIconsFromID(int ID, bool isShiny)
    {
        string shiny = (isShiny) ? "s" : "";
        Texture icons = Resources.Load<Texture>("PokemonIcons/icon" + convertLongID(ID) + shiny);
        if (icons == null)
        {
            Debug.LogWarning("Shiny Variant NOT Found");
            icons = Resources.Load<Texture>("PokemonIcons/icon" + convertLongID(ID));
        }
        return icons;
    }


    public static Sprite[] GetSpriteFromID(int ID, bool isShiny, bool getLight)
    {
        string shiny = (isShiny) ? "s" : "";
        string light = (getLight) ? "Lights/" : "";
        Sprite[] spriteSheet = Resources.LoadAll<Sprite>("OverworldPokemonSprites/" + light + convertLongID(ID) + shiny);
        if (spriteSheet.Length == 0)
        {
            //No Light found AND/OR No Shiny found, load non-shiny
            if (isShiny)
            {
                if (getLight)
                {
                    Debug.LogWarning("Shiny Light NOT Found (may not be required)");
                }
                else
                {
                    Debug.LogWarning("Shiny Variant NOT Found");
                }
            }
            spriteSheet = Resources.LoadAll<Sprite>("OverworldPokemonSprites/" + light + convertLongID(ID));
        }
        if (spriteSheet.Length == 0)
        {
            //No Light found OR No Sprite found, return 8 blank sprites
            if (!getLight)
            {
                Debug.LogWarning("Sprite NOT Found");
            }
            else
            {
                Debug.LogWarning("Light NOT Found (may not be required)");
            }
            return new Sprite[8];
        }
        return spriteSheet;
    }
}
