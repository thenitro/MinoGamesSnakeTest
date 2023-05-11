using UnityEngine;

namespace TymurKoshel.Snake.Settings
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Snake/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        [SerializeField] private float timePerPixel;
        [SerializeField] private int initialSnakeSize;
        
        public float TimePerPixel => timePerPixel;
        public int InitialSnakeSize => initialSnakeSize;
    }
}