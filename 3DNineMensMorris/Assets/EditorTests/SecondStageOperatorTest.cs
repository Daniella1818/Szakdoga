using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SecondStageOperatorTest
{
    //Ez az oper�tor akkor alkalmazhat�, ha a kezd� poz�ci� a saj�t sz�ne, a v�g poz�ci� pedig �res �s
    //b�rmilyen ir�nyban k�zvetlen�l mellette van, illetve second stage-ben kell lenni
    [Test]
    public void Applicability_StartPositionTest()
    {
        //Els� eset amikor a kezd� poz�ci� nem a saj�t korongja
        State state = new State();
        state.CurrentStage = Stage.Second;
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 1, 0, 0));
        Assert.False(op.IsApplicable(state));

        //M�sodik eset amikor az � korongja van az els� poz�ci�n
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        Assert.True(op.IsApplicable(state));
    }
    [Test]
    public void Applicability_EndPositionTest()
    {
        //Harmadik eset amikor a v�g poz�ci� nem �res
        State state = new State();
        state.CurrentStage = Stage.Second;
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        state.Table.Board[0, 1, 0, 0] = Stone.Blue;
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 1, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Negyedik eset amikor a v�g poz�ci� �res
        state.Table.Board[0, 1, 0, 0] = Stone.Empty;
        Assert.True(op.IsApplicable(state));
    }
    [Test]
    public void Applicability_FirstAndSecondParameterRelationTest()
    {
        State state = new State();
        state.CurrentStage = Stage.Second;
        //Sarokb�l csak h�rom ir�nyba tud mozogni 1-et
        state.Table.Board[0, 0, 0, 0] = Stone.Red;

        //V�zszintesen 1-et
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 1, 0, 0));
        Assert.True(op.IsApplicable(state));
        //V�zszintes 2 mozg�s eset�n, nem alkalmazhat�
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 2, 0, 0));
        Assert.False(op.IsApplicable(state));

        //F�gg�legesen 1-et
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 1, 0));
        Assert.True(op.IsApplicable(state));
        //F�gg�legesen 2 mozg�s eset�n, nem alkalmazhat�
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 2, 0));
        Assert.False(op.IsApplicable(state));

        //Dimenzi�san 1-et
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 0, 1));
        Assert.True(op.IsApplicable(state));
        //Dimenzi�san 2 mozg�s eset�n, nem alkalmazhat�
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(0, 0, 0, 2));
        Assert.False(op.IsApplicable(state));

        //Sarokb�l nem lehet szintet v�ltani
        op = new SecondStageOperator(new Position(0, 0, 0, 0), new Position(1, 0, 0, 0));
        Assert.False(op.IsApplicable(state));

        //K�z�ps� poz�ci� eset�n csak azonos oszlopon/soron/dimenzi�n bel�l mehet�nk el egyet valamelyik sarokba
        //Innen szintet is tudunk l�pni de csak egyet
        //A szint els� sor�b�l, 0 vagy 2 sorba tudunk l�pni, vagy 0 vagy 2 szintre
        state.Table.Board[0, 1, 0, 0] = Stone.Red;
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(0, 2, 0, 0));
        Assert.True(op.IsApplicable(state));

        //Els� szintre l�p�s
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(1, 1, 0, 0));
        Assert.True(op.IsApplicable(state));

        //K�t szintet egyszerre nem l�phet�nk
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(2, 1, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Ha egyszerre k�t koordin�t�t is n�vel�nk akkor nem alkalmazhat�
        //Egyszerre v�ltunk dimenzi�t, �s oszlopot
        state.Table.Board[0, 0, 0, 1] = Stone.Red;
        op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(0, 0, 1, 2));
        Assert.False(op.IsApplicable(state));

        //Egyszerre v�ltunk dimenzi�t, �s sort
        op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(0, 1, 0, 2));
        Assert.False(op.IsApplicable(state));

        //Egyszerre v�ltunk dimenzi�t, �s szintet
        op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(1, 0, 0, 2));
        Assert.False(op.IsApplicable(state));

        //Egyszerre v�ltunk szintet �s sort
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(1, 2, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Egyszerre v�ltunk szintet �s oszlopot
        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(1, 1, 2, 0));
        Assert.False(op.IsApplicable(state));

        //Egyszerre v�ltunk oszlopot �s sort

        op = new SecondStageOperator(new Position(0, 1, 0, 0), new Position(0, 2, 1, 0));
        Assert.False(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInSecondStage()
    {
        //�t�dik eset
        //Ha nem remove stage-ben vagyunk akkor nem alkalmazhatunk ilyen oper�tort, de minden m�s teljes�l
        State state = new State();
        state.CurrentStage = Stage.First;
        state.Table.Board[0, 0, 0, 1] = Stone.Red;
        SecondStageOperator op = new SecondStageOperator(new Position(0, 0, 0, 1), new Position(0, 0, 1, 1));
        Assert.False(op.IsApplicable(state));
    }
}
