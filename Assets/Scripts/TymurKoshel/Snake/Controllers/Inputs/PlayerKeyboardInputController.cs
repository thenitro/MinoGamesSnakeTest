using TymurKoshel.Snake.Controller;
using TymurKoshel.Snake.Controllers.Base;
using TymurKoshel.Snake.Settings;
using UnityEngine;

namespace TymurKoshel.Snake.Controllers.Inputs
{
    public class PlayerKeyboardInputController : IPlayerInputController
    {
        private PlayerInputMap inputMap;
        
        public PlayerKeyboardInputController(PlayerInputMap inputMap)
        {
            this.inputMap = inputMap;
        }

        public Direction GetDirection()
        {
            if (Input.GetKeyDown(inputMap.Left)) return Direction.Left;
            if (Input.GetKeyDown(inputMap.Right)) return Direction.Right;
            if (Input.GetKeyDown(inputMap.Up)) return Direction.Up;
            if (Input.GetKeyDown(inputMap.Down)) return Direction.Down;

            return Direction.None;
        }
    }
}