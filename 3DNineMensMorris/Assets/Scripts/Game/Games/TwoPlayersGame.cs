using System.Collections;
using UnityEngine;

public class TwoPlayersGame : AGame
{
    private bool isFirstClick = false;
    GameObject firstGameObject;
    GameObject secondGameObject;
    void Start()
    {
        currentState = new State();
        StartCoroutine(Play());
    }

    void Update()
    {
        if (isPlaying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (currentState.CurrentStage == Stage.First)
                    {
                        firstGameObject = hit.collider.gameObject;
                        FirstStageStep(firstGameObject);
                    }
                    else if (currentState.CurrentStage == Stage.Remove)
                    {
                        firstGameObject = hit.collider.gameObject;
                        RemoveStageStep(firstGameObject);
                    }
                    else if (currentState.CurrentStage == Stage.Second ||
                             currentState.CurrentStage == Stage.Third)
                    {
                        if (!isFirstClick)
                        {
                            firstGameObject = hit.collider.gameObject;
                            isFirstClick = true;
                        }
                        else
                        {
                            secondGameObject = hit.collider.gameObject;
                            currentState.checkForSecondOrThirdStage();
                            SecondOrThirdStageStep(firstGameObject, secondGameObject);
                            isFirstClick = false;
                        }
                    }
                }
            }
        }
    }
    protected override IEnumerator Play()
    {
        while (currentState.GetStatus() == Stone.Empty)
        {
            yield return StartCoroutine(PlayerTurn());
        }
        isPlaying = false;

        Stone status = currentState.GetStatus();
        Debug.Log("Winner: " + status);
    }
    IEnumerator PlayerTurn()
    {
        while (!isPlayersTurn)
        {
            yield return null;
        }

        currentState.ChangePlayer();
        isPlayersTurn = false;
        yield return null;
    }
}
