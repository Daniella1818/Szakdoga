using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private ASolver solver;
    private State currentState;

    private void Start()
    {
        // Inicializáld a solver-t, például egy AI scripttel
        solver = new MiniMaxSolver(5);
        currentState = new State();

        // Indítsd el a játékot
        Play();
    }

    // Update is called once per frame
    private void Play()
    {
        bool playersTurn = true;

        //while (currentState.GetStatus() == Stone.Empty)
        int i = 2;
        while(i >=0)
        {
            i--;
            Debug.Log(currentState);

            if (playersTurn)
            {
                if (Input.GetMouseButtonDown(0)) // 0 az egérgombbal történõ kattintásra vonatkozik
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        // Ellenõrizzük, hogy a kattintott objektum rendelkezik-e AOperator komponenssel
                        GameObject clickedObject = hit.collider.gameObject;
                        string[] matrixCoords = clickedObject.name.Split(',');
                        int w = int.Parse(matrixCoords[0]); int x = int.Parse(matrixCoords[1]);
                        int y = int.Parse(matrixCoords[2]); int z = int.Parse(matrixCoords[3]);
                        FirstStageOperator op = new FirstStageOperator(new Position(w, x, y, z));

                        if (op != null && op.IsApplicable(currentState))
                        {
                            currentState = op.Apply(currentState);
                            Renderer rend = GetComponent<Renderer>();
                            if (rend != null)
                            {
                                Color color;
                                if (currentState.CurrentPlayer == Stone.Red)
                                    color = Color.red;
                                else
                                    color = Color.blue;
                                rend.material.color = color;
                            }
                        }
                    }
                }
            }
            else
            {
                currentState = AIsMove(currentState);
            }

            //playersTurn = !playersTurn;
        }

        Debug.Log(currentState);
        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
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
}
