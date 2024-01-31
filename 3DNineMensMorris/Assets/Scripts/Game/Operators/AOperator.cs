using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AOperator
{
    public abstract bool IsApplicable(State currentState);

    //Operátor alkalmazása
    public abstract State Apply(State currentState);

    //Meg vizsgálja hogy a paraméterben kapott játékosé-e a pozíción levõ kõ
    public bool stoneIsPlayers(State currentState, Stone playersColor, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
                (int)position.Z] == playersColor;
    }
    //Megvizsgálja, hogy a pozíció üres-e
    public bool positionIsEmpty(State currentState, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
            (int)position.Z] == Stone.Empty;
    }

    public void setStoneToPlace(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y, (int)position.Z] = currentState.CurrentPlayer;
    }
    public void setPlaceEmpty(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
           (int)position.Z] = Stone.Empty;
    }
}
