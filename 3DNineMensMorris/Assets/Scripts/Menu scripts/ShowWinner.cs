using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWinner : MonoBehaviour
{
    public GameObject WinnerPanel;
    GameObject Table;
    public GameObject Red;
    public GameObject Blue;
    void Start()
    {
        Table = GameObject.Find("CubeTable");
    }
    void Update()
    {
        if (Table != null && Table.TryGetComponent(out AGame game) && WinnerPanel != null)
        {
            if(game.currentState.GetStatus() == Stone.Blue)
            {
                ShowWinnerPanel(Blue);
            }
            else if(game.currentState.GetStatus() == Stone.Red)
            {
                ShowWinnerPanel(Red);
            }
        }
    }

    private void ShowWinnerPanel(GameObject winnerObj)
    {
        WinnerPanel.SetActive(true);
        winnerObj.SetActive(true);
    }
}
