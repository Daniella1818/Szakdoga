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

    public int redStoneCount = 9, blueStoneCount = 9;
    public int currentPlayersMills = 0;

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

    //Megn�zz�k, hogy az �jonnan elhelyezett korong alkot-e malmot, ha igen akkor visszaadja, hogy
    //mennyit (mivel egy l�p�s alkothat kett� vagy h�rom malmot is!)
    public int CountMill(Position position)
    {
        int mills = 0;

        //Ha a sarokban ker�lt az �j korong, teh�t ezek a koordin�t�k lehetnek: (0.szinten)
        //0,0,0,0 || 0,0,2,0 || 0,2,0,0 || 0,2,2,0 || 0,0,2,2 || 0,2,2,2 || 0,0,0,2 || 0,2,0,2
        if((position.X == 0 || position.X == 2) && 
           (position.Y == 0 || position.Y == 2) && 
           (position.Z == 0 || position.Z == 2))
        {
            //Ilyenkor 3 ir�nyban kell megn�zni hogy j�tt-e l�tre malom
            //Mindh�romn�l marad a szint �rt�ke

            //Azonos sorban, az oszlopokat v�ltoztatva, a dimenzi� ugyanaz marad (v�zszintes)
            if (Table.Board[position.W, position.X, 0, position.Z] == CurrentPlayer &&
               Table.Board[position.W, position.X, 1, position.Z] == CurrentPlayer &&
               Table.Board[position.W, position.X, 2, position.Z] == CurrentPlayer)
                mills++;
            //Azonos oszlopban, a sorokat v�ltoztatva, a dimenzi� ugyanaz marad (f�gg�leges)
            if (Table.Board[position.W, 0, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[position.W, 1, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[position.W, 2, position.Y, position.Z] == CurrentPlayer)
                mills++;
            //Azonos oszlop, �s sor, a dimenzi�t v�ltoztatjuk (dimenzi�s)
            if (Table.Board[position.W, position.X, position.Y, 0] == CurrentPlayer &&
               Table.Board[position.W, position.X, position.Y, 1] == CurrentPlayer &&
               Table.Board[position.W, position.X, position.Y, 2] == CurrentPlayer)
                mills++;
        }

        //Ha k�z�pre ker�l a korong, itt lehet szintek k�z�tt l�pkedni
        //0,0,1,0 || 0,1,0,0 || 0,1,2,0 || 0,2,1,0 || 0,0,2,1 || 0,1,2,2 || 0,2,2,1, || 0,0,0,1 ||
        //0,2,0,1 || 0,1,0,2 || 0,0,1,2 || 0,2,1,2
        if(position.X == 1 || position.Y == 1 || position.Z == 1)
        {
            //Ilyenkor 2 ir�nyba kell megvizsg�lni, hogy j�tt-e l�tre malom
            //Azonos sor, oszlop, dimenzi�, a szinteket kell l�ptetni (szintek k�z�tti)
            if (Table.Board[0, position.X, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[1, position.X, position.Y, position.Z] == CurrentPlayer &&
               Table.Board[2, position.X, position.Y, position.Z] == CurrentPlayer)
                mills++;
            //Annak megfelel�en hogy hol az egyes a koordin�ta meg kell vizsg�lni a sort/oszlopot/dimenzi�t
            //Ha az x=1 akkor f�gg�legesen kell vizsg�lni
            if(position.X == 1)
            {
                if (Table.Board[position.W, 0, position.Y, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, 1, position.Y, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, 2, position.Y, position.Z] == CurrentPlayer)
                    mills++;
            }
            //Ha az y=1 akkor v�zszintesen kell vizsg�lni
            else if(position.Y == 1)
            {
                if (Table.Board[position.W, position.X, 0, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, position.X, 1, position.Z] == CurrentPlayer &&
                    Table.Board[position.W, position.X, 2, position.Z] == CurrentPlayer)
                        mills++;
            }
            //Ha a z=1 akkor dimenzi�san kell vizsg�lni
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
