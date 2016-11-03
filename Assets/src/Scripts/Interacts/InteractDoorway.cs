﻿//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InteractDoorway : MonoBehaviour
{
    /* [UNHANDLED YET]
    private GameObject Player;
    //*/
    private DialogBoxHandler Dialog;

    private Animator myAnimator;
    private SpriteRenderer objectSprite;
    private Light objectLight;
    private Collider hitBox;

    private AudioSource enterSound;

    public bool isLocked = false;
    public bool hasLight = false;
    public bool dontFadeMusic = false;

    public enum EntranceStyle
    {
        STANDSTILL,
        OPEN,
        SWINGLEFT,
        SWINGRIGHT,
        SLIDE
    }
    public EntranceStyle entranceStyle;

    public bool movesForward = false;

    public string transferScene; //If blank, will transfer to the currently loaded scene
    public Vector3 transferPosition;
    public Direction TransferDirection;
    public string examineText;
    public string lockedExamineText;

    public Texture2D fadeTex;

    private bool lockPlayerCamera = false;
    private Vector3 lockedPosition;

    // Use this for initialization
    void Start()
    {
        /* [UNHANDLED YET]
        Player = PlayerMovement.player.gameObject;
        //*/
        Dialog = GameObject.Find("GUI").GetComponent<DialogBoxHandler>();

        objectLight = this.GetComponentInChildren<Light>();
        if (objectLight != null)
        {
            if (!hasLight)
            {
                objectLight.enabled = false;
            }
            else
            {
                objectLight.enabled = true;
            }
        }

        enterSound = this.gameObject.GetComponent<AudioSource>();

    }

    void LateUpdate()
    {
        if (lockPlayerCamera)
        {
            PlayerMovement.Instance.mainCamera.transform.position = lockedPosition;
        }
    }

    public IEnumerator interact()
    {
        if (isLocked)
        {
            if (lockedExamineText.Length > 0)
            {
                if (PlayerMovement.Instance.setCheckBusyWith(this.gameObject))
                {
                    Dialog.drawDialogBox();     //yield return StartCoroutine blocks the next code from running until coroutine is done.
                    yield return Dialog.StartCoroutine("drawText", lockedExamineText);
                    while (!Input.GetButtonDown("Select") && !Input.GetButtonDown("Back"))
                    {   //these 3 lines stop the next bit from running until space is pressed.
                        yield return null;
                    }
                    Dialog.undrawDialogBox();
                    yield return new WaitForSeconds(0.2f);
                    PlayerMovement.Instance.unsetCheckBusyWith(this.gameObject);
                }
            }
        }
        else
        {
            if (examineText.Length > 0)
            {
                if (PlayerMovement.Instance.setCheckBusyWith(this.gameObject))
                {
                    Dialog.drawDialogBox();     //yield return StartCoroutine blocks the next code from running until coroutine is done.
                    yield return Dialog.StartCoroutine("drawText", examineText);
                    while (!Input.GetButtonDown("Select") && !Input.GetButtonDown("Back"))
                    {   //these 3 lines stop the next bit from running until space is pressed.
                        yield return null;
                    }
                    Dialog.undrawDialogBox();
                    yield return new WaitForSeconds(0.2f);
                    PlayerMovement.Instance.unsetCheckBusyWith(this.gameObject);
                }
            }
        }
    }

    public IEnumerator bump()
    {
        if (!isLocked
            && PlayerMovement.Instance.CanInput)
        {
            if (PlayerMovement.Instance.setCheckBusyWith(this.gameObject))
            {
                if (enterSound != null)
                {
                    if (!enterSound.isPlaying)
                    {
                        enterSound.volume = PlayerPrefs.GetFloat("sfxVolume");
                        enterSound.Play();
                    }
                }

                if (entranceStyle == EntranceStyle.SWINGRIGHT)
                {
                    PlayerMovement.Instance.IsRunning = false;
                    PlayerMovement.Instance.CurrentSpeed = PlayerMovement.Instance.WalkSpeed;
                    PlayerMovement.Instance.updateAnimation("walk", PlayerMovement.Instance.walkFPS);

                    float increment = 0f;
                    float speed = 0.25f;
                    float yRotation = transform.localEulerAngles.y;
                    while (increment < 1)
                    {
                        increment += (1f / speed) * Time.deltaTime;
                        if (increment > 1)
                        {
                            increment = 1;
                        }
                        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, yRotation + (90f * increment), transform.localEulerAngles.z);
                        PlayerMovement.Instance.mainCamera.fieldOfView = 20f - (2f * increment);
                        yield return null;
                    }

                    yield return new WaitForSeconds(0.2f);
                }
                else if (entranceStyle == EntranceStyle.SLIDE)
                {
                    PlayerMovement.Instance.IsRunning = false;
                    PlayerMovement.Instance.CurrentSpeed = PlayerMovement.Instance.WalkSpeed;
                    PlayerMovement.Instance.updateAnimation("walk", PlayerMovement.Instance.walkFPS);

                    float increment = 0f;
                    float speed = 0.25f;
                    while (increment < 1)
                    {
                        increment += (1 / speed) * Time.deltaTime;
                        if (increment > 1)
                        {
                            increment = 1;
                        }
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f - (0.92f * increment));
                        PlayerMovement.Instance.mainCamera.fieldOfView = 20f - (2f * increment);
                        yield return null;
                    }
                    yield return new WaitForSeconds(0.2f);
                }

                if (entranceStyle != EntranceStyle.STANDSTILL)
                {
                    if (entranceStyle != EntranceStyle.OPEN)
                    {
                        StartCoroutine(lockCameraPosition());
                        yield return new WaitForSeconds(0.1f);
                    }
                    PlayerMovement.Instance.forceMoveForward();
                }

                //fade out the scene and load a new scene
                GlobalVariables.global.fadeTex = fadeTex;
                //float fadeTime = sceneTransition.FadeOut() + 0.4f;
                float fadeTime = ScreenFade.slowedSpeed + 0.4f;
                //fadeCutouts for doorways not yet implemented
                StartCoroutine(ScreenFade.main.Fade(false, ScreenFade.slowedSpeed));
                if (!dontFadeMusic)
                {
                    BgmHandler.main.PlayMain(null, 0);
                }

                yield return new WaitForSeconds(fadeTime);

                if (!string.IsNullOrEmpty(transferScene))
                {
                    NonResettingHandler.saveDataToGlobal();

                    GlobalVariables.global.playerPosition = transferPosition;
                    GlobalVariables.global.PlayerDirection = TransferDirection;
                    GlobalVariables.global.playerForwardOnLoad = movesForward;
                    GlobalVariables.global.fadeIn = true;
                    SceneManager.LoadScene(transferScene);
                }
                else
                {
                    //uncheck busy with to ensure events at destination can be run.
                    PlayerMovement.Instance.unsetCheckBusyWith(this.gameObject);

                    //transfer to current scene, no saving/loading nessecary
                    PlayerMovement.Instance.updateAnimation("walk", PlayerMovement.Instance.walkFPS);
                    PlayerMovement.Instance.CurrentSpeed = PlayerMovement.Instance.WalkSpeed;

                    PlayerMovement.Instance.transform.position = transferPosition;
                    PlayerMovement.Instance.updateDirection(TransferDirection);
                    if (movesForward)
                    {
                        PlayerMovement.Instance.forceMoveForward();
                    }

                    GlobalVariables.global.fadeIn = true;
                    //SceneTransition.gameScene.FadeIn();
                    StartCoroutine(ScreenFade.main.Fade(true, ScreenFade.slowedSpeed));

                    yield return new WaitForSeconds(0.1f);
                    PlayerMovement.Instance.pauseInput(0.2f);
                }
            }
        }
    }

    private IEnumerator lockCameraPosition()
    {
        lockPlayerCamera = true;
        lockedPosition = PlayerMovement.Instance.mainCamera.transform.position;
        yield return new WaitForSeconds(2f);
        lockPlayerCamera = false;
    }


}
