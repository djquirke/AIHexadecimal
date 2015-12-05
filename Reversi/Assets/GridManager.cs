using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Team
{
    BLACK,
    WHITE,
    NONE
}
public class Tile
{
    public Team team { get; set; }
    public bool occupied { get; set; }

    public Tile(Team team, bool occupied)
    {
        this.team = team;
        this.occupied = occupied;
    }
}

class TreeNode
{
	public move Move;
	public Team team;
	public List<TreeNode> Children;
	public TreeNode Parent = null;
	public float Q = 0,N = 0;
	public List<List<Tile>> board;
	public TreeNode ()
	{
		Children = new List<TreeNode> ();
	}
	
	public float CalculateUCT ()
	{
		return Q + (2*(1/Mathf.Sqrt(2))* Mathf.Sqrt((2*Mathf.Log10(Parent.N))/N));
	}

}

public struct move
{
	public int x, y;

	public move(int x, int y) 
	{
		this.x = x;
		this.y = y;
	}


}
public enum Direction
{
	North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
};
public class GridManager : MonoBehaviour
{
    

    List<List<Tile>> board_ = new List<List<Tile>>();
    public GameObject tileHighlight;
    public GameObject counterInst;

    Team playerTeam = Team.BLACK;
    Team currentGo = Team.BLACK;

    public const int width = 8, height = 8;
    private int blackCount = 0, whiteCount = 0;
    Tile white = new Tile(Team.WHITE, true);
    Tile black = new Tile(Team.BLACK, true);

    // Use this for initialization
    void Start()
    {
        CreateBoard();
        SetUpBoard();
        UpdateBoard(board_);
        ShowValidMoves(currentGo, board_);
    }

    // Update is called once per frame
    void Update()
    {
		DisplayScore();
    }

    void CreateBoard()
    {
        for (int i = 0; i < height; i++)
        {
            List<Tile> tempList = new List<Tile>();
            for (int j = 0; j < width; j++)
            {
                Tile tempTile = new Tile(Team.NONE, false);
                tempList.Add(tempTile);
            }
            board_.Add(tempList);
        }
    }

    void SetUpBoard()
    {
        int wsmaller = (width / 2) - 1;
        int wlarger = (width / 2);
        int lsmaller = (height / 2) - 1;
        int llarger = (height / 2);
        board_[wsmaller][lsmaller] = white;
        board_[wlarger][llarger] = white;
        board_[wsmaller][llarger] = black;
        board_[wlarger][lsmaller] = black;
        whiteCount += 2;
        blackCount += 2;
    }

    void UpdateBoard(List<List<Tile>> board)
    {
        foreach (var counter in GameObject.FindGameObjectsWithTag("Counter"))
        {
            GameObject.Destroy(counter);
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
                    GameObject counter = (GameObject)Instantiate(counterInst, new Vector3(locX, 1.5f, locY), Quaternion.Euler(Vector3.zero));
                    if (board[i][j].team == Team.BLACK)
                    {
                        counter.transform.rotation = Quaternion.Euler(new Vector3(180.0f, 0.0f, 0.0f));
                    }
                }
            }
        }
    }

    void NewTurn(List<List<Tile>> board)
    {
        SwitchPlayerTurn();
        UpdateBoard(board);
        if(!IsMoveAvailable(currentGo))
		{
			blackCount = 0;
			whiteCount = 0;
            for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					if(board_[i][j].team == Team.BLACK)
					{
						blackCount++;
					}
					else if(board_[i][j].team == Team.WHITE)
					{
						whiteCount++;
					}
				}
            }

			if(blackCount > whiteCount)
				Debug.Log("BLACK IS WIN");
			else if(whiteCount > blackCount)
				Debug.Log("WHITE IS WIN");
			else
				Debug.Log("DRAW");
            //SwitchPlayerTurn();
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("Marker"))
        {
            Destroy(item);
        }
        ShowValidMoves(currentGo,board_);

    }

	public void AiTurn(Team team)
	{
		//StoreValidMoves (team);
		TreeNode root = new TreeNode();
		root.board = board_;
		root.team = team;
		TreeNode selected = AISelection (root);
		//Simulation (selected);
		float bestQ = -9999999;
		TreeNode bestNode = new TreeNode();
		foreach(TreeNode node in root.Children)
		{
			if(node.Q > bestQ)
			{
				bestQ = node.Q;
				bestNode = node;
			}
		}
		MakeMove (bestNode.Move.x, bestNode.Move.y, Team.WHITE);
	}
	
	TreeNode AISelection(TreeNode root)
	{
		TreeNode bestchild = new TreeNode();
		float bestUCT = -999999;
		
		if (root.Children.Count == 0)
		{

				return Expand(root);

		} 
		else
		{
			foreach (TreeNode node in root.Children) 
			{
				if ( node.N == 0)
				{

					return Expand(node);
				}
				
				float UCT = node.CalculateUCT ();
				if (UCT > bestUCT) 
				{
					bestchild = node;
				}

			}

		}
		return AISelection(bestchild);
		
	}
	
	TreeNode Expand(TreeNode node)
	{
		List<move> movesList = StoreValidMoves (node.team,node.board);
		foreach (move move in movesList) 
		{
			TreeNode newnode = new TreeNode();
			newnode.Parent = node;
			if(node.team == Team.WHITE)
			{
				newnode.team = Team.BLACK;
				newnode.board = SimulateMove(move, node.team, node.board);
				//newnode.board[move.x][move.y].team = Team.WHITE;
			}
			else
			{
				newnode.team = Team.WHITE;
				newnode.board = SimulateMove(move, node.team, node.board);
				//newnode.board[move.x][move.y].team = Team.BLACK;
			}
			
			newnode.Move = move;	
			node.Children.Add(newnode);
		}
		int x = UnityEngine.Random.Range (0, node.Children.Count -1);

		return node.Children [x];

	}

	void Simulation(TreeNode state)
	{
		bool terminal = false;

		while (!terminal)
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					terminal = IsValidMove(i,j,state.team,state.board);
				}
			}

			List<move> valid_moves = StoreValidMoves(state.team, state.board);
			int r = UnityEngine.Random.Range(0, valid_moves.Count - 1);

			MakeMove(valid_moves[r].x, valid_moves[r].y, state.team, state.board);
		}

		SumCounters (state.board);
	
		if (whiteCount < blackCount) {
			//Black wins
			if(state.team==Team.WHITE)
			{
				Backup(state,-1);
			}
			else{

				Backup(state,1);
			}
		} else if (whiteCount > blackCount) {
			//White wins
			if(state.team==Team.WHITE)
			{
				Backup(state,1);
			}
			else{
				
				Backup(state,-1);
			}
		} else {
			Backup(state,0);
		}




		//THIS STATE'S TEAM WINS
		//PLAYER WINS
		//DRAW
	}

	void Backup(TreeNode selected, int score)
	{
		while (selected != null)
		{
			selected.N ++;
			//AI PLAYS AS WHITE
			if (selected.team.Equals(Team.WHITE))
			{
				selected.Q += score;
			}
			else if (selected.team.Equals(Team.BLACK))
			{
				selected.Q -= score;
			}
			Backup(selected.Parent, -score);
		}
	}


	bool IsValidMove(int x, int y, Team team,  List<List<Tile>> board )
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
    //check if any moves are available for current player
    bool IsMoveAvailable(Team team)
    {
        bool ret = false;

        //loop through all grid spaces
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if tile is occupied - ignore
                if (board_[i][j].occupied)
                    continue;
                //check if the tile could be taken by current player
                else
                {
                    if (IsValidMove(i, j, team, board_))
                    {
                        ret = true;
                        break;
                    }
                }
            }
        }

        return ret;
    }

    //Check if 2 counters are on the same team
    bool OpposingCounter(Team first, Team second)
    {
        if ((first == Team.WHITE && second == Team.BLACK) || (first == Team.BLACK && second == Team.WHITE))
        {
            return true;
        }
        return false;
    }

    void SwitchPlayerTurn()
    {
        if (currentGo == Team.BLACK)
            currentGo = Team.WHITE;
        else
            currentGo = Team.BLACK;
    }

	void ShowValidMoves(Team team, List<List<Tile>> board)
    {

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (IsValidMove(i, j, team, board))
                {
                    //Set tile location
                    float locX = i - (height - 1) / 2.0f;
                    float locY = j - (width - 1) / 2.0f;
                    //Scale up location
                    locX *= 10.0f;
                    locY *= 10.0f;
                    GameObject counter = (GameObject)Instantiate(tileHighlight, new Vector3(locX, 1.5f, locY), tileHighlight.transform.rotation);
                    counter.GetComponent<MoveMarkerManager>().setX(i);
                    counter.GetComponent<MoveMarkerManager>().setY(j);
                }
            }
        }
    }

	List<move> StoreValidMoves(Team team, List<List<Tile>> board)
	{
		List<move> moves = new List<move>();
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (IsValidMove(i, j, team, board))
				{
					move move = new move(i,j);
					moves.Add(move);
				}
			}
		}
		return moves; 
	}



    //checks for game terminating conditions
	bool CheckEndGame( List<List<Tile>> board)
    {
        //check if any moves available for current players go
        if (!IsMoveAvailable(currentGo))
            return true;


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //if not a space is not occupied then board is not full
                if (!board[i][j].occupied)
                {
                    return false;
                }
            }
        }

        return true;
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

    bool EnemyInDirection(Direction direction, Team team, int x, int y, List<List<Tile>> board)
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

	bool SetTilesOnCapture(int x, int y, Team team, Direction direction, List<List<Tile>> board)
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
            if(SetTilesOnCapture(newX, newY, team, direction, board))
            {
                board[newX][newY] = new Tile(team, true);
                return true;
            }
        }
        //If empty tile, return false
        return false;
    }

    public void PlayerMove(int x, int y)
    {
        MakeMove(x, y, Team.BLACK);
    }

    void MakeMove(int x, int y, Team team)
    {
        //Double check move is valid
        if (IsValidMove(x, y, team, board_))
        {
            //Set claimed tile
            board_[x][y] = new Tile(team, true);

            //Check every direction to see if tiles could be claimed
            List<Direction> enemyDirections = new List<Direction>();
			foreach(var dir in Enum.GetValues(typeof(Direction)))
			{
                if (EnemyInDirection((Direction)dir, team, x, y, board_))
                {
                    enemyDirections.Add((Direction)dir);
                }
            }
            //Go though all directions enemy found, see if can claim tiles
            foreach (var direction in enemyDirections)
            {
                Debug.Log("SETTING TILES");
                SetTilesOnCapture(x, y, team, direction, board_);
            }

            NewTurn(board_);
        }
		SumCounters (board_);
    }

	void DisplayScore()
	{
		GameObject.Find ("WhiteScore").GetComponent<Text> ().text = ("WHITE COUNTERS: " + whiteCount);
		GameObject.Find ("BlackScore").GetComponent<Text> ().text = ("BLACK COUNTERS: " + blackCount);
	}

	void MakeMove(int x, int y, Team team, List<List<Tile>> board)
    {
        //Double check move is valid
        if (IsValidMove(x, y, team, board))
        {
            //Set claimed tile
            board[x][y] = new Tile(team, true);

            //Check every direction to see if tiles could be claimed
            List<Direction> enemyDirections = new List<Direction>();
			foreach(var dir in Enum.GetValues(typeof(Direction)))
			{
                if (EnemyInDirection((Direction)dir, team, x, y, board))
                {
                    enemyDirections.Add((Direction)dir);
                }
            }
            //Go though all directions enemy found, see if can claim tiles
            foreach (var direction in enemyDirections)
            {
                Debug.Log("SETTING TILES");
                SetTilesOnCapture(x, y, team, direction, board);
            }

            NewTurn(board);
			SumCounters();
        }
    }

	List<List<Tile>> SimulateMove(move pos, Team team, List<List<Tile>> board)
	{
		List<List<Tile>> newBoard = new List<List<Tile>>();
		newBoard = board;

		if(IsValidMove(pos.x, pos.y, team, newBoard))
		{
			newBoard[pos.x][pos.y] = new Tile(team, false);

			List<Direction> enemyDirections = new List<Direction>();
			foreach(var dir in Enum.GetValues(typeof(Direction)))
			{
				if(EnemyInDirection((Direction)dir, team, pos.x, pos.y, newBoard))
				{
					enemyDirections.Add ((Direction)dir);
				}
			}

			foreach(Direction dir in enemyDirections)
			{
				SetTilesOnCapture(pos.x, pos.y, team, dir, newBoard);
			}
		}

		return newBoard;
	}

	public void SumCounters(List<List<Tile>> board)
	{
		blackCount = 0;
		whiteCount = 0;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if(board[i][j].team == Team.BLACK)
				{
					blackCount++;
				}
				else if(board[i][j].team == Team.WHITE)
				{
					whiteCount++;
				}
			}
		}
	}

	public void SumCounters()
	{
		blackCount = 0;
		whiteCount = 0;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if(board_[i][j].team == Team.BLACK)
				{
					blackCount++;
				}
				else if(board_[i][j].team == Team.WHITE)
				{
					whiteCount++;
				}
			}
		}
	}
}
