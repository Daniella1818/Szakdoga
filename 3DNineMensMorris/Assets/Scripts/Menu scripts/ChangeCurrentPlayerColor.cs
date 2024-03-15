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
            // Most már hozzáférhetünk a myVariable változóhoz
            Stone playerColor = game.currentState.CurrentPlayer;
            color = colorObj.GetComponent<RawImage>();
            if (playerColor == Stone.Blue)
                color.color = Color.blue;
            else if (playerColor == Stone.Red)
                color.color = Color.red;

            // Most itt használhatod a valueFromScriptA változót
            Debug.Log("Változó értéke: " + playerColor);
        }
        else
        {
            Debug.LogError("GameObjectA vagy ScriptA nem található!");
        }
    }
}
