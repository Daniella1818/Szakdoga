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

    public void checkForRemoveStage(State currentState, Position position)
    {
        if (currentState.CountMill(position, currentState.CurrentPlayer) > 0)
        {
            currentState.LastStage = currentState.CurrentStage;
            currentState.CurrentStage = Stage.Remove;
            currentState.currentPlayersMills = currentState.CountMill(position, currentState.CurrentPlayer);
        }
    }
    public int CountPlayersStones(State currentState, Stone playerColor)
    {
        int playerStones = 0;
        for (int w = 0; w < 3; w++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (currentState.Table.Board[w, x, y, z] == playerColor)
                            playerStones++;
                    }
                }
            }
        }
        return playerStones;
    }
}
