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
    //Adott pozícióra teszi az aktuális játékost
    public void setStoneToPlace(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y, (int)position.Z] = currentState.CurrentPlayer;
    }
    //Felszabadítja a pozíciót
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
