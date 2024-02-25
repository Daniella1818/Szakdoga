using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveStageOperator : AOperator
{
    Position Position;
    public RemoveStageOperator(Position position)
    {
        Position = position;
    }

    public override State Apply(State currentState)
    {
        State newState = (State)currentState.Clone();
        newState.Table.Board[(int)Position.W, (int)Position.X, (int)Position.Y, (int)Position.Z] = Stone.Empty;

        if (currentState.LastStage != Stage.First)
        {
            if (currentState.CurrentPlayer == Stone.Red)
                newState.blueStoneCount--;
            else if (currentState.CurrentPlayer == Stone.Blue)
                newState.redStoneCount--;
        }

        newState.currentPlayersMills--;
        checkIfStillRemoveStage(newState);
        return newState;
    }

    public override bool IsApplicable(State currentState)
    {
        if (currentState.CurrentStage == Stage.Remove)
        {
            Stone enemy = Stone.Empty;
            if (currentState.CurrentPlayer == Stone.Red)
                enemy = Stone.Blue;
            else if (currentState.CurrentPlayer == Stone.Blue)
                enemy = Stone.Red;

            if (stoneIsPlayers(currentState, enemy, Position))
            {
                //Ha csak malomban lévõ korongja van akkor, akkor levehet egy malomban lévõ korongot
                if (IsPlayerOnlyHaveStoneInMill(currentState, enemy))
                    return true;
                else
                {
                    if (currentState.CountMill(Position, enemy) == 0)
                        return true;
                }
            }
        }

        return false;
    }
}