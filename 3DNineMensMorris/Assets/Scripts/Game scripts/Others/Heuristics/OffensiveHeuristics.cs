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
        Stone currentPlayer;

        int currentPlayersStone;
        Stone otherPlayer;
        int otherPlayersStone;

        if (player == Stone.Red)
        {
            currentPlayer = Stone.Red;
            currentPlayersStone = currentState.redStoneCount;
            otherPlayer = Stone.Blue;
            otherPlayersStone = currentState.blueStoneCount;
        }
        else
        {
            currentPlayer = Stone.Blue;
            currentPlayersStone = currentState.blueStoneCount;
            otherPlayer = Stone.Red;
            otherPlayersStone = currentState.redStoneCount;
        }

        //Nagyobb legyen a heurisztika, ha a jelenlegi j�t�kosnak t�bb b�buja van
        if (currentPlayersStone > otherPlayersStone)
            result += 1;
        else
            result -= 1;

        result += CountPotentialMills(currentPlayer, otherPlayer);

        if (currentState.CurrentStage == Stage.Second)
        {
            result += MoveabilityOfStones(currentPlayer) * MOVEABILITY;
            result -= MoveabilityOfStones(otherPlayer) * OTHER_PLAYER_MOVEABILITY;
        }

        return result;
    }
}
