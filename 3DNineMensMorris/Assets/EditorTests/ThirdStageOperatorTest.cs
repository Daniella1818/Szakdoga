using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ThirdStageOperatorTest
{
    //Az oper�tor akkor alkalmazhat�, ha az els� poz�ci�n l�v� korong a saj�tja, illetve a m�sodik poz�ci�nak
    //�resnek kell lennie
    //Valamint harmadik stage-ben kell lenni
    [Test]
    public void Applicability_StartPositionTest()
    {
        //El�sz�r a kezd� poz�ci�t vizsg�ljuk, teh�t hogy az �v� a korong vagy sem
        //Kezdetben a piros kezd, teh�t piros sz�n� korongot lehet mozgatni
        State state = new State();
        state.CurrentStage = Stage.Third;

        //Els� eset amikor nem az � korongja van az els� poz�ci�n
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        ThirdStageOperator op = new ThirdStageOperator(new Position(0, 0, 0, 0), new Position(1, 0, 0, 0));
        Assert.False(op.IsApplicable(state));

        //M�sodik eset amikor az � korongja van az els� pozici�n
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        Assert.True(op.IsApplicable(state));

        //Harmadik eset amikor a semmit akarn�nk �tmozgatni, teh�t az els� poz�ci� �res
        state.Table.Board[0, 0, 0, 0] = Stone.Empty;
        Assert.False(op.IsApplicable(state));
    }

    [Test]
    public void Applicability_EndPositionTest()
    {
        //Negyedik eset amikor a m�sodik pozici� nem �res, akkor hamissal kell visszat�rnie
        State state = new State();
        state.CurrentStage = Stage.Third;
        state.Table.Board[1, 0, 0, 0] = Stone.Red;
        state.Table.Board[0, 0, 0, 1] = Stone.Blue;
        ThirdStageOperator op = new ThirdStageOperator(new Position(1, 0, 0, 0), new Position(0, 0, 0, 1));
        Assert.False(op.IsApplicable(state));

        //�t�dik eset amikor a m�sodik poz�ci� �res, akkor igazzal kell visszat�rnie
        state.Table.Board[0, 0, 0, 1] = Stone.Empty;
        Assert.True(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInThirdStage() 
    {
        //Hatodik eset
        //Ha nem third stage-ben vagyunk akkor nem alkalmazhatunk ilyen oper�tort, de minden m�s teljes�l, teh�t az
        //els� poz�ci� nem �res, a jelenlegi j�t�kos sz�ne van ami kezdetben piros
        State state = new State();
        state.Table.Board[1, 0, 0, 0] = Stone.Red;
        state.CurrentStage = Stage.Second;
        ThirdStageOperator op = new ThirdStageOperator(new Position(1, 0, 0, 0), new Position(2, 0, 0, 0));
        Assert.False(op.IsApplicable(state));
    }
}
