using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int X; public int Y; public int Z; public int W;

    /// <summary>
    /// W a szint, hogy a t�bla melyik szintj�n tal�lhat� (k�ls� = 0, k�z�ps� = 1, bels� = 2)
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
