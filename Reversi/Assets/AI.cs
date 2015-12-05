using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {

	Team team = Team.NONE;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialise(Team team)
	{
		this.team = team;
	}

	public List<List<Tile>> AiMove(List<List<Tile>> board)
	{
		return new List<List<Tile>>();
	}
}
