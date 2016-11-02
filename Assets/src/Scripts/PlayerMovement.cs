//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public static PlayerMovement Instance;

    private DialogBoxHandler Dialog;
    private MapNameBoxHandler MapName;

    //before a script runs, it'll check if the player is busy with another script's GameObject.
    public GameObject busyWith = null;

    public bool IsMoving = false;
    public bool IsStill = true;
    public bool IsRunning = false;
    public bool IsSurfing = false;
    public bool IsBiking = false;
    public bool IsUsingStrength = false;
    /// <summary>
    /// Time in seconds taken to walk 1 square.
    /// </summary>
    public float WalkSpeed = 0.3f;
    /// <summary>
    /// Time in seconds taken to run 1 square.
    /// </summary>
    public float RunSpeed = 0.15f;
    /// <summary>
    /// Time in seconds taken to surf 1 square.
    /// </summary>
    public float SurfSpeed = 0.2f;
    public float CurrentSpeed;
    public Direction CurrentDirection = Direction.DOWN;

    public bool CanInput = true;

    public float increment = 1f;

    private GameObject follower;
    public FollowerMovement followerScript;

    private Transform pawn;
    private Transform pawnReflection;
    //private Material pawnReflectionSprite;
    private SpriteRenderer pawnSprite;
    private SpriteRenderer pawnReflectionSprite;

    public Transform hitBox;

    public MapCollider currentMap;
    public MapCollider destinationMap;

    public MapSettings accessedMapSettings;
    private AudioClip accessedAudio;
    private int accessedAudioLoopStartSamples;

    public Camera mainCamera;
    public Vector3 mainCameraDefaultPosition;

    private SpriteRenderer mount;
    private Vector3 mountPosition;

    private string animationName;
    private Sprite[] spriteSheet;
    private Sprite[] mountSpriteSheet;

    private int frame;
    private int frames;
    private int framesPerSec;
    private float secPerFrame;
    private bool animPause;
    private bool overrideAnimPause;

    public int walkFPS = 7;
    public int runFPS = 12;

    private Direction _mostRecentDirectionPressed;
    private float _directionChangeInputDelay = 0.08f;

    //	private SceneTransition sceneTransition;

    private AudioSource PlayerAudio;

    public AudioClip bumpClip;
    public AudioClip jumpClip;
    public AudioClip landClip;


    void Awake()
    {

        PlayerAudio = transform.GetComponent<AudioSource>();

        //set up the reference to this script.
        Instance = this;

        Dialog = GameObject.Find("GUI").GetComponent<DialogBoxHandler>();
        MapName = GameObject.Find("GUI").GetComponent<MapNameBoxHandler>();

        CanInput = true;
        CurrentSpeed = WalkSpeed;

        follower = transform.FindChild("Follower").gameObject;
        followerScript = follower.GetComponent<FollowerMovement>();

        mainCamera = transform.FindChild("Camera").GetComponent<Camera>();
        mainCameraDefaultPosition = mainCamera.transform.localPosition;

        pawn = transform.FindChild("Pawn");
        pawnReflection = transform.FindChild("PawnReflection");
        pawnSprite = pawn.GetComponent<SpriteRenderer>();
        pawnReflectionSprite = pawnReflection.GetComponent<SpriteRenderer>();

        //pawnReflectionSprite = transform.FindChild("PawnReflection").GetComponent<MeshRenderer>().material;

        hitBox = transform.FindChild("Player_Transparent");

        mount = transform.FindChild("Mount").GetComponent<SpriteRenderer>();
        mountPosition = mount.transform.localPosition;



    }

    void Start()
    {

        if (!IsSurfing)
        {
            updateMount(false);
        }

        updateAnimation("walk", walkFPS);
        StartCoroutine("animateSprite");
        animPause = true;

        reflect(false);
        followerScript.reflect(false);

        updateDirection(CurrentDirection);

        //Check current map
        RaycastHit[] hitRays = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down);
        int closestIndex = -1;
        float closestDistance = float.PositiveInfinity;
        if (hitRays.Length > 0)
        {
            for (int i = 0; i < hitRays.Length; i++)
            {
                if (hitRays[i].collider.gameObject.GetComponent<MapCollider>() != null)
                {
                    if (hitRays[i].distance < closestDistance)
                    {
                        closestDistance = hitRays[i].distance;
                        closestIndex = i;
                    }
                }
            }
        }
        if (closestIndex != -1)
        {
            currentMap = hitRays[closestIndex].collider.gameObject.GetComponent<MapCollider>();
        }
        else
        {   //if no map found
            //Check for map in front of player's direction
            hitRays = Physics.RaycastAll(transform.position + Vector3.up + GetForwardVectorRaw(), Vector3.down);
            closestIndex = -1;
            closestDistance = float.PositiveInfinity;
            if (hitRays.Length > 0)
            {
                for (int i = 0; i < hitRays.Length; i++)
                {
                    if (hitRays[i].collider.gameObject.GetComponent<MapCollider>() != null)
                    {
                        if (hitRays[i].distance < closestDistance)
                        {
                            closestDistance = hitRays[i].distance;
                            closestIndex = i;
                        }
                    }
                }
            }
            if (closestIndex != -1)
            {
                currentMap = hitRays[closestIndex].collider.gameObject.GetComponent<MapCollider>();
            }
            else
            {
                Debug.Log("no map found");
            }
        }


        if (currentMap != null)
        {
            accessedMapSettings = currentMap.gameObject.GetComponent<MapSettings>();
            if (accessedAudio != accessedMapSettings.getBGM())
            { //if audio is not already playing
                accessedAudio = accessedMapSettings.getBGM();
                accessedAudioLoopStartSamples = accessedMapSettings.getBGMLoopStartSamples();
                BgmHandler.main.PlayMain(accessedAudio, accessedAudioLoopStartSamples);
            }
            if (accessedMapSettings.mapNameBoxTexture != null)
            {
                MapName.display(accessedMapSettings.mapNameBoxTexture, accessedMapSettings.mapName, accessedMapSettings.mapNameColor);
            }
        }


        //check position for transparent bumpEvents
        Collider transparentCollider = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.4f);
        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].name.ToLowerInvariant().Contains("_transparent"))
                {
                    if (!hitColliders[i].name.ToLowerInvariant().Contains("player") && !hitColliders[i].name.ToLowerInvariant().Contains("follower"))
                    {
                        transparentCollider = hitColliders[i];
                    }
                }
            }
        }
        if (transparentCollider != null)
        {
            //send bump message to the object's parent object
            transparentCollider.transform.parent.gameObject.SendMessage("bump", SendMessageOptions.DontRequireReceiver);
        }

        //DEBUG
        if (accessedMapSettings != null)
        {
            WildPokemonInitialiser[] encounterList = accessedMapSettings.getEncounterList(WildPokemonInitialiser.Location.Standard);
            string namez = "";
            for (int i = 0; i < encounterList.Length; i++)
            {
                namez += PokemonDatabase.getPokemon(encounterList[i].ID).getName() + ", ";
            }
            Debug.Log("Wild Pokemon for map \"" + accessedMapSettings.mapName + "\": " + namez);
        }
        //

        GlobalVariables.global.resetFollower();
    }

    private void OnEnable()
    {
        StartCoroutine(control_Coroutine());
    }

    private void Update()
    {
        setMostRecentDirectionPressed();

        if (Input.GetButtonDown("ToggleFollowing"))
        {
            togglePokemonFollowing();
        }
    }

    /// <summary>
    /// Withdraws following pokemon to its pokeball if there is one;
    /// Summons it otherwise.
    /// </summary>
    private void togglePokemonFollowing()
    {
        Debug.Log(currentMap.getTileTag(transform.position));
        if (followerScript.canMove)
            followerScript.StartCoroutine("withdrawToBall");
        else
            followerScript.canMove = true;
    }

    /// <summary>
    /// Checks inputs and set <see cref="_mostRecentDirectionPressed"/> accordingly.
    /// </summary>
    private void setMostRecentDirectionPressed()
    {
        float rawHorizontalAxis = Input.GetAxisRaw("Horizontal");
        float rawVerticalAxis = Input.GetAxisRaw("Vertical");

        // Horizontal axis.
        if (!Mathf.Approximately(rawHorizontalAxis, 0))
        {
            _mostRecentDirectionPressed = rawHorizontalAxis > 0
                ? Direction.RIGHT
                : Direction.LEFT;
        }
        // Vertical axis.
        else if (!Mathf.Approximately(rawVerticalAxis, 0))
        {
            _mostRecentDirectionPressed = rawVerticalAxis > 0
                ? Direction.UP
                : Direction.DOWN;
        }
        else
        {
            _mostRecentDirectionPressed = Direction.NONE;
        }
    }

    private bool isDirectionKeyHeld(Direction pDirection)
    {
        float rawVertical = Input.GetAxisRaw("Vertical");
        float rawHorizontal = Input.GetAxisRaw("Horizontal");

        if (pDirection == Direction.UP
            && rawVertical > 0)
            return true;
        else if (pDirection == Direction.RIGHT
            && rawHorizontal > 0)
            return true;
        else if (pDirection == Direction.DOWN
            && rawVertical < 0)
            return true;
        else if (pDirection == Direction.LEFT
            && rawHorizontal < 0)
            return true;

        return false;
    }

    private IEnumerator control_Coroutine()
    {
        // ie: not moving.
        bool isStill;
        while (isActiveAndEnabled)
        {
            isStill = true;
            if (CanInput)
            {
                if (!IsSurfing
                    && !IsBiking)
                {
                    checkRunning();
                }

                #region ToDo: Move this elsewhere !!!
                // Open Pause Menu.
                if (Input.GetButton("Start"))
                {
                    if (IsMoving || Input.GetButtonDown("Start"))
                    {
                        if (setCheckBusyWith(Scene.main.Pause.gameObject))
                        {
                            animPause = true;
                            Scene.main.Pause.gameObject.SetActive(true);
                            StartCoroutine(Scene.main.Pause.control());
                            while (Scene.main.Pause.gameObject.activeSelf)
                            {
                                yield return null;
                            }
                            unsetCheckBusyWith(Scene.main.Pause.gameObject);
                        }
                    }
                }
                else if (Input.GetButtonDown("Select"))
                {
                    interact();
                }
                #endregion
                // Movement handling.
                #region Movement handling
                else if (_mostRecentDirectionPressed != Direction.NONE)
                {
                    //if most recent direction pressed is held, but it isn't the current direction, set it to be
                    if (_mostRecentDirectionPressed != CurrentDirection
                        && isDirectionKeyHeld(_mostRecentDirectionPressed))
                    {
                        updateDirection(_mostRecentDirectionPressed);

                        if (!IsMoving)
                        {   // unless player has just moved, wait a small amount of time to ensure that they have time to
                            yield return new WaitForSeconds(_directionChangeInputDelay);
                        } // let go before moving (allows only turning)
                    }
                    //if a new direction wasn't found, direction would have been set, thus ending the update
                    else
                    {
                        //if current direction is not held down, check for the new direction to turn to
                        if (!isDirectionKeyHeld(CurrentDirection))
                        {
                            //it's least likely to have held the opposite direction by accident
                            int directionCheck = ((int)CurrentDirection + 2) % 4;

                            if (isDirectionKeyHeld((Direction)directionCheck))
                            {
                                updateDirection((Direction)directionCheck);
                                if (!IsMoving)
                                {
                                    yield return new WaitForSeconds(_directionChangeInputDelay);
                                }
                            }
                            else
                            {
                                //it's either 90 degrees clockwise, counter, or none at this point. prioritise clockwise.
                                directionCheck = ((int)CurrentDirection + 1) % 4;

                                if (isDirectionKeyHeld((Direction)directionCheck))
                                {
                                    updateDirection((Direction)directionCheck);
                                    if (!IsMoving)
                                    {
                                        yield return new WaitForSeconds(_directionChangeInputDelay);
                                    }
                                }
                                else
                                {
                                    directionCheck = (int)CurrentDirection - 1;
                                    if (directionCheck < 0)
                                        directionCheck += 4;

                                    if (isDirectionKeyHeld((Direction)directionCheck))
                                    {
                                        updateDirection((Direction)directionCheck);
                                        if (!IsMoving)
                                        {
                                            yield return new WaitForSeconds(_directionChangeInputDelay);
                                        }
                                    }
                                }
                            }
                        }
                        //if current direction was held, then we want to attempt to move forward.
                        else
                        {
                            IsMoving = true;
                        }
                    }

                    //if moving is true (including by momentum from the previous step) then attempt to move forward.
                    if (IsMoving)
                    {
                        isStill = false;
                        yield return StartCoroutine(moveForward());
                    }
                }
                #endregion
            }

            // If still is true at this point, then no move function has been called.
            if (isStill)
            {
                animPause = true;
                // The player loses his momentum.
                IsMoving = false;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Determines if we are pressing the run button.
    /// Sets <see cref="IsRunning"/> and <see cref="CurrentSpeed"/>.
    /// </summary>
    private void checkRunning()
    {
        IsRunning = Input.GetButton("Run");
        if (IsRunning)
        {
            if (IsMoving)
                updateAnimation("run", runFPS);
            else
                updateAnimation("walk", walkFPS);

            CurrentSpeed = RunSpeed;
        }
        else
        {
            updateAnimation("walk", walkFPS);
            CurrentSpeed = WalkSpeed;
        }
    }

    public void updateDirection(Direction pDirection)
    {
        CurrentDirection = pDirection;
        pawnSprite.sprite = spriteSheet[(int)CurrentDirection * frames + frame];
        pawnReflectionSprite.sprite = pawnSprite.sprite;
        //pawnReflectionSprite.SetTextureOffset("_MainTex", GetUVSpriteMap(direction*frames+frame));
        if (mount.enabled)
        {
            mount.sprite = mountSpriteSheet[(int)pDirection];
        }
    }

    private void updateMount(bool enabled)
    {
        mount.enabled = enabled;
    }

    private void updateMount(bool enabled, string spriteName)
    {
        mount.enabled = enabled;
        mountSpriteSheet = Resources.LoadAll<Sprite>("PlayerSprites/" + spriteName);
        mount.sprite = mountSpriteSheet[(int)CurrentDirection];
    }

    public void updateAnimation(string newAnimationName, int fps)
    {
        if (animationName != newAnimationName)
        {
            animationName = newAnimationName;
            spriteSheet = Resources.LoadAll<Sprite>("PlayerSprites/" + SaveData.currentSave.getPlayerSpritePrefix() + newAnimationName);
            //pawnReflectionSprite.SetTexture("_MainTex", Resources.Load<Texture>("PlayerSprites/"+SaveData.currentSave.getPlayerSpritePrefix()+newAnimationName));
            framesPerSec = fps;
            secPerFrame = 1f / (float)framesPerSec;
            frames = Mathf.RoundToInt((float)spriteSheet.Length / 4f);
            if (frame >= frames)
            {
                frame = 0;
            }
        }
    }

    public void reflect(bool setState)
    {
        pawnReflectionSprite.enabled = setState;
    }

    private Vector2 GetUVSpriteMap(int index)
    {
        int row = index / 4;
        int column = index % 4;

        return new Vector2(0.25f * column, 0.75f - (0.25f * row));
    }

    private IEnumerator animateSprite()
    {
        frame = 0;
        frames = 4;
        framesPerSec = walkFPS;
        secPerFrame = 1f / (float)framesPerSec;
        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                if (animPause && frame % 2 != 0 && !overrideAnimPause)
                {
                    frame -= 1;
                }
                pawnSprite.sprite = spriteSheet[(int)CurrentDirection * frames + frame];
                pawnReflectionSprite.sprite = pawnSprite.sprite;
                //pawnReflectionSprite.SetTextureOffset("_MainTex", GetUVSpriteMap(direction*frames+frame));
                yield return new WaitForSeconds(secPerFrame / 4f);
            }
            if (!animPause || overrideAnimPause)
            {
                frame += 1;
                if (frame >= frames)
                {
                    frame = 0;
                }
            }
        }
    }
    public void setOverrideAnimPause(bool set)
    {
        overrideAnimPause = set;
    }

    ///Attempts to set player to be busy with "caller" and pauses input, returning true if the request worked.
    public bool setCheckBusyWith(GameObject caller)
    {
        if (busyWith == null)
        {
            busyWith = caller;
        }
        //if the player is definitely busy with caller object
        if (busyWith == caller)
        {
            pauseInput();
            Debug.Log("Busy with " + busyWith);
            return true;
        }
        return false;
    }

    ///Attempts to unset player to be busy with "caller". Will unpause input only if 
    ///the player is still not busy 0.1 seconds after calling.
    public void unsetCheckBusyWith(GameObject caller)
    {
        if (busyWith == caller)
        {
            busyWith = null;
        }
        StartCoroutine(checkBusinessBeforeUnpause(0.1f));
    }

    public IEnumerator checkBusinessBeforeUnpause(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (PlayerMovement.Instance.busyWith == null)
        {
            unpauseInput();
        }
        else
        {
            Debug.Log("Busy with " + PlayerMovement.Instance.busyWith);
        }
    }

    public void pauseInput()
    {
        CanInput = false;
        if (animationName == "run")
        {
            updateAnimation("walk", walkFPS);
        }
        IsRunning = false;
    }

    public void pauseInput(float secondsToWait)
    {
        StartCoroutine(pauseInputWait(secondsToWait));
    }

    private IEnumerator pauseInputWait(float secondsToWait)
    {
        CanInput = false;
        yield return new WaitForSeconds(secondsToWait);
        Debug.Log("unpaused");
        CanInput = true;
    }

    public void unpauseInput()
    {
        Debug.Log("unpaused");
        CanInput = true;
    }

    ///returns the vector relative to the player direction, without any modifications.
    public Vector3 GetForwardVectorRaw()
    {
        return GetForwardVectorRaw(CurrentDirection);
    }
    public Vector3 GetForwardVectorRaw(Direction pDirection)
    {
        switch (pDirection)
        {
            case Direction.UP:
                return Vector3.forward;
            case Direction.RIGHT:
                return Vector3.right;
            case Direction.DOWN:
                return Vector3.back;
            case Direction.LEFT:
                return Vector3.left;

            default:
                return Vector3.zero;
        }
    }

    public Vector3 GetForwardVector()
    {
        return GetForwardVector(CurrentDirection);
    }
    public Vector3 GetForwardVector(Direction pDirection, bool pDoCheckForBridge = true)
    {
        //set initial vector3 based off of direction
        Vector3 movement = GetForwardVectorRaw(pDirection);

        //Check destination map	and bridge																//0.5f to adjust for stair height
        //cast a ray directly downwards from the position directly in front of the player		//1f to check in line with player's head
        RaycastHit[] hitColliders = Physics.RaycastAll(transform.position + movement + new Vector3(0, 1.5f, 0), Vector3.down);
        RaycastHit mapHit = new RaycastHit();
        RaycastHit bridgeHit = new RaycastHit();
        //cycle through each of the collisions
        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                //if map has not been found yet
                if (mapHit.collider == null)
                {
                    //if a collision's gameObject has a mapCollider, it is a map. set it to be the destination map.
                    if (hitColliders[i].collider.gameObject.GetComponent<MapCollider>() != null)
                    {
                        mapHit = hitColliders[i];
                        destinationMap = mapHit.collider.gameObject.GetComponent<MapCollider>();
                    }
                }
                else if (bridgeHit.collider != null && pDoCheckForBridge)
                { //if both have been found
                    i = hitColliders.Length;    //stop searching
                }
                //if bridge has not been found yet
                if (bridgeHit.collider == null && pDoCheckForBridge)
                {
                    //if a collision's gameObject has a BridgeHandler, it is a bridge.
                    if (hitColliders[i].collider.gameObject.GetComponent<BridgeHandler>() != null)
                    {
                        bridgeHit = hitColliders[i];
                    }
                }
                else if (mapHit.collider != null)
                { //if both have been found
                    i = hitColliders.Length;    //stop searching
                }
            }
        }

        if (bridgeHit.collider != null)
        {
            //modify the forwards vector to align to the bridge.
            movement -= new Vector3(0, (transform.position.y - bridgeHit.point.y), 0);
        }
        //if no bridge at destination
        else if (mapHit.collider != null)
        {
            //modify the forwards vector to align to the mapHit.
            movement -= new Vector3(0, (transform.position.y - mapHit.point.y), 0);
        }


        float currentSlope = Mathf.Abs(MapCollider.GetSlopeOfPosition(transform.position, pDirection));
        float destinationSlope = Mathf.Abs(MapCollider.GetSlopeOfPosition(transform.position + GetForwardVectorRaw(), pDirection, pDoCheckForBridge));
        float yDistance = Mathf.Abs((transform.position.y + movement.y) - transform.position.y);
        yDistance = Mathf.Round(yDistance * 100f) / 100f;

        //Debug.Log("currentSlope: "+currentSlope+", destinationSlope: "+destinationSlope+", yDistance: "+yDistance);

        //if either slope is greater than 1 it is too steep.
        if (currentSlope <= 1 && destinationSlope <= 1)
        {
            //if yDistance is greater than both slopes there is a vertical wall between them
            if (yDistance <= currentSlope || yDistance <= destinationSlope)
            {
                return movement;
            }
        }
        return Vector3.zero;
    }

    ///Make the player move one space in the direction they are facing
    private IEnumerator moveForward()
    {
        Vector3 movement = GetForwardVector();

        bool ableToMove = false;

        //without any movement, able to move should stay false
        if (movement != Vector3.zero)
        {
            //check destination for objects/transparents
            Collider objectCollider = null;
            Collider transparentCollider = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + movement + new Vector3(0, 0.5f, 0), 0.4f);
            if (hitColliders.Length > 0)
            {
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    if (hitColliders[i].name.ToLowerInvariant().Contains("_object"))
                    {
                        objectCollider = hitColliders[i];
                    }
                    else if (hitColliders[i].name.ToLowerInvariant().Contains("_transparent"))
                    {
                        transparentCollider = hitColliders[i];
                    }
                }
            }

            if (objectCollider != null)
            {
                //send bump message to the object's parent object
                objectCollider.transform.parent.gameObject.SendMessage("bump", SendMessageOptions.DontRequireReceiver);
            }
            else
            { //if no objects are in the way
                int destinationTileTag = destinationMap.getTileTag(transform.position + movement);

                RaycastHit bridgeHit = MapCollider.getBridgeHitOfPosition(transform.position + movement + new Vector3(0, 0.1f, 0));
                if (bridgeHit.collider != null || destinationTileTag != 1)
                {   //wall tile tag

                    if (bridgeHit.collider == null && !IsSurfing && destinationTileTag == 2)
                    { //(water tile tag)
                    }
                    else
                    {
                        if (IsSurfing && destinationTileTag != 2f)
                        { //disable surfing if not headed to water tile
                            updateAnimation("walk", walkFPS);
                            CurrentSpeed = WalkSpeed;
                            IsSurfing = false;
                            StartCoroutine("dismount");
                            BgmHandler.main.PlayMain(accessedAudio, accessedAudioLoopStartSamples);
                        }

                        if (destinationMap != currentMap)
                        {  //if moving onto a new map
                            currentMap = destinationMap;
                            accessedMapSettings = destinationMap.gameObject.GetComponent<MapSettings>();
                            if (accessedAudio != accessedMapSettings.getBGM())
                            { //if audio is not already playing
                                accessedAudio = accessedMapSettings.getBGM();
                                accessedAudioLoopStartSamples = accessedMapSettings.getBGMLoopStartSamples();
                                BgmHandler.main.PlayMain(accessedAudio, accessedAudioLoopStartSamples);
                            }
                            destinationMap.BroadcastMessage("repair", SendMessageOptions.DontRequireReceiver);
                            if (accessedMapSettings.mapNameBoxTexture != null)
                            {
                                MapName.display(accessedMapSettings.mapNameBoxTexture, accessedMapSettings.mapName, accessedMapSettings.mapNameColor);
                            }
                            Debug.Log(destinationMap.name + "   " + accessedAudio.name);
                        }

                        if (transparentCollider != null)
                        {
                            //send bump message to the transparents's parent object
                            transparentCollider.transform.parent.gameObject.SendMessage("bump", SendMessageOptions.DontRequireReceiver);
                        }

                        ableToMove = true;
                        yield return StartCoroutine(move(movement));
                    }
                }
            }
        }

        //if unable to move anywhere, then set moving to false so that the player stops animating.
        if (!ableToMove)
        {
            Invoke("playBump", 0.05f);
            IsMoving = false;
            animPause = true;
        }
    }




    public IEnumerator move(Vector3 movement)
    {
        yield return StartCoroutine(move(movement, true, false));
    }
    public IEnumerator move(Vector3 movement, bool encounter)
    {
        yield return StartCoroutine(move(movement, encounter, false));
    }
    public IEnumerator move(Vector3 movement, bool encounter, bool lockFollower)
    {
        if (movement != Vector3.zero)
        {
            Vector3 startPosition = hitBox.position;
            IsMoving = true;
            increment = 0;

            if (!lockFollower)
            {
                StartCoroutine(followerScript.move(startPosition, CurrentSpeed));
            }
            animPause = false;
            while (increment < 1f)
            { //increment increases slowly to 1 over the frames
                increment += (1f / CurrentSpeed) * Time.deltaTime; //speed is determined by how many squares are crossed in one second
                if (increment > 1)
                {
                    increment = 1;
                }
                transform.position = startPosition + (movement * increment);
                hitBox.position = startPosition + movement;
                yield return null;
            }

            //check for encounters unless disabled
            if (encounter)
            {
                int destinationTag = currentMap.getTileTag(transform.position);
                if (destinationTag != 1)
                { //not a wall
                    if (destinationTag == 2)
                    { //surf tile
                        StartCoroutine(wildEncounter(WildPokemonInitialiser.Location.Surfing));
                    }
                    else
                    { //land tile
                        StartCoroutine(wildEncounter(WildPokemonInitialiser.Location.Standard));
                    }
                }
            }
        }
    }

    public IEnumerator moveCameraTo(Vector3 targetPosition, float cameraSpeed)
    {
        targetPosition += mainCameraDefaultPosition;
        Vector3 startPosition = mainCamera.transform.localPosition;
        Vector3 movement = targetPosition - startPosition;
        float increment = 0;
        if (cameraSpeed != 0)
        {
            while (increment < 1f)
            { //increment increases slowly to 1 over the frames
                increment += (1f / cameraSpeed) * Time.deltaTime;
                if (increment > 1)
                {
                    increment = 1;
                }
                mainCamera.transform.localPosition = startPosition + (movement * increment);
                yield return null;
            }
        }
        mainCamera.transform.localPosition = targetPosition;
    }




    public void forceMoveForward()
    {
        StartCoroutine(forceMoveForwardIE(1));
    }
    public void forceMoveForward(int spaces)
    {
        StartCoroutine(forceMoveForwardIE(spaces));
    }
    private IEnumerator forceMoveForwardIE(int spaces)
    {
        overrideAnimPause = true;
        for (int i = 0; i < spaces; i++)
        {
            Vector3 movement = GetForwardVector();

            //check destination for transparents
            //Collider objectCollider = null;
            Collider transparentCollider = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + movement + new Vector3(0, 0.5f, 0), 0.4f);
            if (hitColliders.Length > 0)
            {
                for (int i2 = 0; i2 < hitColliders.Length; i2++)
                {
                    if (hitColliders[i2].name.ToLowerInvariant().Contains("_transparent"))
                    {
                        transparentCollider = hitColliders[i2];
                    }
                }
            }
            if (transparentCollider != null)
            {
                //send bump message to the transparents's parent object
                transparentCollider.transform.parent.gameObject.SendMessage("bump", SendMessageOptions.DontRequireReceiver);
            }

            yield return StartCoroutine(move(movement, false));
        }
        overrideAnimPause = false;
    }

    private void interact()
    {
        Vector3 spaceInFront = GetForwardVector();

        Collider[] hitColliders = Physics.OverlapSphere((new Vector3(transform.position.x, (transform.position.y + 0.5f), transform.position.z) + spaceInFront), 0.4f);
        Collider currentInteraction = null;
        if (hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].name.Contains("_Transparent"))
                { //Prioritise a transparent over a solid object.
                    if (hitColliders[i].name != ("Player_Transparent"))
                    {
                        currentInteraction = hitColliders[i];
                        i = hitColliders.Length;
                    } //Stop checking for other interactable events if a transparent was found.
                }
                else if (hitColliders[i].name.Contains("_Object"))
                {
                    currentInteraction = hitColliders[i];
                }
            }
        }
        if (currentInteraction != null)
        {
            //sent interact message to the collider's object's parent object
            currentInteraction.transform.parent.gameObject.SendMessage("interact", SendMessageOptions.DontRequireReceiver);
            currentInteraction = null;
        }
        else if (!IsSurfing)
        {
            if (currentMap.getTileTag(transform.position + spaceInFront) == 2)
            { //water tile tag
                StartCoroutine("surfCheck");
            }
        }
    }

    public IEnumerator jump()
    {
        //float currentSpeed = speed;
        //speed = walkSpeed;
        float increment = 0f;
        float parabola = 0;
        float height = 2.1f;
        Vector3 startPosition = pawn.position;

        playClip(jumpClip);

        while (increment < 1)
        {
            increment += (1 / WalkSpeed) * Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }
            parabola = -height * (increment * increment) + (height * increment);
            pawn.position = new Vector3(pawn.position.x, startPosition.y + parabola, pawn.position.z);
            yield return null;
        }

        playClip(landClip);

        //speed = currentSpeed;
    }

    private IEnumerator stillMount()
    {
        Vector3 holdPosition = mount.transform.position;
        float hIncrement = 0f;
        while (hIncrement < 1)
        {
            hIncrement += (1 / CurrentSpeed) * Time.deltaTime;
            mount.transform.position = holdPosition;
            yield return null;
        }
        mount.transform.position = holdPosition;
    }

    private IEnumerator dismount()
    {
        StartCoroutine("stillMount");
        yield return StartCoroutine("jump");
        followerScript.canMove = true;
        mount.transform.localPosition = mountPosition;
        updateMount(false);
    }

    private IEnumerator surfCheck()
    {
        Pokemon targetPokemon = SaveData.currentSave.PC.getFirstFEUserInParty("Surf");
        if (targetPokemon != null)
        {
            if (GetForwardVector(CurrentDirection, false) != Vector3.zero)
            {
                if (setCheckBusyWith(this.gameObject))
                {

                    Dialog.drawDialogBox();
                    yield return Dialog.StartCoroutine("drawText", "The water is dyed a deep blue. Would you \nlike to surf on it?");
                    Dialog.drawChoiceBox();
                    yield return Dialog.StartCoroutine("choiceNavigate");
                    Dialog.undrawChoiceBox();
                    int chosenIndex = Dialog.chosenIndex;
                    if (chosenIndex == 1)
                    {
                        Dialog.drawDialogBox();
                        yield return Dialog.StartCoroutine("drawText", targetPokemon.getName() + " used " + targetPokemon.getFirstFEInstance("Surf") + "!");
                        while (!Input.GetButtonDown("Select") && !Input.GetButtonDown("Back"))
                        {
                            yield return null;
                        }
                        IsSurfing = true;
                        updateMount(true, "surf");

                        BgmHandler.main.PlayMain(GlobalVariables.global.surfBGM, GlobalVariables.global.surfBgmLoopStart);

                        //determine the vector for the space in front of the player by checking direction
                        mount.transform.position = mount.transform.position + GetForwardVectorRaw();

                        followerScript.StartCoroutine("withdrawToBall");
                        StartCoroutine("stillMount");
                        forceMoveForward();
                        yield return StartCoroutine("jump");

                        updateAnimation("surf", walkFPS);
                        CurrentSpeed = SurfSpeed;
                    }
                    Dialog.undrawDialogBox();
                    unsetCheckBusyWith(this.gameObject);
                }
            }
        }
        else
        {
            if (setCheckBusyWith(this.gameObject))
            {
                Dialog.drawDialogBox();
                yield return Dialog.StartCoroutine("drawText", "The water is dyed a deep blue.");
                while (!Input.GetButtonDown("Select") && !Input.GetButtonDown("Back"))
                {
                    yield return null;
                }
                Dialog.undrawDialogBox();
                unsetCheckBusyWith(this.gameObject);
            }
        }
        yield return new WaitForSeconds(0.2f);

    }

    public IEnumerator wildEncounter(WildPokemonInitialiser.Location encounterLocation)
    {
        if (accessedMapSettings.getEncounterList(encounterLocation).Length > 0)
        {
            if (Random.value <= accessedMapSettings.getEncounterProbability())
            {
                if (setCheckBusyWith(Scene.main.Battle.gameObject))
                {

                    BgmHandler.main.PlayOverlay(Scene.main.Battle.defaultWildBGM, Scene.main.Battle.defaultWildBGMLoopStart);

                    //SceneTransition sceneTransition = Dialog.transform.GetComponent<SceneTransition>();

                    yield return StartCoroutine(ScreenFade.main.FadeCutout(false, ScreenFade.slowedSpeed, null));
                    //yield return new WaitForSeconds(sceneTransition.FadeOut(1f));
                    Scene.main.Battle.gameObject.SetActive(true);
                    StartCoroutine(Scene.main.Battle.control(accessedMapSettings.getRandomEncounter(encounterLocation)));

                    while (Scene.main.Battle.gameObject.activeSelf)
                    {
                        yield return null;
                    }

                    //yield return new WaitForSeconds(sceneTransition.FadeIn(0.4f));
                    yield return StartCoroutine(ScreenFade.main.Fade(true, 0.4f));

                    unsetCheckBusyWith(Scene.main.Battle.gameObject);
                }
            }
        }
    }

    private void playClip(AudioClip clip)
    {
        PlayerAudio.clip = clip;
        PlayerAudio.volume = PlayerPrefs.GetFloat("sfxVolume");
        PlayerAudio.Play();
    }

    private void playBump()
    {
        if (!PlayerAudio.isPlaying)
        {
            if (!IsMoving && !overrideAnimPause)
            {
                playClip(bumpClip);
            }
        }
    }

}