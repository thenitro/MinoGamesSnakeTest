using TymurKoshel.Snake.Controller;
using UnityEngine;

namespace TymurKoshel.Snake.Views
{
    public class LevelBackgroundView : MonoBehaviour
    {
        [SerializeField] private GameObject pixelPrefab;
        [SerializeField] private GameObject backgroundContainer;
        
        //really lazy implementation of the background
        public void Draw(VirtualScreenController screen)
        {
            for (var x = 0; x < screen.ScreenWidth; x++)
            {
                for (var y = 0; y < screen.ScreenHeight; y++)
                {
                    var position = new Vector3(x * screen.PixelSizeScalar, y * screen.PixelSizeScalar);
                    
                    Instantiate(pixelPrefab, position, Quaternion.identity, backgroundContainer.transform);
                }
            }

            transform.localPosition = new Vector3(-(screen.ScreenWidth / 2) * screen.PixelSizeScalar, -(screen.ScreenHeight / 2) * screen.PixelSizeScalar);
        }
    }
}