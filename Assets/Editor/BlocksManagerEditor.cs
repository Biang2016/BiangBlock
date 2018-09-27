using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BlocksManager))]
public class BlocksManagerEditor : Editor
{
    [DrawGizmo(GizmoType.Selected)]
    private void OnSceneGUI()
    {
        BlocksManager bm = target as BlocksManager;
        if (bm.Grid == null) return;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;

        for (int i = 0; i < bm.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < bm.Grid.GetLength(1); j++)
            {
                Vector3 offset = (i - GameManager.Instance.Width / 2 - 0.5f) * Vector3.right * BlocksManager.Instance.InitSize + (j - GameManager.Instance.Height / 2 + 0.5f) * Vector3.up * BlocksManager.Instance.InitSize;
                if (bm.ShowGridColor) Handles.Label(bm.transform.position + offset, bm.Grid[i, j].ToString(), style);
                if (bm.ShowGridIndex) Handles.Label(bm.transform.position + offset, i + "," + j, style);
            }
        }
    }
}