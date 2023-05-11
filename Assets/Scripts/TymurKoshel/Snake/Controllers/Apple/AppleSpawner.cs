using TymurKoshel.Snake.Controller;
using TymurKoshel.Snake.Controllers.Base;
using UnityEngine;

namespace TymurKoshel.Snake.Controllers.Apples
{
    public class AppleSpawner : ICleanable
    {
        private VirtualScreenController screen;
        private GameObject applePrefab;
        private GameObject appleContainer;

        private AppleController currentApple;
        
        public AppleSpawner(VirtualScreenController virtualScreen, GameObject applePrefab, GameObject appleContainer)
        {
            screen = virtualScreen;
            this.applePrefab = applePrefab;
            this.appleContainer = appleContainer;
        }

        public void Start()
        {
            GenerateApple();
        }

        private void GenerateApple()
        {
            var indexWithoutSnake = screen.GetFreeRandomIndex();
            
            var position = new Vector3(
                screen.PixelSizeScalar * indexWithoutSnake.X, 
                screen.PixelSizeScalar * indexWithoutSnake.Y);
            
            var apple = GameObject.Instantiate(applePrefab, appleContainer.transform);
            apple.transform.localPosition = position;
            apple.transform.localScale = screen.PixelSizeVector;

            currentApple = apple.GetComponent<AppleController>();
            currentApple.Destroyed += OnAppleDestroyed;

            screen.SetApple(position, apple);
        }

        private void OnAppleDestroyed(AppleController appleController)
        {
            appleController.Destroyed -= OnAppleDestroyed;
            GenerateApple();
        }

        public void Clean()
        {
            if (currentApple == null)
            {
                return;
            }
            
            screen.ClearApple(currentApple.gameObject);
            currentApple.Destroyed -= OnAppleDestroyed;
            GameObject.Destroy(currentApple.gameObject);
            currentApple = null;
        }
    }
}