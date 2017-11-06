using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject creditsPanel;
    public GameObject settingsPanel;

    private int currentWindow;
    public GameObject[] windows;

    public Text sliderText;

    public void Start()
    {
        currentWindow = 0;
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        ShowWindow(0);
        sliderText.text = "50";
    }

    public void JoinLobby()
    {
        GameController.instance.FindLobby();
    }

    public void ShowCredits()
    {
        ShowWindow(2);
    }

    public void GoBackToMenu()
    {
        ShowWindow(0);
    }

    public void ShowSettings()
    {
        ShowWindow(1);
    }

    public void ShowWindow(int i)
    {
        windows[currentWindow].SetActive(false);
        currentWindow = i;
        windows[i].SetActive(true);
    }

    public void SetValue(Slider slider)
    {
        sliderText.text = slider.value.ToString();
    }







    /*private void OnGUI()
    {
        if (!GameController.instance.lobby.ID.IsValid())
        {
            if (GUILayout.Button("Get Lobby"))
            {
                GameController.instance.FindLobby();
            }
        }

        else
        {
            if (GUILayout.Button("Start Game"))
            {
                GameController.instance.StartGame();
            }

            if (GUILayout.Button("Leave Lobby"))
            {
                GameController.instance.LeaveLobby();
            }
        }
    }*/
}