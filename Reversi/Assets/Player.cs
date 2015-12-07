using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    
	private int x, y;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
		GameObject.Find("Game Manager").GetComponent<GameManager> ().PlayerClick(x, y);
    }

	public void setX(int x)
	{
		this.x = x;
	}

	public void setY(int y)
	{
		this.y = y;
	}
}
