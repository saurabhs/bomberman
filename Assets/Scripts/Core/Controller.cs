using System.Collections.Generic;
using UnityEngine;

namespace Bomberman
{
    public abstract class Controller : MonoBehaviour
    {
        #region globals
        /// <summary>
        /// mvoe speed
        /// </summary>
        public float speed = 1f;

        /// <summary>
        /// 
        /// </summary>
        protected bool canMove = true;

        /// <summary>
        /// 
        /// </summary>
        protected bool isMoving = false;

        /// <summary>
        /// grid data
        /// </summary>
        protected MapData mapData;

        /// <summary>
        /// next move position
        /// </summary>
        protected Vector3 nextPosition = Vector3.zero;

        /// <summary>
        /// blocks list to pass to bomb 
        /// on explotion for grid updation
        /// </summary>
        public List<BlockMapper> wallBlocks;
        #endregion

        #region Setup
        /// <summary>
        /// cache grid data
        /// </summary>
        public void SetMapData()
        {
            //cache grid data
            mapData = Common.GetMapData( UnityEngine.SceneManagement.SceneManager.GetActiveScene().name );
        }

        /// <summary>
        /// cache refernce to block objects
        /// </summary>
        public void SetWallBlocks( List<BlockMapper> wallBlocks )
        {
            this.wallBlocks = wallBlocks;
        }
        #endregion

        #region Movement
        /// <summary>
        /// implements walk functionality in controllers
        /// </summary>
        protected virtual void Walk()
        {
            if ( canMove )
            {
                nextPosition = transform.position;
                Move();
            }

            if ( isMoving )
            {
                if ( transform.position == nextPosition )
                {
                    isMoving = false;
                    canMove = true;
                    Move();
                }

                transform.position = Vector3.MoveTowards( transform.position, nextPosition, speed * Time.deltaTime );
            }
        }

        /// <summary>
        /// implements next position in controllers
        /// </summary>
        protected abstract void Move();

        /// <summary>
        /// check if the controller can move to next available point
        /// </summary>
        protected bool IsValidMove( Point point )
        {
            return (point.x >= 0 && point.x < mapData.width && point.y <= 0 && point.AbsY < mapData.height) &&
                (mapData.data[point.AbsX, point.AbsY] == 0 || (mapData.data[point.AbsX, point.AbsY]) > Constants.INDESTRUCTABLE_WALL_ID);
        }

        /// <summary>
        /// set next move position if availabel and start moving
        /// </summary>
        protected virtual void SetNextPositionIfValid( Point point )
        {
            if ( IsValidMove( point ) )
            {
                nextPosition = new Vector3( point.x, nextPosition.y, point.y );
                canMove = false;
                isMoving = true;
            }
        }
        #endregion
    }
}