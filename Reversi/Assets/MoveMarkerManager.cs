using UnityEngine;
using System.Collections;

public class MoveMarkerManager : MonoBehaviour {
    public int x;
    public int y;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        Debug.Log("CLICK");
		GameObject.Find("Board").GetComponent<GridManager> ().PlayerMove (x, y);
		GameObject.Find("Board").GetComponent<GridManager>().AiTurn(Team.WHITE);
		GameObject.Find("Board").GetComponent<GridManager>().SumCounters();

    }
}
