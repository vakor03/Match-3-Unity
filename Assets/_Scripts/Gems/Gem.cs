using UnityEngine;

namespace Match3._Scripts.Gems
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Gem : MonoBehaviour
    {
        private GemSO _gemSO;
        
        public void SetGemSO(GemSO gemSO)
        {
            _gemSO = gemSO;
            GetComponent<SpriteRenderer>().sprite = _gemSO.sprite;
        }
        
        public GemSO GetGemSO()
        {
            return _gemSO;
        }
    }
}