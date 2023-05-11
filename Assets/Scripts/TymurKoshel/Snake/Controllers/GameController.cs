using System.Collections.Generic;
using System.Linq;
using TymurKoshel.Snake.Controllers.Apples;
using TymurKoshel.Snake.Controllers.Base;
using TymurKoshel.Snake.Controllers.Inputs;
using TymurKoshel.Snake.Settings;
using TymurKoshel.Snake.Views;
using UnityEngine;

namespace TymurKoshel.Snake.Controller
{
    /*
     * I don't quite like that almost everything is in MonoBehaviour's, it's a bit of a dull, I'd rather have a
     * separate classes for logic and use MonoBeh's only for displaying, but I had a lot of stuff to implement, so my
     * decision here was to mix it in this task to complete everything in time
     *
     * Also I would use some IoC container in order to have less dependencies and create a UnitTests for logic & models
     */
    public class GameController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private VirtualScreenSettings virtualScreenSettings;
        [SerializeField] private PlayerSettings playerSettings;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject applePrefab;
        [SerializeField] private PlayerInputMap playerOneInputMap;
        [SerializeField] private PlayerInputMap playerTwoInputMap;
        [Header("Layout")] 
        [SerializeField] private GameObject playersLayer;
        [SerializeField] private GameObject appleContainer;
        [Header("Views")] 
        [SerializeField] private LevelBackgroundView backgroundView;

        private HashSet<ICleanable> objectsToClean = new HashSet<ICleanable>();
        private HashSet<PlayerController> players = new HashSet<PlayerController>();

        private AppleSpawner appleSpawner;
        private VirtualScreenController virtualScreen;
        private void Awake()
        {
            virtualScreen = new VirtualScreenController(virtualScreenSettings);
            backgroundView.Draw(virtualScreen);
            
            appleSpawner = new AppleSpawner(virtualScreen, applePrefab, appleContainer);
        }

        private void Update()
        {
            /*
             * if we had more players, it would be logical to implement something better than this,
             * like just generate players in a loop and take colors from the scriptable object or generate randomly
             */
            var startSingle = Input.GetKeyUp(KeyCode.Alpha1);
            var startTwo = Input.GetKeyUp(KeyCode.Alpha2);

            if (startSingle)
            {
                Clean();
                StartSinglePlayer();

                return;
            }

            if (startTwo)
            {
                Clean();
                StartTwoPlayers();

                return;
            }
        }

        private void StartSinglePlayer()
        {
            CreatePlayer(playerOneInputMap, Color.red);
            
            appleSpawner.Start();
            objectsToClean.Add(appleSpawner);
        }

        private void StartTwoPlayers()
        {
            CreatePlayer(playerOneInputMap, Color.red);
            CreatePlayer(playerTwoInputMap, Color.blue);
            
            appleSpawner.Start();
            objectsToClean.Add(appleSpawner);
        }

        //if we had more entities than player & apple, we could have some Factory that could build us this things
        private void CreatePlayer(PlayerInputMap inputMap, Color color)
        {
            var player = Instantiate(playerPrefab, playersLayer.transform);
            
            var playerController = player.GetComponent<PlayerController>();
            playerController.Init(virtualScreen, playerSettings, new PlayerKeyboardInputController(inputMap), color);
            playerController.OnDeath += PlayerDead;
            playerController.OnPixelAdded += PixelAdded;

            objectsToClean.Add(playerController);
            players.Add(playerController);
        }

        private void PlayerDead(PlayerController player)
        {
            players.Remove(player);
            player.Clean();
            objectsToClean.Remove(player);

            if (players.Count == 0)
            {
                Clean();
            }
        }

        private void PixelAdded()
        {
            if (virtualScreen.ScreenSize == players.Sum(p => p.PixelsAmount))
            {
                Clean();
            }
        }

        private void Clean()
        {
            foreach (var cleanable in objectsToClean)
            {
                cleanable.Clean();
            }

            objectsToClean.Clear();
        }
    }
}