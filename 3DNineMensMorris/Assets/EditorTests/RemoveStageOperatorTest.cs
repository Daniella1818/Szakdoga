using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RemoveStageOperatorTest
{
    //Akkor alkalmazható, ha remove stage-ben van, ha a pozíción az ellenfél színe van, illetve ha az ellenfélnek
    //csak olyan korongját távolíthatjuk el ami nincs malomnak, viszont ha csak olyan korongja van ami malmot alkot akkor
    //eltávolítható a malmot alkotó korong
    [Test]
    public void Applicability_PositionTest()
    {
        //Elsõ eset amikor a saját színét akarja eltávolítani
        State state = new State();
        state.CurrentStage = Stage.Remove;
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        RemoveStageOperator op = new RemoveStageOperator(new Position(0, 0, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Második eset amikor az ellenfél színét akarja eltávolítani
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        Assert.True(op.IsApplicable(state));

        //Harmadik eset amikor a "semmit" akarja eltávolítani
        state.Table.Board[0, 0, 0, 0] = Stone.Empty;
        Assert.False(op.IsApplicable(state));
    }

    [Test]
    public void Applicability_EnemyMillTest()
    {
        //Negyedik eset amikor az ellenfélnek csak olyan korongja van ami malomban van akkor eltávolítható egy közülük
        State state = new State();
        state.CurrentStage = Stage.Remove;
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        state.Table.Board[0, 0, 0, 1] = Stone.Blue;
        state.Table.Board[0, 0, 0, 2] = Stone.Blue;
        RemoveStageOperator op = new RemoveStageOperator(new Position(0, 0, 0, 0));
        Assert.True(op.IsApplicable(state));

        //Ötödik eset amikor már van olyan korongja ami nincs malomba akkor nem távolíthat el olyat ami malomba van
        state.Table.Board[1, 0, 0, 2] = Stone.Blue;
        Assert.False(op.IsApplicable(state));

        //Csak olyat tudunk eltávolítani ami nincs malomba 
        op = new RemoveStageOperator(new Position(1, 0, 0, 2));
        Assert.True(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInRemoveStage()
    {
        //Ötödik eset
        //Ha nem remove stage-ben vagyunk akkor nem alkalmazhatunk ilyen operátort, de minden más teljesül
        State state = new State();
        state.CurrentStage = Stage.Second;
        state.Table.Board[0, 0, 0, 1] = Stone.Blue;
        RemoveStageOperator op = new RemoveStageOperator(new Position(0,0,0,1));
        Assert.False(op.IsApplicable(state));
    }
}
