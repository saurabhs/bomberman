using UnityEngine;
using UnityEngine.UI;

namespace Bomberman
{
    public class Gameover : MonoBehaviour
    {
        public Text result;
        public Button restart;

        private void Start()
        {
            //set result
            result.text = PlayerPrefs.GetString( Constants.GAME_RESULT );

            //restart
            restart.onClick.AddListener( () => UnityEngine.SceneManagement.SceneManager.LoadScene( $"map0{Random.Range( 1, 3 )}" ) );
        }
    }
}