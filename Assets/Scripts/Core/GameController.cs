using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bomberman
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private float levelTime = 120f;

        [SerializeField] private Text timerText;

        /// <summary>
        /// list of player controller refs
        /// </summary>
        public List<GameObject> playerControllers = new List<GameObject>();

        #region unity lifecycle
        private void Start()
        {
            if ( playerControllers == null || playerControllers.Count == 0 )
                throw new System.Exception( "Cannot find any playerController references." );

            //enable yang for coop match
            playerControllers[1].SetActive( PlayerPrefs.GetInt( Constants.GAME_TYPE, Constants.COOP_ID ) == Constants.COOP_ID );
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
        /// setup refs for player controllers
        /// </summary>
        public void AddPlayerController( GameObject player )
        {
            playerControllers.Add( player );
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