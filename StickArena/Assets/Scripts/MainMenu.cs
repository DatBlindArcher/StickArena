using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void OnGUI()
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
    }
}