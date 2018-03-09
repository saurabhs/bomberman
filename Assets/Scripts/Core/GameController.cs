using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bomberman
{
    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float levelTime = 120f;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Text timerText;

        /// <summary>
        /// 
        /// </summary>
        public Text[] playersBombCountText;

        /// <summary>
        /// 
        /// </summary>
        public Text[] playersPowerText;

        /// <summary>
        /// list of player controller refs
        /// </summary>
        public List<PlayerController> playerControllers = new List<PlayerController>();

        #region unity lifecycle
        private void Start()
        {
            if (playerControllers == null || playerControllers.Count == 0)
                throw new System.Exception("Cannot find any playerController references.");

            //enable yang for coop match
            bool isSinglePlayer = false;
            if (isSinglePlayer = PlayerPrefs.GetInt(Constants.GAME_TYPE, Constants.COOP_ID) != Constants.COOP_ID)
            {
                playerControllers[1].gameObject.SetActive(!isSinglePlayer);
                playersBombCountText[1].gameObject.SetActive(false);
                playersPowerText[1].gameObject.SetActive(false);
                playerControllers.RemoveAt(1);
            }
        }

        private void Update()
        {
            if (levelTime <= 0)
            {
                PlayerPrefs.SetString(Constants.GAME_RESULT, Constants.GAME_DRAW);
                UnityEngine.SceneManagement.SceneManager.LoadScene("gameover");
            }
            else
            {
                levelTime -= Time.deltaTime;
                UpdateTimerText();
                UpdateBombCountText();
            }
        }
        #endregion

        #region UI
        /// <summary>
        /// Update Timer text in UI
        /// </summary>
        private void UpdateTimerText()
        {
            var min = ((int)levelTime / 60).ToString();
            var sec = (int)levelTime % 60;

            var secText = sec < 10 ? "0" : "";
            timerText.text = $"{min} : {secText}{sec}";
        }

        private void UpdateBombCountText()
        {
            for (var i = 0; i < playerControllers.Count; i++)
            {
                playersBombCountText[i].text = $"{playerControllers[i].bombsInHand}";
                playersPowerText[i].text = $"{playerControllers[i].powerupName}";
            }
        }

        //private void 
        #endregion

        /// <summary>
        /// setup refs for player controllers
        /// </summary>
        public void AddPlayerController(PlayerController player)
        {
            playerControllers.Add(player);
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