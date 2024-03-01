using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdStageOperator : AOperator
{
    private Position startPosition, endPosition;

    public ThirdStageOperator(Position start, Position end)
    {
        this.startPosition = start;
        this.endPosition = end;
    }

    public override State Apply(State currentState)
    {
        State newState = (State)currentState.Clone();
        SetPlaceEmpty(newState, startPosition);
        SetStoneToPlace(newState, endPosition);
        CheckForRemoveStage(newState, endPosition);
        return newState;
    }

    public override bool IsApplicable(State currentState)
    {
        //Ha a harmadik stage-ben vagyunk
        if (currentState.CurrentStage == Stage.Third)
        {
            if (StoneIsPlayers(currentState, currentState.CurrentPlayer, startPosition))
                return PositionIsEmpty(currentState, endPosition);
        }

        return false;
    }
}
