using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//stores the tile's team
public enum Team
{
    BLACK,
    WHITE,
    NONE
}

//stores a board tile
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

//stores a node in the tree for MCTS
class TreeNode
{
	public Move move;
	public Team team;
	public List<TreeNode> Children;
	public TreeNode Parent = null;
	public float Q = 0,N = 0;
	public List<List<Tile>> board;
	public TreeNode ()
	{
		Children = new List<TreeNode> ();
		board = new List<List<Tile>>();
	}
	
    //evaluation function Upper Confidence Bound
	public float CalculateUCT ()
	{
		return Q + (2*(1/Mathf.Sqrt(2))* Mathf.Sqrt((2*Mathf.Log10(Parent.N))/N));
	}
}

//stores a move that the ai made to get to the next state
public struct Move
{
	public int x, y;

    public Move(int x, int y) 
	{
		this.x = x;
		this.y = y;
	}
}

//definition for each of the 8 directions available as a move
public enum Direction
{
	North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
};