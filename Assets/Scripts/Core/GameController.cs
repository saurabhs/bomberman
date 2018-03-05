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

        #region unity lifecycle
        private void Awake()
        {
            //cache map data
            mapData = Common.GetMapData( UnityEngine.SceneManagement.SceneManager.GetActiveScene().name );
        }

        private void Start()
        {
            FindObjectOfType<PlayerController>().SetMapData( mapData );
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

        /// <summary>
        /// Time over response
        /// </summary>
        private void OnTimeOver()
        {
            Time.timeScale = 0;
        }
    }
}