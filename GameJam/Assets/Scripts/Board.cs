using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Audio.ProcessorInstance;

public class Board : MonoBehaviour
{
    public const int SIZE = 8;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MoveResolver moveResolver;
    [SerializeField] private MoveHighlighter highlighter;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioClip MovePieceSFX;
    [SerializeField] private AudioClip TakePieceSFX;

    [SerializeField] private float squareSize = 1f;
    [SerializeField] private Vector3 boardOrigin = Vector3.zero;

    private Piece[,] grid = new Piece[SIZE, SIZE];
    private Piece selectedPiece;
    private List<Vector2Int> validMoves = new List<Vector2Int>();
    private Vector2Int? enPassantTarget = null;

    //Castling variables
    private bool whiteKingMoved = false;
    private bool blackKingMoved = false;
    private bool whiteRookAMoved = false; 
    private bool whiteRookHMoved = false; 
    private bool blackRookAMoved = false; 
    private bool blackRookHMoved = false;
    public bool CanCastleKingside(PlayerTeam team) =>
    team == PlayerTeam.White
        ? !whiteKingMoved && !whiteRookHMoved
        : !blackKingMoved && !blackRookHMoved;

    public bool CanCastleQueenside(PlayerTeam team) =>
        team == PlayerTeam.White
            ? !whiteKingMoved && !whiteRookAMoved
            : !blackKingMoved && !blackRookAMoved;


    public Vector2Int? EnPassantTarget => enPassantTarget;

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
    private void LogMove(Piece piece, Vector2Int from, Vector2Int to, bool wasCapture)
    {
        // Column letters a-h, row numbers 1-8
        string fromSquare = $"{(char)('a' + from.x)}{from.y + 1}";
        string toSquare = $"{(char)('a' + to.x)}{to.y + 1}";
        string capture = wasCapture ? "x" : "-";
        string pieceName = PieceInitial(piece.ChessIdentity);

        string entry = $"{pieceName}{fromSquare}{capture}{toSquare}";
        uiManager?.AddMoveLogEntry(entry, piece.Team);
    }
    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    private string PieceInitial(ChessIdentity type) => type switch
    {
        ChessIdentity.King => "K",
        ChessIdentity.Queen => "Q",
        ChessIdentity.Rook => "R",
        ChessIdentity.Bishop => "B",
        ChessIdentity.Knight => "N",
        ChessIdentity.Pawn => "", 
        _ => ""
    };
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

            Deselect();
            return;
        }

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
        // only capture moves are legal
        if (gameManager.CurrentPhase == GamePhase.Checkers)
            EnforceCheckersMandatoryCapture(piece.Team);

        highlighter.ShowMoves(validMoves);
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
        highlighter.ClearMoves();
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
        Vector2Int origin = piece.GridPosition;

        if (gameManager.CurrentPhase == GamePhase.Checkers)
        {
            bool wasCapture = TryExecuteCheckersMove(piece, destination);
            Deselect();
            LogMove(piece, origin, destination, wasCapture);
            soundManager.PlaySFXClip(wasCapture ? TakePieceSFX : MovePieceSFX, transform, 1f);

            if (wasCapture)
            {
                TryMultiJump(piece);
                return;
            }
            gameManager.OnMoveComplete();
        }
        else
        {
            var (gameOver, wasCapture) = TryExecuteChessMove(piece, destination);
            Deselect();

            if (!gameOver)
            {
                LogMove(piece, origin, destination, wasCapture);
                soundManager.PlaySFXClip(wasCapture ? TakePieceSFX : MovePieceSFX, transform, 1f);
                gameManager.OnMoveComplete();
            }
        }
    }

    // Chess move
    private (bool gameOver, bool wasCapture) TryExecuteChessMove(Piece piece, Vector2Int destination)
    {
        int originCol = piece.Col;
        int originRow = piece.Row;
        // Store and clear en passant — only valid for one turn
        Vector2Int? lastEnPassantTarget = enPassantTarget;
        enPassantTarget = null;
        bool wasCapture = false;

        // Standard capture
        Piece target = GetPiece(destination);
        if (target != null)
        {
            wasCapture = true;
            bool isKing = target.ChessIdentity == ChessIdentity.King;
            RemovePiece(target);
            if (isKing)
            {
                gameManager.OnKingCaptured(piece.Team);
                return (true,true);
            }
        }

        // En passant capture — destination is empty, remove the pawn beside it
        if (piece.ChessIdentity == ChessIdentity.Pawn &&
            lastEnPassantTarget.HasValue &&
            destination == lastEnPassantTarget.Value)
        {
            int capturedRow = piece.Team == PlayerTeam.White
                ? destination.y - 1
                : destination.y + 1;

            Piece capturedPawn = GetPiece(destination.x, capturedRow);
            if (capturedPawn != null)
                RemovePiece(capturedPawn);
        }

        MovePieceOnGrid(piece, destination);
        // Castling — king moved two squares
        if (piece.ChessIdentity == ChessIdentity.King)
        {
            if (piece.Team == PlayerTeam.White) whiteKingMoved = true;
            else blackKingMoved = true;

            int colDiff = destination.x - originCol;
            if (Mathf.Abs(colDiff) == 2)
            {
                bool kingside = colDiff > 0;
                int rookFromCol = kingside ? 7 : 0;
                int rookToCol = kingside ? 5 : 3;
                int backRank = piece.Team == PlayerTeam.White ? 0 : 7;

                Piece rook = GetPiece(rookFromCol, backRank);
                if (rook != null)
                    MovePieceOnGrid(rook, new Vector2Int(rookToCol, backRank));
            }
        }

        // Track rook moves using stored origin column
        if (piece.ChessIdentity == ChessIdentity.Rook)
        {
            if (piece.Team == PlayerTeam.White)
            {
                if (originCol == 0) whiteRookAMoved = true;
                if (originCol == 7) whiteRookHMoved = true;
            }
            else
            {
                if (originCol == 0) blackRookAMoved = true;
                if (originCol == 7) blackRookHMoved = true;
            }
        }

        if (piece.ChessIdentity == ChessIdentity.Pawn)
        {
            // Pawn double step — set en passant target for opponent next turn
            int startRow = piece.Team == PlayerTeam.White ? 1 : 6;
            if (Mathf.Abs(destination.y - startRow) == 2)
            {
                int epRow = piece.Team == PlayerTeam.White ? 2 : 5;
                enPassantTarget = new Vector2Int(destination.x, epRow);
            }

            // Promotion
            int promotionRow = piece.Team == PlayerTeam.White ? SIZE - 1 : 0;
            if (destination.y == promotionRow)
                piece.PromoteToQueen();
        }

        return (false,wasCapture);
    }

    // Checkers move
    /// <summary>Returns true if this move was a capture.</summary>
    private bool TryExecuteCheckersMove(Piece piece, Vector2Int destination)
    {
        bool isCapture = IsCapture(piece, destination);

        if (isCapture)
        {
            int midCol = (piece.Col + destination.x) / 2;
            int midRow = (piece.Row + destination.y) / 2;
            Piece captured = GetPiece(midCol, midRow);
            if (captured != null)
            {
                bool isKing = captured.ChessIdentity == ChessIdentity.King;
                RemovePiece(captured);
                if (isKing)
                {
                    gameManager.OnKingCaptured(piece.Team);
                    return false;
                }
            }
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
            highlighter.ShowMoves(validMoves);
            piece.SetSelected(true);
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
        ResetCheckersKings();
        enPassantTarget = null; // en passant does not carry across a phase switch
    }
    public void ClearGrid()
    {
        for (int c = 0; c < SIZE; c++)
            for (int r = 0; r < SIZE; r++)
                grid[c, r] = null;

        whiteKingMoved = false;
        blackKingMoved = false;
        whiteRookAMoved = false;
        whiteRookHMoved = false;
        blackRookAMoved = false;
        blackRookHMoved = false;
        enPassantTarget = null;
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
