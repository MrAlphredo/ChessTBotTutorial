using Godot;
using System;
using System.Collections.Generic;

public partial class Bitboard : Godot.Node
{
	// Called when the node enters the scene tree for the first time.
	public ulong[] whitePieces = {0,0,0,0,0,0};
	public ulong[] blackPieces = {0,0,0,0,0,0};
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public ulong GetBlackBitboard(){
		ulong ans = 0;
		foreach(ulong i in blackPieces){
			ans |= i;
		}
		return ans;
	}

	public ulong GetWhiteBitboard(){
		ulong ans = 0;
		foreach(ulong i in whitePieces){
			ans |= i;
		}
		return ans;
	}


	public void ClearBitboard(){
		Array.Clear(whitePieces);
		Array.Clear(blackPieces);
	}

	public void SetBoard(ulong[] Whites, ulong[] Blacks){
		Array.Copy(Whites, whitePieces, Whites.Length);
		Array.Copy(Blacks, blackPieces, Blacks.Length);
	}

	public void InitBitBoard(string fen){
		ClearBitboard();
		string[] fen_split = fen.Split(" ");
		foreach (char i in fen_split[0]){
			if (i.Equals('/'))
				continue;
			if (Char.IsDigit(i)){
				int shiftAmount = int.Parse(i.ToString());
				LeftShift(shiftAmount);
				continue;
			}
			LeftShift(1);
			if(Char.IsUpper(i)){
				whitePieces[DataHandlerCS.FenDict[Char.ToLower(i)]] |= 1UL;
			}else{
				blackPieces[DataHandlerCS.FenDict[i]] |= 1UL;
			}
		}
		GD.Print("Bitboard init successfully");
	}
	private void LeftShift(int shiftAmount){
		for(int piece = 0; piece < blackPieces.Length; piece++){
					blackPieces[piece] <<= shiftAmount;
				}
				for(int piece = 0; piece < whitePieces.Length; piece++){
					whitePieces[piece] <<= shiftAmount;
				}
	}

	public ulong GetBitboard(){
		return blackPieces[1];
	}

	public void RemovePiece(int location, int pieceType){
		if(pieceType>5){//blackpieces
			blackPieces[pieceType%6] &= ~(1UL<<location);
		}
		else{
			whitePieces[pieceType%6] &= ~(1UL<<location);
		}
	}

	public void AddPiece(int location, int pieceType){
		if(pieceType>5){
			blackPieces[pieceType%6] |= 1UL<<location;
		}else{
			whitePieces[pieceType%6] |= 1UL<<location;
		}
	}

	public void MakeMove(DataHandlerCS.Move move, bool isBlackMove){
		ulong[] fromList = isBlackMove? blackPieces : whitePieces;
		ulong[] toList = isBlackMove? whitePieces : blackPieces;

		ulong fromBit = 1UL << move.From;
		ulong toBit = 1UL << move.To;
		for(int i = 0; i<6; i++){
			toList[i] &= ~(toBit);
		}
		for(int i = 0; i<6; i++){
			if ((fromList[i] & fromBit) != 0){
				fromList[i] &= ~fromBit;
				fromList[i] |= toBit;
			}
		}

	}

	public List<DataHandlerCS.Move> GenerateMoveSet(bool isBlackMove){
		ulong[] searchList;
		ulong selfboard, enemyboard;
		List<DataHandlerCS.Move> moveSet = new();
		GeneratePath pathGenerator = new();
		if(isBlackMove){
			selfboard = GetBlackBitboard();
			enemyboard = GetWhiteBitboard();
			searchList = blackPieces;
		}else{
			selfboard = GetWhiteBitboard();
			enemyboard = GetBlackBitboard();
			searchList = whitePieces;
		}
		//Bishop
		for(int i = 0; i < 64; i++){
			if((searchList[0]&(1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.BishopPath(i, selfboard, enemyboard, isBlackMove);
				for(int j = 0; j<64; j++){
					if((currentMoves & (1UL<<j)) != 0){
						DataHandlerCS.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//King
		for(int i = 0; i<64;i++){
			if((searchList[1] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.KingPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCS.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Knight
		for(int i = 0; i<64;i++){
			if((searchList[2] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.KnightPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCS.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Generate Pawn moveset
		for(int i = 0; i<64;i++){
			if((searchList[3] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.PawnPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCS.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Queen
		for(int i = 0; i<64;i++){
			if((searchList[4] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.QueenPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCS.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}
		//Rook
		for(int i = 0; i<64;i++){
			if((searchList[5] & (1UL<<i)) != 0){
				ulong currentMoves = pathGenerator.RookPath(i,selfboard, enemyboard,isBlackMove);
				//search for every available move, j is the position of the destination
				for(int j = 0; j<64;j++){
					if ((currentMoves & (1UL<<j))!=0){
						DataHandlerCS.Move newMove = new(i,j);
						moveSet.Add(newMove);
					}
				}
			}
		}

		return moveSet;

	}

	

}
