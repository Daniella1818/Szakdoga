using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AOperator
{
    public abstract bool IsApplicable(State currentState);

    //Oper�tor alkalmaz�sa
    public abstract State Apply(State currentState);

    //Meg vizsg�lja hogy a param�terben kapott j�t�kos�-e a poz�ci�n lev� k�
    public bool StoneIsPlayers(State currentState, Stone playersColor, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
                (int)position.Z] == playersColor;
    }
    //Megvizsg�lja, hogy a poz�ci� �res-e
    public bool PositionIsEmpty(State currentState, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
            (int)position.Z] == Stone.Empty;
    }
    //Adott poz�ci�ra teszi az aktu�lis j�t�kost
    public void SetStoneToPlace(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y, (int)position.Z] = currentState.CurrentPlayer;
    }
    //Felszabad�tja a poz�ci�t
    public void SetPlaceEmpty(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
           (int)position.Z] = Stone.Empty;
    }
    
    //Minden oper�torban(First,Second,Third) meg kell vizsg�lni, hogy remove-ban vagyunk-e
    public void CheckForRemoveStage(State currentState, Position position)
    {
        if (currentState.CountMill(position, currentState.CurrentPlayer) > 0)
        {
            currentState.LastStage = currentState.CurrentStage;
            currentState.CurrentStage = Stage.Remove;
            currentState.currentPlayersMills = currentState.CountMill(position, currentState.CurrentPlayer);
        }
        else
        {
            currentState.ChangePlayer();
        }
    }
    //Remove state eset�n vizsg�ljuk, hogy m�g abban vagyunk-e
    public void CheckIfStillRemoveStage(State currentState)
    {
        if(currentState.currentPlayersMills == 0)
        {
            currentState.CurrentStage = currentState.LastStage;
            currentState.ChangePlayer();
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

    //Remove-hoz kell, mert csak olyan korongot t�vol�that el ami nincs malomban, viszont, ha csak olyan korongja van ami
    //malomban van akkor elt�vol�that egyet bel�le. Ez azt vizsg�lja, hogy van-e olyan korongja ami nem alkot malmot
    public bool IsPlayerOnlyHaveStoneInMill(State currentState, Stone playerColor)
    {
        //�gy vessz�k, hogy igen, ha tal�lunk egy olyan saj�t korongot ami nem malomban van akkor false-ra
        //v�ltunk �s visszat�r�nk vele
        for (int w = 0; w < 3; w++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (currentState.Table.Board[w, x, y, z] == playerColor)
                        {
                            Position position = new Position(w, x, y, z);
                            //Ha az adott poz�ci�n l�v� korong nem alkot malmot, akkor 0-val fog visszat�rni, teh�t ez
                            //a korong nincs malomban
                            if (currentState.CountMill(position, playerColor) == 0)
                                return false;
                        }
                    }
                }
            }
        }
        //Ha v�gig megy�nk akkor csak olyan korongja van ami malomban van
        return true;
    }
}
