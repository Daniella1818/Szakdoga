using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondStageOperator : AOperator
{
    Position startPosition, endPosition;
    public SecondStageOperator(Position start, Position end)
    {
        this.startPosition = start;
        this.endPosition = end;
    }

    public override State Apply(State currentState)
    {
        State newState = (State)currentState.Clone();
        setPlaceEmpty(newState, startPosition);
        setStoneToPlace(newState, endPosition);
        return newState;
    }

    public override bool IsApplicable(State currentState)
    {
        //Jelenlegi stage az second-e
        if (currentState.CurrentStage == Stage.Second)
        {
            //Ha a kiválasztott kõ az a sajátja-e, és ha a vég pozició üres
            if (stoneIsPlayers(currentState, currentState.CurrentPlayer, startPosition)
                && positionIsEmpty(currentState, endPosition))
            {
                //Vízszintes mozgás, itt az oszlop változik a sor marad
                //Vízszintes mozgás csak a 0 vagy 2 sorban lehet
                //VAGY a dimenzió mozgás esete, de akkor a sor és az oszlop értéke marad csak a Z értéke változik 1-gyel
                //Mindkettõnél azonosnak kell lennie a szintnek
                int sX = startPosition.X; int sY = startPosition.Y; int sZ = startPosition.Z; int sW = startPosition.W;
                int eX = endPosition.X; int eY = endPosition.Y; int eZ = endPosition.Z; int eW = endPosition.W;

                int diffY = eY - sY;
                int diffZ = eZ - sZ;
                if (sW == eW && sX == eX && (sX == 0 || sX == 2) &&
                   ((Math.Abs(diffY) == 1 && sZ == eZ) || (sY == eY && Math.Abs(diffZ) == 1)))
                    return true;

                //Függõleges mozgás, itt az oszlop marad de ez csak 0 vagy 2 lehet!, sor változik 1-gyel
                //Azonos szinten maradunk
                int diffX = eX - sX;
                if (sW == eW && (sY == 0 || sY == 2) && sY == eY && Math.Abs(diffX) == 1)
                    return true;

                //Szintek közötti mozgás, itt a szint 1-gyel mozoghat, oszlop, sor, dimenzio ugyanaz marad
                //Csak az elsõ oszlopból vagy sorból (mátrix indexelés miatt a z-t is bele kell tenni) mozoghatunk szintet!
                int diffW = eW - sW;
                if ((sY == 1 || sX == 1 || sZ == 1) && sY == eY && sX == eX && sZ == eZ && Math.Abs(diffW) == 1)
                    return true;
            }
        }
        return false;
    }
}
