using UnityEngine;

namespace TymurKoshel.Snake.Settings
{
    [CreateAssetMenu(fileName = "VirtualScreenSettings", menuName = "Snake/VirtualScreenSettings")]
    public class VirtualScreenSettings : ScriptableObject
    {
        [SerializeField] private float pixelSize;
        [SerializeField] private int boardWidth;
        [SerializeField] private int boardHeight;
        
        public float PixelSize => pixelSize;
        public int BoardWidth => boardWidth;
        public int BoardHeight => boardHeight;
    }
}
