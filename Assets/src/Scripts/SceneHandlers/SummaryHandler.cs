﻿//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SummaryHandler : MonoBehaviour
{
    public string replacedMove;

    private Image
        selectedCaughtBall,
        selectedSprite,
        selectedStatus,
        selectedShiny;

    // ToDo: Replace all shadows by components !!!
    private Text
        selectedName,
        selectedNameShadow,
        selectedGender,
        selectedGenderShadow,
        selectedLevel,
        selectedLevelShadow,
        selectedHeldItem,
        selectedHeldItemShadow;

    private int frame = 0;
    private Sprite[] selectedSpriteAnimation;

    private GameObject[] pages = new GameObject[8];

    private Text
        dexNo,
        dexNoShadow,
        species,
        speciesShadow,
        OT,
        OTShadow,
        IDNo,
        IDNoShadow,
        expPoints,
        expPointsShadow,
        toNextLevel,
        toNextLevelShadow;

    private Image
        type1,
        type2,
        expBar;

    private Text
        nature,
        natureShadow,
        metDate,
        metDateShadow,
        metMap,
        metMapShadow,
        metLevel,
        metLevelShadow,
        characteristic,
        characteristicShadow;

    private Image HPBar;

    private Text
        //HPText,
        //HPTextShadow,
        HP,
        HPShadow,
        StatsTextShadow,
        Stats,
        StatsShadow,
        abilityName,
        abilityNameShadow,
        abilityDescription,
        abilityDescriptionShadow;

    private RectTransform moves;

    private Image
        moveSelector,
        selectedMove,
        selectedCategory;

    private Text[] moveNames;
    private Text[] moveNameShadows;
    private Image[] moveTypes;
    private Text[] movePPTexts;
    private Text[] movePPTextShadows;
    private Text[] movePPs;
    private Text[] movePPShadows;

    private Text
        selectedPower,
        selectedPowerShadow,
        selectedAccuracy,
        selectedAccuracyShadow,
        selectedDescription,
        selectedDescriptionShadow;

    private GameObject learnScreen;

    private RectTransform newMove;

    private Image
        moveNewType;

    private Text
        moveNewName,
        moveNewNameShadow,
        moveNewPPText,
        moveNewPPTextShadow,
        moveNewPP,
        moveNewPPShadow;

    private GameObject forget;

    //ribbons not yet implemented.

    public AudioClip selectClip;
    public AudioClip scrollClip;
    public AudioClip returnClip;


    void Awake()
    {
        Transform selectedInfo = transform.FindChild("SelectedInfo");

        selectedCaughtBall = selectedInfo.FindChild("SelectedCaughtBall").GetComponent<Image>();
        selectedNameShadow = selectedInfo.FindChild("SelectedName").GetComponent<Text>();
        selectedName = selectedNameShadow.transform.FindChild("Text").GetComponent<Text>();
        selectedGenderShadow = selectedInfo.FindChild("SelectedGender").GetComponent<Text>();
        selectedGender = selectedGenderShadow.transform.FindChild("Text").GetComponent<Text>();
        selectedLevelShadow = selectedInfo.FindChild("SelectedLevel").GetComponent<Text>();
        selectedLevel = selectedLevelShadow.transform.FindChild("Text").GetComponent<Text>();
        selectedSprite = selectedInfo.FindChild("SelectedSprite").GetComponent<Image>();
        selectedHeldItemShadow = selectedInfo.FindChild("SelectedHeldItem").GetComponent<Text>();
        selectedHeldItem = selectedHeldItemShadow.transform.FindChild("Text").GetComponent<Text>();
        selectedStatus = selectedInfo.FindChild("SelectedStatus").GetComponent<Image>();
        selectedShiny = selectedInfo.FindChild("SelectedShiny").GetComponent<Image>();

        pages[1] = transform.FindChild("1").gameObject;

        dexNoShadow = pages[1].transform.FindChild("DexNo").GetComponent<Text>();
        dexNo = dexNoShadow.transform.FindChild("Text").GetComponent<Text>();
        speciesShadow = pages[1].transform.FindChild("Species").GetComponent<Text>();
        species = speciesShadow.transform.FindChild("Text").GetComponent<Text>();
        type1 = pages[1].transform.FindChild("Type1").GetComponent<Image>();
        type2 = pages[1].transform.FindChild("Type2").GetComponent<Image>();
        OTShadow = pages[1].transform.FindChild("OT").GetComponent<Text>();
        OT = OTShadow.transform.FindChild("Text").GetComponent<Text>();
        IDNoShadow = pages[1].transform.FindChild("IDNo").GetComponent<Text>();
        IDNo = IDNoShadow.transform.FindChild("Text").GetComponent<Text>();
        expPointsShadow = pages[1].transform.FindChild("ExpPoints").GetComponent<Text>();
        expPoints = expPointsShadow.transform.FindChild("Text").GetComponent<Text>();
        toNextLevelShadow = pages[1].transform.FindChild("ToNextLevel").GetComponent<Text>();
        toNextLevel = toNextLevelShadow.transform.FindChild("Text").GetComponent<Text>();
        expBar = pages[1].transform.FindChild("ExpBar").GetComponent<Image>();

        pages[2] = transform.FindChild("2").gameObject;

        natureShadow = pages[2].transform.FindChild("Nature").GetComponent<Text>();
        nature = natureShadow.transform.FindChild("Text").GetComponent<Text>();
        metDateShadow = pages[2].transform.FindChild("MetDate").GetComponent<Text>();
        metDate = metDateShadow.transform.FindChild("Text").GetComponent<Text>();
        metMapShadow = pages[2].transform.FindChild("MetMap").GetComponent<Text>();
        metMap = metMapShadow.transform.FindChild("Text").GetComponent<Text>();
        metLevelShadow = pages[2].transform.FindChild("MetLevel").GetComponent<Text>();
        metLevel = metLevelShadow.transform.FindChild("Text").GetComponent<Text>();
        characteristicShadow = pages[2].transform.FindChild("Characteristic").GetComponent<Text>();
        characteristic = characteristicShadow.transform.FindChild("Text").GetComponent<Text>();

        pages[3] = transform.FindChild("3").gameObject;

        //HPTextShadow = pages[3].transform.FindChild("HPText").GetComponent<Text>();
        //HPText = HPTextShadow.transform.FindChild("Text").GetComponent<Text>();
        HPShadow = pages[3].transform.FindChild("HP").GetComponent<Text>();
        HP = HPShadow.transform.FindChild("Text").GetComponent<Text>();
        HPBar = pages[3].transform.FindChild("HPBar").GetComponent<Image>();
        StatsShadow = pages[3].transform.FindChild("Stats").GetComponent<Text>();
        Stats = StatsShadow.transform.FindChild("Text").GetComponent<Text>();
        StatsTextShadow = pages[3].transform.FindChild("StatsText").GetComponent<Text>();
        abilityNameShadow = pages[3].transform.FindChild("AbilityName").GetComponent<Text>();
        abilityName = abilityNameShadow.transform.FindChild("Text").GetComponent<Text>();
        abilityDescriptionShadow = pages[3].transform.FindChild("AbilityDescription").GetComponent<Text>();
        abilityDescription = abilityDescriptionShadow.transform.FindChild("Text").GetComponent<Text>();

        pages[4] = transform.FindChild("4").gameObject;

        moves = pages[4].transform.FindChild("Moves").GetComponent<RectTransform>();

        moveSelector = pages[4].transform.FindChild("MoveSelector").GetComponent<Image>();
        selectedMove = pages[4].transform.FindChild("SelectedMove").GetComponent<Image>();

        Text[] moveNames = new Text[OwnedPokemon.MOVESET_SIZE];
        Text[] moveNameShadows = new Text[OwnedPokemon.MOVESET_SIZE];
        Image[] moveTypes = new Image[OwnedPokemon.MOVESET_SIZE];
        Text[] movePPTexts = new Text[OwnedPokemon.MOVESET_SIZE];
        Text[] movePPTextShadows = new Text[OwnedPokemon.MOVESET_SIZE];
        Text[] movePPs = new Text[OwnedPokemon.MOVESET_SIZE];
        Text[] movePPShadows = new Text[OwnedPokemon.MOVESET_SIZE];

        for (int i = 1; i <= OwnedPokemon.MOVESET_SIZE; ++i)
        {
            moveNameShadows[i] = moves.FindChild(string.Format("Move{0}", i.ToString())).GetComponent<Text>();
            moveNames[i] = moveNameShadows[i].transform.FindChild("Text").GetComponent<Text>();
            moveTypes[i] = moves.FindChild(string.Format("Move{0}Type", i.ToString())).GetComponent<Image>();
            movePPTextShadows[i] = moves.FindChild(string.Format("Move{0}PPText", i.ToString())).GetComponent<Text>();
            movePPTexts[i] = movePPTextShadows[i].transform.FindChild("Text").GetComponent<Text>();
            movePPShadows[i] = moves.FindChild(string.Format("Move{0}PP", i.ToString())).GetComponent<Text>();
            movePPs[i] = movePPShadows[i].transform.FindChild("Text").GetComponent<Text>();
        }

        selectedCategory = pages[4].transform.FindChild("SelectedCategory").GetComponent<Image>();
        selectedPowerShadow = pages[4].transform.FindChild("SelectedPower").GetComponent<Text>();
        selectedPower = selectedPowerShadow.transform.FindChild("Text").GetComponent<Text>();
        selectedAccuracyShadow = pages[4].transform.FindChild("SelectedAccuracy").GetComponent<Text>();
        selectedAccuracy = selectedAccuracyShadow.transform.FindChild("Text").GetComponent<Text>();
        selectedDescriptionShadow = pages[4].transform.FindChild("SelectedDescription").GetComponent<Text>();
        selectedDescription = selectedDescriptionShadow.transform.FindChild("Text").GetComponent<Text>();

        learnScreen = pages[4].transform.FindChild("Learn").gameObject;

        newMove = pages[4].transform.FindChild("NewMove").GetComponent<RectTransform>();

        moveNewNameShadow = newMove.FindChild("MoveNew").GetComponent<Text>();
        moveNewName = moveNewNameShadow.transform.FindChild("Text").GetComponent<Text>();
        moveNewType = newMove.FindChild("MoveNewType").GetComponent<Image>();
        moveNewPPTextShadow = newMove.FindChild("MoveNewPPText").GetComponent<Text>();
        moveNewPPText = moveNewPPTextShadow.transform.FindChild("Text").GetComponent<Text>();
        moveNewPPShadow = newMove.FindChild("MoveNewPP").GetComponent<Text>();
        moveNewPP = moveNewPPShadow.transform.FindChild("Text").GetComponent<Text>();
        forget = newMove.FindChild("Forget").gameObject;


        pages[5] = transform.FindChild("5").gameObject;

        pages[6] = transform.FindChild("6").gameObject;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }


    private void updateSelection(OwnedPokemon selectedPokemon)
    {
        frame = 0;

        PlayCry(selectedPokemon);

        selectedCaughtBall.sprite = Resources.Load<Sprite>("null");
        selectedCaughtBall.sprite = Resources.Load<Sprite>("PCSprites/summary" + selectedPokemon.MetData.CaughtBall);
        selectedName.text = selectedPokemon.GetName();
        selectedNameShadow.text = selectedName.text;
        if (selectedPokemon.Gender == PokemonGender.FEMALE)
        {
            selectedGender.text = "♀";
            selectedGender.color = new Color(1, 0.2f, 0.2f, 1);
        }
        else if (selectedPokemon.Gender == PokemonGender.MALE)
        {
            selectedGender.text = "♂";
            selectedGender.color = new Color(0.2f, 0.4f, 1, 1);
        }
        else
        {
            selectedGender.text = null;
        }
        selectedGenderShadow.text = selectedGender.text;
        selectedLevel.text = "" + selectedPokemon.CurrentLevel;
        selectedLevelShadow.text = selectedLevel.text;
        selectedSpriteAnimation = selectedPokemon.GetFrontAnim_();
        if (selectedSpriteAnimation.Length > 0)
        {
            selectedSprite.sprite = selectedSpriteAnimation[0];
        }
        if (string.IsNullOrEmpty(selectedPokemon.getHeldItem()))
        {
            selectedHeldItem.text = "None";
        }
        else
        {
            selectedHeldItem.text = selectedPokemon.getHeldItem();
        }
        selectedHeldItemShadow.text = selectedHeldItem.text;
        if (selectedPokemon.CurrentStatus != PokemonStatus.NONE)
        {
            selectedStatus.sprite = Resources.Load<Sprite>("PCSprites/status" + selectedPokemon.CurrentStatus.ToString());
        }
        else
        {
            selectedStatus.sprite = Resources.Load<Sprite>("null");
        }

        if (selectedPokemon.IsShiny)
        {
            selectedShiny.sprite = Resources.Load<Sprite>("PCSprites/shiny");
        }
        else
        {
            selectedShiny.sprite = Resources.Load<Sprite>("null");
        }

        dexNo.text = selectedPokemon.Species.GameId;
        dexNoShadow.text = dexNo.text;
        species.text = selectedPokemon.Species.Name;
        speciesShadow.text = species.text;

        // ToDo: implement types.
        //string type1string = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(selectedPokemon.getID()).getType1().ToString();
        //string type2string = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(selectedPokemon.getID()).getType2().ToString();
        //type1.sprite = Resources.Load<Sprite>("null");
        //type2.sprite = Resources.Load<Sprite>("null");
        //if (type1string != "NONE")
        //{
        //    type1.sprite = Resources.Load<Sprite>("PCSprites/type" + type1string);
        //    type1.rectTransform.localPosition = new Vector3(71, type1.rectTransform.localPosition.y);
        //}
        //if (type2string != "NONE")
        //{
        //    type2.sprite = Resources.Load<Sprite>("PCSprites/type" + type2string);
        //}
        //else
        //{
        //    //if single type pokemon, center the type icon
        //    type1.rectTransform.localPosition = new Vector3(89, type1.rectTransform.localPosition.y);
        //}

        // ToDo: implement owner.
        //OT.text = selectedPokemon.getOT();
        //OTShadow.text = OT.text;
        //IDNo.text = "" + selectedPokemon.getIDno();
        //IDNoShadow.text = IDNo.text;

        expPoints.text = "" + selectedPokemon.CurrentExperience;
        expPointsShadow.text = expPoints.text;
        float expCurrentLevel =
            PokemonLevelingRateHelper.GetRequiredExperienceToTargetLevel(selectedPokemon.Species.LevelingRate,
                selectedPokemon.CurrentLevel);
        float expNextlevel =
            PokemonLevelingRateHelper.GetRequiredExperienceToTargetLevel(selectedPokemon.Species.LevelingRate,
                selectedPokemon.CurrentLevel + 1);
        float expAlong = selectedPokemon.CurrentExperience - expCurrentLevel;
        float expDistance = expAlong / (expNextlevel - expCurrentLevel);
        toNextLevel.text = "" + (expNextlevel - selectedPokemon.CurrentExperience);
        toNextLevelShadow.text = toNextLevel.text;
        expBar.rectTransform.sizeDelta = new Vector2(Mathf.Floor(expDistance * 64f), expBar.rectTransform.sizeDelta.y);

        string natureFormatted = PokemonNatureHelper.GetNatureName(selectedPokemon.Nature);
        natureFormatted = natureFormatted.Substring(0, 1) + natureFormatted.Substring(1).ToLowerInvariant();
        nature.text = "<color=#F22F>" + natureFormatted + "</color> nature.";
        natureShadow.text = natureFormatted + " nature.";
        metDate.text = "Met on " + selectedPokemon.MetData.Date;
        metDateShadow.text = metDate.text;
        metMap.text = "<color=#F22F>" + selectedPokemon.MetData.Location + "</color>";
        metMapShadow.text = selectedPokemon.MetData.Location;
        metLevel.text = "Met at Level " + selectedPokemon.MetData.Level + ".";
        metLevelShadow.text = metLevel.text;

        string[][] characteristics = new string[][]
        {
            new string[]
            {
                "Loves to eat", "Takes plenty of siestas", "Nods off a lot", "Scatters things often", "Likes to relax"
            },
            new string[]
            {
                "Proud of its power", "Likes to thrash about", "A little quick tempered", "Likes to fight",
                "Quick tempered"
            },
            new string[]
            {
                "Sturdy body", "Capable of taking hits", "Highly persistent", "Good endurance", "Good perseverance"
            },
            new string[]
            {
                "Highly curious", "Mischievous", "Thoroughly cunning", "Often lost in thought", "Very finicky"
            },
            new string[]
            {
                "Strong willed", "Somewhat vain", "Strongly defiant", "Hates to lose", "Somewhat stubborn"
            },
            new string[]
            {
                "Likes to run", "Alert to sounds", "Impetuous and silly", "Somewhat of a clown", "Quick to flee"
            }
        };

        var highestIV = selectedPokemon.GetHighestIV();
        characteristic.text = characteristics[(int)highestIV][selectedPokemon.GetIV(highestIV) % 5] + ".";
        characteristicShadow.text = characteristic.text;

        float currentHP = selectedPokemon.GetCurrentStatValue(PokemonStatType.HP);
        float maxHP = selectedPokemon.GetCurrentLevelStatValue(PokemonStatType.HP);
        HP.text = currentHP + "/" + maxHP;
        HPShadow.text = HP.text;
        HPBar.rectTransform.sizeDelta = new Vector2(selectedPokemon.getPercentHP() * 48f,
            HPBar.rectTransform.sizeDelta.y);

        if (currentHP < (maxHP / 4f))
        {
            HPBar.color = new Color(1, 0.125f, 0, 1);
        }
        else if (currentHP < (maxHP / 2f))
        {
            HPBar.color = new Color(1, 0.75f, 0, 1);
        }
        else
        {
            HPBar.color = new Color(0.125f, 1, 0.065f, 1);
        }

        //float[] natureMod = new float[]
        //{
        //    NatureDatabase.getNature(selectedPokemon.Nature).getATK(),
        //    NatureDatabase.getNature(selectedPokemon.Nature).getDEF(),
        //    NatureDatabase.getNature(selectedPokemon.Nature).getSPA(),
        //    NatureDatabase.getNature(selectedPokemon.Nature).getSPD(),
        //    NatureDatabase.getNature(selectedPokemon.Nature).getSPE()
        //};
        Stats.text =
            selectedPokemon.GetCurrentLevelStatValue(PokemonStatType.Attack) + "\n" +
            selectedPokemon.GetCurrentLevelStatValue(PokemonStatType.Defence) + "\n" +
            selectedPokemon.GetCurrentLevelStatValue(PokemonStatType.SpecialAttack) + "\n" +
            selectedPokemon.GetCurrentLevelStatValue(PokemonStatType.SpecialDefence) + "\n" +
            selectedPokemon.GetCurrentLevelStatValue(PokemonStatType.Speed);
        StatsShadow.text = Stats.text;

        //string[] statsLines = new string[] { "Attack", "Defence", "Sp. Atk", "Sp. Def", "Speed" };
        StatsTextShadow.text = "";
        //for (int i = 0; i < 5; i++)
        //{
        //    if (natureMod[i] > 1)
        //    {
        //        StatsTextShadow.text += "<color=#A01010FF>" + statsLines[i] + "</color>\n";
        //    }
        //    else if (natureMod[i] < 1)
        //    {
        //        StatsTextShadow.text += "<color=#0030A2FF>" + statsLines[i] + "</color>\n";
        //    }
        //    else
        //    {
        //        StatsTextShadow.text += statsLines[i] + "\n";
        //    }
        //}


        //abilityName.text = PokemonDatabase.Instance.GetPokemonSpeciesByGameId(selectedPokemon.getID().ToString()).getAbility(selectedPokemon.getAbility());
        abilityNameShadow.text = abilityName.text;
        //abilities not yet implemented
        abilityDescription.text = "";
        abilityDescriptionShadow.text = abilityDescription.text;

        updateSelectionMoveset(selectedPokemon);
    }

    private void updateSelectionMoveset(OwnedPokemon selectedPokemon)
    {
        var moveset = selectedPokemon.CurrentMoveset;

        for (int i = 0; i != OwnedPokemon.MOVESET_SIZE; ++i)
        {
            var ownedPokemonMove = moveset[i];
            if (ownedPokemonMove != null)
            {
                moveNames[i].text = ownedPokemonMove.Move.Name;

                moveTypes[i].sprite = Resources.Load<Sprite>("PCSprites/type" + PokemonTypeHelper.GetName(ownedPokemonMove.Move.CurrentType));

                movePPTexts[i].text = "PP";

                movePPs[i].text = ownedPokemonMove.CurrentPP + "/" + ownedPokemonMove.CurrentMaxPP;
            }
            else
            {
                moveNames[i].text = null;

                moveTypes[i].sprite = Resources.Load<Sprite>("null");

                movePPTexts[i].text = null;

                movePPs[i].text = null;
            }

            moveNameShadows[i].text = moveNames[i].text;
            movePPTextShadows[i].text = movePPTexts[i].text;
            movePPShadows[i].text = movePPs[i].text;
        }

        updateSelectedMove(null);
    }

    private void updateMoveToLearn(string moveName)
    {
        MoveData move = MoveDatabase.getMove(moveName);
        moveNewName.text = moveName;
        moveNewNameShadow.text = moveNewName.text;
        moveNewType.sprite = Resources.Load<Sprite>("PCSprites/type" + move.getType().ToString());
        moveNewPPText.text = "PP";
        moveNewPPTextShadow.text = moveNewPPText.text;
        moveNewPP.text = move.getPP() + "/" + move.getPP();
        moveNewPPShadow.text = moveNewPP.text;
    }

    private void updateSelectedMove(string moveName)
    {
        if (string.IsNullOrEmpty(moveName))
        {
            selectedCategory.sprite = Resources.Load<Sprite>("null");
            selectedPower.text = null;
            selectedPowerShadow.text = selectedPower.text;
            selectedAccuracy.text = null;
            selectedAccuracyShadow.text = selectedAccuracy.text;
            selectedDescription.text = null;
            selectedDescriptionShadow.text = selectedDescription.text;
        }
        else
        {
            MoveData selectedMove = MoveDatabase.getMove(moveName);
            selectedCategory.sprite =
                Resources.Load<Sprite>("PCSprites/category" + selectedMove.getCategory().ToString());
            selectedPower.text = "" + selectedMove.getPower();
            if (selectedPower.text == "0")
            {
                selectedPower.text = "-";
            }
            selectedPowerShadow.text = selectedPower.text;
            selectedAccuracy.text = "" + Mathf.Round(selectedMove.getAccuracy() * 100f);
            if (selectedAccuracy.text == "0")
            {
                selectedAccuracy.text = "-";
            }
            selectedAccuracyShadow.text = selectedAccuracy.text;
            selectedDescription.text = selectedMove.getDescription();
            selectedDescriptionShadow.text = selectedDescription.text;
        }
    }

    private IEnumerator moveMoveSelector(Vector3 destinationPosition)
    {
        Vector3 startPosition = moveSelector.rectTransform.localPosition;
        Vector3 distance = destinationPosition - startPosition;

        float increment = 0f;
        float speed = 0.2f;
        while (increment < 1)
        {
            increment += (1f / speed) * Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }
            moveSelector.rectTransform.localPosition = startPosition + (distance * increment);
            yield return null;
        }
    }

    private IEnumerator animatePokemon()
    {
        frame = 0;
        while (true)
        {
            if (selectedSpriteAnimation.Length > 0)
            {
                frame = (frame < selectedSpriteAnimation.Length - 1) ? frame + 1 : 0;
                selectedSprite.sprite = selectedSpriteAnimation[frame];
            }
            yield return new WaitForSeconds(0.08f);
        }
    }

    private void PlayCry(OwnedPokemon pokemon)
    {
        SfxHandler.Play(pokemon.GetCry(), pokemon.GetCryPitch());
    }


    public IEnumerator control(OwnedPokemon[] pokemonList, int currentPosition)
    {
        yield return StartCoroutine(control(pokemonList, currentPosition, false, null));
    }

    public IEnumerator control(OwnedPokemon pokemon, string newMoveString)
    {
        yield return StartCoroutine(control(new OwnedPokemon[] { pokemon }, 0, true, newMoveString));
    }

    public IEnumerator control(OwnedPokemon[] pokemonList, int currentPosition, bool learning, string newMoveString)
    {
        moves.localPosition = (learning) ? new Vector3(0, 32) : Vector3.zero;
        newMove.gameObject.SetActive(learning);
        learnScreen.SetActive(learning);

        moveSelector.enabled = false;
        selectedMove.enabled = false;

        forget.SetActive(false);

        pages[1].SetActive(!learning);
        pages[2].SetActive(false);
        pages[3].SetActive(false);
        pages[4].SetActive(learning);
        pages[5].SetActive(false);
        pages[6].SetActive(false);

        updateSelection(pokemonList[currentPosition]);
        if (learning)
        {
            updateMoveToLearn(newMoveString);
        }

        StartCoroutine("animatePokemon");

        bool running = true;
        int currentPage = (learning) ? 4 : 1;
        int checkPosition = currentPosition;

        replacedMove = null;

        yield return StartCoroutine(ScreenFade.main.Fade(true, ScreenFade.defaultSpeed));

        if (learning)
        {
            yield return StartCoroutine(NavigateMoves(pokemonList[currentPosition], true, newMoveString));
        }
        else
        {
            while (running)
            {
                //cycle through the pages
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    if (currentPage > 1)
                    {
                        pages[currentPage - 1].SetActive(true);
                        pages[currentPage].SetActive(false);
                        currentPage -= 1;
                        SfxHandler.Play(scrollClip);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                else if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    if (currentPage < 6)
                    {
                        pages[currentPage + 1].SetActive(true);
                        pages[currentPage].SetActive(false);
                        currentPage += 1;
                        SfxHandler.Play(scrollClip);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                //cycle through pokemon
                else if (Input.GetAxisRaw("Vertical") > 0)
                {
                    checkPosition = currentPosition;
                    if (checkPosition > 0)
                    {
                        checkPosition -= 1;
                    }
                    while (checkPosition > 0 && pokemonList[checkPosition] == null)
                    {
                        checkPosition -= 1;
                    }
                    if (pokemonList[checkPosition] != null && checkPosition != currentPosition)
                    {
                        currentPosition = checkPosition;
                        updateSelection(pokemonList[checkPosition]);
                        //SfxHandler.Play(scrollClip);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                else if (Input.GetAxisRaw("Vertical") < 0)
                {
                    checkPosition = currentPosition;
                    if (checkPosition < pokemonList.Length - 1)
                    {
                        checkPosition += 1;
                    }
                    while (checkPosition < pokemonList.Length - 1 && pokemonList[checkPosition] == null)
                    {
                        checkPosition += 1;
                    }
                    if (pokemonList[checkPosition] != null && checkPosition != currentPosition)
                    {
                        currentPosition = checkPosition;
                        updateSelection(pokemonList[checkPosition]);
                        //SfxHandler.Play(scrollClip);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                //rearrange moves/close summary
                else if (Input.GetButton("Select"))
                {
                    if (currentPage == 4)
                    {
                        if (pokemonList[currentPosition].GetMoveset()[0] != null)
                        {
                            //if there are moves to rearrange
                            SfxHandler.Play(selectClip);
                            yield return StartCoroutine(NavigateMoves(pokemonList[currentPosition], false, ""));
                        }
                    }
                    else if (currentPage == 6)
                    {
                        running = false;
                    }
                }
                else if (Input.GetButton("Back"))
                {
                    running = false;
                }

                yield return null;
            }
        }

        yield return StartCoroutine(ScreenFade.main.Fade(false, ScreenFade.defaultSpeed));
        this.gameObject.SetActive(false);
    }

    private IEnumerator NavigateMoves(OwnedPokemon pokemon, bool learning, string newMoveString)
    {
        yield break;
        // ToDO: Redo this.
        //learnScreen.SetActive(learning);
        //newMove.gameObject.SetActive(learning);
        //Vector3 positionMod = (learning) ? new Vector3(0, 32) : new Vector3(0, 0);
        //moves.localPosition = positionMod;
        //if (learning)
        //{
        //    updateMoveToLearn(newMoveString);
        //}

        //string[] pokeMoveset = pokemon.GetMoveset();
        //string[] moveset = new string[]
        //{
        //    pokeMoveset[0], pokeMoveset[1],
        //    pokeMoveset[2], pokeMoveset[3],
        //    newMoveString, newMoveString
        //};
        //Vector3[] positions = new Vector3[]
        //{
        //    new Vector3(21, 32), new Vector3(108, 32),
        //    new Vector3(21, 0), new Vector3(108, 0),
        //    new Vector3(64, -32), new Vector3(64, -32)
        //};

        //moveSelector.enabled = true;
        //selectedMove.enabled = false;

        //bool navigatingMoves = true;
        //bool selectingMove = false;
        //int currentMoveNumber = 0;
        //int selectedMoveNumber = -1;

        //moveSelector.rectTransform.localPosition = positions[0] + positionMod;
        //updateSelectedMove(moveset[currentMoveNumber]);
        //yield return null;
        //while (navigatingMoves)
        //{
        //    if (Input.GetAxisRaw("Horizontal") < 0)
        //    {
        //        if (currentMoveNumber == 1)
        //        {
        //            currentMoveNumber = 0;
        //            updateSelectedMove(moveset[currentMoveNumber]);
        //            SfxHandler.Play(scrollClip);
        //            yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //        }
        //        else if (currentMoveNumber == 3)
        //        {
        //            if (!string.IsNullOrEmpty(moveset[2]))
        //            {
        //                currentMoveNumber = 2;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //        else if (learning)
        //        {
        //            if (currentMoveNumber == 5)
        //            {
        //                currentMoveNumber = 4;
        //            }
        //        }
        //    }
        //    else if (Input.GetAxisRaw("Horizontal") > 0)
        //    {
        //        if (currentMoveNumber == 0)
        //        {
        //            if (!string.IsNullOrEmpty(moveset[1]))
        //            {
        //                currentMoveNumber = 1;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //        else if (currentMoveNumber == 2)
        //        {
        //            if (!string.IsNullOrEmpty(moveset[3]))
        //            {
        //                currentMoveNumber = 3;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //        else if (learning)
        //        {
        //            if (currentMoveNumber == 4)
        //            {
        //                currentMoveNumber = 5;
        //            }
        //        }
        //    }
        //    else if (Input.GetAxisRaw("Vertical") > 0)
        //    {
        //        if (currentMoveNumber == 2)
        //        {
        //            currentMoveNumber = 0;
        //            updateSelectedMove(moveset[currentMoveNumber]);
        //            SfxHandler.Play(scrollClip);
        //            yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //        }
        //        else if (currentMoveNumber == 3)
        //        {
        //            if (!string.IsNullOrEmpty(moveset[1]))
        //            {
        //                currentMoveNumber = 1;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //        else if (learning)
        //        {
        //            if (currentMoveNumber == 4)
        //            {
        //                if (!string.IsNullOrEmpty(moveset[2]))
        //                {
        //                    currentMoveNumber = 2;
        //                }
        //                else
        //                {
        //                    currentMoveNumber = 0;
        //                }
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //            else if (currentMoveNumber == 5)
        //            {
        //                if (!string.IsNullOrEmpty(moveset[3]))
        //                {
        //                    currentMoveNumber = 3;
        //                }
        //                else if (!string.IsNullOrEmpty(moveset[1]))
        //                {
        //                    currentMoveNumber = 1;
        //                }
        //                else
        //                {
        //                    currentMoveNumber = 0;
        //                }
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //    }
        //    else if (Input.GetAxisRaw("Vertical") < 0)
        //    {
        //        if (currentMoveNumber == 0)
        //        {
        //            if (!string.IsNullOrEmpty(moveset[2]))
        //            {
        //                currentMoveNumber = 2;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //            else if (learning)
        //            {
        //                currentMoveNumber = 4;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //        else if (currentMoveNumber == 1)
        //        {
        //            if (!string.IsNullOrEmpty(moveset[3]))
        //            {
        //                currentMoveNumber = 3;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //            else if (learning)
        //            {
        //                currentMoveNumber = 5;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //        else if (learning)
        //        {
        //            if (currentMoveNumber == 2)
        //            {
        //                currentMoveNumber = 4;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //            else if (currentMoveNumber == 3)
        //            {
        //                currentMoveNumber = 5;
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(scrollClip);
        //                yield return StartCoroutine(moveMoveSelector(positions[currentMoveNumber] + positionMod));
        //            }
        //        }
        //    }
        //    else if (Input.GetButtonDown("Back"))
        //    {
        //        if (!learning)
        //        {
        //            if (selectingMove)
        //            {
        //                selectingMove = false;
        //                selectedMove.enabled = false;
        //                yield return new WaitForSeconds(0.2f);
        //            }
        //            else
        //            {
        //                navigatingMoves = false;
        //                moveSelector.enabled = false;
        //                updateSelectedMove(null);
        //                SfxHandler.Play(returnClip);
        //                yield return new WaitForSeconds(0.2f);
        //            }
        //        }
        //        else
        //        {
        //            //Cancel learning move
        //            navigatingMoves = false;
        //            SfxHandler.Play(returnClip);
        //            yield return new WaitForSeconds(0.2f);
        //        }
        //    }
        //    else if (Input.GetButtonDown("Select"))
        //    {
        //        if (!learning)
        //        {
        //            if (selectingMove)
        //            {
        //                pokemon.SwapMoves(selectedMoveNumber, currentMoveNumber);
        //                selectingMove = false;
        //                selectedMove.enabled = false;
        //                moveset = pokemon.GetMoveset();
        //                updateSelectionMoveset(pokemon);
        //                updateSelectedMove(moveset[currentMoveNumber]);
        //                SfxHandler.Play(selectClip);
        //                yield return new WaitForSeconds(0.2f);
        //            }
        //            else
        //            {
        //                selectedMoveNumber = currentMoveNumber;
        //                selectingMove = true;
        //                selectedMove.rectTransform.localPosition = positions[currentMoveNumber] + positionMod;
        //                selectedMove.enabled = true;
        //                SfxHandler.Play(selectClip);
        //                yield return new WaitForSeconds(0.2f);
        //            }
        //        }
        //        else
        //        {
        //            if (currentMoveNumber < 4)
        //            {
        //                //Forget learned move
        //                forget.SetActive(true);
        //                selectedMove.enabled = true;
        //                selectedMove.rectTransform.localPosition = positions[currentMoveNumber] + positionMod;
        //                moveSelector.rectTransform.localPosition = positions[4] + positionMod;
        //                SfxHandler.Play(selectClip);
        //                yield return new WaitForSeconds(0.2f);

        //                bool forgetPrompt = true;
        //                while (forgetPrompt)
        //                {
        //                    if (Input.GetButtonDown("Select"))
        //                    {
        //                        replacedMove = moveset[currentMoveNumber];
        //                        pokemon.ReplaceMove(currentMoveNumber, newMoveString);

        //                        forgetPrompt = false;
        //                        navigatingMoves = false;
        //                        SfxHandler.Play(selectClip);
        //                        yield return new WaitForSeconds(0.2f);
        //                    }
        //                    else if (Input.GetButtonDown("Back"))
        //                    {
        //                        forget.SetActive(false);
        //                        selectedMove.enabled = false;
        //                        moveSelector.rectTransform.localPosition = positions[currentMoveNumber] + positionMod;

        //                        forgetPrompt = false;
        //                        SfxHandler.Play(returnClip);
        //                        yield return new WaitForSeconds(0.2f);
        //                    }
        //                    yield return null;
        //                }
        //            }
        //            else
        //            {
        //                //Cancel learning move
        //                navigatingMoves = false;
        //                SfxHandler.Play(selectClip);
        //                yield return new WaitForSeconds(0.2f);
        //            }
        //        }
        //    }

        //    yield return null;
        //}
    }
}