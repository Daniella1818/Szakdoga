using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public State State;
    public int Depth;
    public Node Parent;
    public List<Node> Children = new List<Node>();
    public Node(State state, Node parent = null)
    {
        Parent = parent;
        State = state;
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

    public int GetHeuristics(Stone currentPlayer)
    {
        if (Children.Count == 0)
        {
            return State.GetHeuristics(currentPlayer);
        }
        return Children[0].GetHeuristics(currentPlayer);
    }
}