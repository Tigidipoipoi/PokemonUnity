//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;

public class InteractShop : MonoBehaviour
{

    private DialogBoxHandler Dialog;

    private NPCHandler thisNPC;
    public string interactDialog = "Welcome! \nWhat do you need?";
    public string returnDialog = "Is there anything else I may do\nfor you?";
    public string leaveDialog = "Please come again!";

    public string[] itemCatalog;
    //custom prices not yet implemented
    public int[] customPrices;


    void Awake()
    {
        Dialog = GameObject.Find("GUI").GetComponent<DialogBoxHandler>();

        if (transform.GetComponent<NPCHandler>() != null)
        {
            thisNPC = transform.GetComponent<NPCHandler>();
        }
    }



    public IEnumerator interact()
    {
        if (PlayerMovement.Instance.setCheckBusyWith(this.gameObject))
        {

            if (thisNPC != null)
            {
                Direction direction;
                //calculate player's position relative to this npc's and set direction accordingly.
                float xDistance = thisNPC.transform.position.x - PlayerMovement.Instance.transform.position.x;
                float zDistance = thisNPC.transform.position.z - PlayerMovement.Instance.transform.position.z;
                if (xDistance >= Mathf.Abs(zDistance))
                { //Mathf.Abs() converts zDistance to a positive always.
                    direction = Direction.LEFT;
                }           //this allows for better accuracy when checking orientation.
                else if (xDistance <= Mathf.Abs(zDistance) * -1)
                {
                    direction = Direction.RIGHT;
                }
                else if (zDistance >= Mathf.Abs(xDistance))
                {
                    direction = Direction.DOWN;
                }
                else
                {
                    direction = Direction.UP;
                }
                thisNPC.SetDirection(direction);
            }

            string[] choices = new string[]{
                "Shop","Leave"};

            Dialog.drawDialogBox();
            yield return StartCoroutine(Dialog.drawText(interactDialog));
            Dialog.drawChoiceBox(choices);
            yield return StartCoroutine(Dialog.choiceNavigate(choices));
            int chosenIndex = Dialog.chosenIndex;
            Dialog.undrawChoiceBox();
            while (chosenIndex != 0)
            {
                if (chosenIndex == 1)
                {
                    Dialog.undrawDialogBox();
                    yield return StartCoroutine(PlayerMovement.Instance.moveCameraTo(new Vector3(7, 0, 0), 0.35f));

                    Scene.main.Bag.gameObject.SetActive(true);
                    StartCoroutine(Scene.main.Bag.control(itemCatalog));

                    while (Scene.main.Bag.gameObject.activeSelf)
                    {
                        yield return null;
                    }

                    yield return StartCoroutine(PlayerMovement.Instance.moveCameraTo(new Vector3(0, 0, 0), 0.35f));

                }

                Dialog.drawDialogBox();
                yield return StartCoroutine(Dialog.drawText(returnDialog));
                Dialog.drawChoiceBox(choices);
                yield return StartCoroutine(Dialog.choiceNavigate(choices));
                chosenIndex = Dialog.chosenIndex;
                Dialog.undrawChoiceBox();
            }

            Dialog.drawDialogBox();
            yield return StartCoroutine(Dialog.drawText(leaveDialog));
            while (!Input.GetButtonDown("Select") && !Input.GetButtonDown("Back"))
            {
                yield return null;
            }
            Dialog.undrawDialogBox();
        }
        PlayerMovement.Instance.unsetCheckBusyWith(this.gameObject);
    }

}
