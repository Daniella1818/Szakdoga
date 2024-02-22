using UnityEngine;
using UnityEngine.UI;

public class ChangeCurrentPlayerColor : MonoBehaviour
{
    GameObject table;
    GameObject colorObj;
    RawImage color;
    void Start()
    {
        table = GameObject.Find("CubeTable");
        colorObj = GameObject.Find("CurrentPlayer");
    }

    void Update()
    {
        if (table != null && table.TryGetComponent(out AGame game))
        {
            // Most m�r hozz�f�rhet�nk a myVariable v�ltoz�hoz
            Stone playerColor = game.currentState.CurrentPlayer;
            color = colorObj.GetComponent<RawImage>();
            if (playerColor == Stone.Blue)
                color.color = Color.blue;
            else if (playerColor == Stone.Red)
                color.color = Color.red;

            // Most itt haszn�lhatod a valueFromScriptA v�ltoz�t
            Debug.Log("V�ltoz� �rt�ke: " + playerColor);
        }
        else
        {
            Debug.LogError("GameObjectA vagy ScriptA nem tal�lhat�!");
        }
    }
}
