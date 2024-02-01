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
            //Ha a kiv�lasztott k� az a saj�tja-e, �s ha a v�g pozici� �res
            if (stoneIsPlayers(currentState, currentState.CurrentPlayer, startPosition)
                && positionIsEmpty(currentState, endPosition))
            {
                //V�zszintes mozg�s, itt az oszlop v�ltozik a sor marad
                //V�zszintes mozg�s csak a 0 vagy 2 sorban lehet
                //VAGY a dimenzi� mozg�s esete, de akkor a sor �s az oszlop �rt�ke marad csak a Z �rt�ke v�ltozik 1-gyel
                //Mindkett�n�l azonosnak kell lennie a szintnek
                int sX = startPosition.X; int sY = startPosition.Y; int sZ = startPosition.Z; int sW = startPosition.W;
                int eX = endPosition.X; int eY = endPosition.Y; int eZ = endPosition.Z; int eW = endPosition.W;

                int diffY = eY - sY;
                int diffZ = eZ - sZ;
                if (sW == eW && sX == eX && (sX == 0 || sX == 2) &&
                   ((Math.Abs(diffY) == 1 && sZ == eZ) || (sY == eY && Math.Abs(diffZ) == 1)))
                    return true;

                //F�gg�leges mozg�s, itt az oszlop marad de ez csak 0 vagy 2 lehet!, sor v�ltozik 1-gyel
                //Azonos szinten maradunk
                int diffX = eX - sX;
                if (sW == eW && (sY == 0 || sY == 2) && sY == eY && Math.Abs(diffX) == 1)
                    return true;

                //Szintek k�z�tti mozg�s, itt a szint 1-gyel mozoghat, oszlop, sor, dimenzio ugyanaz marad
                //Csak az els� oszlopb�l vagy sorb�l (m�trix indexel�s miatt a z-t is bele kell tenni) mozoghatunk szintet!
                int diffW = eW - sW;
                if ((sY == 1 || sX == 1 || sZ == 1) && sY == eY && sX == eX && sZ == eZ && Math.Abs(diffW) == 1)
                    return true;
            }
        }
        return false;
    }
}
