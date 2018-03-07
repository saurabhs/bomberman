using System.Collections.Generic;
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
        private Vector3 nextPosition = Vector3.zero;

        /// <summary>
        /// blocks list to pass to bomb 
        /// on explotion for grid updation
        /// </summary>
        private List<BlockMapper> wallBlocks;
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

        /// <summary>
        /// set wall blocks list
        /// </summary>
        public void SetWallBlocks( List<BlockMapper> wallBlocks )
        {
            this.wallBlocks = wallBlocks;
        }
        #endregion

        #region Movement
        private void Walk()
        {
            if ( canMove )
            {
                nextPosition = transform.position;
                MovementInput();
            }

            if ( isMoving )
            {
                if ( transform.position == nextPosition )
                {
                    isMoving = false;
                    canMove = true;
                    MovementInput();
                }

                transform.position = Vector3.MoveTowards( transform.position, nextPosition, speed * Time.deltaTime );
            }
        }

        private bool IsValidMove( Point point )
        {
            return (point.x >= 0 && point.x < mapData.width && point.y <= 0 && Mathf.Abs( point.y ) < mapData.height) &&
                (mapData.data[Mathf.Abs( point.x ), Mathf.Abs( point.y )] == 0 || (mapData.data[Mathf.Abs( point.x ), Mathf.Abs( point.y )]) > 2);
        }
        #endregion

        #region Input
        private void DropBomb()
        {
            if ( !_isBombActive && bombsInHand > 0 && Input.GetKeyDown( keyDropBomb ) )
            {
                Instantiate( bombGO, transform.position, Quaternion.identity ).StarDetonation( this, mapData, wallBlocks );
            }
        }

        private void MovementInput()
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

        private void SetNextPositionIfValid( Point point )
        {
            if ( IsValidMove( point ) )
            {
                nextPosition = new Vector3( point.x, nextPosition.y, point.y );
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
}
