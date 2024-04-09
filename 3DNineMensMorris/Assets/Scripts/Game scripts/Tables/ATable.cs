using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ATable : ICloneable
{
    public Stone[,,,] Board;
    public ATable(int sizeX, int sizeY, int sizeZ)
    {
        Board = new Stone[3, sizeX, sizeY, sizeZ];
    }

    public object Clone()
    {
        ATable clonedTable = new ATable(Board.GetLength(1), Board.GetLength(2), Board.GetLength(3));

        for (int w = 0; w < Board.GetLength(0); w++)
        {
            for (int x = 0; x < Board.GetLength(1); x++)
            {
                for (int y = 0; y < Board.GetLength(2); y++)
                {
                    for (int z = 0; z < Board.GetLength(3); z++)
                    {
                        clonedTable.Board[w, x, y, z] = Board[w, x, y, z];
                    }
                }
            }
        }
        return clonedTable;
    }
}
