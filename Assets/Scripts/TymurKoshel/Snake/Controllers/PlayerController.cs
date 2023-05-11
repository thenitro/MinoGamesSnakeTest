using System;
using System.Collections.Generic;
using System.Linq;
using TymurKoshel.Snake.Controllers.Base;
using TymurKoshel.Snake.Controllers.Inputs;
using TymurKoshel.Snake.Settings;
using UnityEngine;
using Index = TymurKoshel.Snake.Controllers.Base.Index;

namespace TymurKoshel.Snake.Controller
{
    public class PlayerController : MonoBehaviour, ICleanable
    {
        private static readonly Dictionary<Direction, Vector3> DirectionMap = new Dictionary<Direction, Vector3>()
        {
            { Direction.Left, Vector3.left },
            { Direction.Right, Vector3.right },
            { Direction.Up, Vector3.up },
            { Direction.Down, Vector3.down },
        };

        [SerializeField] private GameObject pixelPrefab;
        
        public Action<PlayerController> OnDeath;
        public Action OnPixelAdded;
        
        public int PixelsAmount => body.Count;
        
        private VirtualScreenController screen;
        private PlayerSettings playerSettings;
        private IPlayerInputController inputController;

        private Direction direction;
        private float timeStamp;
        
        private Queue<GameObject> body = new Queue<GameObject>();
        private Queue<Vector3> positions = new Queue<Vector3>();
        
        private bool alive = false;
        private bool collisionOnNextMove = false;

        private Color playerColor;

        public void Init(VirtualScreenController screen, PlayerSettings playerSettings, IPlayerInputController inputController, Color playerColor)
        {
            this.screen = screen;
            this.playerSettings = playerSettings;
            this.inputController = inputController;
            this.playerColor = playerColor;

            CreateInitialBody();

            alive = true;
        }
        
        public void Clean()
        {
            foreach (var pixel in body)
            {
                Destroy(pixel);
            }

            body = null;
            screen.ClearSnake(gameObject);
            
            Destroy(gameObject);
        }

        private void Update()
        {
            if (!alive) return;
            
            direction = UpdateDirection(inputController.GetDirection());

            if (!IsTimeToUpdate())
            {
                return;
            }
            
            var pixelsPositions = positions.ToArray();

            var headPosition = pixelsPositions[^1];
            var headIndex = screen.ConvertToIndex(headPosition);

            if (IsDead(headIndex))
            {
                return;
            }

            var isEatApple = IsEatApple(headIndex);
            if (!isEatApple)
            {
                positions.Dequeue();
            }

            var nextHeadMovePosition = headPosition + DirectionMap[direction] * screen.PixelSizeScalar;
            var nextMoveHeadIndex = screen.ConvertToIndex(nextHeadMovePosition);

            collisionOnNextMove = screen.GetSnake(nextMoveHeadIndex) != null; 
            
            positions.Enqueue(nextHeadMovePosition);

            var pixels = body.ToArray();
            pixelsPositions = positions.ToArray();
            
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i].transform.localPosition = pixelsPositions[pixelsPositions.Length - i - 1];
            }

            UpdateScreen(pixelsPositions);
        }

        private Direction UpdateDirection(Direction inputDirection)
        {
            if (inputDirection == Direction.Right && direction == Direction.Left) return direction;
            if (inputDirection == Direction.Left && direction == Direction.Right) return direction;
            if (inputDirection == Direction.Up && direction == Direction.Down) return direction;
            if (inputDirection == Direction.Down && direction == Direction.Up) return direction;
            
            return inputDirection == Direction.None ? direction : inputDirection;
        }

        private bool IsTimeToUpdate()
        {
            var currentTime = Time.realtimeSinceStartup; 
            var diff = currentTime - timeStamp;
            if (diff < playerSettings.TimePerPixel)
            {
                return false;
            }
           
            timeStamp = currentTime;
            
            return true;
        }
        
        private bool IsDead(Index headIndex)
        {
            if (screen.IsInBounds(headIndex) && !collisionOnNextMove)
            {
                return false;
            }
            
            OnDeath.Invoke(this);
            alive = false;
            
            return true;
        }

        private bool IsEatApple(Index headIndex)
        {
            var appleCollision = screen.GetApple(headIndex);
            if (appleCollision == null)
            {
                return false;
            }
            
            var newPixel = CreatePixel();
            body.Enqueue(newPixel);
                
            Destroy(appleCollision);
            screen.ClearApple(appleCollision);
                
            OnPixelAdded.Invoke();

            return true;
        }
        
        private void UpdateScreen(Vector3[] pixelsPositions)
        {
            screen.ClearSnake(gameObject);

            foreach (var pixelsPosition in pixelsPositions)
            {
                screen.SetSnake(pixelsPosition, gameObject);
            }
        }
        
        private void CreateInitialBody()
        {
            var spawnIndexes = screen.GetFreeContiguousIndexes(playerSettings.InitialSnakeSize);
            
            foreach (var spawnIndex in spawnIndexes)
            {
                var pixel = CreatePixel();
                pixel.transform.localPosition = new Vector3(spawnIndex.X * screen.PixelSizeScalar, spawnIndex.Y * screen.PixelSizeScalar);
                
                body.Enqueue(pixel);
                positions.Enqueue(pixel.transform.localPosition);
            }

            positions = new Queue<Vector3>(positions.Reverse());
            direction = spawnIndexes.Count < 2 ? Direction.Left : GetDirection(spawnIndexes[0], spawnIndexes[1]);
        }

        private Direction GetDirection(Index startIndex, Index nextIndex)
        {
            var dx = nextIndex.X - startIndex.X;
            var dy = nextIndex.Y - startIndex.Y;

            if (dx != 0)
            {
                return dx < 0 ? Direction.Right : Direction.Left;
            }

            if (dy != 0)
            {
                return dy < 0 ? Direction.Up : Direction.Down;
            }

            return Direction.Left;
        }

        //yeah, creating pixels each time is bad stuff, would add some Pooling here and reuse it
        private GameObject CreatePixel()
        {
            var result = Instantiate(pixelPrefab, Vector3.zero, Quaternion.identity, transform);
            result.transform.localScale = screen.PixelSizeVector;
            result.GetComponentInChildren<SpriteRenderer>().color = playerColor;
            return result;
        }
    }
}