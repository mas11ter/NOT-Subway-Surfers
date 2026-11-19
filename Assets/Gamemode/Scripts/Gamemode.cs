using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemode : MonoBehaviour
{
    [SerializeField] private InputContext InputContext;
    [SerializeField] private GameObject PauseMenu;
    public TMP_Text PlayerName;
    public SO_PersistenceData Data;

    private bool IsPaused;

    public void Pause(bool bPause)
    {
        IsPaused = bPause;
        if (PauseMenu)
        {
            PauseMenu.SetActive(IsPaused);
        }

        SetCursorState(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
        if (IsPaused)
        {
            InputContext.SetUIInput();
        }
        else
        {
            InputContext.SetGameInput();
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        if (PauseMenu)
        {
            PauseMenu.SetActive(IsPaused);
        }
        
        SetCursorState(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
        if (IsPaused)
        {
            InputContext.SetUIInput();
        }
        else
        {
            InputContext.SetGameInput();
        }
    }


    private void SetCursorState(bool State)
    {
        Cursor.lockState = State ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = State;
    }


    public void RestartGame()
    {
        Pause(false);
        Data.CurrentScore = 0;
        SceneManager.LoadScene(1);
        Data.OnApplicationQuit();
    }

    public void MainMenu()
    {
        //TogglePause();
        Data.CurrentScore = 0;
        SceneManager.LoadScene(0);
        Data.OnApplicationQuit();
    }

    public void StartPlaying()
    {
        Pause(false);
        string Name = PlayerName.text;
        if (Name == "")
        {
            Name = "Player1";
        }
        Data.LastPlayer = Data.CurrentPlayer;
        Data.CurrentPlayer = Name;
        if (Data.LastPlayer == "")
        {
            Data.LastPlayer = Name;
        }
        Data.LastPlayer = Name;
        Data.CurrentScore = 0;

        SceneManager.LoadScene(1);
    }


    public void Quit()
    {
        Application.Quit();
    }

}
