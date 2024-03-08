using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SecondStageOperatorTest
{
    //Ez az operátor akkor alkalmazható, ha a kezdõ pozíció a saját színe, a vég pozíció pedig üres és
    //bármilyen irányban közvetlenül mellette van, illetve second stage-ben kell lenni
    [Test]
    public void Applicability_StartPositionTest()
    {
        //Elsõ eset amikor a kezdõ pozíció nem a saját korongja
        State state = new State();
        state.CurrentStage = Stage.Second;
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 1, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Második eset amikor az õ korongja van az elsõ pozíción
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        Assert.True(op.IsApplicable(state));
    }
    [Test]
    public void Applicability_EndPositionTest()
    {
        //Harmadik eset amikor a vég pozíció nem üres
        State state = new State();
        state.CurrentStage = Stage.Second;
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        state.Table.Board[0, 1, 0, 0] = Stone.Blue;
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 1, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Negyedik eset amikor a vég pozíció üres
        state.Table.Board[0, 1, 0, 0] = Stone.Empty;
        Assert.True(op.IsApplicable(state));
    }
    [Test]
    public void Applicability_FirstAndSecondParameterRelationTest()
    {
        State state = new State();
        state.CurrentStage = Stage.Second;
        //Sarokból csak három irányba tud mozogni 1-et
        state.Table.Board[0, 0, 0, 0] = Stone.Red;

        //Vízszintesen 1-et
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 1, 0, 0));
        Assert.True(op.IsApplicable(state));
        //Vízszintes 2 mozgás esetén, nem alkalmazható
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 2, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Függõlegesen 1-et
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 1, 0));
        Assert.True(op.IsApplicable(state));
        //Függõlegesen 2 mozgás esetén, nem alkalmazható
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 2, 0));
        Assert.False(op.IsApplicable(state));

        //Dimenziósan 1-et
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 0, 1));
        Assert.True(op.IsApplicable(state));
        //Dimenziósan 2 mozgás esetén, nem alkalmazható
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 0, 2));
        Assert.False(op.IsApplicable(state));

        //Sarokból nem lehet szintet váltani
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(1, 0, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Középsõ pozíció esetén csak azonos oszlopon/soron/dimenzión belül mehetünk el egyet valamelyik sarokba
        //Innen szintet is tudunk lépni de csak egyet
        //A szint elsõ sorából, 0 vagy 2 sorba tudunk lépni, vagy 0 vagy 2 szintre
        state.Table.Board[0, 1, 0, 0] = Stone.Red;
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(0, 2, 0, 0));
        Assert.True(op.IsApplicable(state));

        //Elsõ szintre lépés
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(1, 1, 0, 0));
        Assert.True(op.IsApplicable(state));

        //Két szintet egyszerre nem léphetünk
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(2, 1, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Ha egyszerre két koordinátát is növelünk akkor nem alkalmazható
        //Egyszerre váltunk dimenziót, és oszlopot
        state.Table.Board[0, 0, 0, 1] = Stone.Red;
        op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(0, 0, 1, 2));
        Assert.False(op.IsApplicable(state));

        //Egyszerre váltunk dimenziót, és sort
        op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(0, 1, 0, 2));
        Assert.False(op.IsApplicable(state));

        //Egyszerre váltunk dimenziót, és szintet
        op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(1, 0, 0, 2));
        Assert.False(op.IsApplicable(state));

        //Egyszerre váltunk szintet és sort
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(1, 2, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Egyszerre váltunk szintet és oszlopot
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(1, 1, 2, 0));
        Assert.False(op.IsApplicable(state));

        //Egyszerre váltunk oszlopot és sort

        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(0, 2, 1, 0));
        Assert.False(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInSecondStage()
    {
        //Ötödik eset
        //Ha nem remove stage-ben vagyunk akkor nem alkalmazhatunk ilyen operátort, de minden más teljesül
        State state = new State();
        state.CurrentStage = Stage.First;
        state.Table.Board[0, 0, 0, 1] = Stone.Red;
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(0, 0, 1, 1));
        Assert.False(op.IsApplicable(state));
    }
}
