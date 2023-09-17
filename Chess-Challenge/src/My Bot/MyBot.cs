using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;
public class MyBot : IChessBot
{
    // Random Rng = new();
    // public Move Think(Board board, Timer timer)
    // {
    //     return board.GetLegalMoves()[Rng.Next(board.GetLegalMoves().Length)];
    // }
    int[] pieceValues = {1, 3, 3, 5, 10, 1000, -1, -3, -3, -5, -10, -1000 };
    Random rng = new();
    Move PreviousMove = Move.NullMove;
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Move moveToPlay = moves[rng.Next(moves.Length)];
        int highestValue = int.MinValue;
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            if (board.IsInCheckmate())
            {
                return move;
            }
            int Value = -Search(board, 4, -1000, 1000); 

            if (Value >= highestValue)
            {
                highestValue = Value;
                moveToPlay = move;
            }
            board.UndoMove(move);
        }
        Console.WriteLine(highestValue);
        return moveToPlay;
    }
    int Search(Board board, int iteration, int alpha, int beta)
    {
        Move[] moves = MoveOrder(board);

        if (iteration == 0)
        {
            return CountMaterial(board);
        }
        if (board.IsInCheckmate())
        {
            return int.MaxValue;
        }
        if (board.IsDraw())
        {
            return 0;
        }
        foreach (Move move in moves)
        {
            // Find highest value capture
            board.MakeMove(move);
            int Value = -Search(board, iteration - 1, -beta, -alpha);
            board.UndoMove(move);
            if (beta <= Value)
            {
                return beta;
            }
            alpha = Math.Max(Value, alpha);
        }

        return alpha;
    }
    int CountMaterial(Board board)
    {
        bool side = board.IsWhiteToMove;
        int Material = 0;
        PieceList[] all = board.GetAllPieceLists();
        foreach (PieceList piece in all)
        {
            Material += pieceValues[(int)piece.TypeOfPieceInList] * (piece.IsWhitePieceList ? 1 : -1) * (side ? 1 : -1) * piece.Count;
        }
        Material += (side ? 1 : -1) * (board.IsInCheck() ? 3 : 0) + (side ? 1 : -1) * (board.IsInCheckmate() ? 1000 : 0);
        // if(board.IsDraw()){Material = 0;}
        return Material;
    }
    int CountBoard(Board board)
    {
        bool side = board.IsWhiteToMove;
        int Material = 0;
        for (int i = 0; i < 64; i++)
        {
            Material += ((int)board.GetPiece(new Square(i)).PieceType != 0 ? 1 : 0);
        }
        return Material;
    }
    Move[] MoveOrder(Board board)
    {
        Move[] moves = board.GetLegalMoves();
        var check = new List<Move>() { };
        var capture = new List<Move>() { };
        var else1 = new List<Move>() { };
        var a = new List<Move>(){};
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            if (board.IsInCheckmate())
            {
                a.Add(move);
            }
            if (move.IsCapture) { capture.Add(move); }
            else if (board.IsInCheck()) { check.Add(move); }
            else { else1.Add(move); }
            board.UndoMove(move);
        }

        
        return a.Concat(capture).Concat(check).Concat(else1).ToArray();
    }
}
