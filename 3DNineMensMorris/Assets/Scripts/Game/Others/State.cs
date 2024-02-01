using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public CubeTable Table;
    public State()
    {
        Table = new CubeTable();
    }

    public Stone CurrentPlayer = Stone.Red;
    public Stage CurrentStage = Stage.First;

    //Ha remove stage van akkor ebbe mentj�k az aktu�lisat, hogy visszatudjunk t�rni r�
    public Stage LastStage;

    public int redStoneCount = 0, blueStoneCount = 0;

    //Jelenlegi j�t�kost cser�li, ez biztos�tja, hogy egym�s ut�n l�pjenek
    public void ChangePlayer()
    {
        if (CurrentPlayer == Stone.Red)
            CurrentPlayer = Stone.Blue;
        else if (CurrentPlayer == Stone.Blue)
            CurrentPlayer = Stone.Red;
    }

    //Megvizsg�lja, hogy van e nyertes, akkor vesz�t valaki ha 2 korongja maradt �sszesen, ha EMPTY-t ad vissza akkor m�g tudnak 
    //l�pni a j�t�kosok.
    public Stone GetStatus()
    {
        if (CurrentStage == Stage.Second || CurrentStage == Stage.Third)
        {
            if (blueStoneCount <= 2)
                return Stone.Red;
            else if (redStoneCount <= 2)
                return Stone.Blue;
            else
                return Stone.Empty;
        }
        else
            return Stone.Empty;
    }

    public int CountMill()
    {
        int mills = 0;
        for (int w = 0; w < 3; w++)
        {
            //Azonos szinten a sorok vizsg�lata - az els� sor �s az utols� sort kellene vizsg�lni
            for (int x = 0; x < 3; x += 2)
            {
                if (Table.Board[w, x, 0, 0] == CurrentPlayer && Table.Board[w, x, 1, 0] == CurrentPlayer &&
                    Table.Board[w, x, 2, 0] == CurrentPlayer)
                    mills++;
                if (Table.Board[w, x, 0, 0] == CurrentPlayer && Table.Board[w, x, 0, 1] == CurrentPlayer &&
                    Table.Board[w, x, 0, 2] == CurrentPlayer)
                    mills++;
                if (Table.Board[w, x, 0, 2] == CurrentPlayer && Table.Board[w, x, 1, 2] == CurrentPlayer &&
                    Table.Board[w, x, 2, 2] == CurrentPlayer)
                    mills++;
                if (Table.Board[w, x, 2, 0] == CurrentPlayer && Table.Board[w, x, 2, 1] == CurrentPlayer &&
                    Table.Board[w, x, 2, 2] == CurrentPlayer)
                    mills++;
            }
            //Azonos szinten az oszlopok vizsg�lata - az els� oszlop �s az utols� oszlopot kellene vizsg�lni
            for (int y = 0; y < 3; y += 2)
            {
                if (Table.Board[w, 0, y, 0] == CurrentPlayer && Table.Board[w, 1, y, 0] == CurrentPlayer &&
                   Table.Board[w, 2, y, 0] == CurrentPlayer)
                    mills++;
                if (Table.Board[w, 0, y, 2] == CurrentPlayer && Table.Board[w, 1, y, 2] == CurrentPlayer &&
                    Table.Board[w, 2, y, 2] == CurrentPlayer)
                    mills++;
            }
        }
        //Szintek k�z�tti �tmenet
        for (int z = 0; z < 3; z += 2)
        {
            //0,0,1,0 && 0,0,1,2
            if (Table.Board[0, 0, 1, z] == CurrentPlayer && Table.Board[1, 0, 1, z] == CurrentPlayer
                && Table.Board[2, 0, 1, z] == CurrentPlayer)
                mills++;
            //0,1,0,0 && 0,1,0,2
            if (Table.Board[0, 1, 0, z] == CurrentPlayer && Table.Board[1, 1, 0, z] == CurrentPlayer
                && Table.Board[2, 1, 0, z] == CurrentPlayer)
                mills++;

            //0,2,1,0 && 0,2,1,2
            if (Table.Board[0, 2, 1, z] == CurrentPlayer && Table.Board[1, 2, 1, z] == CurrentPlayer
                && Table.Board[2, 2, 1, z] == CurrentPlayer)
                mills++;
            //0,1,2,0 && 0,1,2,2
            if (Table.Board[0, 1, 2, z] == CurrentPlayer && Table.Board[1, 1, 2, z] == CurrentPlayer
                && Table.Board[2, 2, 2, z] == CurrentPlayer)
                mills++;
        }
        for (int y = 0; y < 3; y += 2)
        {
            for (int x = 0; x < 3; x += 2)
            {
                //0,0,0,1 && 0,2,0,1 && 0,0,2,1 && 0,2,2,1
                if (Table.Board[0, x, y, 1] == CurrentPlayer && Table.Board[1, x, y, 1] == CurrentPlayer &&
                Table.Board[2, x, y, 1] == CurrentPlayer)
                    mills++;
            }
        }
        return mills;
    }

    public object Clone()
    {
        State newState = new State();
        newState.CurrentStage = CurrentStage;
        newState.CurrentPlayer = CurrentPlayer;
        newState.Table = Table;
        newState.redStoneCount = redStoneCount;
        newState.blueStoneCount = blueStoneCount;
        return newState;
    }

    internal int GetHeuristics(Stone currentPlayer)
    {
        return 0;
    }
}
