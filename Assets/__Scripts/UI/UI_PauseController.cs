using TMPro;
using UnityEngine;

public class UI_PauseController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI header;
    [SerializeField] GameObject levelSelectGO;
    public void InitStuck()
    {
        levelSelectGO.SetActive(false);
        header.text = "Stuck?";
    }

    public void InitPause()
    {
        levelSelectGO.SetActive(true);
        header.text = "Menu";
    }
}
