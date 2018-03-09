using System.Collections.Generic;
using UnityEngine;

namespace Bomberman
{
    public class PlayerController : Controller
    {
        #region globals
        /// <summary>
        /// bombs player have and 
        /// use at a given time
        /// </summary>
        [HideInInspector] public int bombsInHand = 1;

        /// <summary>
        /// 
        /// </summary>
        [HideInInspector] public string powerupName = string.Empty;

        /// <summary>
        /// bomb gameobject
        /// </summary>
        public Bomb bombGO;

        /// <summary>
        /// multiplies bomb explosion range when 
        /// longer bomb radius powerup is active
        /// </summary>
        [HideInInspector] public float rangeMultiplier = 1f;

        /// <summary>
        /// flag for tracking remtoe bomb activation
        /// </summary>
        public bool hasRemoteBomb = false;

        /// <summary>
        /// bomb ref for remote explosion
        /// </summary>
        private Bomb bomb = null;

        /// <summary>
        /// Bomb.StartDetonation() coroutine 
        /// explosion ref for remote explosion
        /// </summary>
        private Coroutine bombDetonationRoutine;
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
            bombsInHand = 1;
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
            if ( Input.GetKeyDown( keyDropBomb ) )
            {
                if ( bombsInHand > 0 && (bomb == null || !hasRemoteBomb) )
                {
                    //bomb ref
                    bomb = Instantiate( bombGO, transform.position, Quaternion.identity );

                    //coroutine ref
                    bombDetonationRoutine = bomb.StartDetonation( this, mapData, wallBlocks, rangeMultiplier, hasRemoteBomb );
                }
                else if ( hasRemoteBomb )
                {
                    //reset remote button flag
                    hasRemoteBomb = false;

                    //stop coroutine fired from bomb class
                    //to detonate bomb in 10s
                    StopCoroutine( bombDetonationRoutine );

                    //fire explosion manually
                    StartCoroutine( bomb.Explosion( 0.1f ) );
                }
            }
        }

        /// <summary>
        /// movement
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
            if ( trigger.gameObject.layer == Constants.LAYER_ENEMY )
            {
                PlayerPrefs.SetString( Constants.GAME_RESULT, (gameObject.name.Split( '_' )[1].ToLower().Equals( "yang" ) ? Constants.GAME_YING : Constants.GAME_YANG) );
                UnityEngine.SceneManagement.SceneManager.LoadScene( "gameover" );
            }
            else if ( trigger.gameObject.layer == Constants.LAYER_POWERUP )
            {
                //get powerup ref
                var powerup = trigger.gameObject.GetComponentInChildren<Powerup>();

                //update UI with powerup name
                powerupName = $"{trigger.gameObject.name}";

                //add a new comp to player
                var powerComponenet = gameObject.AddComponent( powerup.GetType() ).GetComponent<Powerup>();

                //copy the values from the powerup obj to new comp
                powerComponenet.Setup( powerup.multiplier, powerup.lifetime );

                //activate component
                powerComponenet.gameObject.SetActive( true );

                //Activate powerup
                powerComponenet.OnPickup();

                //destroy powerup object
                trigger.gameObject.transform.parent = null;
                Destroy( trigger.gameObject );
            }
        }
        #endregion

        #region combat
        public void OnBombDetonateStart()
        {
            bombsInHand--;
        }

        public void OnBombDetonateEnd()
        {
            bombsInHand++;
        }

        #endregion
    }
}
