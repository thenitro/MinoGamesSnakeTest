using System;
using UnityEngine;

namespace TymurKoshel.Snake.Controllers.Apples
{
    public class AppleController : MonoBehaviour
    {
        public Action<AppleController> Destroyed;

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}