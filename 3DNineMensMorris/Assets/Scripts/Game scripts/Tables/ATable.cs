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

        Array.Copy(Board, clonedTable.Board, Board.Length);

        return clonedTable;
    }
}
