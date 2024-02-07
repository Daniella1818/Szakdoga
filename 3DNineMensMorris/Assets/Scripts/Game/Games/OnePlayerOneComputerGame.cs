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
        solver = new MiniMaxSolver(1);
        Play();
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
                            currentState.checkForSecondOrThirdStage();
                            SecondOrThirdStageStep(firstGameObject, secondGameObject);
                            isFirstClick = false;
                        }
                    }
                }
            }
        }
    }

    protected void Playe() // Módosított, nem Coroutine
    {
        while (currentState.GetStatus() == Stone.Empty)
        {
            if (isPlayersTurn)
                PlayerTurn();
            else
                AITurn();

            isPlayersTurn = !isPlayersTurn;
        }
        isPlaying = false;

        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
    }

    void PlayerTurn()
    {
        currentState.ChangePlayer();
    }

    void AITurn()
    {
        currentState = AIsMove(solver);
        ColorTableAfterAIsMove(currentState);
        currentState.ChangePlayer();
        isPlayersTurn = !isPlayersTurn;
    }

    protected override IEnumerator Play()
    {
        throw new System.NotImplementedException();
    }
}
