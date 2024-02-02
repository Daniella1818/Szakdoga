using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AOperator
{
    public abstract bool IsApplicable(State currentState);

    //Oper�tor alkalmaz�sa
    public abstract State Apply(State currentState);

    //Meg vizsg�lja hogy a param�terben kapott j�t�kos�-e a poz�ci�n lev� k�
    public bool stoneIsPlayers(State currentState, Stone playersColor, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
                (int)position.Z] == playersColor;
    }
    //Megvizsg�lja, hogy a poz�ci� �res-e
    public bool positionIsEmpty(State currentState, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
            (int)position.Z] == Stone.Empty;
    }
    //Adott poz�ci�ra teszi az aktu�lis j�t�kost
    public void setStoneToPlace(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y, (int)position.Z] = currentState.CurrentPlayer;
    }
    //Felszabad�tja a poz�ci�t
    public void setPlaceEmpty(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
           (int)position.Z] = Stone.Empty;
    }

    public void checkForRemoveStage(State currentState)
    {
        if(currentState.CountMill() > 0)
        {
            currentState.LastStage = currentState.CurrentStage;
            currentState.CurrentStage = Stage.Remove;
        }
    }
}
