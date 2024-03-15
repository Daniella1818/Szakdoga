using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoComputerGame : AGame
{
    private ASolver solver;
    // Start is called before the first frame update
    void Start()
    {
        currentState = new State();
        solver = new MiniMaxSolver(3);
        StartCoroutine(Play());
    }
    IEnumerator AIsTurn()
    {
        currentState = AIsMove(solver);
        currentState.ChangeHeuristics();
        ColorTableAfterAIsMove(currentState);
        yield return null;
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
}
