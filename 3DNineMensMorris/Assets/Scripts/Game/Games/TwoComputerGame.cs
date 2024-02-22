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
        solver = new MiniMaxSolver(2);
        Playe();
    }

    // Update is called once per frame
    protected void Playe()
    {
        while (currentState.GetStatus() == Stone.Empty)
        {
            AIsTurn();
        }
        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
    }
    private void AIsTurn()
    {
        currentState = AIsMove(solver);
        ColorTableAfterAIsMove(currentState);
    }

    protected override IEnumerator Play()
    {
        throw new System.NotImplementedException();
    }
}
