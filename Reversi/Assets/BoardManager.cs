using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	
	private const int width = 8, height = 8;
	public GameObject highlight, counter;
	Tile white = new Tile(Team.WHITE, true);
	Tile black = new Tile(Team.BLACK, true);

	public List<List<Tile>> Initialise(Team currentGo)
	{
		List<List<Tile>> board = new List<List<Tile>>();
		board = CreateBoard(board);
		board = SetUpBoard(board);
		UpdateBoard(ref board);
		ShowValidMoves(currentGo, board);

		return board;
	}

	//create the initial board with empty tiles
	List<List<Tile>> CreateBoard(List<List<Tile>> board)
	{
		for (int i = 0; i < height; i++)
		{
			List<Tile> tempList = new List<Tile>();
			for (int j = 0; j < width; j++)
			{
				Tile tempTile = new Tile(Team.NONE, false);
				tempList.Add(tempTile);
			}
			board.Add(tempList);
		}

		return board;
	}

	//setup the 4 tiles in the centre of the board
	List<List<Tile>> SetUpBoard(List<List<Tile>> board)
	{
		int wsmaller = (width / 2) - 1;
		int wlarger = (width / 2);
		int lsmaller = (height / 2) - 1;
		int llarger = (height / 2);
		board[wsmaller][lsmaller] = white;
		board[wlarger][llarger] = white;
		board[wsmaller][llarger] = black;
		board[wlarger][lsmaller] = black;

		return board;
	}

	//draw new board
	public void UpdateBoard(ref List<List<Tile>> board)
	{
		foreach (var counter in GameObject.FindGameObjectsWithTag("Counter"))
		{
			GameObject.Destroy(counter);
		}
		
		foreach (var valid_move in GameObject.FindGameObjectsWithTag("Marker"))
		{
			GameObject.Destroy(valid_move);
		}
		
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (board[i][j].occupied)
				{
					//Set tile location
					float locX = i - (height - 1) / 2.0f;
					float locY = j - (width - 1) / 2.0f;
					//Scale up location
					locX *= 10.0f;
					locY *= 10.0f;
					GameObject counterInst = (GameObject)Instantiate(counter, new Vector3(locX, 1.5f, locY), Quaternion.Euler(Vector3.zero));
					if (board[i][j].team == Team.BLACK)
					{
						counterInst.transform.rotation = Quaternion.Euler(new Vector3(180.0f, 0.0f, 0.0f));
					}
				}
			}
		}
	}

	//check if any move available for team passed in
	public bool IsMoveAvailable(Team team, List<List<Tile>> board)
	{
		bool ret = false;
		MoveLogic logic = GameObject.Find("MoveLogic").GetComponent<MoveLogic>();
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (board[i][j].occupied)
					continue;
				//check if the tile could be taken by current player
				else
				{
					if (logic.IsValidMove(i, j, team, board))
					{
						ret = true;
						break;
					}
				}
			}
		}
		
		return ret;
	}

	//show all valid moves for a team
	public void ShowValidMoves(Team team, List<List<Tile>> board)
	{
		foreach (var item in GameObject.FindGameObjectsWithTag("Marker"))
		{
			Destroy(item);
		}

		//if no moves are available, quit
		if(!GameObject.Find("Board").GetComponent<BoardManager>().IsMoveAvailable(team, board))
			return;

		MoveLogic logic = GameObject.Find("MoveLogic").GetComponent<MoveLogic>();
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				//setup the hint counter when a valid move is found
				if (logic.IsValidMove(i, j, team, board))
				{
					//Set tile location
					float locX = i - (height - 1) / 2.0f;
					float locY = j - (width - 1) / 2.0f;
					//Scale up location
					locX *= 10.0f;
					locY *= 10.0f;
					GameObject counter = (GameObject)Instantiate(highlight, new Vector3(locX, 1.5f, locY), highlight.transform.rotation);
					counter.GetComponent<Player>().setX(i);
					counter.GetComponent<Player>().setY(j);
				}
			}
		}
	}
}
