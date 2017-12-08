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
        GameController.instance.steam.FindLobby();
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

    private void OnGUI()
    {
        if (GameController.instance.lobby != null && GameController.instance.lobby.ID.IsValid())
        {
            foreach (Player player in GameController.instance.lobby.players.Values)
            {
                GUILayout.TextArea(player.name);
            }

            if (GUILayout.Button("Start Game"))
            {
                GameController.instance.StartGame(new GameInfo() { map = GameMap.Arena, mode = GameMode.DeathMatch });
            }
        }
    }
}