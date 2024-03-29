using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoComputerGame : AGame, IComputerGame
{
    private ASolver solver;
    void Start()
    {
        currentState = new State();
        solver = new MiniMaxSolver(3);
        StartCoroutine(Play());
    }

    protected override IEnumerator Play()
    {
        while (currentState.GetStatus() == Stone.Empty)
        {
            yield return StartCoroutine(AIsTurn());
        }

        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
    }

    public IEnumerator AIsTurn()
    {
        currentState = AIsMove(solver);
        currentState.ChangeHeuristics();
        ColorTableAfterAIsMove(currentState);
        ChangeColorBasedOnPlayer();
        yield return null;
    }
}
