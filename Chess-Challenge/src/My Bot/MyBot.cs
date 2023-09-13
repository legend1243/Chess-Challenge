using ChessChallenge.API;
using System;
public class MyBot : IChessBot
{
    Random Rng = new();
    public Move Think(Board board, Timer timer)
    {
        return board.GetLegalMoves()[Rng.Next(board.GetLegalMoves().Length)];
    }
    
}

