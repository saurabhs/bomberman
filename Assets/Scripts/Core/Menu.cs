using UnityEngine;
using UnityEngine.UI;

namespace Bomberman
{
    public class Menu : MonoBehaviour
    {
        public Button playButton;

        private void Start()
        {
            playButton.onClick.AddListener( () => StartGame() );
        }

        private void StartGame()
        {
            PlayerPrefs.SetInt( Constants.GAME_TYPE, Constants.COOP_ID );
            UnityEngine.SceneManagement.SceneManager.LoadScene( $"map0{Random.Range( 1, 3 )}" );
        }
    }
}