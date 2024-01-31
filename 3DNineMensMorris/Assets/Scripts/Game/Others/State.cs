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

    //Ha remove stage van akkor ebbe mentjük az aktuálisat, hogy visszatudjunk térni rá
    public Stage LastStage;

    public int blackStoneCount = 9, whiteStoneCount = 9;

    //Jelenlegi játékost cseréli, ez biztosítja, hogy egymás után lépjenek
    public void ChangePlayer()
    {
        if (CurrentPlayer == Stone.Red)
            CurrentPlayer = Stone.Blue;
        else if (CurrentPlayer == Stone.Blue)
            CurrentPlayer = Stone.Red;
    }

    //Megvizsgálja, hogy van e nyertes, akkor veszít valaki ha 2 korongja maradt összesen, ha EMPTY-t ad vissza akkor még tudnak 
    //lépni a játékosok.
    public Stone GetStatus()
    {
        if (whiteStoneCount <= 2)
            return Stone.Red;
        else if (blackStoneCount <= 2)
            return Stone.Blue;
        else
            return Stone.Empty;
    }

    private int CountMill()
    {
        int mills = 0;
        for (int w = 0; w < 3; w++)
        {
            //Azonos szinten a sorok vizsgálata - az elsõ sor és az utolsó sort kellene vizsgálni
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
            //Azonos szinten az oszlopok vizsgálata - az elsõ oszlop és az utolsó oszlopot kellene vizsgálni
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
        //Szintek közötti átmenet
        for (int z = 0; z < 3; z += 2)
        {
            if (Table.Board[0, 0, 1, z] == CurrentPlayer && Table.Board[1, 0, 1, z] == CurrentPlayer
                && Table.Board[2, 0, 1, z] == CurrentPlayer)
                mills++;
            if (Table.Board[0, 1, 0, z] == CurrentPlayer && Table.Board[1, 1, 0, z] == CurrentPlayer
                && Table.Board[2, 1, 0, z] == CurrentPlayer)
                mills++;
            if (Table.Board[0, 2, 1, z] == CurrentPlayer && Table.Board[1, 2, 1, z] == CurrentPlayer
                && Table.Board[2, 2, 1, z] == CurrentPlayer)
                mills++;
            if (Table.Board[0, 1, 2, z] == CurrentPlayer && Table.Board[1, 1, 2, z] == CurrentPlayer
                && Table.Board[2, 2, 2, z] == CurrentPlayer)
                mills++;
        }
        for (int y = 0; y < 3; y += 2)
        {
            for (int x = 0; x < 3; x += 2)
            {
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
        return newState;
    }

    internal int GetHeuristics(Stone currentPlayer)
    {
        return 0;
    }
}
