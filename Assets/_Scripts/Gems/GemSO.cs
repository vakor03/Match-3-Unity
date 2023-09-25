using UnityEngine;

namespace Match3._Scripts.Gems
{
    [CreateAssetMenu(menuName = "Create GemSO", fileName = "GemSO", order = 0)]
    public class GemSO : ScriptableObject
    {
        public string gemType;
        public Sprite sprite;
    }
}