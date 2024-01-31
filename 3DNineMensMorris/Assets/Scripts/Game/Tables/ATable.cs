using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATable
{
    public Stone[,,,] Board;
    public ATable(int sizeX, int sizeY, int sizeZ)
    {
        Board = new Stone[3, sizeX, sizeY, sizeZ];
    }
}
