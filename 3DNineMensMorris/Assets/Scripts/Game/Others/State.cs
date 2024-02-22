using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State : ICloneable
{
    public CubeTable Table;
    public State()
    {
        Table = new CubeTable();
    }

    public Stone CurrentPlayer = Stone.Red;
    public Stage CurrentStage = Stage.First;

    //Ha remove stage van akkor ebbe mentj�k az aktu�lisat, hogy visszatudjunk t�rni r�
    public Stage LastStage;

    public int redStoneCount = 9, blueStoneCount = 9;
    public int currentPlayersMills = 0;

    //Jelenlegi j�t�kost cser�li, ez biztos�tja, hogy egym�s ut�n l�pjenek
    public void ChangePlayer()
    {
        if (CurrentPlayer == Stone.Red)
            CurrentPlayer = Stone.Blue;
        else if (CurrentPlayer == Stone.Blue)
            CurrentPlayer = Stone.Red;
    }

    //Megvizsg�lja, hogy van e nyertes, akkor vesz�t valaki ha 2 korongja maradt �sszesen, ha EMPTY-t ad vissza akkor m�g tudnak 
    //l�pni a j�t�kosok.
    public Stone GetStatus()
    {
        if (CurrentStage == Stage.Second || CurrentStage == Stage.Third)
        {
            //Azt is meg kellene vizsg�lni, hogy a jelenlegi j�t�kos tud-e l�pni, de csak a m�sodik stage-t�l
            //Meg kellene n�zni az �sszes saj�t korongj�t, �s mellette l�v� helyeket, ha legal�bb egy olyan korongja van
            //ami mellett �res a hely akkor tud l�pni egy�bk�nt nem
            //Ha nem tud l�pni akkor az ellenfelet adjuk vissza nyeretesk�nt
            if (isCurrentPlayersHaveMovableStone() == false)
            {
                if (CurrentPlayer == Stone.Red)
                    return Stone.Blue;
                else if (CurrentPlayer == Stone.Blue)
                    return Stone.Red;
            }

            if (blueStoneCount <= 2)
                return Stone.Red;
            else if (redStoneCount <= 2)
                return Stone.Blue;
            else
                return Stone.Empty;
        }
        else
            return Stone.Empty;
    }

    //Megn�zz�k, hogy az �jonnan elhelyezett korong alkot-e malmot, ha igen akkor visszaadja, hogy
    //mennyit (mivel egy l�p�s alkothat kett� vagy h�rom malmot is!)
    public int CountMill(Position position, Stone player)
    {
        int mills = 0;

        //Ha a sarokban ker�lt az �j korong, teh�t ezek a koordin�t�k lehetnek: (0.szinten)
        //0,0,0,0 || 0,0,2,0 || 0,2,0,0 || 0,2,2,0 || 0,0,2,2 || 0,2,2,2 || 0,0,0,2 || 0,2,0,2
        if((position.X == 0 || position.X == 2) && 
           (position.Y == 0 || position.Y == 2) && 
           (position.Z == 0 || position.Z == 2))
        {
            //Ilyenkor 3 ir�nyban kell megn�zni hogy j�tt-e l�tre malom
            //Mindh�romn�l marad a szint �rt�ke

            //Azonos sorban, az oszlopokat v�ltoztatva, a dimenzi� ugyanaz marad (v�zszintes)
            if (HorizontalCheck(position, player))
                mills++;
            //Azonos oszlopban, a sorokat v�ltoztatva, a dimenzi� ugyanaz marad (f�gg�leges)
            if (VerticalCheck(position, player))
                mills++;
            //Azonos oszlop, �s sor, a dimenzi�t v�ltoztatjuk (dimenzi�s)
            if (DimensionalCheck(position, player))
                mills++;
        }

        //Ha k�z�pre ker�l a korong, itt lehet szintek k�z�tt l�pkedni
        //0,0,1,0 || 0,1,0,0 || 0,1,2,0 || 0,2,1,0 || 0,0,2,1 || 0,1,2,2 || 0,2,2,1, || 0,0,0,1 ||
        //0,2,0,1 || 0,1,0,2 || 0,0,1,2 || 0,2,1,2
        if(position.X == 1 || position.Y == 1 || position.Z == 1)
        {
            //Ilyenkor 2 ir�nyba kell megvizsg�lni, hogy j�tt-e l�tre malom
            //Azonos sor, oszlop, dimenzi�, a szinteket kell l�ptetni (szintek k�z�tti)
            if (BetweenLevelsCheck(position, player))
                mills++;
            //Annak megfelel�en hogy hol az egyes a koordin�ta meg kell vizsg�lni a sort/oszlopot/dimenzi�t
            //Ha az x=1 akkor f�gg�legesen kell vizsg�lni
            if(position.X == 1)
            {
                if (VerticalCheck(position, player))
                    mills++;
            }
            //Ha az y=1 akkor v�zszintesen kell vizsg�lni
            else if(position.Y == 1)
            {
                if (HorizontalCheck(position, player))
                        mills++;
            }
            //Ha a z=1 akkor dimenzi�san kell vizsg�lni
            else if(position.Z == 1)
            {
                if (DimensionalCheck(position, player))
                    mills++;
            }
        }
        
        return mills;
    }

    public override bool Equals(object obj)
    {
        State other = obj as State;
        if (other.CurrentPlayer != CurrentPlayer ||
            other.CurrentStage != CurrentStage ||
            other.LastStage != LastStage ||
            other.redStoneCount != redStoneCount ||
            other.blueStoneCount != blueStoneCount ||
            other.currentPlayersMills != currentPlayersMills)
            return false;

        for (int w = 0; w < 3; w++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if (other.Table.Board[w, x, y, z] != Table.Board[w, x, y, z])
                            return false;
                    }
                }
            }
        }
        return true;
    }
    public object Clone()
    {
        State newState = new State();
        newState.CurrentStage = CurrentStage;
        newState.LastStage = LastStage;
        newState.currentPlayersMills = currentPlayersMills;
        newState.CurrentPlayer = CurrentPlayer;
        newState.Table = (CubeTable)Table.Clone();
        newState.redStoneCount = redStoneCount;
        newState.blueStoneCount = blueStoneCount;
        return newState;
    }

    private static int WIN = 100;
    private static int LOSE = -100;
    //Ez akkor amikor az ellenf�lnek az adott helyen 2 korongja van �s a jelenlegi j�t�kos odateszi a korongj�t,
    //hogy ne legyen malma az ellenf�lnek
    private static int POSSIBLE_MILL = 11;
    //Ez akkor amikor az ellenf�lnek m�r az adott helyen a 2 korongja van a jelenlegi j�t�kosnak 0 korongja 
    private static int CREATE_A_MILL = 10;

    //V�dekez� heurisztika
    private int CalculateHeuristic(int currentPlayersCount, int otherPlayersCount) 
    {
        int result = 0;

        //Akkor legyen j� a heurisztika, hogyha kiv�di az ellenkez� j�t�kos malomk�sz�t�s�t
        
        // kiv�dte a lehets�ges malmot
        //if (currentPlayersCount == 1 && otherPlayersCount == 2)
        //    result += POSSIBLE_ENEMY_MILL_AVOIDED;
        //// lehets�ges malom a jelenlegi j�t�kosnak
        //if (currentPlayersCount == 2 && otherPlayersCount == 0)
        //    result += POSSIBLE_MILL_FOR_CURRENTPLAYER;
        //// lehets�ges malom az ellenf�lnek
        //if (currentPlayersCount == 0 && otherPlayersCount == 2)
        //    result += POSSIBLE_ENEMY_MILL_CREATE;
        //// Nem j� d�nt�s nem odarakni, ahol csak maga lenne a saj�tkorongja
        //if (currentPlayersCount == 1)
        //    result += NOT_TOO_GOOD_CHOICE;
        ////A j�t�kos l�trehoz egy malmot
        //if (currentPlayersCount == 3 && otherPlayersCount == 0)
        //    result += PLAYER_CREATE_A_MILL;
        ////J� d�nt�s, ha odateszi ahol m�r van egy ellenf�l korong
        //if (currentPlayersCount == 1 && otherPlayersCount == 1)
        //    result += GOOD_CHOICE;

        return result;
    }
    public int GetHeuristics(Stone player)
    {
        if (GetStatus() == player)
            return WIN;
        else if (GetStatus() != Stone.Empty)
            return LOSE;

        int result = 0;
        Stone currentPlayer;

        int currentPlayersStone;
        Stone otherPlayer;
        int otherPlayersStone;

        if (player == Stone.Red)
        {
            currentPlayer = Stone.Red;
            currentPlayersStone = redStoneCount;
            otherPlayer = Stone.Blue;
            otherPlayersStone = blueStoneCount;
        }
        else
        {
            currentPlayer = Stone.Blue;
            currentPlayersStone = blueStoneCount;
            otherPlayer = Stone.Red;
            otherPlayersStone = redStoneCount;
        }

        //Nagyobb legyen a heurisztika, ha a jelenlegi j�t�kosnak t�bb b�buja van
        //if (currentPlayersStone > otherPlayersStone)
        //    result += currentPlayersStone - otherPlayersStone;
        //else
        //    result -= otherPlayersStone - currentPlayersStone;

        int currentCount = 0;
        int otherCount = 0;

        //Cs�kkents�k a heurisztik�t az ellenf�l lehets�ges malmainak sz�m�val
        result -= CountPotentialMillsAndNeighborCellCheck(otherPlayer, currentPlayer);

        #region r�gi heurisztika
        ////Sorban sz�molja meg a lerakott korongokat a j�t�kosokhoz
        ////Minden szinten megn�zz�k az nulladik �s m�sodik sort a nulladik �s m�sodik dimenzi�ban 
        //for (int w = 0; w < 3; w++)
        //{
        //    for (int x = 0; x < 3; x += 2)
        //    {
        //        for (int z = 0; z < 3; z += 2)
        //        {
        //            for (int y = 0; y < 3; y++)
        //            {
        //                if (Table.Board[w, x, y, z] == currentPlayer)
        //                {
        //                    currentCount++;
        //                }
        //                else if (Table.Board[w, x, y, z] == otherPlayer)
        //                {
        //                    otherCount++;
        //                }
        //            }
        //            result += CalculateHeuristic(currentCount, otherCount);
        //            currentCount = 0;
        //            otherCount = 0;
        //        }
        //    }
        //}
        ////Oszlopban megsz�molja a lerakott korongokat a j�t�kosokhoz
        ////Minden szinten megn�zz�k a nulladik �s m�sodik oszlopot a nulladik �s m�sodik dimenzi�ban
        //for (int w = 0; w < 3; w++)
        //{
        //    for (int y = 0; y < 3; y += 2)
        //    {
        //        for (int z = 0; z < 3; z += 2)
        //        {
        //            for (int x = 0; x < 3; x++)
        //            {
        //                if (Table.Board[w, x, y, z] == currentPlayer)
        //                {
        //                    currentCount++;
        //                }
        //                else if (Table.Board[w, x, y, z] == otherPlayer)
        //                {
        //                    otherCount++;
        //                }
        //            }
        //            result += CalculateHeuristic(currentCount, otherCount);
        //            currentCount = 0;
        //            otherCount = 0;
        //        }
        //    }
        //}

        ////Ugyan�gy sort n�z�nk csak dimenzi�san l�p�nk
        ////Mindenszinten megn�zz�k a nulladik �s m�sodik sort a nulladik �s oszlopban
        //for (int w = 0; w < 3; w++)
        //{
        //    for (int x = 0; x < 3; x += 2)
        //    {
        //        for (int y = 0; y < 3; y += 2)
        //        {
        //            for (int z = 0; z < 3; z++)
        //            {
        //                if (Table.Board[w, x, y, z] == currentPlayer)
        //                {
        //                    currentCount++;
        //                }
        //                else if (Table.Board[w, x, y, z] == otherPlayer)
        //                {
        //                    otherCount++;
        //                }
        //            }
        //            result += CalculateHeuristic(currentCount, otherCount);
        //            currentCount = 0;
        //            otherCount = 0;
        //        }
        //    }
        //}

        ////Szintek k�z�tt n�zz�k
        ////Els� sorban a nulladik �s m�sodik oszlopban ezt a nulladik �s m�sodik dimenzi�ban
        ////Els� oszlopban a nulladik �s m�sodik sorban ezt a nulladik �s m�sodik dimenzi�ban
        ////Els� dimenzi�ban nulladik �s m�sodik sorban ezt a nulladik �s m�sodik oszlopban
        //for (int y = 0; y < 3; y += 2)
        //{
        //    for (int z = 0; z < 3; z += 2)
        //    {
        //        for (int w = 0; w < 3; w++)
        //        {
        //            if (Table.Board[w, 1, y, z] == currentPlayer)
        //            {
        //                currentCount++;
        //            }
        //            else if (Table.Board[w, 1, y, z] == otherPlayer)
        //            {
        //                otherCount++;
        //            }
        //        }
        //        result += CalculateHeuristic(currentCount, otherCount);
        //        currentCount = 0;
        //        otherCount = 0;
        //    }
        //}
        //for (int x = 0; x < 3; x += 2)
        //{
        //    for (int z = 0; z < 3; z += 2)
        //    {
        //        for (int w = 0; w < 3; w++)
        //        {
        //            if (Table.Board[w, x, 1, z] == currentPlayer)
        //            {
        //                currentCount++;
        //            }
        //            else if (Table.Board[w, x, 1, z] == otherPlayer)
        //            {
        //                otherCount++;
        //            }
        //        }
        //        result += CalculateHeuristic(currentCount, otherCount);
        //        currentCount = 0;
        //        otherCount = 0;
        //    }
        //}
        //for (int x = 0; x < 3; x += 2)
        //{
        //    for (int y = 0; y < 3; y += 2)
        //    {
        //        for (int w = 0; w < 3; w++)
        //        {
        //            if (Table.Board[w, x, y, 1] == currentPlayer)
        //            {
        //                currentCount++;
        //            }
        //            else if (Table.Board[w, x, y, 1] == otherPlayer)
        //            {
        //                otherCount++;
        //            }
        //        }
        //        result += CalculateHeuristic(currentCount, otherCount);
        //        currentCount = 0;
        //        otherCount = 0;
        //    }
        //}
        #endregion
        return result;
    }

    private bool HorizontalCheck(Position position, Stone player)
    {
        return Table.Board[position.W, position.X, 0, position.Z] == player &&
               Table.Board[position.W, position.X, 1, position.Z] == player &&
               Table.Board[position.W, position.X, 2, position.Z] == player;
    }
    private bool VerticalCheck(Position position, Stone player)
    {
        return Table.Board[position.W, 0, position.Y, position.Z] == player &&
               Table.Board[position.W, 1, position.Y, position.Z] == player &&
               Table.Board[position.W, 2, position.Y, position.Z] == player;
    }
    private bool DimensionalCheck(Position position, Stone player)
    {
        return Table.Board[position.W, position.X, position.Y, 0] == player &&
               Table.Board[position.W, position.X, position.Y, 1] == player &&
               Table.Board[position.W, position.X, position.Y, 2] == player;
    }
    private bool BetweenLevelsCheck(Position position, Stone player)
    {
        return Table.Board[0, position.X, position.Y, position.Z] == player &&
               Table.Board[1, position.X, position.Y, position.Z] == player &&
               Table.Board[2, position.X, position.Y, position.Z] == player;
    }

    //Megvizsg�lja hogy a currentPlayer-nek 3 db korongja van-e vagy sem, ha igen akkor a harmadik stage-be
    //v�ltunk, egy�bk�nt a second stage marad
    public void checkForSecondOrThirdStage()
    {
        int stones = 0;
        if (CurrentPlayer == Stone.Blue)
            stones = blueStoneCount;
        else if(CurrentPlayer == Stone.Red)
            stones = redStoneCount;

        if (stones == 3)
            CurrentStage = Stage.Third;
        else
            CurrentStage = Stage.Second;
    }
    
    private bool isCurrentPlayersHaveMovableStone()
    {
        bool canMove = true;
        //Ha tal�lok egy olyan korongot amit tud b�rmerre mozgatni akkor tud mozogni a j�t�kos
        for (int w = 0; w < 3; w++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        if(Table.Board[w, x, y, z] == CurrentPlayer)
                        {
                            //Ha a sarokban van akkor h�rom mellette l�v�t kell megn�zni
                            //Bal oldali sarkr�l van sz�, ha az y = 0, jobb oldalir�l ha y = 2
                            //Mindkett� eset�n a nulladik �s a m�sodik sort kell vizsg�lni
                            if((y == 0 || y == 2) && (x == 0 || x == 2))
                            {
                                //El�l �s h�tul is meg kell n�zni
                                if(z == 0 || z == 2)
                                { 
                                    if (Table.Board[w, x, y, 1] == Stone.Empty ||
                                        Table.Board[w, 1, y, z] == Stone.Empty ||
                                        Table.Board[w, x, 1, z] == Stone.Empty)
                                        return canMove;
                                }
                            }

                            //Ha k�z�pen van, kett�t mellette, �s egyet/kett�t a k�vi szinten
                            if(x == 1 || y == 1 || z == 1)
                            {
                                if (x == 1 && (Table.Board[w, 0, y, z] == Stone.Empty || Table.Board[w, 2, y, z] == Stone.Empty))
                                    return canMove;
                                else if (y == 1 && (Table.Board[w, x, 0, z] == Stone.Empty || Table.Board[w, x, 2, z] == Stone.Empty))
                                    return canMove;
                                else if (z == 1 && (Table.Board[w, x, y, 0] == Stone.Empty || Table.Board[w, x, y, 2] == Stone.Empty))
                                    return canMove;

                                //Ha a nulladik szinten vagyunk 
                                if (w == 0 && Table.Board[1, x, y, z] == Stone.Empty)
                                    return canMove;
                                //Ha az els� szinten vagyunk
                                else if (w == 1 && (Table.Board[0, x, y, z] == Stone.Empty || Table.Board[2, x, y, z] == Stone.Empty))
                                    return canMove;
                                //Ha a m�sodik szinten vagyunk
                                else if (w == 2 && Table.Board[1, x, y, z] == Stone.Empty)
                                    return canMove;
                            }
                        }
                    }
                }
            }
        }
        //Ha v�gig megy�nk �s nem tal�lunk olyan korongot ami mozgathat� akkor false-sal t�r�nk vissza
        canMove = false;
        return canMove;
    }
    
    //Heurisztik�hoz sz�ks�ges met�dus, egym�s mellett k�t ugyanolyan korong kell hozz�
    public int CountPotentialMillsAndNeighborCellCheck(Stone player1, Stone player2)
    {
        int score = 0;
        for (int w = 0; w < 3; w++)
        {
            //Nulladik sor, m�sodik sor nulladik �s m�sodik dimenzi�ban
            for (int x = 0; x < 3; x+=2)
            {
                for (int z = 0; z < 3; z+=2)
                {
                    int currentPlayerCount = HorizontalCheckForHeuristics(w, x, z, player1);
                    int otherPlayerCount = HorizontalCheckForHeuristics(w, x, z, player2);
                    if (currentPlayerCount == 2 && otherPlayerCount != 1)
                        score += POSSIBLE_MILL;
                    else if(currentPlayerCount == 3)
                    {
                        //Ha az ellenf�lnek ott malma van akkor meg kellene n�zni a szomsz�dos �res r�szeket
                        //ezeket min�l jobban blokkolni kellene, teh�t azt is hozz�adjuk, ah�ny �res r�sze van
                        score += CREATE_A_MILL;
                        for (int y = 0; y < 3; y+=2)
                        {
                            if (IsNeigborPlaceEmpty(w, 1, y, z))
                                score++;
                            if (IsNeigborPlaceEmpty(w, x, y, 1))
                                score++;
                        }
                    }
                }
            }
            //Nulladik oszlop, harmadik oszlop nulladik �s m�sodik dimenzi�ban
            for (int y = 0; y < 3; y+=2)
            {
                for (int z = 0; z < 3; z+=2)
                {
                    int currentPlayerCount = VerticalCheckForHeuristics(w, y, z, player1);
                    int otherPlayerCount = VerticalCheckForHeuristics(w, y, z, player2);
                    //Csak akkor sz�m�t potenci�lis malomnak, ha 2 ellenf�l korongja maga van
                    if (currentPlayerCount == 2 && otherPlayerCount != 1)
                        score += POSSIBLE_MILL;
                    else if(currentPlayerCount == 3)
                    {
                        score += CREATE_A_MILL;
                        for (int x = 0; x < 3; x+=2)
                        {
                            if (IsNeigborPlaceEmpty(w, x, 1, z))
                                score++;
                            if (IsNeigborPlaceEmpty(w, x, y, 1))
                                score++;
                        }
                    }
                }
            }
            //Nulladik oszlop, masodik oszlop nulladik sor �s m�sodik sorban
            for (int x = 0; x < 3; x+=2)
            {
                for (int y = 0; y < 3; y+=2)
                {
                    int currentPlayerCount = DimensionalCheckForHeuristics(w, x, y, player1);
                    int otherPlayerCount = DimensionalCheckForHeuristics(w, x, y, player2);
                    if (currentPlayerCount == 2 && otherPlayerCount != 1)
                        score += POSSIBLE_MILL;
                    else if(currentPlayerCount == 3) 
                    {
                        score += CREATE_A_MILL;
                        for (int z = 0; z < 3; z+=2)
                        {
                            if (IsNeigborPlaceEmpty(w, 1, y, z))
                                score++;
                            if (IsNeigborPlaceEmpty(w, x, 1, z))
                                score++;
                        }
                    }
                }
            }
        }

        //Szintek k�z�tt
        //Amikor x = 1
        for (int y = 0; y < 3; y += 2)
        {
            for (int z = 0; z < 3; z += 2)
            {
                int currentPlayerCount = BetweenLevelsCheckForHeuristics(1, y, z, player1);
                int otherPlayerCount = BetweenLevelsCheckForHeuristics(1, y, z, player2);
                if (currentPlayerCount == 2 && otherPlayerCount != 1)
                    score += POSSIBLE_MILL;
                else if (currentPlayerCount == 3)
                {
                    score += CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int x = 0; x < 3; x += 2)
                        {
                            if (IsNeigborPlaceEmpty(w, x, y, z))
                                score++;
                        }
                    }
                }
            }
        }
        //Amikor y = 1
        for (int x = 0; x < 3; x += 2)
        {
            for (int z = 0; z < 3; z += 2)
            {
                int currentPlayerCount = BetweenLevelsCheckForHeuristics(x, 1, z, player1);
                int otherPlayerCount = BetweenLevelsCheckForHeuristics(x, 1, z, player2);
                if (currentPlayerCount == 2 && otherPlayerCount != 1)
                    score += POSSIBLE_MILL;
                else if (currentPlayerCount == 3)
                {
                    score += CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int y = 0; y < 3; y += 2)
                        {
                            if (IsNeigborPlaceEmpty(w, x, y, z))
                                score++;
                        }
                    }
                }
            }
        }
        //Amikor z = 1
        for (int x = 0; x < 3; x += 2)
        {
            for (int y = 0; y < 3; y += 2)
            {
                int currentPlayerCount = BetweenLevelsCheckForHeuristics(x, y, 1, player1);
                int otherPlayerCount = BetweenLevelsCheckForHeuristics(x, y, 1, player2);
                if (currentPlayerCount == 2 && otherPlayerCount != 1)
                    score += POSSIBLE_MILL;
                else if (currentPlayerCount == 3)
                {
                    score += CREATE_A_MILL;
                    for (int w = 0; w < 3; w++)
                    {
                        for (int z = 0; z < 3; z += 2)
                        {
                            if (IsNeigborPlaceEmpty(w, x, y, z))
                                score++;
                        }
                    }
                }
            }
        }

        return score;
    }
    private int HorizontalCheckForHeuristics(int w, int x, int z, Stone player)
    {
        int playersCount = 0;
        if (Table.Board[w, x, 0, z] == player)
            playersCount++;
        if (Table.Board[w, x, 1, z] == player)
            playersCount++;
        if (Table.Board[w, x, 2, z] == player)
            playersCount++;

        return playersCount;
    }
    private int VerticalCheckForHeuristics(int w, int y, int z, Stone player)
    {
        int playersCount = 0;
        if (Table.Board[w, 0, y, z] == player)
            playersCount++;
        if (Table.Board[w, 1, y, z] == player)
            playersCount++;
        if (Table.Board[w, 2, y, z] == player)
            playersCount++;

        return playersCount;
    }
    private int DimensionalCheckForHeuristics(int w, int x, int y, Stone player)
    {
        int playersCount = 0;
        if (Table.Board[w, x, y, 0] == player)
            playersCount++;
        if (Table.Board[w, x, y, 1] == player)
            playersCount++;
        if (Table.Board[w, x, y, 2] == player)
            playersCount++;

        return playersCount;
    }
    private int BetweenLevelsCheckForHeuristics(int x, int y, int z, Stone player)
    {
        int playersCount = 0;
        if (Table.Board[0, x, y, z] == player)
            playersCount++;
        if (Table.Board[1, x, y, z] == player)
            playersCount++;
        if (Table.Board[2, x, y, z] == player)
            playersCount++;

        return playersCount;
    }
    private bool IsNeigborPlaceEmpty(int w, int x, int y, int z)
    {
        return Table.Board[w, x, y, z] == Stone.Empty;
    }
}
