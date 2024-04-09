using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnePlayerOneComputerGame : AGame, IPlayerGame, IComputerGame
{
    private ASolver solver;

    private void Start()
    {
        currentState = new State(new CubeTable(), new DefensiveHeuristics());
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
                    ClickWatcher(hit);
                }
            }
        }
    }

    protected override IEnumerator Play()
    {
        while (currentState.GetStatus() == Stone.Empty)
        {
            if (isNextPlayerCanPlay)
                yield return StartCoroutine(PlayerTurn());
            else
                yield return StartCoroutine(AIsTurn());
        }

        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
    }
    public IEnumerator PlayerTurn()
    {
        while (isNextPlayerCanPlay)
        {
            yield return null;
        }
        ChangeColorBasedOnPlayer();
        yield return null;
    }

    public IEnumerator AIsTurn()
    {
        currentState = AIsMove(solver);
        ColorTableAfterAIsMove(currentState);
        IsStateRemove();
        ChangeColorBasedOnPlayer();
        yield return null;
    }
}
