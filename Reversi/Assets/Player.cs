using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	Team team = Team.NONE;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public List<List<Tile>> PlayerMove(int x, int y, List<List<Tile>> board, Team tempteam)
	{
		List<List<Tile>> temp = new List<List<Tile>>();
		temp = board;
		//Double check move is valid
		if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().IsValidMove(x, y, tempteam, temp))
		{
			//Set claimed tile
			temp[x][y] = new Tile(tempteam, true);
			
			//Check every direction to see if tiles could be claimed
			List<Direction> enemyDirections = new List<Direction>();
			MoveLogic logic = GameObject.Find("MoveLogic").GetComponent<MoveLogic>();
			foreach(var dir in Enum.GetValues(typeof(Direction)))
			{
				if (logic.EnemyInDirection((Direction)dir, tempteam, x, y, temp))
				{
					enemyDirections.Add((Direction)dir);
				}
			}
			//Go though all directions enemy found, see if can claim tiles
			foreach (var direction in enemyDirections)
			{
				Debug.Log("SETTING TILES");
				logic.SetTilesOnCapture(x, y, tempteam, direction, ref temp);
			}
		}

		return temp;
	}

	public void Initialise(Team team)
	{
		this.team = team;
	}
}
