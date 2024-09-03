using UnityEngine;

public class UI_Stars : MonoBehaviour
{
    [SerializeField] GameObject[] stars;

    public void ShowStars(int amount)
    {
        if (amount < 0 || amount > 3)
            return;

        for (int i = 0; i < amount; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void DisableStars()
    {
        foreach (var start in stars)
        {
            start.SetActive(false);
        }
    }
}
