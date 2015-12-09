using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    
	private int x, y;

	//attach to each hint and run the player click when the hint tile is clicked
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
