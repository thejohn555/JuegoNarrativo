using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        UIManager.Instance.LoadScene("Taberna");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
