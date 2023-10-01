using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;
namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
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
        // Console.WriteLine(highestValue);
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
        return Material;
    }

    // int[] pieceValues = {0,100,300,350,500,900,10000};
    // Random rng = new();

    // public Move Think(Board board, Timer timer)
    // {
    //     Move[] moves = board.GetLegalMoves();
    //     Move moveToPlay = moves[rng.Next(moves.Length)];
    //     int highestValue = int.MinValue;
    //     foreach (Move move in moves)
    //     {
    //         board.MakeMove(move);
    //         if (board.IsInCheckmate())
    //         {
    //             return move;
    //         }
    //         int Value = -Search(board, 2);

    //         if (Value >= highestValue)
    //         {
    //             highestValue = Value;
    //             moveToPlay = move;
    //         }
    //         board.UndoMove(move);
    //     }
    //     return moveToPlay;
    // }
    // int Search(Board board, int iteration)
    // {
    //     Move[] moves = board.GetLegalMoves();

    //     if (iteration == 0)
    //     {
    //         return CountMaterial(board);
    //     }

    //     int HighestValue = int.MinValue;
    //     if (moves.Length == 0)
    //     {
    //         if (board.IsInCheck())
    //         {
    //             return int.MinValue;
    //         }
    //         return 0;
    //     }
    //     foreach (Move move in moves)
    //     {
    //         // Find highest value capture
    //         board.MakeMove(move);
    //         int Value = -Search(board, iteration - 1);
    //         board.UndoMove(move);
    //         HighestValue = Math.Max(HighestValue, Value);
    //     }

    //     return HighestValue;
    // }
    // int CountMaterial(Board board)
    // {
    //     bool side = board.IsWhiteToMove;
    //     int Material = 0;
    //     for (int i = 0; i < 64; i += 1)
    //     {
    //         Piece a = board.GetPiece(new Square(i));
    //         Material += pieceValues[(int)a.PieceType] * (a.IsWhite ? 1 : -1) * (side ? 1 : -1);
    //     }
    //     Material += (side ? 1 : -1) * (board.IsInCheck() ? 100 : 0);
    //     Material += (side ? 1 : -1) * (board.IsInCheckmate() ? int.MaxValue : 0);
    //     return Material;
    // }
}
}