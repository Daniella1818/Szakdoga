using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int X; public int Y; public int Z; public int W;

    /// <summary>
    /// W a szint, hogy a tábla melyik szintjén található (külsõ = 0, középsõ = 1, belsõ = 2)
    /// </summary>
    /// <param name="w"></param>
    /// <returns></returns>
    public Position(int w, int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
}
