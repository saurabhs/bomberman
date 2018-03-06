using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bomberman
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private float _levelTime = 120f;

        /// <summary>
        /// map data
        /// </summary>
        public MapData mapData;

        [Header( "UI" )]
        [SerializeField]
        private Text timerText;

        /// <summary>
        /// list of blocks in grid
        /// </summary>
        public List<GameObject> wallBlocks;

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

            var playerControllers = FindObjectsOfType<PlayerController>();
            for ( var i = 0; i < playerControllers.Length; i++ )
            {
                playerControllers[i].SetMapData( mapData );
                playerControllers[i].SetWallBlocksList( wallBlocks );
            }
        }

        private void Update()
        {
            if ( _levelTime <= 0 )
            {
                //show result

                //enable restart button
            }
            else
            {
                _levelTime -= Time.deltaTime;
                UpdateTimerText();
            }
        }
        #endregion

        #region map data
        #endregion

        #region UI
        /// <summary>
        /// Update Timer text in UI
        /// </summary>
        private void UpdateTimerText()
        {
            var min = (_levelTime / 60).ToString( "0" );
            var sec = (_levelTime % 60).ToString( "0" );

            timerText.text = $"{min}:{sec}";
        }
        #endregion

        public void SetWallBlocks( List<GameObject> wallBlocks )
        {
            this.wallBlocks = wallBlocks;
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