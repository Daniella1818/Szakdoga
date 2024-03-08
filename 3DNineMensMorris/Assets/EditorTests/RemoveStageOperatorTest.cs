using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RemoveStageOperatorTest
{
    //Akkor alkalmazhat�, ha remove stage-ben van, ha a poz�ci�n az ellenf�l sz�ne van, illetve ha az ellenf�lnek
    //csak olyan korongj�t t�vol�thatjuk el ami nincs malomnak, viszont ha csak olyan korongja van ami malmot alkot akkor
    //elt�vol�that� a malmot alkot� korong
    [Test]
    public void Applicability_PositionTest()
    {
        //Els� eset amikor a saj�t sz�n�t akarja elt�vol�tani
        State state = new State();
        state.CurrentStage = Stage.Remove;
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        RemoveStageOperator op = new RemoveStageOperator(new Position(0, 0, 0, 0));
        Assert.False(op.IsApplicable(state));

        //M�sodik eset amikor az ellenf�l sz�n�t akarja elt�vol�tani
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        Assert.True(op.IsApplicable(state));

        //Harmadik eset amikor a "semmit" akarja elt�vol�tani
        state.Table.Board[0, 0, 0, 0] = Stone.Empty;
        Assert.False(op.IsApplicable(state));
    }

    [Test]
    public void Applicability_EnemyMillTest()
    {
        //Negyedik eset amikor az ellenf�lnek csak olyan korongja van ami malomban van akkor elt�vol�that� egy k�z�l�k
        State state = new State();
        state.CurrentStage = Stage.Remove;
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        state.Table.Board[0, 0, 0, 1] = Stone.Blue;
        state.Table.Board[0, 0, 0, 2] = Stone.Blue;
        RemoveStageOperator op = new RemoveStageOperator(new Position(0, 0, 0, 0));
        Assert.True(op.IsApplicable(state));

        //�t�dik eset amikor m�r van olyan korongja ami nincs malomba akkor nem t�vol�that el olyat ami malomba van
        state.Table.Board[1, 0, 0, 2] = Stone.Blue;
        Assert.False(op.IsApplicable(state));

        //Csak olyat tudunk elt�vol�tani ami nincs malomba 
        op = new RemoveStageOperator(new Position(1, 0, 0, 2));
        Assert.True(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInRemoveStage()
    {
        //�t�dik eset
        //Ha nem remove stage-ben vagyunk akkor nem alkalmazhatunk ilyen oper�tort, de minden m�s teljes�l
        State state = new State();
        state.CurrentStage = Stage.Second;
        state.Table.Board[0, 0, 0, 1] = Stone.Blue;
        RemoveStageOperator op = new RemoveStageOperator(new Position(0,0,0,1));
        Assert.False(op.IsApplicable(state));
    }
}
