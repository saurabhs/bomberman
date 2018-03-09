using System.Collections.Generic;
using UnityEngine;

namespace Bomberman
{
    public class PlayerController : Controller
    {
        #region globals
        /// <summary>
        /// bombs player have
        /// </summary>
        public int bombsInHand = 1;

        /// <summary>
        /// bomb gameobject
        /// </summary>
        public Bomb bombGO;

        /// <summary>
        /// bomb drop state
        /// </summary>
        public bool isBombActive = false;
        #endregion

        #region input
        /// <summary>
        /// keys mapping for each players
        /// </summary>
        public KeyCode keyUp;
        public KeyCode keyDown;
        public KeyCode keyRight;
        public KeyCode keyLeft;
        public KeyCode keyDropBomb;
        #endregion

        #region unity lifecycle
        private void Awake()
        {
            SetMapData();
        }

        private void Update()
        {
            Walk();
            DropBomb();
        }
        #endregion

        #region Input
        private void DropBomb()
        {
            if ( !isBombActive && bombsInHand > 0 && Input.GetKeyDown( keyDropBomb ) )
            {
                Instantiate( bombGO, transform.position, Quaternion.identity ).StartDetonation( this, mapData, wallBlocks );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Move()
        {
            if ( Input.GetKey( keyUp ) )
            {
                SetNextPositionIfValid( new Point( nextPosition + Vector3.forward ) );
            }
            else if ( Input.GetKey( keyDown ) )
            {
                SetNextPositionIfValid( new Point( nextPosition - Vector3.forward ) );
            }
            else if ( Input.GetKey( keyRight ) )
            {
                SetNextPositionIfValid( new Point( nextPosition + Vector3.right ) );
            }
            else if ( Input.GetKey( keyLeft ) )
            {
                SetNextPositionIfValid( new Point( nextPosition - Vector3.right ) );
            }
        }
        #endregion

        #region trigger detection and response
        private void OnTriggerEnter( Collider trigger )
        {
            PlayerPrefs.SetString( Constants.GAME_RESULT, (gameObject.name.Split( '_' )[1].ToLower().Equals( "yang" ) ? Constants.GAME_YING : Constants.GAME_YANG) );
            UnityEngine.SceneManagement.SceneManager.LoadScene( "gameover" );
        }
        #endregion

        #region combat
        public void OnBombDetonateStart()
        {
            isBombActive = !isBombActive;
            bombsInHand--;
        }

        public void OnBombDetonateEnd()
        {
            isBombActive = !isBombActive;
            bombsInHand++;
        }

        #endregion
    }
}
