using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveHeuristics : AHeuristics
{
    public DefensiveHeuristics()
    {
        POSSIBLE_MILL = 7;
        POSSIBLE_MILL_FOR_OTHER_PLAYER = 10;
        CREATE_A_MILL = 8;
        OTHER_PLAYER_CREATE_A_MILL = 11;
        PROTECT_FROM_MILL = 2;
        OTHER_PLAYER_MOVEABILITY = 3;
        MOVEABILITY = 2;
        PLAYERS_STONES = 1;
        OTHER_PLAYERS_STONES = 2;
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
            if (currentPlayersStone > otherPlayersStone)
                result += PLAYERS_STONES;
            else
                result -= OTHER_PLAYERS_STONES;
        }
        else if(currentState.CurrentStage == Stage.Third)
        {
            CREATE_A_MILL = CREATE_A_MILL * 2;
            POSSIBLE_MILL = POSSIBLE_MILL * 2;
        }

        result += CountPotentialMills(currentPlayer, otherPlayer);

        return result;
    }
}
