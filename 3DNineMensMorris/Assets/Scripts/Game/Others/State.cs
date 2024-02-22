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

    //Ha remove stage van akkor ebbe mentjük az aktuálisat, hogy visszatudjunk térni rá
    public Stage LastStage;

    public int redStoneCount = 9, blueStoneCount = 9;
    public int currentPlayersMills = 0;

    //Jelenlegi játékost cseréli, ez biztosítja, hogy egymás után lépjenek
    public void ChangePlayer()
    {
        if (CurrentPlayer == Stone.Red)
            CurrentPlayer = Stone.Blue;
        else if (CurrentPlayer == Stone.Blue)
            CurrentPlayer = Stone.Red;
    }

    //Megvizsgálja, hogy van e nyertes, akkor veszít valaki ha 2 korongja maradt összesen, ha EMPTY-t ad vissza akkor még tudnak 
    //lépni a játékosok.
    public Stone GetStatus()
    {
        if (CurrentStage == Stage.Second || CurrentStage == Stage.Third)
        {
            //Azt is meg kellene vizsgálni, hogy a jelenlegi játékos tud-e lépni, de csak a második stage-tõl
            //Meg kellene nézni az összes saját korongját, és mellette lévõ helyeket, ha legalább egy olyan korongja van
            //ami mellett üres a hely akkor tud lépni egyébként nem
            //Ha nem tud lépni akkor az ellenfelet adjuk vissza nyeretesként
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

    //Megnézzük, hogy az újonnan elhelyezett korong alkot-e malmot, ha igen akkor visszaadja, hogy
    //mennyit (mivel egy lépés alkothat kettõ vagy három malmot is!)
    public int CountMill(Position position, Stone player)
    {
        int mills = 0;

        //Ha a sarokban került az új korong, tehát ezek a koordináták lehetnek: (0.szinten)
        //0,0,0,0 || 0,0,2,0 || 0,2,0,0 || 0,2,2,0 || 0,0,2,2 || 0,2,2,2 || 0,0,0,2 || 0,2,0,2
        if((position.X == 0 || position.X == 2) && 
           (position.Y == 0 || position.Y == 2) && 
           (position.Z == 0 || position.Z == 2))
        {
            //Ilyenkor 3 irányban kell megnézni hogy jött-e létre malom
            //Mindháromnál marad a szint értéke

            //Azonos sorban, az oszlopokat változtatva, a dimenzió ugyanaz marad (vízszintes)
            if (HorizontalCheck(position, player))
                mills++;
            //Azonos oszlopban, a sorokat változtatva, a dimenzió ugyanaz marad (függõleges)
            if (VerticalCheck(position, player))
                mills++;
            //Azonos oszlop, és sor, a dimenziót változtatjuk (dimenziós)
            if (DimensionalCheck(position, player))
                mills++;
        }

        //Ha középre kerül a korong, itt lehet szintek között lépkedni
        //0,0,1,0 || 0,1,0,0 || 0,1,2,0 || 0,2,1,0 || 0,0,2,1 || 0,1,2,2 || 0,2,2,1, || 0,0,0,1 ||
        //0,2,0,1 || 0,1,0,2 || 0,0,1,2 || 0,2,1,2
        if(position.X == 1 || position.Y == 1 || position.Z == 1)
        {
            //Ilyenkor 2 irányba kell megvizsgálni, hogy jött-e létre malom
            //Azonos sor, oszlop, dimenzió, a szinteket kell léptetni (szintek közötti)
            if (BetweenLevelsCheck(position, player))
                mills++;
            //Annak megfelelõen hogy hol az egyes a koordináta meg kell vizsgálni a sort/oszlopot/dimenziót
            //Ha az x=1 akkor függõlegesen kell vizsgálni
            if(position.X == 1)
            {
                if (VerticalCheck(position, player))
                    mills++;
            }
            //Ha az y=1 akkor vízszintesen kell vizsgálni
            else if(position.Y == 1)
            {
                if (HorizontalCheck(position, player))
                        mills++;
            }
            //Ha a z=1 akkor dimenziósan kell vizsgálni
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
    //Ez akkor amikor az ellenfélnek az adott helyen 2 korongja van és a jelenlegi játékos odateszi a korongját,
    //hogy ne legyen malma az ellenfélnek
    private static int POSSIBLE_MILL = 11;
    //Ez akkor amikor az ellenfélnek már az adott helyen a 2 korongja van a jelenlegi játékosnak 0 korongja 
    private static int CREATE_A_MILL = 10;

    //Védekezõ heurisztika
    private int CalculateHeuristic(int currentPlayersCount, int otherPlayersCount) 
    {
        int result = 0;

        //Akkor legyen jó a heurisztika, hogyha kivédi az ellenkezõ játékos malomkészítését
        
        // kivédte a lehetséges malmot
        //if (currentPlayersCount == 1 && otherPlayersCount == 2)
        //    result += POSSIBLE_ENEMY_MILL_AVOIDED;
        //// lehetséges malom a jelenlegi játékosnak
        //if (currentPlayersCount == 2 && otherPlayersCount == 0)
        //    result += POSSIBLE_MILL_FOR_CURRENTPLAYER;
        //// lehetséges malom az ellenfélnek
        //if (currentPlayersCount == 0 && otherPlayersCount == 2)
        //    result += POSSIBLE_ENEMY_MILL_CREATE;
        //// Nem jó döntés nem odarakni, ahol csak maga lenne a sajátkorongja
        //if (currentPlayersCount == 1)
        //    result += NOT_TOO_GOOD_CHOICE;
        ////A játékos létrehoz egy malmot
        //if (currentPlayersCount == 3 && otherPlayersCount == 0)
        //    result += PLAYER_CREATE_A_MILL;
        ////Jó döntés, ha odateszi ahol már van egy ellenfél korong
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

        //Nagyobb legyen a heurisztika, ha a jelenlegi játékosnak több bábuja van
        //if (currentPlayersStone > otherPlayersStone)
        //    result += currentPlayersStone - otherPlayersStone;
        //else
        //    result -= otherPlayersStone - currentPlayersStone;

        int currentCount = 0;
        int otherCount = 0;

        //Csökkentsük a heurisztikát az ellenfél lehetséges malmainak számával
        result -= CountPotentialMillsAndNeighborCellCheck(otherPlayer, currentPlayer);

        #region régi heurisztika
        ////Sorban számolja meg a lerakott korongokat a játékosokhoz
        ////Minden szinten megnézzük az nulladik és második sort a nulladik és második dimenzióban 
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
        ////Oszlopban megszámolja a lerakott korongokat a játékosokhoz
        ////Minden szinten megnézzük a nulladik és második oszlopot a nulladik és második dimenzióban
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

        ////Ugyanúgy sort nézünk csak dimenziósan lépünk
        ////Mindenszinten megnézzük a nulladik és második sort a nulladik és oszlopban
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

        ////Szintek között nézzük
        ////Elsõ sorban a nulladik és második oszlopban ezt a nulladik és második dimenzióban
        ////Elsõ oszlopban a nulladik és második sorban ezt a nulladik és második dimenzióban
        ////Elsõ dimenzióban nulladik és második sorban ezt a nulladik és második oszlopban
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

    //Megvizsgálja hogy a currentPlayer-nek 3 db korongja van-e vagy sem, ha igen akkor a harmadik stage-be
    //váltunk, egyébként a second stage marad
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
        //Ha találok egy olyan korongot amit tud bármerre mozgatni akkor tud mozogni a játékos
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
                            //Ha a sarokban van akkor három mellette lévõt kell megnézni
                            //Bal oldali sarkról van szó, ha az y = 0, jobb oldaliról ha y = 2
                            //Mindkettõ esetén a nulladik és a második sort kell vizsgálni
                            if((y == 0 || y == 2) && (x == 0 || x == 2))
                            {
                                //Elõl és hátul is meg kell nézni
                                if(z == 0 || z == 2)
                                { 
                                    if (Table.Board[w, x, y, 1] == Stone.Empty ||
                                        Table.Board[w, 1, y, z] == Stone.Empty ||
                                        Table.Board[w, x, 1, z] == Stone.Empty)
                                        return canMove;
                                }
                            }

                            //Ha középen van, kettõt mellette, és egyet/kettõt a kövi szinten
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
                                //Ha az elsõ szinten vagyunk
                                else if (w == 1 && (Table.Board[0, x, y, z] == Stone.Empty || Table.Board[2, x, y, z] == Stone.Empty))
                                    return canMove;
                                //Ha a második szinten vagyunk
                                else if (w == 2 && Table.Board[1, x, y, z] == Stone.Empty)
                                    return canMove;
                            }
                        }
                    }
                }
            }
        }
        //Ha végig megyünk és nem találunk olyan korongot ami mozgatható akkor false-sal térünk vissza
        canMove = false;
        return canMove;
    }
    
    //Heurisztikához szükséges metódus, egymás mellett két ugyanolyan korong kell hozzá
    public int CountPotentialMillsAndNeighborCellCheck(Stone player1, Stone player2)
    {
        int score = 0;
        for (int w = 0; w < 3; w++)
        {
            //Nulladik sor, második sor nulladik és második dimenzióban
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
                        //Ha az ellenfélnek ott malma van akkor meg kellene nézni a szomszédos üres részeket
                        //ezeket minél jobban blokkolni kellene, tehát azt is hozzáadjuk, ahány üres része van
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
            //Nulladik oszlop, harmadik oszlop nulladik és második dimenzióban
            for (int y = 0; y < 3; y+=2)
            {
                for (int z = 0; z < 3; z+=2)
                {
                    int currentPlayerCount = VerticalCheckForHeuristics(w, y, z, player1);
                    int otherPlayerCount = VerticalCheckForHeuristics(w, y, z, player2);
                    //Csak akkor számít potenciális malomnak, ha 2 ellenfél korongja maga van
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
            //Nulladik oszlop, masodik oszlop nulladik sor és második sorban
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

        //Szintek között
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
