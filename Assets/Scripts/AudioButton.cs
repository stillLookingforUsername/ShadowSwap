using UnityEngine;

public class AudioButton : MonoBehaviour
{
    public GameObject audioPanel;

    public void ShowAudioPanel()
    {
        if (audioPanel != null)
        {
            audioPanel.SetActive(true);
        }
    }
    public void HideAudioPanel()
    {
        if (audioPanel != null)
        {
            audioPanel.SetActive(false);
        }
    }

}