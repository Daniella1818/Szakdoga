using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoPlayersGame : AGame
{
    private State currentState;
    //Addig amíg nem rakott a jelenlegi játékos addig maradjon false, azaz ne váltsunk játékost, majd ha
    //rakott akkor
    private bool isNextPlayerCanPlay = false;
    private bool isPlaying = true;
    void Start()
    {
        currentState = new State();
        StartCoroutine(Play());
    }

    void Update()
    {
        if (isPlaying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject clickedObj = hit.collider.gameObject;
                    Debug.Log("Kattintott objektum: " + clickedObj.name);
                    Color color;
                    if (currentState.CurrentPlayer == Stone.Red)
                        color = Color.red;
                    else
                        color = Color.blue;

                    string[] coords = clickedObj.name.Split(',');
                    int w = int.Parse(coords[0]); int x = int.Parse(coords[1]);
                    int y = int.Parse(coords[2]); int z = int.Parse(coords[3]);
                    Position p = new Position(w, x, y, z);
                    currentState = PlayersMove(currentState, p);
                    Debug.Log(currentState.Table.Board[w, x, y, z]);
                    Debug.Log("Red: " + currentState.redStoneCount + ", Blue: "
                    + currentState.blueStoneCount);
                    Debug.Log(currentState.Table.Board[0, 0, 0, 0]);
                    ChangeColor(color, clickedObj);
                    isNextPlayerCanPlay = true;
                    Debug.Log(currentState.CurrentPlayer + " : " + currentState.CountMill());
                }
            }
        }
    }
    protected override IEnumerator Play()
    {
        //while (currentState.GetStatus() == Stone.Empty)
        int i = 18;
        while(i > 0)
        {
            i--;
            yield return StartCoroutine(PlayTurn());
        }
        isPlaying = false;
        Debug.Log(currentState.CurrentStage);

        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
    }
    IEnumerator PlayTurn()
    {
        while (!isNextPlayerCanPlay)
        {
            yield return null;
        }

        currentState.ChangePlayer();
        isNextPlayerCanPlay = false;
        yield return null;
    }

    private State PlayersMove(State currentState, Position position)
    {
        FirstStageOperator op = null;
        while (op == null || !op.IsApplicable(currentState))
        {
            op = new FirstStageOperator(position);
        }
        return op.Apply(currentState);
    }
}
