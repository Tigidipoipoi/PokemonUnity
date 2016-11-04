using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region Members
    #region Tweakables
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

    /// <summary>
    /// Number of frame per second of the walk movement.
    /// </summary>
    public int WalkFPS = 7;
    /// <summary>
    /// Number of frame per second of the run movement.
    /// </summary>
    public int RunFPS = 12;
    #endregion Tweakables

    private DialogBoxHandler Dialog;
    private MapNameBoxHandler MapName;

    [Header("Hidden")]
    /// <summary>
    /// Before a script runs, it'll check if the player is busy with another script's GameObject.
    /// </summary>
    public GameObject busyWith = null;

    public bool IsMoving = false;
    public bool IsStill = true;
    public bool IsRunning = false;
    public bool IsSurfing = false;
    public bool IsBiking = false;
    public bool IsUsingStrength = false;
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

    private Direction _mostRecentDirectionPressed;
    private float _directionChangeInputDelay = 0.08f;

    //	private SceneTransition sceneTransition;

    private AudioSource PlayerAudio;

    public AudioClip bumpClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    #endregion Members

    private void Update()
    {
        if (!CanInput)
            return;

        // May be useless.
        setMostRecentDirectionPressed();

        #region Movement handling
        {
            if (_mostRecentDirectionPressed == Direction.NONE)
                return;

            /* WIP!!
            //if most recent direction pressed is held, but it isn't the current direction, set it to be
            if (isDirectionKeyHeld(CurrentDirection))
            {
                updateDirection(_mostRecentDirectionPressed);

                if (!IsMoving)
                {
                    // Unless player has just moved, wait a small amount of time to ensure that they have time to
                    // let go before moving (allows only turning)
                    //yield return new WaitForSeconds(_directionChangeInputDelay);
                }
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
            //*/
        }
        #endregion Movement handling
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

    /// <summary>
    /// Determines if we are pressing the run button.
    /// Sets <see cref="IsRunning"/> and <see cref="CurrentSpeed"/>.
    /// </summary>
    private void checkRunning()
    {
        IsRunning = Input.GetButton("Run");

        if (IsRunning)
            CurrentSpeed = RunSpeed;
        else
            CurrentSpeed = WalkSpeed;
    }

    /// <summary>
    /// Checks if <paramref name="pDirection"/> corresponds to <see cref="_mostRecentDirectionPressed"/>.
    /// </summary>
    /// <param name="pDirection">The direction to check.</param>
    private bool isDirectionKeyHeld(Direction pDirection)
    {
        return pDirection == _mostRecentDirectionPressed;
    }
}
