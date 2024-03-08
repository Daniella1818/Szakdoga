using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FirstStageOperatorTest
{
    //Leteszteli az alkalmazhat�s�g�t a first stage oper�tornak, akkor alkalmazhat� ha first stage-ben vagyunk
    //�s a hely ahov� rakn� az �res
    [Test]
    public void Applicability_PositionTest()
    {
        //Ha olyan helyre tenn� ami nem �res, akkor hamissal t�r vissza
        State state = new State();
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        FirstStageOperator op = new FirstStageOperator(new Position(0,0,0,0));
        Assert.False(op.IsApplicable(state));

        //Ha olyan helyre tenn� ami �res, akkor igazzal t�r vissza
        state.Table.Board[0, 0, 0, 0] = Stone.Empty;
        Assert.True(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInFirstStage()
    {
        //Hamissal kell visszat�rnie ha nem First stage-be vagyunk
        State state = new State();
        state.CurrentStage = Stage.Second;
        FirstStageOperator op = new FirstStageOperator(new Position(1, 0, 0, 0));
        Assert.False(op.IsApplicable(state));
    }
}
