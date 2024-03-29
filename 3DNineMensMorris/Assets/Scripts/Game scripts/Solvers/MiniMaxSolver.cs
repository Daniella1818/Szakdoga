using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMaxSolver : ASolver
{
    private bool ShouldGenerateChildren;
    public MiniMaxSolver(int depth) : base()
    {
        Depth = depth;
    }
    public int Depth;


    public override State NextMove(State currentState)
    {
        ShouldGenerateChildren = true;
        Node currentNode = new Node(currentState);
        int bestValue = Minimax(currentNode, int.MinValue, int.MaxValue, true);

        List<Node> bestChildren = new List<Node>();
        ShouldGenerateChildren = false;
        foreach (Node child in currentNode.Children)
        {
            int childValue = Minimax(child, int.MinValue, int.MaxValue, false);
            if (childValue == bestValue)
            {
                bestChildren.Add(child);
            }
        }
        int index = Random.Range(0,bestChildren.Count);

        return bestChildren[index] != null ? bestChildren[index].State : null;
    }

    private int Minimax(Node node, int alpha, int beta, bool maximizingPlayer)
    {
        if (node.GetStatus() != Stone.Empty || node.Depth >= Depth) 
            return node.GetHeuristics(node.State.CurrentPlayer);

        if(ShouldGenerateChildren)
            GenerateChildrenForNode(node);

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (Node child in node.Children)
            {
                int eval = Minimax(child, alpha, beta, false);
                maxEval = Mathf.Max(eval, maxEval);
                alpha = Mathf.Max(alpha, maxEval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (Node child in node.Children)
            {
                int eval = Minimax(child, alpha, beta, true);
                minEval = Mathf.Min(eval, minEval);
                beta = Mathf.Min(beta, minEval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }

    private void GenerateChildrenForNode(Node node)
    {
        if (node.State.CurrentStage == Stage.First)
        {
            foreach (AOperator op in FirstStageOperators)
            {
                ProcessApplicableOperator(node, op);
            }
        }
        else if (node.State.CurrentStage == Stage.Second)
        {
            foreach (AOperator op in SecondStageOperators)
            {
                ProcessApplicableOperator(node, op);
            }
        }
        else if (node.State.CurrentStage == Stage.Third)
        {
            foreach (AOperator op in ThirdStageOperators)
            {
                ProcessApplicableOperator(node, op);
            }
        }
        else if (node.State.CurrentStage == Stage.Remove)
        {
            foreach (AOperator op in RemoveStageOperators)
            {
                ProcessApplicableOperator(node, op);
            }
        }
    }

    private void ProcessApplicableOperator(Node node, AOperator op)
    {
        if (op.IsApplicable(node.State))
        {
            State newState = op.Apply(node.State);
            Node newNode = new Node(newState, node);
            node.Children.Add(newNode);
        }
    }
}
