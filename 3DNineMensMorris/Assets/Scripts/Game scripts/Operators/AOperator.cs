using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AOperator
{
    public abstract bool IsApplicable(State currentState);

    //Oper�tor alkalmaz�sa
    public abstract State Apply(State currentState);

    //Meg vizsg�lja hogy a param�terben kapott j�t�kos�-e a poz�ci�n lev� k�
    protected bool StoneIsPlayers(State currentState, Stone playersColor, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
                (int)position.Z] == playersColor;
    }
    //Megvizsg�lja, hogy a poz�ci� �res-e
    protected bool PositionIsEmpty(State currentState, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
            (int)position.Z] == Stone.Empty;
    }
    //Adott poz�ci�ra teszi az aktu�lis j�t�kost
    protected void SetStoneToPlace(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y, (int)position.Z] = currentState.CurrentPlayer;
    }
    //Felszabad�tja a poz�ci�t
    protected void SetPlaceEmpty(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
           (int)position.Z] = Stone.Empty;
    }
    
    //Minden oper�torban(First,Second,Third) meg kell vizsg�lni, hogy remove-be kell �tv�ltani
    protected void CheckForRemoveStage(State currentState, Position position)
    {
        if (currentState.CountMill(position, currentState.CurrentPlayer) > 0)
        {
            currentState.LastStage = currentState.CurrentStage;
            currentState.CurrentStage = Stage.Remove;
            currentState.CurrentPlayersMills = currentState.CountMill(position, currentState.CurrentPlayer);
        }
        else
        {
            SwitchBetweenSecondAndThridStage(currentState);
            currentState.ChangePlayer();
        }
    }
    //Remove state eset�n vizsg�ljuk, hogy m�g abban vagyunk-e
    protected void CheckIfStillRemoveStage(State currentState)
    {
        if(currentState.CurrentPlayersMills == 0)
        {
            currentState.CurrentStage = currentState.LastStage;
            SwitchBetweenSecondAndThridStage(currentState);
            currentState.ChangePlayer();
        }
    }

    //Megvizsg�lja hogy a m�sik j�t�kosnak 3 db korongja van-e vagy sem, ha igen akkor a harmadik stage-be
    //v�ltunk, egy�bk�nt a second stage marad, mivel a kett� j�t�kos nem mindig ugyanakkor van harmadik stage-ben!
    protected void SwitchBetweenSecondAndThridStage(State currentState)
    {
        if (currentState.CurrentStage == Stage.Second || currentState.CurrentStage == Stage.Third)
        {
            int stones = 0;
            if (currentState.CurrentPlayer == Stone.Blue)
                stones = currentState.RedStoneCount;
            else if (currentState.CurrentPlayer == Stone.Red)
                stones = currentState.BlueStoneCount;

            if (stones == 3)
                currentState.CurrentStage = Stage.Third;
            else
                currentState.CurrentStage = Stage.Second;
        }
    }

    protected int CountPlayersStones(State currentState, Stone playerColor)
    {
        int playerStones = 0;
        for (int w = 0; w < currentState.Table.Board.GetLength(0); w++)
        {
            for (int x = 0; x < currentState.Table.Board.GetLength(1); x++)
            {
                for (int y = 0; y < currentState.Table.Board.GetLength(2); y++)
                {
                    for (int z = 0; z < currentState.Table.Board.GetLength(3); z++)
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
    protected bool IsPlayerOnlyHaveStoneInMill(State currentState, Stone playerColor)
    {
        //�gy vessz�k, hogy igen, ha tal�lunk egy olyan saj�t korongot ami nem malomban van akkor false-ra
        //v�ltunk �s visszat�r�nk vele
        for (int w = 0; w < currentState.Table.Board.GetLength(0); w++)
        {
            for (int x = 0; x < currentState.Table.Board.GetLength(1); x++)
            {
                for (int y = 0; y < currentState.Table.Board.GetLength(2); y++)
                {
                    for (int z = 0; z < currentState.Table.Board.GetLength(3); z++)
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
