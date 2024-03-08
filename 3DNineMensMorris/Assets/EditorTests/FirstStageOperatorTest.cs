using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FirstStageOperatorTest
{
    //Leteszteli az alkalmazhatóságát a first stage operátornak, akkor alkalmazható ha first stage-ben vagyunk
    //és a hely ahová rakná az üres
    [Test]
    public void Applicability_PositionTest()
    {
        //Ha olyan helyre tenné ami nem üres, akkor hamissal tér vissza
        State state = new State();
        state.Table.Board[0, 0, 0, 0] = Stone.Blue;
        FirstStageOperator op = new FirstStageOperator(new Position(0,0,0,0));
        Assert.False(op.IsApplicable(state));

        //Ha olyan helyre tenné ami üres, akkor igazzal tér vissza
        state.Table.Board[0, 0, 0, 0] = Stone.Empty;
        Assert.True(op.IsApplicable(state));
    }

    [Test]
    public void ApplicabilityWhenStateIsNotInFirstStage()
    {
        //Hamissal kell visszatérnie ha nem First stage-be vagyunk
        State state = new State();
        state.CurrentStage = Stage.Second;
        FirstStageOperator op = new FirstStageOperator(new Position(1, 0, 0, 0));
        Assert.False(op.IsApplicable(state));
    }
}
