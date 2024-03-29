using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStageOperator : AOperator
{
    private Position position;
    public FirstStageOperator(Position p)
    {
        position = p;
    }

    public override State Apply(State currentState)
    {
        State newState = (State)currentState.Clone();
        SetStoneToPlace(newState, position);

        //Cs�kkentem a lerakhat� korongok sz�m�t
        if (currentState.CurrentPlayer == Stone.Red)
            newState.RedStoneCount--;
        else if (currentState.CurrentPlayer == Stone.Blue)
            newState.BlueStoneCount--;

        //Megn�zz�k, hogy mindkett� j�t�kosnak le van-e rakva a 9-9 korongja, ha igen v�ltunk stage-t
        //Megsz�moljuk kinek mennyi van �s azt t�roljuk el
        if (newState.RedStoneCount == 0 && newState.BlueStoneCount == 0)
        {
            newState.CurrentStage = Stage.Second;
            newState.RedStoneCount = CountPlayersStones(newState, Stone.Red);
            newState.BlueStoneCount = CountPlayersStones(newState, Stone.Blue);
        }

        CheckForRemoveStage(newState, position);
        return newState;
    }

    public override bool IsApplicable(State currentState)
    {
        //Ha a first stage-ben vagyunk akkor lehet csak alakalmazva
        if (currentState.CurrentStage == Stage.First)
            return PositionIsEmpty(currentState, position);

        return false;
    }
}
