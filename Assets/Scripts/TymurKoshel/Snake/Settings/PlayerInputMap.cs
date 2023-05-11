using UnityEngine;

namespace TymurKoshel.Snake.Settings
{
    [CreateAssetMenu(fileName = "PlayerInputMap", menuName = "Snake/PlayerInputMap")]
    public class PlayerInputMap : ScriptableObject
    {
        [SerializeField] private KeyCode left;
        [SerializeField] private KeyCode right;
        [SerializeField] private KeyCode up;
        [SerializeField] private KeyCode down;

        public KeyCode Left => left;
        public KeyCode Right => right;
        public KeyCode Up => up;
        public KeyCode Down => down;
    }
}