using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AOperator
{
    public abstract bool IsApplicable(State currentState);

    //Operátor alkalmazása
    public abstract State Apply(State currentState);

    //Meg vizsgálja hogy a paraméterben kapott játékosé-e a pozíción levõ kõ
    protected bool StoneIsPlayers(State currentState, Stone playersColor, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
                (int)position.Z] == playersColor;
    }
    //Megvizsgálja, hogy a pozíció üres-e
    protected bool PositionIsEmpty(State currentState, Position position)
    {
        return currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
            (int)position.Z] == Stone.Empty;
    }
    //Adott pozícióra teszi az aktuális játékost
    protected void SetStoneToPlace(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y, (int)position.Z] = currentState.CurrentPlayer;
    }
    //Felszabadítja a pozíciót
    protected void SetPlaceEmpty(State currentState, Position position)
    {
        currentState.Table.Board[(int)position.W, (int)position.X, (int)position.Y,
           (int)position.Z] = Stone.Empty;
    }
    
    //Minden operátorban(First,Second,Third) meg kell vizsgálni, hogy remove-be kell átváltani
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
    //Remove state esetén vizsgáljuk, hogy még abban vagyunk-e
    protected void CheckIfStillRemoveStage(State currentState)
    {
        if(currentState.CurrentPlayersMills == 0)
        {
            currentState.CurrentStage = currentState.LastStage;
            SwitchBetweenSecondAndThridStage(currentState);
            currentState.ChangePlayer();
        }
    }

    //Megvizsgálja hogy a másik játékosnak 3 db korongja van-e vagy sem, ha igen akkor a harmadik stage-be
    //váltunk, egyébként a second stage marad, mivel a kettõ játékos nem mindig ugyanakkor van harmadik stage-ben!
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

    //Remove-hoz kell, mert csak olyan korongot távolíthat el ami nincs malomban, viszont, ha csak olyan korongja van ami
    //malomban van akkor eltávolíthat egyet belõle. Ez azt vizsgálja, hogy van-e olyan korongja ami nem alkot malmot
    protected bool IsPlayerOnlyHaveStoneInMill(State currentState, Stone playerColor)
    {
        //Úgy vesszük, hogy igen, ha találunk egy olyan saját korongot ami nem malomban van akkor false-ra
        //váltunk és visszatérünk vele
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
                            //Ha az adott pozíción lévõ korong nem alkot malmot, akkor 0-val fog visszatérni, tehát ez
                            //a korong nincs malomban
                            if (currentState.CountMill(position, playerColor) == 0)
                                return false;
                        }
                    }
                }
            }
        }
        //Ha végig megyünk akkor csak olyan korongja van ami malomban van
        return true;
    }
}
