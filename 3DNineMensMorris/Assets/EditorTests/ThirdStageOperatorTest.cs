using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ThirdStageOperatorTest
{
    //Az operátor akkor alkalmazható, ha az elsõ pozíción lévõ korong a sajátja, illetve a második pozíciónak
    //üresnek kell lennie
    //Valamint harmadik stage-ben kell lenni
    [Test]
    public void Applicability_StartPositionTest()
    {
        //Elõször a kezdõ pozíciót vizsgáljuk, tehát hogy az övé a korong vagy sem
        //Kezdetben a piros kezd, tehát piros színû korongot lehet mozgatni
        State state = new State();
        state.CurrentStage = Stage.Third;

        //Elsõ eset amikor nem az õ korongja van az elsõ pozíción
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        ThirdStageOperator op = new ThirdStageOperator(new Position(0, 0, 0, 0), new Position(1, 0, 0, 0));
        Assert.False(op.IsApplicable(state));

        //Második eset amikor az õ korongja van az elsõ pozición
        state.Table.Board[0, 0, 0, 0] = Stone.Red;
        Assert.True(op.IsApplicable(state));

        //Harmadik eset amikor a semmit akarnánk átmozgatni, tehát az elsõ pozíció üres
        state.Table.Board[0, 0, 0, 0] = Stone.Empty;
        Assert.False(op.IsApplicable(state));
    }

    [Test]
    public void Applicability_EndPositionTest()
    {
        //Negyedik eset amikor a második pozició nem üres, akkor hamissal kell visszatérnie
        State state = new State();
        state.CurrentStage = Stage.Third;
        state.Table.Board[1, 0, 0, 0] = Stone.Red;
        state.Table.Board[0, 0, 0, 1] = Stone.Blue;
        ThirdStageOperator op = new ThirdStageOperator(new Position(1, 0, 0, 0), new Position(0, 0, 0, 1));
        Assert.False(op.IsApplicable(state));

        //Ötödik eset amikor a második pozíció üres, akkor igazzal kell visszatérnie
        state.Table.Board[0, 0, 0, 1] = Stone.Empty;
        Assert.True(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInThirdStage() 
    {
        //Hatodik eset
        //Ha nem third stage-ben vagyunk akkor nem alkalmazhatunk ilyen operátort, de minden más teljesül, tehát az
        //elsõ pozíció nem üres, a jelenlegi játékos színe van ami kezdetben piros
        State state = new State();
        state.Table.Board[1, 0, 0, 0] = Stone.Red;
        state.CurrentStage = Stage.Second;
        ThirdStageOperator op = new ThirdStageOperator(new Position(1, 0, 0, 0), new Position(2, 0, 0, 0));
        Assert.False(op.IsApplicable(state));
    }
}
