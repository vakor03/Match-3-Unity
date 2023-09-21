using TMPro;
using UnityEngine;

namespace Match3._Scripts.Grid
{
    public static class Helper
    {
        public static TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 localPosition, Vector3 direction,
            int fontSize = 2, Color color = default,
            TextAlignmentOptions textAlignmentOptions = TextAlignmentOptions.Center, int sortingOrder = 0)
        {
            GameObject gameObject = new GameObject($"DebugText_{text}", typeof(TextMeshPro));
            gameObject.transform.SetParent(parent.transform);
            gameObject.transform.position = localPosition;
            gameObject.transform.forward = direction;

            TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
            textMeshPro.text = text;
            textMeshPro.fontSize = fontSize;
            textMeshPro.color = color == default ? Color.white : color;
            textMeshPro.alignment = textAlignmentOptions;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMeshPro;
        }
    }
}