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

            //restart, - 1 for removing the gameover scene
            restart.onClick.AddListener( () => UnityEngine.SceneManagement.SceneManager.LoadScene( $"map0{Random.Range( 1, UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1 )}" ) );
        }
    }
}