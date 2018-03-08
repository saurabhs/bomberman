using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bomberman
{
    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// time for which the level will run in seconds
        /// </summary>
        [SerializeField] private float levelTime = 120f;

        /// <summary>
        /// raw grid data
        /// </summary>
        public MapData mapData;

        /// <summary>
        /// timer text gameobject ref 
        /// </summary>
        [SerializeField] private Text timerText;

        /// <summary>
        /// list for saving destructible wall go 
        /// for destroyinng later in game
        /// </summary>
        public List<BlockMapper> wallBlocks = new List<BlockMapper>();

        /// <summary>
        /// list of player controller refs
        /// </summary>
        public List<GameObject> playerControllers = new List<GameObject>();

        #region unity lifecycle
        private void Awake()
        {
            //cache map data 
            mapData = Common.GetMapData( UnityEngine.SceneManagement.SceneManager.GetActiveScene().name );
        }

        private void Start()
        {
            if ( wallBlocks == null || wallBlocks.Count == 0 )
                throw new System.Exception( "Cannot find any blocks reference." );

            if ( playerControllers == null || playerControllers.Count == 0 )
                throw new System.Exception( "Cannot find any playerController references." );

            //enable yang for coop match
            playerControllers[1].SetActive( PlayerPrefs.GetInt( Constants.GAME_TYPE, Constants.COOP_ID ) == Constants.COOP_ID );
            foreach ( var pc in playerControllers )
            {
                //assign data only to enabled playerController
                if ( pc.activeInHierarchy )
                {
                    pc.GetComponent<PlayerController>().SetMapData( mapData );
                    pc.GetComponent<PlayerController>().SetWallBlocks( wallBlocks );
                }
            }
        }

        private void Update()
        {
            if ( levelTime <= 0 )
            {
                PlayerPrefs.SetString( Constants.GAME_RESULT, Constants.GAME_DRAW );
                UnityEngine.SceneManagement.SceneManager.LoadScene( "gameover" );
            }
            else
            {
                levelTime -= Time.deltaTime;
                UpdateTimerText();
            }
        }
        #endregion

        #region UI
        /// <summary>
        /// Update Timer text in UI
        /// </summary>
        private void UpdateTimerText()
        {
            var min = (( int )levelTime / 60).ToString();
            var sec = ( int )levelTime % 60;

            var secText = sec < 10 ? "0" : "";
            timerText.text = $"{min} : {secText}{sec}";
        }
        #endregion

        /// <summary>
        /// cache refernce to block objects
        /// </summary>
        public void SetWallBlocks( List<BlockMapper> wallBlocks )
        {
            this.wallBlocks = wallBlocks;
        }

        /// <summary>
        /// setup refs for player controllers
        /// </summary>
        public void SetPlayerControllers( GameObject p1, GameObject p2 )
        {
            playerControllers.Add( p1 );
            playerControllers.Add( p2 );
        }

        /// <summary>
        /// Time over response
        /// </summary>
        private void OnTimeOver()
        {
            Time.timeScale = 0;
        }
    }
}