//Original Scripts by IIColour (IIColour_Spectrum)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BumpRespawnSet : MonoBehaviour
{

    public Vector3 respawnPositionOffset;
    public Direction RespawnDirection;
    public string respawnText;

    private IEnumerator bump()
    {
        SaveData.currentSave.respawnScenePosition = new SeriV3(transform.position + respawnPositionOffset);
        SaveData.currentSave.RespawnSceneDirection = RespawnDirection;
        SaveData.currentSave.respawnText = respawnText;
        //Old:  Application.loadedLevelName
        SaveData.currentSave.respawnSceneName = SceneManager.GetActiveScene().name;
        yield return null;
    }

}
