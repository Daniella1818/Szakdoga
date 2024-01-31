using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASolver
{
    public List<AOperator> AllOperator = new List<AOperator>();

    public ASolver()
    {
        generateOperators();
    }
    private void generateOperators()
    {
        for (int w = 0; w < 3; w++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        AllOperator.Add(new FirstStageOperator(new Position(w, x, y, z)));
                        //AllOperator.Add(new RemoveStoneOperator(new Position(w, x, y, z)));
                        for (int w2 = 0; w2 < 3; w2++)
                        {
                            for (int x2 = 0; x2 < 3; x2++)
                            {
                                for (int y2 = 0; y2 < 3; y2++)
                                {
                                    for (int z2 = 0; z2 < 3; z2++)
                                    {
                                        //AllOperator.Add(new SecondStageOperator(new Position(w, x, y, z), new Position(w2, x2, y2, z2)));
                                        //AllOperator.Add(new ThirdStageOperator(new Position(w, x, y, z), new Position(w2, x2, y2, z2)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public abstract State NextMove(State currentState);
}
