using System.Collections;
using UnityEngine;

public class TwoPlayersGame : AGame, IPlayerGame
{
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
                    ClickWatcher(hit);
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

    public IEnumerator PlayerTurn()
    {
        while (!isNextPlayerCanPlay)
        {
            yield return null;
        }

        ChangeColorBasedOnPlayer();
        yield return null;
    }
}
