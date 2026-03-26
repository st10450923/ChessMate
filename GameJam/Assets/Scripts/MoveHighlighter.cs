using System.Collections.Generic;
using UnityEngine;

public class MoveHighlighter : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private GameObject captureHighlightPrefab;
    [SerializeField] private Transform highlightRoot;
    [SerializeField] private float zOffset = -0.1f;
    private List<GameObject> activeHighlights = new List<GameObject>();

    public void ShowMoves(List<Vector2Int> moves)
    {
        ClearMoves();

        foreach (Vector2Int pos in moves)
        {
            Vector3 worldPos = board.GridToWorld(pos.x, pos.y);
            worldPos.z += zOffset;

            GameObject prefab = GetHighlightPrefab(pos);
            GameObject highlight = Instantiate(prefab, worldPos, Quaternion.identity, highlightRoot);
            activeHighlights.Add(highlight);
        }
    }
    public void ClearMoves()

    {
        foreach (GameObject h in activeHighlights)
        {
            if (h != null)
                Destroy(h);
        }
        activeHighlights.Clear();
    }
    private GameObject GetHighlightPrefab(Vector2Int pos)
    {
        if (captureHighlightPrefab == null)
            return highlightPrefab;

        Piece target = board.GetPiece(pos);
        bool isCapture = target != null;

        return isCapture ? captureHighlightPrefab : highlightPrefab;
    }
}

