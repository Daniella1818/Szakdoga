using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AHeuristics
{
    protected State currentState;
    public AHeuristics()
    {
    }

    protected static int WIN = 100;
    protected static int LOSE = -100;
    //Ez akkor amikor az ellenfélnek az adott helyen 2 korongja van és a jelenlegi játékos odateszi a korongját,
    //hogy ne legyen malma az ellenfélnek
    protected static int POSSIBLE_MILL;
    protected static int POSSIBLE_MILL_FOR_OTHER_PLAYER;
    //Ez akkor amikor az ellenfélnek már az adott helyen a 2 korongja van a jelenlegi játékosnak 0 korongja 
    protected static int CREATE_A_MILL;
    protected static int OTHER_PLAYER_CREATE_A_MILL;
    protected static int PROTECT_FROM_MILL;
    protected static int OTHER_PLAYER_MOVEABILITY;
    protected static int MOVEABILITY;
    protected static int PLAYERS_STONES;
    protected static int OTHER_PLAYERS_STONES;
    public abstract int GetHeuristics(State currentState, Stone player);
    private int CalculateHeuristics(int playerCount, int otherPlayerCount)
    {
        int score = 0;

        if (playerCount == 2 && otherPlayerCount != 1)
            score = score + POSSIBLE_MILL;
        else if (playerCount == 3)
            score = score + CREATE_A_MILL;
        //Másik játékosnak lehetséges malom
        else if (otherPlayerCount == 2 && playerCount != 1)
            score = score - POSSIBLE_MILL_FOR_OTHER_PLAYER;
        //Másik játékosnak malom alakult ki
        else if (otherPlayerCount == 3)
            score = score - OTHER_PLAYER_CREATE_A_MILL;
        //Ez lehet pozitív és negatív is, attól függ, hogy milyen heurisztika
        else if (playerCount == 1 && otherPlayerCount == 1)
            score = score + PROTECT_FROM_MILL;

        return score;
    }

    protected int CountPotentialMills(Stone player, Stone otherPlayer)
    {
        int score = 0;
        int playerCount = 0;
        int otherPlayerCount = 0;

        for (int w = 0; w < currentState.Table.Board.GetLength(0); w++)
        {
            //Nulladik sor, második sor nulladik és második dimenzióban
            for (int x = 0; x < currentState.Table.Board.GetLength(1); x += 2)
            {
                for (int z = 0; z < currentState.Table.Board.GetLength(3); z += 2)
                {
                    playerCount = HorizontalCheckForHeuristics(w, x, z, player);
                    otherPlayerCount = HorizontalCheckForHeuristics(w, x, z, otherPlayer);

                    score += CalculateHeuristics(playerCount, otherPlayerCount);
                }
            }
            //Nulladik oszlop, harmadik oszlop nulladik és második dimenzióban
            for (int y = 0; y < currentState.Table.Board.GetLength(2); y += 2)
            {
                for (int z = 0; z < currentState.Table.Board.GetLength(3); z += 2)
                {
                    playerCount = VerticalCheckForHeuristics(w, y, z, player);
                    otherPlayerCount = VerticalCheckForHeuristics(w, y, z, otherPlayer);
                    score += CalculateHeuristics(playerCount, otherPlayerCount);
                }
            }
            //Nulladik oszlop, masodik oszlop nulladik sor és második sorban
            for (int x = 0; x < currentState.Table.Board.GetLength(1); x += 2)
            {
                for (int y = 0; y < currentState.Table.Board.GetLength(2); y += 2)
                {
                    playerCount = DimensionalCheckForHeuristics(w, x, y, player);
                    otherPlayerCount = DimensionalCheckForHeuristics(w, x, y, otherPlayer);
                    score += CalculateHeuristics(playerCount, otherPlayerCount);
                }
            }
        }

        //Szintek között
        //Amikor x = 1
        for (int y = 0; y < currentState.Table.Board.GetLength(2); y += 2)
        {
            for (int z = 0; z < currentState.Table.Board.GetLength(3); z += 2)
            {
                playerCount = BetweenLevelsCheckForHeuristics(1, y, z, player);
                otherPlayerCount = BetweenLevelsCheckForHeuristics(1, y, z, otherPlayer);
                score += CalculateHeuristics(playerCount, otherPlayerCount);
            }
        }
        //Amikor y = 1
        for (int x = 0; x < currentState.Table.Board.GetLength(1); x += 2)
        {
            for (int z = 0; z < currentState.Table.Board.GetLength(2); z += 2)
            {
                playerCount = BetweenLevelsCheckForHeuristics(x, 1, z, player);
                otherPlayerCount = BetweenLevelsCheckForHeuristics(x, 1, z, otherPlayer);
                score += CalculateHeuristics(playerCount, otherPlayerCount);
            }
        }
        //Amikor z = 1
        for (int x = 0; x < currentState.Table.Board.GetLength(1); x += 2)
        {
            for (int y = 0; y < currentState.Table.Board.GetLength(2); y += 2)
            {
                playerCount = BetweenLevelsCheckForHeuristics(x, y, 1, player);
                otherPlayerCount = BetweenLevelsCheckForHeuristics(x, y, 1, otherPlayer);
                score += CalculateHeuristics(playerCount, otherPlayerCount);
            }
        }

        return score;
    }
    //Megnézi a játékos korongjainak mozgathatóságát
    protected int MoveabilityOfStones(Stone player)
    {
        return CountPlayerMoveableStones(player);
    }

    public int CountPlayerMoveableStones(Stone player)
    {
        int moveableStone = 0;
        for (int w = 0; w < currentState.Table.Board.GetLength(0); w++)
        {
            for (int x = 0; x < currentState.Table.Board.GetLength(1); x++)
            {
                for (int y = 0; y < currentState.Table.Board.GetLength(2); y++)
                {
                    for (int z = 0; z < currentState.Table.Board.GetLength(3); z++)
                    {
                        if (currentState.Table.Board[w, x, y, z] == player)
                        {
                            //Ha a sarokban van akkor három mellette lévõt kell megnézni
                            //Bal oldali sarkról van szó, ha az y = 0, jobb oldaliról ha y = 2
                            //Mindkettõ esetén a nulladik és a második sort kell vizsgálni
                            if ((y == 0 || y == 2) && (x == 0 || x == 2))
                            {
                                //Elõl és hátul is meg kell nézni
                                if (z == 0 || z == 2)
                                {
                                    if (currentState.Table.Board[w, x, y, 1] == Stone.Empty)
                                        moveableStone++;
                                    if (currentState.Table.Board[w, 1, y, z] == Stone.Empty)
                                        moveableStone++;
                                    if (currentState.Table.Board[w, x, 1, z] == Stone.Empty)
                                        moveableStone++;
                                }
                            }

                            //Ha középen van, kettõt mellette, és egyet/kettõt a kövi szinten
                            if (x == 1)
                            {
                                if (currentState.Table.Board[w, 0, y, z] == Stone.Empty)
                                    moveableStone++;
                                if (currentState.Table.Board[w, 2, y, z] == Stone.Empty)
                                    moveableStone++;
                            }
                            else if (y == 1)
                            {
                                if (currentState.Table.Board[w, x, 0, z] == Stone.Empty)
                                    moveableStone++;
                                if (currentState.Table.Board[w, x, 2, z] == Stone.Empty)
                                    moveableStone++;

                            }
                            else if (z == 1)
                            {
                                if (currentState.Table.Board[w, x, y, 0] == Stone.Empty)
                                    moveableStone++;
                                if (currentState.Table.Board[w, x, y, 2] == Stone.Empty)
                                    moveableStone++;
                            }

                            if (x == 1 || y == 1 || z == 1)
                            {
                                //Ha a nulladik szinten vagyunk 
                                if (w == 0 && currentState.Table.Board[1, x, y, z] == Stone.Empty)
                                    moveableStone++;
                                //Ha az elsõ szinten vagyunk
                                else if (w == 1)
                                {
                                    if (currentState.Table.Board[0, x, y, z] == Stone.Empty)
                                        moveableStone++;
                                    if (currentState.Table.Board[2, x, y, z] == Stone.Empty)
                                        moveableStone++;
                                }
                                //Ha a második szinten vagyunk
                                else if (w == 2 && currentState.Table.Board[1, x, y, z] == Stone.Empty)
                                    moveableStone++;
                            }
                        }
                    }
                }
            }
        }
        return moveableStone;
    }

    private int HorizontalCheckForHeuristics(int w, int x, int z, Stone player)
    {
        int playersCount = 0;
        if (currentState.Table.Board[w, x, 0, z] == player)
            playersCount++;
        if (currentState.Table.Board[w, x, 1, z] == player)
            playersCount++;
        if (currentState.Table.Board[w, x, 2, z] == player)
            playersCount++;

        return playersCount;
    }
    private int VerticalCheckForHeuristics(int w, int y, int z, Stone player)
    {
        int playersCount = 0;
        if (currentState.Table.Board[w, 0, y, z] == player)
            playersCount++;
        if (currentState.Table.Board[w, 1, y, z] == player)
            playersCount++;
        if (currentState.Table.Board[w, 2, y, z] == player)
            playersCount++;

        return playersCount;
    }
    private int DimensionalCheckForHeuristics(int w, int x, int y, Stone player)
    {
        int playersCount = 0;
        if (currentState.Table.Board[w, x, y, 0] == player)
            playersCount++;
        if (currentState.Table.Board[w, x, y, 1] == player)
            playersCount++;
        if (currentState.Table.Board[w, x, y, 2] == player)
            playersCount++;

        return playersCount;
    }
    private int BetweenLevelsCheckForHeuristics(int x, int y, int z, Stone player)
    {
        int playersCount = 0;
        if (currentState.Table.Board[0, x, y, z] == player)
            playersCount++;
        if (currentState.Table.Board[1, x, y, z] == player)
            playersCount++;
        if (currentState.Table.Board[2, x, y, z] == player)
            playersCount++;

        return playersCount;
    }
}
