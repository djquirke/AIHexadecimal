using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private List<List<Tile>> game_board;
	private int whiteCount = 0, blackCount = 0;
	private const int width = 8, height = 8;
	private bool game_over = false;
	private Team currentGo = Team.BLACK;

	// Use this for initialization
	void Start () {
		game_board = new List<List<Tile>>();
		game_board = GameObject.Find("Board").GetComponent<BoardManager>().Initialise(currentGo);
		GameObject.Find("Player").GetComponent<Player>().Initialise(Team.BLACK);
		GameObject.Find("AiAgent").GetComponent<AI>().Initialise(Team.WHITE);
		CalculateScores();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//display scores
	void OnGUI()
	{
		GameObject.Find ("WhiteScore").GetComponent<Text> ().text = ("WHITE COUNTERS: " + whiteCount);
		GameObject.Find ("BlackScore").GetComponent<Text> ().text = ("BLACK COUNTERS: " + blackCount);

		if(game_over)
		{
			if(whiteCount > blackCount)
			{
				DisplayGameOver(Team.WHITE.ToString());
			}
			else if (blackCount > whiteCount)
			{
				DisplayGameOver(Team.BLACK.ToString());
			}
			else
			{
				DisplayGameOver("DRAW");
			}
		}
	}

	public void PlayerClick(int x, int y)
	{
		if(!game_over)
		{
			Debug.Log("CLICK");

			game_board = GameObject.Find("Player").GetComponent<Player>().PlayerMove(x, y, game_board, currentGo);
			SwitchPlayerTurn();
			GameObject.Find("Board").GetComponent<BoardManager>().UpdateBoard(currentGo, ref game_board);
			CalculateScores();
			
			if(GameOver())
			{
				game_over = true;
				return;
			}

//			game_board = GameObject.Find("AI").GetComponent<AI>().AiMove(game_board);
//			GameObject.Find("AiAgent").GetComponent<BoardManager>().UpdateBoard(ref game_board);
//			CalculateScores();
//
//			if(GameOver())
//			{
//				game_over = true;
//				return;
//			}
//			SwitchPlayerTurn();
		}
	}



	void SwitchPlayerTurn()
	{
		if(currentGo == Team.BLACK)
			currentGo = Team.WHITE;
		else
			currentGo = Team.BLACK;
	}

	
	//sum counters for each team
	void CalculateScores()
	{
		blackCount = 0;
		whiteCount = 0;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if(game_board[i][j].team == Team.BLACK)
				{
					blackCount++;
				}
				else if(game_board[i][j].team == Team.WHITE)
				{
					whiteCount++;
				}
			}
		}
	}

	//check if the game is over
	bool GameOver()
	{
		//check if any moves available for current players go
		if (!GameObject.Find ("Board").GetComponent<BoardManager>().IsMoveAvailable(currentGo, game_board))
			return true;

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				//if not a space is not occupied then board is not full
				if (!game_board[i][j].occupied)
				{
					return false;
				}
			}
		}
		
		return true;
	}



	void DisplayGameOver(string winner)
	{
		Debug.Log(winner + " wins");
	}
}
