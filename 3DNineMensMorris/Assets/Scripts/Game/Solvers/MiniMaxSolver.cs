using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMaxSolver : ASolver
{
    public MiniMaxSolver(int depth) : base()
    {
        Depth = depth;
    }
    public int Depth;


    public override State NextMove(State currentState)
    {
        Node currentNode = new Node(currentState);
        extendNode(currentNode);
        currentNode.SortChildrenMinimax(currentState.CurrentPlayer);
        return currentNode.Children[0].State;
    }

    private void extendNode(Node node)
    {
        if (node.GetStatus() != Stone.Empty || node.Depth >= Depth) return;

        if(node.State.CurrentStage == Stage.First) 
        {
            foreach (AOperator op in FirstStageOperators)
            {
                CheckTheOperator(node, op);
            }
        }
        else if(node.State.CurrentStage == Stage.Second)
        {
            foreach (AOperator op in SecondStageOperators)
            {
                CheckTheOperator(node, op);
            }
        }
        else if(node.State.CurrentStage == Stage.Third)
        {
            foreach (AOperator op in ThirdStageOperators)
            {
                CheckTheOperator(node, op);
            }
        }
        else if(node.State.CurrentStage == Stage.Remove)
        {
            foreach (AOperator op in RemoveStageOperators)
            {
                CheckTheOperator(node, op);
            }
        }
    }

    private void CheckTheOperator(Node node, AOperator op)
    {
        if (op.IsApplicable(node.State))
        {
            State newState = op.Apply(node.State);
            Node newNode = new Node(newState, node);
            node.Children.Add(newNode);
            extendNode(newNode);
        }
    }
}
