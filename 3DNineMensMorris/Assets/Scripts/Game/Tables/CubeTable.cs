using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                        Board[w, x, y, z] = Stone.Empty;
                    }
                }
            }
        }
    }
}
