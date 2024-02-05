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

        //Cs�kkentem a lerakhat� korongok sz�m�t
        if (currentState.CurrentPlayer == Stone.Red)
            newState.redStoneCount--;
        else if (currentState.CurrentPlayer == Stone.Blue)
            newState.blueStoneCount--;

        //Megn�zz�k, hogy mindkett� j�t�kosnak le van-e rakva a 9-9 korongja, ha igen v�ltunk stage-t
        //Megsz�moljuk kinek mennyi van �s azt t�roljuk el
        if (newState.redStoneCount == 0 && newState.blueStoneCount == 0)
        {
            newState.CurrentStage = Stage.Second;
            newState.redStoneCount = CountPlayersStones(newState, Stone.Red);
            newState.blueStoneCount = CountPlayersStones(newState, Stone.Blue);
        }

        checkForRemoveStage(newState, position);
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
