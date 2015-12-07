using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	public float CalculateUCT ()
	{
		return Q + (2*(1/Mathf.Sqrt(2))* Mathf.Sqrt((2*Mathf.Log10(Parent.N))/N));
	}
}

public struct Move
{
	public int x, y;

    public Move(int x, int y) 
	{
		this.x = x;
		this.y = y;
	}
}

public enum Direction
{
	North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
};