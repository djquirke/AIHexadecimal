using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private List<List<Tile>> game_board;
	private int whiteCount = 0, blackCount = 0;
	private const int width = 8, height = 8;
	private bool game_over = false, ai_move = false, can_run = false;
	private Team currentGo = Team.BLACK, playerTeam = Team.NONE;

	// Use this for initialization
	void Start () {
		game_board = new List<List<Tile>>();
		game_board = GameObject.Find("Board").GetComponent<BoardManager>().Initialise(currentGo);
		playerTeam = Team.BLACK;
		GameObject.Find("AiAgent").GetComponent<AI>().Initialise(Team.WHITE);
		CalculateScores();
	}

	// Update is called once per frame
	void Update () {
		//if true, the AI can make a move
		if(ai_move && can_run && !game_over)
		{
			game_board = GameObject.Find("AiAgent").GetComponent<AI>().AiMove(game_board);
			SwitchPlayerTurn();
			GameObject.Find("Board").GetComponent<BoardManager>().UpdateBoard(ref game_board);
			GameObject.Find("Board").GetComponent<BoardManager>().ShowValidMoves(currentGo, game_board);
			CalculateScores();
			
			if(GameOver())
			{
				GameObject.Find("Board").GetComponent<BoardManager>().UpdateBoard(ref game_board);
				game_over = true;
			}
			can_run = false;
			ai_move = false;
		}
		//skip a frame to allow Unity render function to show the players move
		if(ai_move)
			can_run = true;

		DisplayScores();
	}

	//display win/lose on game over
	void OnGUI()
	{
		if(game_over)
		{
			if(whiteCount > blackCount)
			{
				GUI.Label(new Rect(600, 35, 100, 100), "YOU LOSE!");
			}
			else if (blackCount > whiteCount)
			{
				GUI.Label(new Rect(600, 35, 100, 100), "YOU WIN!");
			}
			else
			{
				GUI.Label(new Rect(600, 35, 100, 100), "IT'S A DRAW!");
			}
		}
	}

	//display scores
	void DisplayScores()
	{
		GameObject.Find ("WhiteScore").GetComponent<Text> ().text = ("WHITE COUNTERS: " + whiteCount);
		GameObject.Find ("BlackScore").GetComponent<Text> ().text = ("BLACK COUNTERS: " + blackCount);
	}

	//run the player move logic when the player clicks on a hint tile on their turn
	public void PlayerClick(int x, int y)
	{
		if(!game_over && !ai_move)
		{
			game_board = GameObject.Find("MoveLogic").GetComponent<MoveLogic>().MakeMove(x, y, playerTeam, game_board);
			GameObject.Find("Board").GetComponent<BoardManager>().UpdateBoard(ref game_board);
			CalculateScores();
			SwitchPlayerTurn();
			
			if(GameOver())
			{
				game_over = true;
			}

			ai_move = true;
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
}
