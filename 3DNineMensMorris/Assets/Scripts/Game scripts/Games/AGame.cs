using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AGame : MonoBehaviour
{
    public State currentState;
    protected bool isPlaying = true;
    protected bool isNextPlayerCanPlay = true;


    private bool isFirstClick = false;

    private GameObject firstGameObject;
    private GameObject secondGameObject;

    protected void ClickWatcher(RaycastHit hit)
    {
        if (currentState.CurrentStage == Stage.First || currentState.CurrentStage == Stage.Remove)
        {
            firstGameObject = hit.collider.gameObject;
            FirstOrRemoveStageStep(firstGameObject);
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

    protected abstract IEnumerator Play();
    private void ChangeColor(Color newColor, GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = newColor;
        }
        else
        {
            Debug.LogWarning("A GameObject nem rendelkezik Renderer komponenssel, �gy nem lehet megv�ltoztatni a sz�n�t.");
        }
    }
    private Color GetCurrentPlayerColor()
    {
        Color color;
        if (currentState.CurrentPlayer == Stone.Red)
            color = Color.red;
        else
            color = Color.blue;
        return color;
    }

    //K�t j�t�kos m�d, egy g�p �s egy j�t�kos m�d
    //A j�t�kos kattint�sa alapj�n hozza l�tre az oper�tort
    protected State PlayersMove(Position startPosition, Position endPosition = null)
    {
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
    //Kattint�s alapj�n kiolvassa a m�trix koordin�t�t a nev�b�l
    private Position GetPositionOfGameObject(GameObject gameObject)
    {
        if (gameObject != null && gameObject.name.Contains(','))
        {
            string[] coords = gameObject.name.Split(',');
            int w = int.Parse(coords[0]); int x = int.Parse(coords[1]);
            int y = int.Parse(coords[2]); int z = int.Parse(coords[3]);
            Position p = new Position(w, x, y, z);
            return p;
        }
        return null;
    }

    //Itt egy kattint�s ut�n v�ltunk
    protected void FirstOrRemoveStageStep(GameObject clickedObj)
    {
        Position p = GetPositionOfGameObject(clickedObj);
        //Ezzel biztos�tjuk, hogy ha rosszul kattint, akkor ne csin�ljon semmit
        if (p != null)
        {
            State newState = PlayersMove(p);
            if (newState != null)
            {
                if(currentState.CurrentStage == Stage.First)
                    ChangeColor(GetCurrentPlayerColor(), clickedObj);
                else
                    ChangeColor(Color.black, clickedObj);
                currentState = newState;
                IsStateRemove();
            }
        }
    }
    //Ehhez m�r k�t kattint�s kell
    protected void SecondOrThirdStageStep(GameObject startPositionObj, GameObject endPositionObj)
    {
        Position startPosition = GetPositionOfGameObject(startPositionObj);
        Position endPosition = GetPositionOfGameObject(endPositionObj);

        if (startPosition != null && endPosition != null)
        {
            State newState = PlayersMove(startPosition, endPosition);
            if (newState != null)
            {
                ChangeColor(Color.black, startPositionObj);
                ChangeColor(GetCurrentPlayerColor(), endPositionObj);
                currentState = newState;
                IsStateRemove();
            }
        }
    }
    public void IsStateRemove()
    {
        //Ha nem vagyunk remove stage-be akkor az adott stage oper�tora elv�gzi a j�t�kos v�lt�st!
        if (currentState.CurrentStage != Stage.Remove)
            isNextPlayerCanPlay = !isNextPlayerCanPlay;
    }

    //K�t g�p m�d, egy g�p �s egy j�t�kos m�d
    protected State AIsMove(ASolver solver)
    {
        State nextState = solver.NextMove(currentState);

        if (nextState == null)
        {
            throw new System.Exception("Cannot select next move.");
        }
        return nextState;
    }
    protected void ColorTableAfterAIsMove(State currentState)
    {
        for (int w = 0; w < currentState.Table.Board.GetLength(0); w++)
        {
            for (int x = 0; x < currentState.Table.Board.GetLength(1); x++)
            {
                for (int y = 0; y < currentState.Table.Board.GetLength(2); y++)
                {
                    for (int z = 0; z < currentState.Table.Board.GetLength(3); z++)
                    {
                        Color color;
                        if (currentState.Table.Board[w, x, y, z] == Stone.Red)
                            color = Color.red;
                        else if (currentState.Table.Board[w, x, y, z] == Stone.Blue)
                            color = Color.blue;
                        else
                            color = Color.black;

                        string name = w.ToString() + "," + x.ToString() + "," +
                                      y.ToString() + "," + z.ToString();

                        if (GameObject.Find(name) != null)
                        {
                            GameObject gameObject = GameObject.Find(name);
                            ChangeColor(color, gameObject);
                        }
                    }
                }
            }
        }
    }

    protected void ChangeColorBasedOnPlayer()
    {
        GameObject colorObj = GameObject.Find("CurrentPlayer");
        if (colorObj != null)
        {
            RawImage colorView = colorObj.GetComponent<RawImage>();
            Color color = GetCurrentPlayerColor();
            colorView.color = color;
        }
    }
}
