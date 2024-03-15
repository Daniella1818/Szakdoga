using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectGame : MonoBehaviour
{
    private void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void TwoPlayersGame() 
    {
        GameManager.SelectedGameType = GameType.TwoPlayer;
        PlayGame();
    }
    public void TwoComputersGame() 
    {
        GameManager.SelectedGameType = GameType.TwoComputer;
        PlayGame();
    }
    public void OnePlayerOneComputerGame() 
    {
        GameManager.SelectedGameType = GameType.OnePlayerOneComputer;
        PlayGame();
    }
}
