using System.Collections.Generic;
using TymurKoshel.Snake.Controllers.Base;
using TymurKoshel.Snake.Settings;
using UnityEngine;

namespace TymurKoshel.Snake.Controller
{
    public class VirtualScreenController
    {
        /*
         * if we had a more entities than snakes and apples (like powerups, mines, else),
         * I would replace this two array's for something better
         */
        private readonly GameObject[,] snakeScreen;
        private readonly GameObject[,] appleScreen;
        
        private readonly VirtualScreenSettings settings;

        public Vector3 PixelSizeVector => new Vector3(settings.PixelSize, settings.PixelSize, settings.PixelSize);
        public float PixelSizeScalar => settings.PixelSize;
        
        public int ScreenWidth => settings.BoardWidth;
        public int ScreenHeight => settings.BoardHeight;
        public int ScreenSize { get; }

        public VirtualScreenController(VirtualScreenSettings settings)
        {
            this.settings = settings;
            snakeScreen = new GameObject[settings.BoardWidth, settings.BoardHeight];
            appleScreen = new GameObject[settings.BoardWidth, settings.BoardHeight];
            ScreenSize = settings.BoardWidth * settings.BoardHeight;
        }

        public Index ConvertToIndex(Vector3 position)
        {
            var indexX = (int)(position.x / PixelSizeScalar);
            var indexY = (int)(position.y / PixelSizeScalar);

            return new Index(indexX, indexY);
        }

        //we could save positions of snake and apples and save us a bit of a cpu time
        public void ClearSnake(GameObject owner)
        {
            for (var i = 0; i < ScreenWidth; i++)
            {
                for (var j = 0; j < ScreenHeight; j++)
                {
                    if (owner == snakeScreen[i, j])
                    {
                        snakeScreen[i, j] = null;
                    }
                }
            }
        }
        
        public void ClearApple(GameObject owner)
        {
            for (var i = 0; i < ScreenWidth; i++)
            {
                for (var j = 0; j < ScreenHeight; j++)
                {
                    if (owner == appleScreen[i, j])
                    {
                        appleScreen[i, j] = null;
                    }
                }
            }
        }

        public bool SetSnake(Vector3 position, GameObject owner)
        {
            var index = ConvertToIndex(position);
            if (!IsInBounds(index))
            {
                return false;
            }
            
            snakeScreen[index.X, index.Y] = owner;
            return true;
        }
        
        public bool SetApple(Vector3 position, GameObject owner)
        {
            var index = ConvertToIndex(position);
            if (!IsInBounds(index))
            {
                return false;
            }
            
            appleScreen[index.X, index.Y] = owner;
            return true;
        }

        public GameObject GetSnake(Index index)
        {
            if (!IsInBounds(index))
            {
                return null;
            }
            
            return snakeScreen[index.X, index.Y];
        }
        
        public GameObject GetSnake(int indexX, int indexY)
        {
            if (!IsInBounds(indexX, indexY))
            {
                return null;
            }
            
            return snakeScreen[indexX, indexY];
        }
        
        public GameObject GetApple(Index index)
        {
            if (!IsInBounds(index))
            {
                return null;
            }
            
            return appleScreen[index.X, index.Y];
        }
        
        public bool IsInBounds(Index index)
        {
            return IsInBounds(index.X, index.Y);
        }
        
        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < ScreenWidth && y >= 0 && y < ScreenHeight;
        }

        public List<Index> GetFreeContiguousIndexes(int amount)
        {
            var result = new List<Index>();
            var freeIndexes = GetFreeIndexes();

            ShuffleList(freeIndexes);

            var freeIndexesQueue = new Queue<Index>(freeIndexes);

            while (freeIndexesQueue.Count > 0)
            {
                var randomIndex = freeIndexesQueue.Dequeue();
                
                var nodesToProcess = new Stack<Index>();
                    nodesToProcess.Push(randomIndex);

                var visitedNodes = new bool[ScreenWidth, ScreenHeight];
                
                var currentAmount = amount;
                result.Clear();
                
                while (nodesToProcess.Count > 0 || currentAmount > 0)
                {
                    var index = nodesToProcess.Pop();
                    
                    if (!IsInBounds(index) || visitedNodes[index.X, index.Y])
                    {
                        continue;
                    }
                    
                    if (GetSnake(index) == null)
                    {
                        currentAmount--;
                        result.Add(index);
                    }
                    
                    if (currentAmount == 0)
                    {
                        return result;
                    }
                    
                    nodesToProcess.Push(new Index(index.X, index.Y - 1));
                    nodesToProcess.Push(new Index(index.X, index.Y + 1));
                    nodesToProcess.Push(new Index(index.X + 1, index.Y));
                    nodesToProcess.Push(new Index(index.X - 1, index.Y));
                }
            }

            return result;
        }

        private void ShuffleList(List<Index> list)
        {
            var random = new System.Random();
            var itemsLeft = list.Count;

            while (itemsLeft > 1)
            {
                var newPosition = random.Next(itemsLeft--);
                (list[newPosition], list[itemsLeft]) = (list[itemsLeft], list[newPosition]);
            }
        }

        public Index GetFreeRandomIndex()
        {
            var freeIndexes = GetFreeIndexes(); 
            return GetFreeRandomIndex(freeIndexes);
        }

        private Index GetFreeRandomIndex(List<Index> freeIndexes)
        {
            return freeIndexes[Random.Range(0, freeIndexes.Count)];
        }

        private List<Index> GetFreeIndexes()
        {
            var freeIndexes = new List<Index>();

            for (var x = 0; x < ScreenWidth; x++)
            {
                for (var y = 0; y < ScreenHeight; y++)
                {
                    if (GetSnake(x, y) == null)
                    {
                        freeIndexes.Add(new Index(x, y));
                    }
                }
            }

            return freeIndexes;
        }
    }
}