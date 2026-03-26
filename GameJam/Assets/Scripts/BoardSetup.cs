using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Transform piecesRoot;

    [SerializeField] private GameObject whitePawnPrefab;
    [SerializeField] private GameObject whiteRookPrefab;
    [SerializeField] private GameObject whiteKnightPrefab;
    [SerializeField] private GameObject whiteBishopPrefab;
    [SerializeField] private GameObject whiteQueenPrefab;
    [SerializeField] private GameObject whiteKingPrefab;

    [SerializeField] private GameObject blackPawnPrefab;
    [SerializeField] private GameObject blackRookPrefab;
    [SerializeField] private GameObject blackKnightPrefab;
    [SerializeField] private GameObject blackBishopPrefab;
    [SerializeField] private GameObject blackQueenPrefab;
    [SerializeField] private GameObject blackKingPrefab;
    void Start()
    {
        SpawnAllPieces();
    }
    public void Restart()
    {
        ClearAllPieces();
        board.ClearGrid();
        SpawnAllPieces();
    }
    private void SpawnAllPieces()
    {
        SpawnWhitePieces();
        SpawnBlackPieces();
    }
    private void SpawnWhitePieces()
    {
        // Back rank (row 0)
        SpawnPiece(whiteRookPrefab, ChessIdentity.Rook, PlayerTeam.White, 0, 0);
        SpawnPiece(whiteKnightPrefab, ChessIdentity.Knight, PlayerTeam.White, 1, 0);
        SpawnPiece(whiteBishopPrefab, ChessIdentity.Bishop, PlayerTeam.White, 2, 0);
        SpawnPiece(whiteQueenPrefab, ChessIdentity.Queen, PlayerTeam.White, 3, 0);
        SpawnPiece(whiteKingPrefab, ChessIdentity.King, PlayerTeam.White, 4, 0);
        SpawnPiece(whiteBishopPrefab, ChessIdentity.Bishop, PlayerTeam.White, 5, 0);
        SpawnPiece(whiteKnightPrefab, ChessIdentity.Knight, PlayerTeam.White, 6, 0);
        SpawnPiece(whiteRookPrefab, ChessIdentity.Rook, PlayerTeam.White, 7, 0);

        // Pawn rank (row 1)
        for (int col = 0; col < Board.SIZE; col++)
            SpawnPiece(whitePawnPrefab, ChessIdentity.Pawn, PlayerTeam.White, col, 1);
    }
    private void SpawnBlackPieces()
    {
        // Back rank (row 7)
        SpawnPiece(blackRookPrefab, ChessIdentity.Rook, PlayerTeam.Black, 0, 7);
        SpawnPiece(blackKnightPrefab, ChessIdentity.Knight, PlayerTeam.Black, 1, 7);
        SpawnPiece(blackBishopPrefab, ChessIdentity.Bishop, PlayerTeam.Black, 2, 7);
        SpawnPiece(blackQueenPrefab, ChessIdentity.Queen, PlayerTeam.Black, 3, 7);
        SpawnPiece(blackKingPrefab, ChessIdentity.King, PlayerTeam.Black, 4, 7);
        SpawnPiece(blackBishopPrefab, ChessIdentity.Bishop, PlayerTeam.Black, 5, 7);
        SpawnPiece(blackKnightPrefab, ChessIdentity.Knight, PlayerTeam.Black, 6, 7);
        SpawnPiece(blackRookPrefab, ChessIdentity.Rook, PlayerTeam.Black, 7, 7);

        // Pawn rank (row 6)
        for (int col = 0; col < Board.SIZE; col++)
            SpawnPiece(blackPawnPrefab, ChessIdentity.Pawn, PlayerTeam.Black, col, 6);
    }
    private void SpawnPiece(
        GameObject prefab,
        ChessIdentity type,
        PlayerTeam team,
        int col, int row)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"BoardSetup: prefab for {team} {type} is not assigned.");
            return;
        }

        Vector3 worldPos = board.GridToWorld(col, row);
        GameObject go = Instantiate(prefab, worldPos, Quaternion.identity, piecesRoot);
        go.name = $"{team}_{type}_{col}{row}";

        Piece piece = go.GetComponent<Piece>();
        if (piece == null)
        {
            Debug.LogError($"BoardSetup: prefab {prefab.name} has no Piece component.");
            Destroy(go);
            return;
        }

        piece.Initialise(type, team, col, row);
        board.PlacePiece(piece, col, row);
    }
    private void ClearAllPieces()
    {
        foreach (Transform child in piecesRoot)
            Destroy(child.gameObject);
    }
}
