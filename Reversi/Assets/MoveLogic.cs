using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveLogic : MonoBehaviour {

	private const int width = 8, height = 8;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}

	public bool IsValidMove(int x, int y, Team team, List<List<Tile>> board)
	{
		//Return false if out of bounds
		if (x >= width || y >= width || x < 0 || y < 0)
			return false;
		//Return false if tile is occupied
		if (board[x][y].occupied == true)
			return false;
		
		//Get a list of directions that there is an enemy in
		bool enemyFound = false;
		List<Direction> enemyDirections = new List<Direction>();
		for (int i = 0; i < 10; i++)
		{
			if (EnemyInDirection((Direction)i, team, x, y, board))
			{
				enemyFound = true;
				enemyDirections.Add((Direction)i);
			}
		}
		
		//Check if a valid move could be made from every direction there is an enemy
		bool validMove = false;
		if (enemyFound == false)
			return false;
		else
			foreach (var direction in enemyDirections)
		{
			if (ValidMoveRecursion(x, y, team, direction, board))
			{
				validMove = true;
			}
		}
		return validMove;
	}

	public bool EnemyInDirection(Direction direction, Team team, int x, int y, List<List<Tile>> board)
	{
		switch (direction)
		{
		case Direction.North:
			if (y >= height - 1)
			{
				return false;
			}
			if (board[x][y + 1].occupied == true && board[x][y + 1].team != team)
			{
				return true;
			}
			break;
		case Direction.NorthEast:
			if (y >= height - 1 || x >= width - 1)
			{
				return false;
			}
			if (board[x + 1][y + 1].occupied == true && board[x + 1][y + 1].team != team)
			{
				return true;
			}
			break;
		case Direction.East:
			if (x >= width - 1)
			{
				return false;
			}
			if (board[x + 1][y].occupied == true && board[x + 1][y].team != team)
			{
				return true;
			}
			break;
		case Direction.SouthEast:
			if (y <= 0 || x >= width - 1)
			{
				return false;
			}
			if (board[x + 1][y - 1].occupied == true && board[x + 1][y - 1].team != team)
			{
				return true;
			}
			break;
		case Direction.South:
			if (y <= 0)
			{
				return false;
			}
			if (board[x][y - 1].occupied == true && board[x][y - 1].team != team)
			{
				return true;
			}
			break;
		case Direction.SouthWest:
			if (y <= 0 || x <= 0)
			{
				return false;
			}
			if (board[x - 1][y - 1].occupied == true && board[x - 1][y - 1].team != team)
			{
				return true;
			}
			break;
		case Direction.West:
			if (x <= 0)
			{
				return false;
			}
			if (board[x - 1][y].occupied == true && board[x - 1][y].team != team)
			{
				return true;
			}
			break;
		case Direction.NorthWest:
			if (y >= height - 1 || x <= 0)
			{
				return false;
			}
			if (board[x - 1][y + 1].occupied == true && board[x - 1][y + 1].team != team)
			{
				return true;
			}
			break;
		default:
			break;
		}
		return false;
	}

	bool ValidMoveRecursion(int x, int y, Team team, Direction direction, List<List<Tile>> board)
	{
		//Generate new coordinates based on direction
		int newX = GetCoordinatesFromDirection(direction)[0] + x;
		int newY = GetCoordinatesFromDirection(direction)[1] + y;
		
		//Call the function again if there is an enemy
		if (EnemyInDirection(direction, team, x, y, board))
		{
			return ValidMoveRecursion(newX, newY, team, direction, board);
		}
		
		//If there isn't an enemy in the tile, run the checks to see if it's a valid move
		if (newX < 0 || newX >= width || newY < 0 || newY >= height)
		{
			return false;
		}
		if (board[newX][newY].team == team)
		{
			return true;
			
		}
		return false;
	}

	static int[] GetCoordinatesFromDirection(Direction direction)
	{
		int[] coordinates = new int[2] { 0, 0 };
		switch (direction)
		{
		case Direction.North:
			coordinates = new int[] { 0, 1 };
			break;
		case Direction.NorthEast:
			coordinates = new int[] { 1, 1 };
			break;
		case Direction.East:
			coordinates = new int[] { 1, 0 };
			break;
		case Direction.SouthEast:
			coordinates = new int[] { 1, -1 };
			break;
		case Direction.South:
			coordinates = new int[] { 0, -1 };
			break;
		case Direction.SouthWest:
			coordinates = new int[] { -1, -1 };
			break;
		case Direction.West:
			coordinates = new int[] { -1, 0 };
			break;
		case Direction.NorthWest:
			coordinates = new int[] { -1, 1 };
			break;
		default:
			break;
		}
		return coordinates;
	}

	public bool SetTilesOnCapture(int x, int y, Team team, Direction direction, ref List<List<Tile>> board)
	{
		//Generate new coordinates based on direction
		int newX = GetCoordinatesFromDirection(direction)[0] + x;
		int newY = GetCoordinatesFromDirection(direction)[1] + y;
		//If the direction contains a friendly tile, return that a move can be made in that direction
		if (newX < 0 || newY < 0 || newX >= width || newY >= height || board[newX][newY].occupied == false)
			return false;
		if(board[newX][newY].team == team)
		{
			return true;
		}
		//If enemy tile, check the next tile to see if it is friendly
		if (EnemyInDirection(direction, team, x, y, board))
		{
			if(SetTilesOnCapture(newX, newY, team, direction, ref board))
			{
				board[newX][newY] = new Tile(team, true);
				return true;
			}
		}
		//If empty tile, return false
		return false;
	}
}
