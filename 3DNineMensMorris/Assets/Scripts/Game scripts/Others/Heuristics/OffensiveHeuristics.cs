using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveHeuristics : AHeuristics
{
    public OffensiveHeuristics()
    {
        POSSIBLE_MILL = 11;
        POSSIBLE_MILL_FOR_OTHER_PLAYER = 8;
        CREATE_A_MILL = 10;
        OTHER_PLAYER_CREATE_A_MILL = 7;
        PROTECT_FROM_MILL = -2;
        OTHER_PLAYER_MOVEABILITY = 2;
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
        else if (currentState.CurrentStage == Stage.Third)
        {
            CREATE_A_MILL = CREATE_A_MILL * 2;
            POSSIBLE_MILL = POSSIBLE_MILL * 2;
        }

        result += CountPotentialMills(currentPlayer, otherPlayer);

        return result;
    }
}
