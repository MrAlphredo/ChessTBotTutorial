using Godot;
using System;

public partial class GeneratePath : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public ulong RookPath(int rookPosition, ulong selfboard, ulong enemyboard, bool isBlack){
		ulong legalMoves = 0;
		//calculate moves to the right
		for (int i = rookPosition+1; i<=63&&i%8!=0; i++){
			//check if the spot is occupied and that its not an enemy
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			
			legalMoves |= 1UL << i;
		}
		//left
		for (int i = rookPosition -1; i>=0 && i%8 != 7; i--){
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			legalMoves |= 1UL << i;
		}
		//up
		for (int i = rookPosition + 8; i < 64; i+=8){
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			legalMoves |= 1UL << i;
		}
		//down
		for (int i = rookPosition - 8; i >= 0; i-=8){
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			legalMoves |= 1UL << i;
		}
		return legalMoves;
	}

	public ulong KnightPath(int knightPosition, ulong selfboard, ulong enemyboard, bool isBlack){
		ulong knightMask = 43234889994;
		int shiftAmount = knightPosition - 18;
		if (shiftAmount >= 0){
			knightMask <<= shiftAmount;
		}else{
			knightMask >>= -shiftAmount;
		}
		if (knightPosition % 8 >5){
			knightMask &= ~0x303030303030303UL;
		}
		if (knightPosition % 8 < 2){
			knightMask &= ~0xC0C0C0C0C0C0C0C0UL;
		} 
		knightMask ^= selfboard&knightMask;
		return knightMask;
	}

	public ulong BishopPath(int bishopPosition, ulong selfboard, ulong enemyboard, bool isBlack){
		ulong legalMoves = 0;
		//calculate moves to the top left
		for (int i = bishopPosition+9; i<=63&&i%8!=0; i+=9){
			//check if the spot is occupied and that its not an enemy
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			legalMoves |= 1UL << i;
		}
		//top right
		for (int i = bishopPosition +7; i<=63 &&i%8 != 7; i+=7){
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			legalMoves |= 1UL << i;
		}
		//bot left
		for (int i = bishopPosition - 7 ; i>=0&&i%8!=0 ; i-=7){
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			legalMoves |= 1UL << i;
		}
		//bot right
		for (int i = bishopPosition - 9; i >= 0&&i%8 != 7; i-=9){
			if ((enemyboard & (1UL << i)) != 0){
				legalMoves |= 1UL << i;
				break;
			}
			if ((selfboard & (1UL << i)) != 0){
				break;
			}
			legalMoves |= 1UL << i;
		}
		return legalMoves;
	}

	public ulong QueenPath(int queenPosition, ulong selfboard, ulong enemyboard, bool isBlack){
		ulong legalMoves = 0;
		legalMoves = BishopPath(queenPosition, selfboard, enemyboard, isBlack) | RookPath(queenPosition, selfboard, enemyboard, isBlack);
		return legalMoves;
	}

	public ulong KingPath(int kingPosition, ulong selfboard, ulong enemyboard, bool isBlack){
		ulong kingMask = 460039;
		int shiftAmount = kingPosition - 9;
		if (shiftAmount >= 0){
			kingMask <<= shiftAmount;
		}else{
			kingMask >>= -shiftAmount;
		}
		if (kingPosition % 8 >6){
			kingMask &= ~0x303030303030303UL;
		}
		if (kingPosition % 8 < 1){
			kingMask &= ~0xC0C0C0C0C0C0C0C0UL;
		} 
		kingMask ^= selfboard&kingMask;
		return kingMask;
	}

	public ulong PawnPath(int pawnPosition, ulong selfboard, ulong enemyboard, bool isBlack){
		ulong pawnMask = 0;
		ulong attackMask = 0;
		if (isBlack){
			if (pawnPosition>=47 && pawnPosition<56){
				//pawnMask for black set at position 63
				pawnMask = 0x80800000000000;
			}else{
				pawnMask = 0x80000000000000;
			}
			pawnMask >>= 63 - pawnPosition;
			//check if there is a piece blocking, if so, cancel the two step movement
			if(((selfboard|enemyboard)&(1UL<<(pawnPosition-8)))!=0){
				pawnMask=0;
			}
			attackMask = 0xA0000000000000;
			//attackMask preset the pawn position at 62
			if (pawnPosition == 63){
				attackMask<<=1;
			}else{
				attackMask >>= 62-pawnPosition;
			}
		}else{
			if (pawnPosition>=7 && pawnPosition<16){
				//pawnMask for white set at position 0
				pawnMask = 0x10100;
			}else{
				pawnMask = 0x100;
			}
			attackMask = 0x500;
			pawnMask <<= pawnPosition;
			//check if there is a piece blocking, if so, cancel the two step movement
			if(((selfboard|enemyboard)&(1UL<<(pawnPosition+8)))!=0){
				pawnMask=0;
			}
			//attackMask preset the pawn position at 1
			if (pawnPosition == 0){
				attackMask>>=1;
			}else{
				attackMask <<= pawnPosition - 1;
			}
		}
		if (pawnPosition % 8 >6){
			attackMask &= ~0x303030303030303UL;
		}
		if (pawnPosition % 8 < 1){
			attackMask &= ~0xC0C0C0C0C0C0C0C0UL;
		} 
		attackMask&=enemyboard;
		pawnMask^=pawnMask&(selfboard|enemyboard);
		pawnMask|=attackMask;
		return pawnMask;
	}
}
