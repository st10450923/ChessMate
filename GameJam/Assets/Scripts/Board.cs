using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;

public class Board : MonoBehaviour
{
    public const int SIZE = 8;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MoveResolver moveResolver;
    [SerializeField] private MoveHighlighter highlighter;

    [SerializeField] private float squareSize = 1f;
    [SerializeField] private Vector3 boardOrigin = Vector3.zero;

    private Piece[,] grid = new Piece[SIZE, SIZE];
    private Piece selectedPiece;
    private List<Vector2Int> validMoves = new List<Vector2Int>();
    public Piece GetPiece(int col, int row)
    {
        if (!InBounds(col, row)) return null;
        return grid[col, row];
    }
    public Piece GetPiece(Vector2Int pos) => GetPiece(pos.x, pos.y);

    public bool InBounds(int col, int row) =>
        col >= 0 && col < SIZE && row >= 0 && row < SIZE;
    public bool InBounds(Vector2Int pos) => InBounds(pos.x, pos.y);

    public bool IsEmpty(int col, int row) =>
    InBounds(col, row) && grid[col, row] == null;

    public bool IsEmpty(Vector2Int pos) => IsEmpty(pos.x, pos.y);

    public bool IsEnemy(int col, int row, PlayerTeam myTeam)
    {
        Piece p = GetPiece(col, row);
        return p != null && p.Team != myTeam;
    }

    public bool IsEnemy(Vector2Int pos, PlayerTeam myTeam) =>
        IsEnemy(pos.x, pos.y, myTeam);

    public void PlacePiece(Piece piece, int col, int row)
    {
        if (!InBounds(col, row))
        {
            Debug.LogWarning($"PlacePiece: ({col},{row}) is out of bounds.");
            return;
        }
        if (grid[col, row] != null)
            Debug.LogWarning($"PlacePiece: overwriting existing piece at ({col},{row}).");

        grid[col, row] = piece;
        piece.SetGridPosition(col, row);
        piece.transform.position = GridToWorld(col, row);
    }
    public void OnSquareClicked(int col, int row)
    {
        if (gameManager.InputLocked) return;

        Piece clickedPiece = GetPiece(col, row);
        Vector2Int clickedPos = new Vector2Int(col, row);

        // --- Case 1: A friendly piece is already selected ---
        if (selectedPiece != null)
        {
            // Is the click a valid destination?
            if (validMoves.Contains(clickedPos))
            {
                ExecuteMove(selectedPiece, clickedPos);
                return;
            }

            // Clicked a different friendly piece — switch selection
            if (clickedPiece != null && clickedPiece.Team == gameManager.CurrentTurn)
            {
                SelectPiece(clickedPiece);
                return;
            }

            // Clicked empty or enemy square that isn't a valid move — deselect
            Deselect();
            return;
        }

        // --- Case 2: Nothing selected yet ---
        if (clickedPiece != null && clickedPiece.Team == gameManager.CurrentTurn)
        {
            SelectPiece(clickedPiece);
        }
    }
    public void OnRightClick()
    {
        Deselect();
    }
    private void SelectPiece(Piece piece)
    {
        Deselect();
        selectedPiece = piece;
        validMoves = moveResolver.GetValidMoves(piece, this, gameManager.CurrentPhase);

        // Checkers: if any capture is available for the current player,
        // only capture moves are legal (mandatory capture rule)
        if (gameManager.CurrentPhase == GamePhase.Checkers)
            EnforceCheckersMandatoryCapture(piece.Team);

        //highlighter.ShowMoves(validMoves);
        piece.SetSelected(true);
    }

    private void Deselect()
    {
        if (selectedPiece != null)
        {
            selectedPiece.SetSelected(false);
            selectedPiece = null;
        }
        validMoves.Clear();
        //highlighter.ClearMoves();
    }

    private void EnforceCheckersMandatoryCapture(PlayerTeam team)
    {
        bool anyCaptureAvailable = false;

        for (int c = 0; c < SIZE; c++)
        {
            for (int r = 0; r < SIZE; r++)
            {
                Piece p = grid[c, r];
                if (p == null || p.Team != team) continue;

                List<Vector2Int> moves =
                    moveResolver.GetValidMoves(p, this, GamePhase.Checkers);

                foreach (var move in moves)
                {
                    if (IsCapture(p, move))
                    {
                        anyCaptureAvailable = true;
                        break;
                    }
                }
                if (anyCaptureAvailable) break;
            }
            if (anyCaptureAvailable) break;
        }

        if (anyCaptureAvailable)
        {
            // Strip non-captures from validMoves
            validMoves.RemoveAll(move => !IsCapture(selectedPiece, move));
        }
    }

    private bool IsCapture(Piece piece, Vector2Int destination)
    {
        int dc = Mathf.Abs(destination.x - piece.Col);
        int dr = Mathf.Abs(destination.y - piece.Row);
        return dc == 2 && dr == 2;
    }

    private void ExecuteMove(Piece piece, Vector2Int destination)
    {
        bool wasCapture = false;

        if (gameManager.CurrentPhase == GamePhase.Checkers)
        {
            wasCapture = TryExecuteCheckersMove(piece, destination);
        }
        else
        {
            // TryExecuteChessMove returns true if the game is already over
            // (king captured) so we don't call OnMoveComplete in that case
            bool gameOver = TryExecuteChessMove(piece, destination);
            Deselect();
            if (!gameOver)
                gameManager.OnMoveComplete();
            return;
        }

        Deselect();

        if (wasCapture)
        {
            TryMultiJump(piece);
            return;
        }

        gameManager.OnMoveComplete();
    }

    // Chess move
    private bool TryExecuteChessMove(Piece piece, Vector2Int destination)
    {
        Piece target = GetPiece(destination);
        if (target != null)
        {
            bool isKing = target.ChessIdentity == ChessIdentity.King;
            RemovePiece(target);
            if (isKing)
            {
                gameManager.OnKingCaptured(piece.Team);
                return true; // game over, don't advance turn
            }
        }

        MovePieceOnGrid(piece, destination);

        if (piece.ChessIdentity == ChessIdentity.Pawn)
        {
            int promotionRow = (piece.Team == PlayerTeam.White) ? SIZE - 1 : 0;
            if (destination.y == promotionRow)
                piece.PromoteToQueen();
        }

        return false;
    }

    // Checkers move
    /// <summary>Returns true if this move was a capture.</summary>
    private bool TryExecuteCheckersMove(Piece piece, Vector2Int destination)
    {
        bool isCapture = IsCapture(piece, destination);

        if (isCapture)
        {
            // The jumped-over square is the midpoint
            int midCol = (piece.Col + destination.x) / 2;
            int midRow = (piece.Row + destination.y) / 2;
            Piece captured = GetPiece(midCol, midRow);
            if (captured != null) RemovePiece(captured);
        }

        MovePieceOnGrid(piece, destination);

        // Checkers kinging: reached opponent's back rank
        int kingRow = (piece.Team == PlayerTeam.White) ? SIZE - 1 : 0;
        if (destination.y == kingRow && !piece.IsCheckersKing)
            piece.SetCheckersKing(true);

        return isCapture;
    }

    // Multi-jump (checkers)
    // -------------------------------------------------------------------------

    /// <summary>
    /// After a capture, check if the same piece can jump again from its new
    /// position. If yes, force-select it and wait for player input.
    /// If no further jumps, end the turn normally.
    /// </summary>
    private void TryMultiJump(Piece piece)
    {
        List<Vector2Int> jumps =
            moveResolver.GetCapturesOnly(piece, this);

        if (jumps.Count > 0)
        {
            // Force re-select the same piece — player must continue jumping
            gameManager.SetInputLocked(false);
            selectedPiece = piece;
            validMoves = jumps;
            //highlighter.ShowMoves(validMoves);
            piece.SetSelected(true);
            // Do NOT call gameManager.OnMoveComplete — turn continues
        }
        else
        {
            gameManager.OnMoveComplete();
        }
    }

    public void MovePieceOnGrid(Piece piece, Vector2Int destination)
    {
        grid[piece.Col, piece.Row] = null;
        grid[destination.x, destination.y] = piece;
        piece.SetGridPosition(destination.x, destination.y);
        piece.AnimateTo(GridToWorld(destination.x, destination.y));
    }

    public void RemovePiece(Piece piece)
    {
        grid[piece.Col, piece.Row] = null;
        piece.OnCaptured(); // plays capture animation, then destroys self
    }

    public Vector3 GridToWorld(int col, int row)
    {
        float offset = (Board.SIZE - 1) * squareSize * 0.5f;
        return boardOrigin
            + new Vector3(col * squareSize - offset, row * squareSize - offset, 0f);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float offset = (Board.SIZE - 1) * squareSize * 0.5f;
        Vector3 local = worldPos - boardOrigin;
        int col = Mathf.RoundToInt((local.x + offset) / squareSize);
        int row = Mathf.RoundToInt((local.y + offset) / squareSize);
        return new Vector2Int(col, row);
    }

    // Phase switch support
    /// <summary>
    /// Called by SwitchController before the phase toggles.
    /// Clears any active selection so the new phase starts clean.
    /// Also resets temporary checkers-king status on all pieces.
    /// </summary>
    public void OnPhaseWillSwitch()
    {
        Deselect();
        //ResetCheckersKings();
    }

    private void ResetCheckersKings()
    {
        for (int c = 0; c < SIZE; c++)
            for (int r = 0; r < SIZE; r++)
                if (grid[c, r] != null)
                    grid[c, r].SetCheckersKing(false);
    }
    // Win condition helpers

    /// <summary>
    /// Returns true if the given team has no valid moves in the current phase.
    /// Used for checkers stalemate detection.
    /// </summary>
    public bool HasNoValidMoves(PlayerTeam team)
    {
        for (int c = 0; c < SIZE; c++)
        {
            for (int r = 0; r < SIZE; r++)
            {
                Piece p = grid[c, r];
                if (p == null || p.Team != team) continue;
                var moves = moveResolver.GetValidMoves(p, this, gameManager.CurrentPhase);
                if (moves.Count > 0) return false;
            }
        }
        return true;
    }

    public int GetPieceCount(PlayerTeam team)
    {
        int count = 0;
        for (int c = 0; c < SIZE; c++)
            for (int r = 0; r < SIZE; r++)
                if (grid[c, r] != null && grid[c, r].Team == team)
                    count++;
        return count;
    }
}
