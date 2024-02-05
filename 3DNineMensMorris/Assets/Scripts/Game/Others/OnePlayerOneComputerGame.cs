using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnePlayerOneComputerGame : AGame
{
    private ASolver solver;
    private State currentState;
    //Addig amíg nem rakott a jelenlegi játékos addig maradjon false, azaz ne váltsunk játékost, majd ha
    //rakott akkor
    private bool isNextPlayerCanPlay = false;
    private bool isPlaying = true;
    private bool isFirstClick = false;

    private void Start()
    {
        solver = new MiniMaxSolver(5);
        currentState = new State();
        Play();
    }
    void Update()
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
                PlayersMove(currentState, p);
                ChangeColor(color, clickedObj);
            }
        }
    }

    private State PlayersMove(State currentState, Position position)
    {
        FirstStageOperator op = null;
        while (op == null || !op.IsApplicable(currentState))
        {
            op = new FirstStageOperator(position);
            Debug.Log(op.IsApplicable(currentState));
        }
        return op.Apply(currentState);
    }

    private State AIsMove(State currentState)
    {
        State nextState = solver.NextMove(currentState);

        if (nextState == null)
        {
            throw new System.Exception("Cannot select next move.");
        }

        return nextState;
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
}
