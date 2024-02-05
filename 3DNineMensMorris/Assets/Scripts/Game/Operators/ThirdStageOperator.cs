using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdStageOperator : AOperator
{
    Position startPosition, endPosition;

    public ThirdStageOperator(Position start, Position end)
    {
        this.startPosition = start;
        this.endPosition = end;
    }

    public override State Apply(State currentState)
    {
        State newState = (State)currentState.Clone();
        setPlaceEmpty(newState, startPosition);
        setStoneToPlace(newState, endPosition);
        checkForRemoveStage(newState, endPosition);
        return newState;
    }

    public override bool IsApplicable(State currentState)
    {
        //Ha a harmadik stage-ben vagyunk
        if (currentState.CurrentStage == Stage.Third)
        {
            if (stoneIsPlayers(currentState, currentState.CurrentPlayer, startPosition))
                return positionIsEmpty(currentState, endPosition);
        }

        return false;
    }
}
