//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;

public class BumpLedge : MonoBehaviour
{
    public Direction movementDirection = Direction.DOWN;

    private IEnumerator bump()
    {
        if (PlayerMovement.Instance.CurrentDirection == movementDirection)
        {
            PlayerMovement.Instance.pauseInput();

            PlayerMovement.Instance.forceMoveForward(2);
            PlayerMovement.Instance.followerScript.canMove = false;
            if (!PlayerMovement.Instance.IsRunning)
            {
                yield return new WaitForSeconds(PlayerMovement.Instance.CurrentSpeed * 0.5f);
                StartCoroutine(PlayerMovement.Instance.jump());
                yield return new WaitForSeconds(PlayerMovement.Instance.CurrentSpeed * 0.5f);
            }
            else
            {
                StartCoroutine(PlayerMovement.Instance.jump());
                yield return new WaitForSeconds(PlayerMovement.Instance.CurrentSpeed);
            }
            yield return new WaitForSeconds(PlayerMovement.Instance.CurrentSpeed);

            PlayerMovement.Instance.followerScript.canMove = true;
            PlayerMovement.Instance.unpauseInput();
        }
    }
}
