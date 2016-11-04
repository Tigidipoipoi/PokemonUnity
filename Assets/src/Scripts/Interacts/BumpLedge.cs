//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using System.Collections;

public class BumpLedge : MonoBehaviour
{
    public Direction movementDirection = Direction.DOWN;

    private IEnumerator bump()
    {
        if (PlayerMovementOld.Instance.CurrentDirection == movementDirection)
        {
            PlayerMovementOld.Instance.pauseInput();

            PlayerMovementOld.Instance.forceMoveForward(2);
            PlayerMovementOld.Instance.followerScript.canMove = false;
            if (!PlayerMovementOld.Instance.IsRunning)
            {
                yield return new WaitForSeconds(PlayerMovementOld.Instance.CurrentSpeed * 0.5f);
                StartCoroutine(PlayerMovementOld.Instance.jump());
                yield return new WaitForSeconds(PlayerMovementOld.Instance.CurrentSpeed * 0.5f);
            }
            else
            {
                StartCoroutine(PlayerMovementOld.Instance.jump());
                yield return new WaitForSeconds(PlayerMovementOld.Instance.CurrentSpeed);
            }
            yield return new WaitForSeconds(PlayerMovementOld.Instance.CurrentSpeed);

            PlayerMovementOld.Instance.followerScript.canMove = true;
            PlayerMovementOld.Instance.unpauseInput();
        }
    }
}
