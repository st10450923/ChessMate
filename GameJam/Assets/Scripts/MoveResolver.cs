using UnityEngine;
using System.Collections.Generic;

public class MoveResolver : MonoBehaviour
{
    /// <summary>
    /// Returns all valid destination squares for the given piece in the
    /// current phase. Does not enforce mandatory capture — Board handles that
    /// after calling this method.
    /// </summary>
    public List<Vector2Int> GetValidMoves(Piece piece, Board board, GamePhase phase)
    {
        return phase == GamePhase.Chess
            ? GetChessMoves(piece, board)
            : GetCheckersMoves(piece, board);
    }
    public List<Vector2Int> GetCapturesOnly(Piece piece, Board board)
    {
        List<Vector2Int> all = GetCheckersMoves(piece, board);
        all.RemoveAll(dest =>
        {
            int dc = Mathf.Abs(dest.x - piece.Col);
            int dr = Mathf.Abs(dest.y - piece.Row);
            return !(dc == 2 && dr == 2);
        });
        return all;
    }

    private List<Vector2Int> GetCheckersMoves(Piece piece, Board board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // Regular pieces move forward only; kings move in all four diagonals.
        // "Forward" for White = increasing row; for Black = decreasing row.
        int[] rowDirs = GetCheckersRowDirections(piece);
        int[] colDirs = { -1, 1 };

        foreach (int rd in rowDirs)
        {
            foreach (int cd in colDirs)
            {
                int stepCol = piece.Col + cd;
                int stepRow = piece.Row + rd;

                if (!board.InBounds(stepCol, stepRow)) continue;

                if (board.IsEmpty(stepCol, stepRow))
                {
                    // Simple diagonal step
                    moves.Add(new Vector2Int(stepCol, stepRow));
                }
                else if (board.IsEnemy(stepCol, stepRow, piece.Team))
                {
                    // Potential jump — check the landing square
                    int landCol = stepCol + cd;
                    int landRow = stepRow + rd;

                    if (board.InBounds(landCol, landRow) &&
                        board.IsEmpty(landCol, landRow))
                    {
                        moves.Add(new Vector2Int(landCol, landRow));
                    }
                }
            }
        }

        return moves;
    }
    private int[] GetCheckersRowDirections(Piece piece)
    {
        if (piece.IsCheckersKing)
            return new int[] { 1, -1 };

        return piece.Team == PlayerTeam.White
            ? new int[] { 1 }   // White moves up the board (increasing row)
            : new int[] { -1 }; // Black moves down the board (decreasing row)
    }
    private List<Vector2Int> GetChessMoves(Piece piece, Board board)
    {
        // Route to the correct movement pattern for this piece type.
        // Promoted queens use Queen logic automatically since ChessIdentity
        // is updated to Queen on promotion.
        return piece.ChessIdentity switch
        {
            ChessIdentity.Pawn => GetPawnMoves(piece, board),
            ChessIdentity.Rook => GetSlidingMoves(piece, board, RookDirections()),
            ChessIdentity.Bishop => GetSlidingMoves(piece, board, BishopDirections()),
            ChessIdentity.Queen => GetSlidingMoves(piece, board, QueenDirections()),
            ChessIdentity.Knight => GetKnightMoves(piece, board),
            ChessIdentity.King => GetKingMoves(piece, board),
            _ => new List<Vector2Int>()
        };
    }
    private List<Vector2Int> GetPawnMoves(Piece piece, Board board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // White moves up (+row), Black moves down (-row)
        int forward = piece.Team == PlayerTeam.White ? 1 : -1;
        int startRow = piece.Team == PlayerTeam.White ? 1 : 6;

        int col = piece.Col;
        int row = piece.Row;

        // --- Single step forward ---
        if (board.InBounds(col, row + forward) &&
            board.IsEmpty(col, row + forward))
        {
            moves.Add(new Vector2Int(col, row + forward));

            // --- Double step from starting row ---
            if (row == startRow &&
                board.IsEmpty(col, row + forward * 2))
            {
                moves.Add(new Vector2Int(col, row + forward * 2));
            }
        }

        // --- Diagonal captures ---
        foreach (int dc in new int[] { -1, 1 })
        {
            int captureCol = col + dc;
            int captureRow = row + forward;

            if (board.InBounds(captureCol, captureRow) &&
                board.IsEnemy(captureCol, captureRow, piece.Team))
            {
                moves.Add(new Vector2Int(captureCol, captureRow));
            }
        }

        // Note: En passant is out of scope 
        return moves;
    }
    private List<Vector2Int> GetKnightMoves(Piece piece, Board board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // All 8 possible L-shaped offsets
        int[,] offsets =
        {
            { -2, -1 }, { -2,  1 },
            { -1, -2 }, { -1,  2 },
            {  1, -2 }, {  1,  2 },
            {  2, -1 }, {  2,  1 }
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int destCol = piece.Col + offsets[i, 0];
            int destRow = piece.Row + offsets[i, 1];

            if (!board.InBounds(destCol, destRow)) continue;

            // Knights can land on empty squares or enemy squares 
            if (board.IsEmpty(destCol, destRow) ||
                board.IsEnemy(destCol, destRow, piece.Team))
            {
                moves.Add(new Vector2Int(destCol, destRow));
            }
        }

        return moves;
    }

    private List<Vector2Int> GetKingMoves(Piece piece, Board board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        for (int dc = -1; dc <= 1; dc++)
        {
            for (int dr = -1; dr <= 1; dr++)
            {
                if (dc == 0 && dr == 0) continue;

                int destCol = piece.Col + dc;
                int destRow = piece.Row + dr;

                if (!board.InBounds(destCol, destRow)) continue;

                if (board.IsEmpty(destCol, destRow) ||
                    board.IsEnemy(destCol, destRow, piece.Team))
                {
                    moves.Add(new Vector2Int(destCol, destRow));
                }
            }
        }

        // Note: Castling is out of scope
        return moves;
    }
    private List<Vector2Int> GetSlidingMoves(
       Piece piece, Board board, Vector2Int[] directions)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (Vector2Int dir in directions)
        {
            int col = piece.Col + dir.x;
            int row = piece.Row + dir.y;

            while (board.InBounds(col, row))
            {
                if (board.IsEmpty(col, row))
                {
                    moves.Add(new Vector2Int(col, row));
                }
                else if (board.IsEnemy(col, row, piece.Team))
                {
                    moves.Add(new Vector2Int(col, row)); // capture
                    break; // can't slide through a piece
                }
                else
                {
                    break; // blocked by friendly piece
                }

                col += dir.x;
                row += dir.y;
            }
        }

        return moves;
    }
    private static Vector2Int[] RookDirections() => new Vector2Int[]
  {
        Vector2Int.up, Vector2Int.down,
        Vector2Int.left, Vector2Int.right
  };

    private static Vector2Int[] BishopDirections() => new Vector2Int[]
    {
        new Vector2Int( 1,  1),
        new Vector2Int( 1, -1),
        new Vector2Int(-1,  1),
        new Vector2Int(-1, -1)
    };

    private static Vector2Int[] QueenDirections()
    {
        // Queen = Rook + Bishop
        var dirs = new List<Vector2Int>();
        dirs.AddRange(RookDirections());
        dirs.AddRange(BishopDirections());
        return dirs.ToArray();
    }
}
