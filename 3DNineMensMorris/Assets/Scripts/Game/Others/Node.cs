using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public State State;
    public int Depth;
    public Node Parent;
    public List<Node> Children = new List<Node>();
    public int OperatorIndex;
    public Node(State state, Node parent = null)
    {
        Parent = parent;
        State = state;
        OperatorIndex = 0;
        if (Parent == null)
        {
            Depth = 0;
        }
        else
        {
            Depth = Parent.Depth + 1;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Node)) return false;
        Node other = obj as Node;
        return State.Equals(other.State);
    }
    public Stone GetStatus()
    {
        return State.GetStatus();
    }


    public void SortChildrenMinimax(Stone currentPlayer, bool isCurrentPlayer = true)
    {
        foreach (Node node in Children)
        {
            node.SortChildrenMinimax(currentPlayer, !isCurrentPlayer);
        }
        if (isCurrentPlayer) // ha a jelenlegi játékos lép (a gép), akkor csökkenõ sorrend
        {
            Children.Sort((x, y) => y.GetHeuristics(currentPlayer).CompareTo(x.GetHeuristics(currentPlayer)));
        }
        else // ha a másik játékos, akkor növekvõ sorrendbe rakjuk a gyerekelemek listáját
        {
            Children.Sort((x, y) => x.GetHeuristics(currentPlayer).CompareTo(y.GetHeuristics(currentPlayer)));
        }
    }

    public int GetHeuristics(Stone currentPlayer)
    {
        if (Children.Count == 0)
        {
            return State.GetHeuristics(currentPlayer);
        }
        return Children[0].GetHeuristics(currentPlayer);
    }

    public bool HasLoop()
    {
        Node temp = Parent;
        while (temp != null)
        {
            if (temp.Equals(this))
            {
                return true;
            }
            temp = temp.Parent;
        }
        return false;
    }
}