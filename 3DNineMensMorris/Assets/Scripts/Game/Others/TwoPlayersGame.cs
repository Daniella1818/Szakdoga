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
    private bool isFirstClick = false;

    GameObject firstGameObject;
    GameObject secondGameObject;
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
                    if (currentState.CurrentStage == Stage.First)
                    {
                        firstGameObject = hit.collider.gameObject;
                        FirstStageStep(firstGameObject);
                    }
                    else if (currentState.CurrentStage == Stage.Second ||
                             currentState.CurrentStage == Stage.Third)
                    {
                        if (!isFirstClick)
                        {
                            firstGameObject = hit.collider.gameObject;
                            isFirstClick = true;
                        }
                        else
                        {
                            secondGameObject = hit.collider.gameObject;
                            SecondOrThirdStageStep(firstGameObject, secondGameObject);
                            isFirstClick = false;
                        }
                    }
                }
            }
        }
    }
    protected override IEnumerator Play()
    {
        //while (currentState.GetStatus() == Stone.Empty)
        int i = 30;
        while(i >= 0)
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

    private State PlayersMove(Position startPosition, Position endPosition = null)
    {
        AOperator op = null;

        if (currentState.CurrentStage == Stage.First)
            op = new FirstStageOperator(startPosition);
        else if (currentState.CurrentStage == Stage.Second)
            op = new SecondStageOperator(startPosition, endPosition);

        if (op.IsApplicable(currentState))
        {
            isNextPlayerCanPlay = true;
            Debug.Log("Current player: " + currentState.CurrentPlayer);
            Debug.Log("Red: " + currentState.redStoneCount + ", Blue: " + currentState.blueStoneCount);
            return op.Apply(currentState);
        }
        else
        {
            firstGameObject = null;
            secondGameObject = null;
        }

        return null;
    }
    //Itt egy kattintás után váltunk
    private void FirstStageStep(GameObject clickedObj)
    {
        Position p = GetPositionOfGameObject(clickedObj);
        State newState = PlayersMove(p);
        if (newState != null)
        {
            ChangeColor(GetCurrentPlayerColor(currentState), clickedObj);
            currentState = newState;
        }
    }
    //Ehhez már két kattintás kell
    private void SecondOrThirdStageStep(GameObject startPositionObj, GameObject endPositionObj)
    {
        Debug.Log("Elsõ kattintás: " + firstGameObject.name);
        Debug.Log("Második kattintás: " + secondGameObject.name);
        Position startPosition = GetPositionOfGameObject(startPositionObj);
        Position endPosition = GetPositionOfGameObject(endPositionObj);

        State newState = PlayersMove(startPosition, endPosition);
        if (newState != null)
        {
            ChangeColor(Color.black, firstGameObject);
            ChangeColor(GetCurrentPlayerColor(currentState), secondGameObject);
            currentState = newState;
        }
    }

    private Position GetPositionOfGameObject(GameObject gameObject)
    {
        if (gameObject.name.Contains(','))
        {
            string[] coords = gameObject.name.Split(',');
            int w = int.Parse(coords[0]); int x = int.Parse(coords[1]);
            int y = int.Parse(coords[2]); int z = int.Parse(coords[3]);
            Position p = new Position(w, x, y, z);
            return p;
        }
        return null;
    }
}
