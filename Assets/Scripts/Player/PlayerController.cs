using UnityEngine;

namespace Bomberman
{
    public class PlayerController : MonoBehaviour
    {
        #region globals
        /// <summary>
        /// mvoe speed
        /// </summary>
        public float speed = 1f;

        /// <summary>
        /// bombs player have
        /// </summary>
        public int bombsInHand = 1;

        /// <summary>
        /// bomb gameobject
        /// </summary>
        public Bomb bombGO;

        /// <summary>
        /// player id to differentiate between player1 and player2
        /// </summary>
        public EPlayerID playerID;

        /// <summary>
        /// direction in which the player is moving
        /// </summary>
        private EDirection direction;

        /// <summary>
        /// bomb drop state
        /// </summary>
        private bool _isBombActive = false;

        /// <summary>
        /// grid data
        /// </summary>
        private MapData mapData;

        /// <summary>
        /// 
        /// </summary>
        private bool canMove = true;

        /// <summary>
        /// 
        /// </summary>
        private bool isMoving = false;

        /// <summary>
        /// next move position
        /// </summary>
        private Vector3 position = Vector3.zero;

        /// <summary>
        /// cooldown between two movement input
        /// </summary>
        private float inputCooldown = 2f;
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
        private void Start()
        {
        }

        private void Update()
        {
            Walk();
            DropBomb();
        }
        #endregion

        #region Setup
        /// <summary>
        /// cache grid data
        /// </summary>
        /// <param name="mapData"></param>
        public void SetMapData( MapData mapData )
        {
            this.mapData = mapData;
        }
        #endregion

        #region Movement
        private void Walk()
        {
            if ( canMove )
            {
                position = transform.position;
                MovementInput();
            }

            if ( isMoving )
            {
                if ( transform.position == position )
                {
                    isMoving = false;
                    canMove = true;
                    MovementInput();
                }

                transform.position = Vector3.MoveTowards( transform.position, position, speed * Time.deltaTime );
            }
        }

        private bool IsValidMove( Vector3 nextPosition )
        {
            return (mapData.data[Mathf.Abs( ( int )nextPosition.x )][Mathf.Abs( ( int )nextPosition.z )]) == 0 ||
                (mapData.data[Mathf.Abs( ( int )nextPosition.x )][Mathf.Abs( ( int )nextPosition.z )]) > 2;
        }
        #endregion

        #region Input
        private void DropBomb()
        {
            if ( !_isBombActive && bombsInHand > 0 && Input.GetKeyDown( keyDropBomb ) )
            {
                Instantiate( bombGO, transform.position, Quaternion.identity ).StarDetonation( this, mapData );
            }
        }

        private void MovementInput()
        {
            var nextPosition = Vector3.zero;

            if ( Input.GetKeyUp( keyUp ) )
            {
                if ( direction != EDirection.Up )
                {
                    direction = EDirection.Up;
                }
                else
                {
                    //nextPosition = position + Vector3.forward;
                    SetNextPositionIfValid( position + Vector3.forward );
                }
            }
            else if ( Input.GetKeyUp( keyDown ) )
            {
                if ( direction != EDirection.Down )
                {
                    direction = EDirection.Down;
                }
                else
                {
                    SetNextPositionIfValid( position - Vector3.forward );
                }
            }
            else if ( Input.GetKeyUp( keyRight ) )
            {
                if ( direction != EDirection.Right )
                {
                    direction = EDirection.Right;
                }
                else
                {
                    SetNextPositionIfValid( position + Vector3.right );
                }
            }
            else if ( Input.GetKeyUp( keyLeft ) )
            {
                if ( direction != EDirection.Left )
                {
                    direction = EDirection.Left;
                }
                else
                {
                    SetNextPositionIfValid( position - Vector3.right );
                }
            }
        }

        private void SetNextPositionIfValid( Vector3 nextPosition )
        {
            if ( IsValidMove( nextPosition ) )
            {
                position = nextPosition;
                canMove = false;
                isMoving = true;
            }
        }
        #endregion

        #region combat
        public void OnBombDetonateStart()
        {
            _isBombActive = !_isBombActive;
            bombsInHand--;
        }

        public void OnBombDetonateEnd()
        {
            _isBombActive = !_isBombActive;
            bombsInHand++;
        }

        #endregion
    }

    public enum EPlayerID
    {
        P1,
        P2
    }

    public enum EDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
