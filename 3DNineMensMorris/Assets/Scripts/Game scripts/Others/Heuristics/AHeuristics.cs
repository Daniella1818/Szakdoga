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
    //Ez akkor amikor az ellenf�lnek az adott helyen 2 korongja van �s a jelenlegi j�t�kos odateszi a korongj�t,
    //hogy ne legyen malma az ellenf�lnek
    protected static int POSSIBLE_MILL;
    protected static int POSSIBLE_MILL_FOR_OTHER_PLAYER;
    //Ez akkor amikor az ellenf�lnek m�r az adott helyen a 2 korongja van a jelenlegi j�t�kosnak 0 korongja 
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
        //M�sik j�t�kosnak lehets�ges malom
        else if (otherPlayerCount == 2 && playerCount != 1)
            score = score - POSSIBLE_MILL_FOR_OTHER_PLAYER;
        //M�sik j�t�kosnak malom alakult ki
        else if (otherPlayerCount == 3)
            score = score - OTHER_PLAYER_CREATE_A_MILL;
        //Ez lehet pozit�v �s negat�v is, att�l f�gg, hogy milyen heurisztika
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
            //Nulladik sor, m�sodik sor nulladik �s m�sodik dimenzi�ban
            for (int x = 0; x < currentState.Table.Board.GetLength(1); x += 2)
            {
                for (int z = 0; z < currentState.Table.Board.GetLength(3); z += 2)
                {
                    playerCount = HorizontalCheckForHeuristics(w, x, z, player);
                    otherPlayerCount = HorizontalCheckForHeuristics(w, x, z, otherPlayer);

                    score += CalculateHeuristics(playerCount, otherPlayerCount);
                }
            }
            //Nulladik oszlop, harmadik oszlop nulladik �s m�sodik dimenzi�ban
            for (int y = 0; y < currentState.Table.Board.GetLength(2); y += 2)
            {
                for (int z = 0; z < currentState.Table.Board.GetLength(3); z += 2)
                {
                    playerCount = VerticalCheckForHeuristics(w, y, z, player);
                    otherPlayerCount = VerticalCheckForHeuristics(w, y, z, otherPlayer);
                    score += CalculateHeuristics(playerCount, otherPlayerCount);
                }
            }
            //Nulladik oszlop, masodik oszlop nulladik sor �s m�sodik sorban
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

        //Szintek k�z�tt
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
    //Megn�zi a j�t�kos korongjainak mozgathat�s�g�t
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
                            //Ha a sarokban van akkor h�rom mellette l�v�t kell megn�zni
                            //Bal oldali sarkr�l van sz�, ha az y = 0, jobb oldalir�l ha y = 2
                            //Mindkett� eset�n a nulladik �s a m�sodik sort kell vizsg�lni
                            if ((y == 0 || y == 2) && (x == 0 || x == 2))
                            {
                                //El�l �s h�tul is meg kell n�zni
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

                            //Ha k�z�pen van, kett�t mellette, �s egyet/kett�t a k�vi szinten
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
                                //Ha az els� szinten vagyunk
                                else if (w == 1)
                                {
                                    if (currentState.Table.Board[0, x, y, z] == Stone.Empty)
                                        moveableStone++;
                                    if (currentState.Table.Board[2, x, y, z] == Stone.Empty)
                                        moveableStone++;
                                }
                                //Ha a m�sodik szinten vagyunk
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
