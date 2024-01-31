using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStageOperator : AOperator
{
    Position position;
    public FirstStageOperator(Position p)
    {
        position = p;
    }

    public override State Apply(State currentState)
    {
        State newState = (State)currentState.Clone();
        setStoneToPlace(newState, position);

        //Megnövelem a lerakott kövek számát a játékosnak megfelelõen
        if (currentState.CurrentPlayer == Stone.Red)
            newState.blackStoneCount++;
        else if (currentState.CurrentPlayer == Stone.Blue)
            newState.whiteStoneCount++;

        return newState;
    }

    public override bool IsApplicable(State currentState)
    {
        //Megnézzük, hogy mindkettõ játékosnak le van-e rakva a 9-9 korongja, ha igen váltunk stage-t
        if (currentState.blackStoneCount == 9 && currentState.whiteStoneCount == 9)
            currentState.CurrentStage = Stage.Second;

        //Ha a first stage-ben vagyunk akkor lehet csak alakalmazva
        if (currentState.CurrentStage == Stage.First)
            return positionIsEmpty(currentState, position);

        return false;
    }
}
