using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AHeuristics
{
    protected State currentState;
    public AHeuristics(State state)
    {
        currentState = state;
    }

    protected static int WIN = 100;
    protected static int LOSE = -100;
    //Ez akkor amikor az ellenf�lnek az adott helyen 2 korongja van �s a jelenlegi j�t�kos odateszi a korongj�t,
    //hogy ne legyen malma az ellenf�lnek
    protected static int POSSIBLE_MILL = 0;
    protected static int POSSIBLE_MILL_FOR_OTHER_PLAYER = 0;
    //Ez akkor amikor az ellenf�lnek m�r az adott helyen a 2 korongja van a jelenlegi j�t�kosnak 0 korongja 
    protected static int CREATE_A_MILL = 0;
    protected static int OTHER_PLAYER_CREATE_A_MILL = 0;
    protected static int PROTECT_FROM_MILL = 1 ;

    public abstract int GetHeuristics(Stone player);

    protected int CountPotentialMillsAndNeighborCellCheck(Stone player, Stone otherPlayer)
    {
        int score = 0;
        int playerCount = 0;
        int otherPlayerCount = 0;

        for (int w = 0; w < 3; w++)
        {
            //Nulladik sor, m�sodik sor nulladik �s m�sodik dimenzi�ban
            for (int x = 0; x < 3; x += 2)
            {
                for (int z = 0; z < 3; z += 2)
                {
                    playerCount = HorizontalCheckForHeuristics(w, x, z, player);
                    otherPlayerCount = HorizontalCheckForHeuristics(w, x, z, otherPlayer);

                    //Kett� korong eset�n meg kellene n�zni a szomsz�dos helyeket, hogy van-e ott korongja, ha van akkor
                    //n�velni k�ne m�g a heurisztik�t
                    if (playerCount == 2 && otherPlayerCount != 1)
                        score += POSSIBLE_MILL;
                    else if (playerCount == 3)
                    {
                        score += CREATE_A_MILL;
                        for (int y = 0; y < 3; y += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, 1, y, z, Stone.Empty))
                                score++;
                            if (IsNeigborPlaceIsStone(w, x, y, 1, Stone.Empty))
                                score++;
                        }
                    }
                    //M�sik j�t�kosnak lehets�ges malom
                    else if (otherPlayerCount == 2 && playerCount != 1)
                        score -= POSSIBLE_MILL_FOR_OTHER_PLAYER;
                    //M�sik j�t�kosnak malom alakult ki
                    else if (otherPlayerCount == 3)
                    {
                        score -= OTHER_PLAYER_CREATE_A_MILL;
                        for (int y = 0; y < 3; y += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, 1, y, z, Stone.Empty))
                                score--;
                            if (IsNeigborPlaceIsStone(w, x, y, 1, Stone.Empty))
                                score--;
                        }
                    }
                    else if (playerCount == 1 && otherPlayerCount == 1)
                        score += PROTECT_FROM_MILL;
                }
            }
            //Nulladik oszlop, harmadik oszlop nulladik �s m�sodik dimenzi�ban
            for (int y = 0; y < 3; y += 2)
            {
                for (int z = 0; z < 3; z += 2)
                {
                    playerCount = VerticalCheckForHeuristics(w, y, z, player);
                    otherPlayerCount = VerticalCheckForHeuristics(w, y, z, otherPlayer);
                    //Csak akkor sz�m�t potenci�lis malomnak, ha 2 ellenf�l korongja maga van
                    if (playerCount == 2 && otherPlayerCount != 1)
                        score += POSSIBLE_MILL;
                    else if (playerCount == 3)
                    {
                        score += CREATE_A_MILL;
                        for (int x = 0; x < 3; x += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, 1, z, Stone.Empty))
                                score++;
                            if (IsNeigborPlaceIsStone(w, x, y, 1, Stone.Empty))
                                score++;
                        }
                    }
                    else if (otherPlayerCount == 2 && playerCount != 1)
                        score -= POSSIBLE_MILL_FOR_OTHER_PLAYER;
                    else if (otherPlayerCount == 3)
                    {
                        score -= OTHER_PLAYER_CREATE_A_MILL;
                        for (int x = 0; x < 3; x += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, 1, z, Stone.Empty))
                                score--;
                            if (IsNeigborPlaceIsStone(w, x, y, 1, Stone.Empty))
                                score--;
                        }
                    }
                    else if (playerCount == 1 && otherPlayerCount == 1)
                        score -= PROTECT_FROM_MILL;
                }
            }
            //Nulladik oszlop, masodik oszlop nulladik sor �s m�sodik sorban
            for (int x = 0; x < 3; x += 2)
            {
                for (int y = 0; y < 3; y += 2)
                {
                    playerCount = DimensionalCheckForHeuristics(w, x, y, player);
                    otherPlayerCount = DimensionalCheckForHeuristics(w, x, y, otherPlayer);
                    if (playerCount == 2 && otherPlayerCount != 1)
                        score += POSSIBLE_MILL;
                    else if (playerCount == 3)
                    {
                        score += CREATE_A_MILL;
                        for (int z = 0; z < 3; z += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, 1, y, z, Stone.Empty))
                                score++;
                            if (IsNeigborPlaceIsStone(w, x, 1, z, Stone.Empty))
                                score++;
                        }
                    }
                    else if (otherPlayerCount == 2 && playerCount != 1)
                        score -= POSSIBLE_MILL_FOR_OTHER_PLAYER;
                    else if (otherPlayerCount == 3)
                    {
                        score -= OTHER_PLAYER_CREATE_A_MILL;
                        for (int z = 0; z < 3; z += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, 1, y, z, Stone.Empty))
                                score--;
                            if (IsNeigborPlaceIsStone(w, x, 1, z, Stone.Empty))
                                score--;
                        }
                    }
                    else if (playerCount == 1 && otherPlayerCount == 1)
                        score -= PROTECT_FROM_MILL;
                }
            }
        }

        //Szintek k�z�tt
        //Amikor x = 1
        for (int y = 0; y < 3; y += 2)
        {
            for (int z = 0; z < 3; z += 2)
            {
                playerCount = BetweenLevelsCheckForHeuristics(1, y, z, player);
                otherPlayerCount = BetweenLevelsCheckForHeuristics(1, y, z, otherPlayer);
                if (playerCount == 2 && otherPlayerCount != 1)
                    score += POSSIBLE_MILL;
                else if (playerCount == 3)
                {
                    score += CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int x = 0; x < 3; x += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, y, z, Stone.Empty))
                                score++;
                        }
                    }
                }
                else if (otherPlayerCount == 2 && playerCount != 1)
                    score -= POSSIBLE_MILL_FOR_OTHER_PLAYER;
                else if (otherPlayerCount == 3)
                {
                    score -= OTHER_PLAYER_CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int x = 0; x < 3; x += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, y, z, Stone.Empty))
                                score--;
                        }
                    }
                }
                else if (playerCount == 1 && otherPlayerCount == 1)
                    score -= PROTECT_FROM_MILL;
            }
        }
        //Amikor y = 1
        for (int x = 0; x < 3; x += 2)
        {
            for (int z = 0; z < 3; z += 2)
            {
                playerCount = BetweenLevelsCheckForHeuristics(x, 1, z, player);
                otherPlayerCount = BetweenLevelsCheckForHeuristics(x, 1, z, otherPlayer);
                if (playerCount == 2 && otherPlayerCount != 1)
                    score += POSSIBLE_MILL;
                else if (playerCount == 3)
                {
                    score += CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int y = 0; y < 3; y += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, y, z, Stone.Empty))
                                score++;
                        }
                    }
                }
                else if (otherPlayerCount == 2 && playerCount != 1)
                    score -= POSSIBLE_MILL_FOR_OTHER_PLAYER;
                else if (otherPlayerCount == 3)
                {
                    score -= OTHER_PLAYER_CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int y = 0; y < 3; y += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, y, z, Stone.Empty))
                                score--;
                        }
                    }
                }
                else if (playerCount == 1 && otherPlayerCount == 1)
                    score -= PROTECT_FROM_MILL;
            }
        }
        //Amikor z = 1
        for (int x = 0; x < 3; x += 2)
        {
            for (int y = 0; y < 3; y += 2)
            {
                playerCount = BetweenLevelsCheckForHeuristics(x, y, 1, player);
                otherPlayerCount = BetweenLevelsCheckForHeuristics(x, y, 1, otherPlayer);
                if (playerCount == 2 && otherPlayerCount != 1)
                    score += POSSIBLE_MILL;
                else if (playerCount == 3)
                {
                    score += CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int z = 0; z < 3; z += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, y, z, Stone.Empty))
                                score++;
                        }
                    }
                }
                else if (otherPlayerCount == 2 && playerCount != 1)
                    score -= POSSIBLE_MILL_FOR_OTHER_PLAYER;
                else if (otherPlayerCount == 3)
                {
                    score -= OTHER_PLAYER_CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int z = 0; z < 3; z += 2)
                        {
                            if (IsNeigborPlaceIsStone(w, x, y, z, Stone.Empty))
                                score--;
                        }
                    }
                }
                else if (playerCount == 1 && otherPlayerCount == 1)
                    score -= PROTECT_FROM_MILL;
            }
        }

        return score;
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
    private bool IsNeigborPlaceIsStone(int w, int x, int y, int z, Stone stone)
    {
        return currentState.Table.Board[w, x, y, z] == stone;
    }
}
