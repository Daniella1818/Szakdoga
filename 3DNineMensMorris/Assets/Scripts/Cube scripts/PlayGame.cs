using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGame : MonoBehaviour
{
    public GameObject cubeTable;
    void Start()
    {
        cubeTable = GameObject.Find("CubeTable");
        switch (GameManager.SelectedGameType)
        {
            case GameType.TwoPlayer:
                cubeTable.AddComponent<TwoPlayersGame>();
                break;
            case GameType.TwoComputer:
                cubeTable.AddComponent<TwoComputerGame>();
                break;
            case GameType.OnePlayerOneComputer:
                cubeTable.AddComponent<OnePlayerOneComputerGame>();
                break;
            default:
                Debug.LogError("Unknown game type.");
                break;
        }
    }
}
