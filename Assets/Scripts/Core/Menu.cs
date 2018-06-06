using UnityEngine;
using UnityEngine.UI;

namespace Bomberman
{
    public class Menu : MonoBehaviour
    {
        public Button playSoloButton;
        public Button playCoopButton;

        private void Start()
        {
            playSoloButton.onClick.AddListener( () => StartGame( Constants.SINGLEPLAYER_ID ) );
            playCoopButton.onClick.AddListener( () => StartGame( Constants.COOP_ID ) );
        }

        private void StartGame( byte playType )
        {
            PlayerPrefs.SetInt( Constants.GAME_TYPE, ( int )playType );
            UnityEngine.SceneManagement.SceneManager.LoadScene( $"map0{Random.Range( 1, 3 )}" );
        }
    }
}