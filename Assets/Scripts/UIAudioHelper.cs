using UnityEngine;

public class UIAudioHelper : MonoBehaviour
{  
    public void PlayClickSFX() => AudioManager.Instance.PlaySFX(AudioManager.Instance.clickSFX);
}
