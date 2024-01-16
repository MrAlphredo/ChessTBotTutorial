using Godot;
using System;
using System.Collections.Generic;

public partial class DataHandlerCS : Node
{
	// Called when the node enters the scene tree for the first time.
	public enum PieceNames {BISHOP, KING, KNIGHT, PAWN, QUEEN, ROOK}
    public static Dictionary<char, int> FenDict = new Dictionary<char, int>()
    {
		{'b', 0},{'k', 1},{'n', 2},{'p', 3},{'q', 4},{'r', 5}
	};

	public struct Move{
		public int From{get;set;}
		public int To{get;set;}
		public Move(int a, int b){
			From = a;
			To=b;
		}
	}

	public int[] pieceValues = {3,100000, 3,1,9,5};
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
