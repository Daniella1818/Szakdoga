using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveHeuristics : AHeuristics
{
    public DefensiveHeuristics(State state)
        :base(state){}

    protected new static int POSSIBLE_MILL = 8;
    protected new static int POSSIBLE_MILL_FOR_OTHER_PLAYER = 11;
    //Ez akkor amikor az ellenfélnek már az adott helyen a 2 korongja van a jelenlegi játékosnak 0 korongja 
    protected new static int CREATE_A_MILL = 7;
    protected new static int OTHER_PLAYER_CREATE_A_MILL = 10;
    protected new static int PROTECT_FROM_MILL = 2;
    public override int GetHeuristics(Stone player)
    {
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
            result += currentPlayersStone - otherPlayersStone;
        else
            result -= otherPlayersStone - currentPlayersStone;

        result += CountPotentialMillsAndNeighborCellCheck(currentPlayer, otherPlayer);
        return result;
    }
}
