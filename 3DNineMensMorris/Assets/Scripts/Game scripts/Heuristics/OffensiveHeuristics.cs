using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveHeuristics : AHeuristics
{
    public OffensiveHeuristics()
    {
        POSSIBLE_MILL = 9;
        POSSIBLE_MILL_FOR_OTHER_PLAYER = 6;
        CREATE_A_MILL = 11;
        OTHER_PLAYER_CREATE_A_MILL = 7;
        PROTECT_FROM_MILL = -2;
        OTHER_PLAYER_MOVEABILITY = 1;
        MOVEABILITY = 3;
    }

    public override int GetHeuristics(State state, Stone player)
    {
        currentState = state;

        if (currentState.GetStatus() == player)
            return WIN;
        else if (currentState.GetStatus() != Stone.Empty)
            return LOSE;

        int result = 0;
        UpdatePlayersStatus(player);

        if (currentState.CurrentStage == Stage.Second)
        {
            result += CountPlayerMoveableStones(currentPlayer) * MOVEABILITY;
            result -= CountPlayerMoveableStones(otherPlayer) * OTHER_PLAYER_MOVEABILITY;
        }

        result += CountPotentialMills(currentPlayer, otherPlayer);

        return result;
    }
}
