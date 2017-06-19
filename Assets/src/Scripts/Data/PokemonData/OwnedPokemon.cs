//Original Scripts by IIColour (IIColour_Spectrum)

using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Since a pokemon's type can change in battle, we need to store its current types separated from its specie's base types.
    /// </summary>
    public PokemonType CurrentTypes { get; set; }

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
    /// May be a simple getter.
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

    /// <summary>
    /// All the data related to the 1st meeting.
    /// </summary>
    public MetData MetData;

    // ToDo: implement owner.
    ///// <summary>
    ///// if OT = null, pokemon may be caught.
    ///// </summary>
    //private string OT;
    //private int DONumber;

    public PokemonAbility CurrentAbility;

    #region ToDo: finish this implementation.
    public const int MOVESET_SIZE = 4;
    /// <summary>
    /// </summary>
    /// <remarks>Always size of 4 !</remarks>
    public OwnedPokemonMove[] CurrentMoveset;
    public List<PokemonMove> MoveHistory;

    // OLD
    private string[] _currentMoveset;
    private string[] _moveHistory;
    private int[] PPups;
    private int[] maxPP;
    private int[] PP;
    #endregion
    #endregion

    #region Constructors
    public OwnedPokemon() : this("-1") { }

    public OwnedPokemon(string pGameId)
    {
        // ToDo: Find in Database.
        Species = GameController.Instance.PokemonDb.GetPokemonSpeciesByGameId(pGameId);

        if (Species != null)
            Stats = Species.BaseStats.GenerateStatList();
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
        // Set status.
        CurrentStatus = PokemonStatus.NONE;

        // Set level and experience.
        CurrentLevel = pLevel;
        CurrentExperience = PokemonLevelingRateHelper.GetRequiredExperienceToTargetLevel(Species.LevelingRate, CurrentLevel);

        // Set friendship.
        CurrentFriendship = Species.BaseFriendship;

        // Set gender.
        generateGender(pGender);

        // Set rare value.
        generateRareValue();

        // Set met data.
        generateMetData(pLevel, pCaughtBall);

        // Set DO n°.
        generateDONumber();

        // Set IVs and EVs.
        generateIVsAndEVs();

        // Set nature.
        Nature = PokemonNatureHelper.GetRandomNature();

        // Set ability.
        generateAbility();

        // ToDo: Set default move set as 4 highest level learnable moves
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
        Nickname = pNickname;

        // Set status.
        CurrentStatus = pPokemon.CurrentStatus;

        // Set level and experience.
        CurrentLevel = pPokemon.CurrentLevel;
        CurrentExperience = pPokemon.CurrentExperience;

        // Set friendship.
        CurrentFriendship = pPokemon.Species.BaseFriendship;

        // Set gender.
        Gender = pPokemon.Gender;

        // Set rare value.
        RareValue = pPokemon.RareValue;

        // Set met data.
        MetData = pPokemon.MetData;

        // Set DO n°.
        generateDONumber();


        // Set nature.
        Nature = pPokemon.Nature;

        // Set ability.
        CurrentAbility = pPokemon.CurrentAbility;

        Stats = pPokemon.Stats;
        //// Copy IVs.
        //Stats[PokemonStatType.HP].IV = pPokemon.GetIV(PokemonStatType.HP);
        //Stats[PokemonStatType.Attack].IV = pPokemon.GetIV(PokemonStatType.Attack);
        //Stats[PokemonStatType.Defence].IV = pPokemon.GetIV(PokemonStatType.Defence);
        //Stats[PokemonStatType.SpecialAttack].IV = pPokemon.GetIV(PokemonStatType.SpecialAttack);
        //Stats[PokemonStatType.SpecialDefence].IV = pPokemon.GetIV(PokemonStatType.SpecialDefence);
        //Stats[PokemonStatType.Speed].IV = pPokemon.GetIV(PokemonStatType.Speed);

        //// Copy EVs.
        //Stats[PokemonStatType.HP].EV = pPokemon.GetEV(PokemonStatType.HP);
        //Stats[PokemonStatType.Attack].EV = pPokemon.GetEV(PokemonStatType.Attack);
        //Stats[PokemonStatType.Defence].EV = pPokemon.GetEV(PokemonStatType.Defence);
        //Stats[PokemonStatType.SpecialAttack].EV = pPokemon.GetEV(PokemonStatType.SpecialAttack);
        //Stats[PokemonStatType.SpecialDefence].EV = pPokemon.GetEV(PokemonStatType.SpecialDefence);
        //Stats[PokemonStatType.Speed].EV = pPokemon.GetEV(PokemonStatType.Speed);

        // MoveSet.
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

    #region Generation methods
    private void generateAbility()
    {
        int randomIndex = Random.Range(0, Species.PossibleAbilities.Count);
        CurrentAbility = Species.PossibleAbilities[randomIndex];
    }

    private void generateMetData(int pLevel, string pCaughtBall)
    {
        MetData.Level = pLevel;

        if (PlayerMovement.player == null
            || PlayerMovement.player.accessedMapSettings == null)
        {
            MetData.Location = "Somewhere";
        }
        else
        {
            MetData.Location = PlayerMovement.player.accessedMapSettings.mapName;
        }

        MetData.Date = DateTime.Now;

        MetData.CaughtBall = pCaughtBall;
    }

    private void generateGender(PokemonGender pGender)
    {
        if (pGender == PokemonGender.CALCULATE)
        {
            if (Species.MaleRatio < 0)
                Gender = PokemonGender.NONE;
            else if (Random.Range(0f, 100f) <= Species.MaleRatio)
                Gender = PokemonGender.MALE;
            else
                Gender = PokemonGender.FEMALE;
        }
        else
            Gender = pGender;
    }

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

    private void generateIVsAndEVs()
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

    private void generateDONumber()
    {
        // ToDo: implement owner.
        /*
          OT = (string.IsNullOrEmpty(pOT)) ? SaveData.currentSave.playerName : pOT;
          if (OT != SaveData.currentSave.playerName)
          {
              // if owned by another trainer, assign a random number. 
              DONumber = Random.Range(0, 65536); 
          }
          // this way if they trade it to you, it will have a different number to the player's.
          else
          {
              DONumber = SaveData.currentSave.playerID;
          }
        //*/
    }
    #endregion

    #region Stats
    public int GetCurrentLevelStatValue(PokemonStatType pStatType)
    {
        return Stats[pStatType].GetCurrentLevelValue(CurrentLevel, Nature);
    }

    public int GetCurrentStatValue(PokemonStatType pStatType)
    {
        return Stats[pStatType].GetCurrentValue(CurrentLevel, Nature);
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
    #endregion

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

    public void Tame()
    {
        // ToDo: implement pokemon catch.
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

    /// <summary>
    /// Checks PokemonData.cs for list of evolution method names.
    /// </summary>
    /// <param name="currentMethod"></param>
    /// <returns></returns>
    public bool canEvolve(string currentMethod)
    {
        // ToDo: Implement evolutions
        //var thisPokemonData = GameController.Instance.PokemonDb.GetPokemonSpeciesByGameId(Species.GameId);
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

    #region HP, PP and status battle
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

    public int healPP(int move, float amount)
    {
        // ToDo: implement moves and PP.
        //int excess = 0;
        //int intAmount = Mathf.RoundToInt(amount);
        //PP[move] += intAmount;
        //if (PP[move] > maxPP[move])
        //{
        //    excess = PP[move] - maxPP[move];
        //    PP[move] = maxPP[move];
        //}

        //return intAmount - excess;

        return 0;
    }

    public void removePP(string move, float amount)
    {
        // ToDo: implement moves and PP.
        //removePP(getMoveIndex(move), amount);
    }

    public void removePP(int move, float amount)
    {
        // ToDo: implement moves and PP.
        //if (move >= 0)
        //{
        //    int intAmount = Mathf.RoundToInt(amount);
        //    PP[move] -= intAmount;
        //    if (PP[move] < 0)
        //    {
        //        PP[move] = 0;
        //    }
        //}
    }

    public void healStatus()
    {
        CurrentStatus = PokemonStatus.NONE;
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

    public void healFull()
    {
        var lostHPMod = Stats[PokemonStatType.HP].Modifiers[PokemonStatModifierType.LostHP];
        if (lostHPMod != null)
            healHP(lostHPMod.CurrentValue);

        for (int i = 0; i != 4; ++i)
        {
            healPP(i, maxPP[i]);
        }

        healStatus();
    }
    #endregion


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

    public string getHeldItem()
    {
        // ToDo: implement items.
        return "";
    }

    public float getPercentHP()
    {
        return (float)GetCurrentStatValue(PokemonStatType.HP) / GetCurrentLevelStatValue(PokemonStatType.HP);
    }

    #region Moves
    public int GetMoveIndex(PokemonMove pMove)
    {
        for (int i = 0; i != MOVESET_SIZE; ++i)
        {
            if (CurrentMoveset[i].Move == pMove)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Keep ?
    /// </summary>
    /// <returns></returns>
    public PokemonMove[] GetMoveset()
    {
        PokemonMove[] moveset = new PokemonMove[MOVESET_SIZE];
        for (int i = 0; i != MOVESET_SIZE; ++i)
        {
            moveset[i] = CurrentMoveset[i].Move;
        }

        return moveset;
    }

    public void SwapMoves(int pFirstIndex, int pSecondIndex)
    {
        var temp = CurrentMoveset[pFirstIndex];
        CurrentMoveset[pFirstIndex] = CurrentMoveset[pSecondIndex];
        CurrentMoveset[pSecondIndex] = temp;
    }

    /// <summary>
    /// Tries to add a new move.
    /// </summary>
    /// <param name="pNewMove">The move to add.</param>
    /// <returns>Returns false if there is no room to add the new move OR move is already learned.</returns>
    public bool TryAddMove(PokemonMove pNewMove)
    {
        if (HasMove(pNewMove))
        {
            return false;
        }

        for (int i = 0; i != MOVESET_SIZE; ++i)
        {
            if (CurrentMoveset[i] == null)
            {
                CurrentMoveset[i] = new OwnedPokemonMove(pNewMove);
                return true;
            }
        }

        // All 4 slots are used.
        return false;
    }

    public void ReplaceMove(int pIndex, PokemonMove pNewMove)
    {
        if (pIndex < 0
            || pIndex >= 4)
            return;

        CurrentMoveset[pIndex] = new OwnedPokemonMove(pNewMove);
        addMoveToHistory(pNewMove);
    }

    /// <summary>
    /// Tries to forget the move at the specified index.
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns>Returns false if index is wrong or only one move is left in the moveset.</returns>
    public bool TryForgetMoveAt(int pIndex)
    {
        if (pIndex < 0
            || pIndex >= MOVESET_SIZE
            || GetMoveCount() <= 1)
            return false;

        CurrentMoveset[pIndex] = null;
        packMoveset();

        return true;
    }

    /// <summary>
    /// Gets the moves count.
    /// </summary>
    public int GetMoveCount()
    {
        int count = 0;
        for (int i = 0; i != MOVESET_SIZE; ++i)
        {
            if (CurrentMoveset[i] != null)
                ++count;
        }

        return count;
    }

    /// <summary>
    /// Reorders <see cref="CurrentMoveset"/> so every null entries are at the end of the array.
    /// </summary>
    private void packMoveset()
    {
        for (int i = 0, lastNullIndex = MOVESET_SIZE; i != MOVESET_SIZE; ++i)
        {
            // This entry is null.
            if (CurrentMoveset[i] == null)
            {
                // We store this index if it is the smallest index with a null entry.
                if (lastNullIndex >= i)
                    lastNullIndex = i;
            }
            // This entry is set.
            else
            {
                // Is there a null entry before ?
                if (lastNullIndex < i)
                {
                    // We swap the moves.
                    SwapMoves(lastNullIndex, i);

                    // We go back to the just set index.
                    i = lastNullIndex;

                    // We reset our lastNullIndex.
                    lastNullIndex = MOVESET_SIZE;
                }
            }
        }
    }

    private void addMoveToHistory(PokemonMove pMove)
    {
        if (MoveHistory.Contains(pMove))
            return;

        MoveHistory.Add(pMove);
    }

    public bool HasMove(PokemonMove pMove)
    {
        for (int i = 0; i != MOVESET_SIZE; ++i)
        {
            if (CurrentMoveset[i].Move == pMove)
                return true;
        }

        return false;
    }

    public bool CanLearnMove(string move)
    {
        // ToDo: Implement moves.
        //var thisPokemonData = GameController.Instance.PokemonDb.GetPokemonSpeciesByGameId(Species.GameId);

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
        //var thisPokemonData = GameController.Instance.PokemonDb.GetPokemonSpeciesByGameId(Species.GameId);

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
    #endregion

    #region Sound
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
    #endregion

    #region View
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
    #endregion
}
