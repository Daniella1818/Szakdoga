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

    public int redStoneCount = 9, blueStoneCount = 9;
    public int currentPlayersMills = 0;

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

    //Megnézzük, hogy az újonnan elhelyezett korong alkot-e malmot, ha igen akkor visszaadja, hogy
    //mennyit (mivel egy lépés alkothat kettõ vagy három malmot is!)
    public int CountMill(Position position)
    {
        int mills = 0;

        //Ha a sarokban került az új korong, tehát ezek a koordináták lehetnek: (0.szinten)
        //0,0,0,0 || 0,0,2,0 || 0,2,0,0 || 0,2,2,0 || 0,0,2,2 || 0,2,2,2 || 0,0,0,2 || 0,2,0,2
        if((position.X == 0 || position.X == 2) && 
           (position.Y == 0 || position.Y == 2) && 
           (position.Z == 0 || position.Z == 2))
        {
            //Ilyenkor 3 irányban kell megnézni hogy jött-e létre malom
            //Mindháromnál marad a szint értéke

            //Azonos sorban, az oszlopokat változtatva, a dimenzió ugyanaz marad (vízszintes)
            if (Table.Board[position.W, position.X, 0, position.Z] == CurrentPlayer &&
               Table.Board[position.W, position.X, 1, position.Z] == CurrentPlayer &&
               Table.Board[position.W, position.X, 2, position.Z] == CurrentPlayer)
                mills++;
            //Azonos oszlopban, a sorokat változtatva, a dimenzió ugyanaz marad (függõleges)
            if (Table.Board[position.W, 0, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[position.W, 1, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[position.W, 2, position.Y, position.Z] == CurrentPlayer)
                mills++;
            //Azonos oszlop, és sor, a dimenziót változtatjuk (dimenziós)
            if (Table.Board[position.W, position.X, position.Y, 0] == CurrentPlayer &&
               Table.Board[position.W, position.X, position.Y, 1] == CurrentPlayer &&
               Table.Board[position.W, position.X, position.Y, 2] == CurrentPlayer)
                mills++;
        }

        //Ha középre kerül a korong, itt lehet szintek között lépkedni
        //0,0,1,0 || 0,1,0,0 || 0,1,2,0 || 0,2,1,0 || 0,0,2,1 || 0,1,2,2 || 0,2,2,1, || 0,0,0,1 ||
        //0,2,0,1 || 0,1,0,2 || 0,0,1,2 || 0,2,1,2
        if(position.X == 1 || position.Y == 1 || position.Z == 1)
        {
            //Ilyenkor 2 irányba kell megvizsgálni, hogy jött-e létre malom
            //Azonos sor, oszlop, dimenzió, a szinteket kell léptetni (szintek közötti)
            if (Table.Board[0, position.X, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[1, position.X, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[2, position.X, position.Y, position.Z] == CurrentPlayer)
                mills++;
            //Annak megfelelõen hogy hol az egyes a koordináta meg kell vizsgálni a sort/oszlopot/dimenziót
            //Ha az x=1 akkor függõlegesen kell vizsgálni
            if(position.X == 1)
            {
                if (Table.Board[position.W, 0, position.Y, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, 1, position.Y, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, 2, position.Y, position.Z] == CurrentPlayer)
                    mills++;
            }
            //Ha az y=1 akkor vízszintesen kell vizsgálni
            else if(position.Y == 1)
            {
                if (Table.Board[position.W, position.X, 0, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, position.X, 1, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, position.X, 2, position.Z] == CurrentPlayer)
                        mills++;
            }
            //Ha a z=1 akkor dimenziósan kell vizsgálni
            else if(position.Z == 1)
            {
                if (Table.Board[position.W, position.X, position.Y, 0] == CurrentPlayer &&
                    Table.Board[position.W, position.X, position.Y, 1] == CurrentPlayer &&
                    Table.Board[position.W, position.X, position.Y, 2] == CurrentPlayer)
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
