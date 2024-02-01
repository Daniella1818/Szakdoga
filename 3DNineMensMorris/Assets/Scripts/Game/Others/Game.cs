using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private ASolver solver;
    private State currentState;

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

    // Update is called once per frame
    private void Play()
    {
        bool playersTurn = true;

        //while (currentState.GetStatus() == Stone.Empty)
        int i = 2;
        while(i >=0)
        {
            i--;

            if (playersTurn)
            {

            }
            else
            {
                //currentState = AIsMove(currentState);
                PlayersMove(currentState, new Position(0,0,1,0));
                ChangeColor(Color.blue, GameObject.Find("0,0,1,0"));
            }

            playersTurn = !playersTurn;
        }

        //Stone status = currentState.GetStatus();
        //Debug.Log("Winner: " + status);
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

    private void ChangeColor(Color newColor, GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = newColor;
        }
        else
        {
            Debug.LogWarning("A GameObject nem rendelkezik Renderer komponenssel, így nem lehet megváltoztatni a színét.");
        }
    }
}
