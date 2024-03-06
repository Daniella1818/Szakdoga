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
        OTHER_PLAYER_MOVEABILITY = 2;
        MOVEABILITY = 2;
    }

    //protected new int POSSIBLE_MILL = 8;
    //protected new int POSSIBLE_MILL_FOR_OTHER_PLAYER = 11;
    ////Ez akkor amikor az ellenfélnek már az adott helyen a 2 korongja van a jelenlegi játékosnak 0 korongja 
    //protected new int CREATE_A_MILL = 7;
    //protected new int OTHER_PLAYER_CREATE_A_MILL = 10;
    //protected new int PROTECT_FROM_MILL = 2;
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

        //Nagyobb legyen a heurisztika, ha a jelenlegi játékosnak több bábuja van
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
