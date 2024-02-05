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
    private int removeCount = 0;

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
                    else if(currentState.CurrentStage == Stage.Remove)
                    {
                        firstGameObject = hit.collider.gameObject;
                        RemoveStageStep(firstGameObject);
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
        while (currentState.GetStatus() == Stone.Empty)
        {
            yield return StartCoroutine(PlayTurn());
        }
        isPlaying = false;

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
        Debug.Log("Current player: " + currentState.CurrentPlayer);
        Debug.Log("Red: " + currentState.redStoneCount + ", Blue: " + currentState.blueStoneCount);
        Debug.Log("Current stage: " + currentState.CurrentStage);

        AOperator op = null;

        if (currentState.CurrentStage == Stage.First)
            op = new FirstStageOperator(startPosition);
        else if (currentState.CurrentStage == Stage.Second)
            op = new SecondStageOperator(startPosition, endPosition);
        else if (currentState.CurrentStage == Stage.Third)
            op = new ThirdStageOperator(startPosition, endPosition);
        else if (currentState.CurrentStage == Stage.Remove)
            op = new RemoveStageOperator(startPosition);

        if (op.IsApplicable(currentState))
        { 
            return op.Apply(currentState);
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
            CheckIfTheStateIsRemove(currentState);
        }
    }
    //Ehhez már két kattintás kell
    private void SecondOrThirdStageStep(GameObject startPositionObj, GameObject endPositionObj)
    {
        Position startPosition = GetPositionOfGameObject(startPositionObj);
        Position endPosition = GetPositionOfGameObject(endPositionObj);

        State newState = PlayersMove(startPosition, endPosition);
        if (newState != null)
        {
            ChangeColor(Color.black, firstGameObject);
            ChangeColor(GetCurrentPlayerColor(currentState), secondGameObject);
            currentState = newState;
            CheckIfTheStateIsRemove(currentState);
        }
    }
    private void RemoveStageStep(GameObject removeObject)
    {
        Position p = GetPositionOfGameObject(removeObject);
        State newState = PlayersMove(p);
        if(newState != null)
        {
            ChangeColor(Color.black, removeObject);
            currentState = newState;
            CheckIfTheStateIsRemove(currentState);
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
    private void CheckIfTheStateIsRemove(State currentState)
    {
        if (currentState.CurrentStage != Stage.Remove)
            isNextPlayerCanPlay = true;
        else
        {
            if (removeCount < currentState.currentPlayersMills)
            {
                removeCount++;
            }
            else
            {
                currentState.CurrentStage = currentState.LastStage;
                removeCount = 0;
                currentState.currentPlayersMills = 0;
                isNextPlayerCanPlay = true;
            }
        }
    }
}
