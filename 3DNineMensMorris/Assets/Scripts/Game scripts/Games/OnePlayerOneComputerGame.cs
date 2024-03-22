using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnePlayerOneComputerGame : AGame
{
    private ASolver solver;
    private bool isFirstClick = false;

    GameObject firstGameObject;
    GameObject secondGameObject;

    private void Start()
    {
        currentState = new State();
        solver = new MiniMaxSolver(3);
        StartCoroutine(Play());
    }
    void Update()
    {
        if (isNextPlayerCanPlay)
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
                    else if (currentState.CurrentStage == Stage.Remove)
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

    IEnumerator PlayerTurn()
    {
        while (isNextPlayerCanPlay)
        {
            yield return null;
        }
        ChangeColorBasedOnPlayer();
        yield return null;
    }

    IEnumerator AITurn()
    {
        currentState = AIsMove(solver);
        ColorTableAfterAIsMove(currentState);
        IsStateRemove();
        ChangeColorBasedOnPlayer();
        yield return null;
    }

    protected override IEnumerator Play()
    {
        while (currentState.GetStatus() == Stone.Empty)
        {
            if (isNextPlayerCanPlay)
                yield return StartCoroutine(PlayerTurn());
            else
                yield return StartCoroutine(AITurn());
        }

        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
    }
}
