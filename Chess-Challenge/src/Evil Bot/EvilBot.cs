using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 1, 3, 3, 5, 10, 100 };
    Random rng = new();
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Move moveToPlay = moves[rng.Next(moves.Length)];
        int highestValue = 0;
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            if (board.IsInCheckmate())
            {
                return move;
            }
            int Value = -Search(board, 2);

            if (Value >= highestValue)
            {
                highestValue = Value;
                moveToPlay = move;
            }
            board.UndoMove(move);
        }
        return moveToPlay;
    }
    int Search(Board board, int iteration)
    {
        Move[] moves = board.GetLegalMoves();

        if (iteration == 0)
        {
            return CountMaterial(board);
        }

        int HighestValue = int.MinValue;
        if (moves.Length == 0)
        {
            if (board.IsInCheck())
            {
                return int.MinValue;
            }
            return 0;
        }
        foreach (Move move in moves)
        {
            // Find highest value capture
            board.MakeMove(move);
            int Value = -Search(board, iteration - 1);
            board.UndoMove(move);
            HighestValue = Math.Max(HighestValue, Value);
        }

        return HighestValue;
    }
    int CountMaterial(Board board)
    {
        bool side = board.IsWhiteToMove;
        int Material = 0;
        for (int i = 0; i < 64; i += 1)
        {
            Piece a = board.GetPiece(new Square(i));
            Material += pieceValues[(int)a.PieceType] * (a.IsWhite ? 1 : -1) * (side ? 1 : -1);
        }
        Material += (side ? 1 : -1) * (board.IsInCheck() ? 1 : 0);
        Material += (side ? 1 : -1) * (board.IsInCheckmate() ? int.MaxValue : 0);
        return Material;
    }
}
}