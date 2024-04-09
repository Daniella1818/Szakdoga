using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CubeTable : ATable
{
    public CubeTable()
    : base(3, 3, 3)
    {
        for (int w = 0; w < Board.GetLength(0); w++)
        {
            for (int x = 0; x < Board.GetLength(1); x++)
            {
                for (int y = 0; y < Board.GetLength(2); y++)
                {
                    for (int z = 0; z < Board.GetLength(3); z++)
                    {
                        //A mátrix középét nem használjuk!
                        if ((x == 1 && y == 1) && (z == 0 || z == 1 || z == 2) ||
                            (x == 0 && y == 1 && z == 1) ||
                            (x == 1 && (y == 0 || y == 2) && z == 1) ||
                            (x == 2 && y == 1 && z == 1))
                            Board[w, x, y, z] = Stone.NotBelongToTable;
                        else
                            Board[w, x, y, z] = Stone.Empty;
                    }
                }
            }
        }
    }
}
