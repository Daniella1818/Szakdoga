using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStageOperator : AOperator
{
    public Position position;
    public FirstStageOperator(Position p)
    {
        position = p;
    }

    public override State Apply(State currentState)
    {
        State newState = (State)currentState.Clone();
        setStoneToPlace(newState, position);

        //Megn�velem a lerakott k�vek sz�m�t a j�t�kosnak megfelel�en
        if (currentState.CurrentPlayer == Stone.Red)
            newState.redStoneCount++;
        else if (currentState.CurrentPlayer == Stone.Blue)
            newState.blueStoneCount++;

        //Megn�zz�k, hogy mindkett� j�t�kosnak le van-e rakva a 9-9 korongja, ha igen v�ltunk stage-t
        if (newState.redStoneCount == 9 && newState.blueStoneCount == 9)
            newState.CurrentStage = Stage.Second;

        checkForRemoveStage(newState);
        return newState;
    }

    public override bool IsApplicable(State currentState)
    {
        //Ha a first stage-ben vagyunk akkor lehet csak alakalmazva
        if (currentState.CurrentStage == Stage.First)
            return positionIsEmpty(currentState, position);

        return false;
    }
}
