using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Bomberman
{
    public class AIController : Controller
    {
        /// <summary>
        /// direction in which this controller is moving
        /// </summary>
        public EDirection direction;

        /// <summary>
        /// Axis in which this controller will move
        /// </summary>
        public EAxis axis;

        #region unity lifecycle
        private void Awake()
        {
            SetMapData();
        }

        private void Update()
        {
            Walk();
        }
        #endregion

        #region Movement
        protected override void Move()
        {
            SetNextPositionIfValid( new Point( nextPosition + GetNextMoveDirection() ) );
        }

        private Vector3 GetNextMoveDirection()
        {
            return axis == EAxis.Vertical ? (direction == EDirection.Up ? Vector3.forward : -Vector3.forward) : (direction == EDirection.Right ? Vector3.right : -Vector3.right);
        }

        private void InvertDirection()
        {
            if ( direction == EDirection.Up )
                direction = EDirection.Down;
            else if ( direction == EDirection.Down )
                direction = EDirection.Up;
            else if ( direction == EDirection.Right )
                direction = EDirection.Left;
            else //if (direction == EDirection.Left)
                direction = EDirection.Right;
        }

        /// <summary>
        /// set next move position if availabel and start moving
        /// invert direction because enemy hit some wall or end
        /// </summary>
        protected override void SetNextPositionIfValid( Point point )
        {
            if ( IsValidMove( point ) )
            {
                nextPosition = new Vector3( point.x, nextPosition.y, point.y );
                canMove = false;
                isMoving = true;
            }
            else
            {
                InvertDirection();

                //change direction evertime the enemy hits a deadend
                axis = GetRandomAxis();
                direction = axis == EAxis.Vertical ? GetRandomVerticalDirection() : GetRandomHorizontalDirection();
            }
        }

        /// <summary>
        /// get random axis
        /// </summary>
        private EAxis GetRandomAxis()
        {
            //return (EAxis)Enum.GetValues(typeof(EAxis)).GetValue(Random.Range(0, Enum.GetValues(typeof(EAxis)).Length));
            return axis == EAxis.Vertical ? EAxis.Horizontal : EAxis.Vertical;
        }

        private EDirection GetRandomVerticalDirection()
        {
            return ( EDirection )Enum.GetValues( typeof( EDirection ) ).GetValue( Random.Range( 0, 2 ) );
        }

        private EDirection GetRandomHorizontalDirection()
        {
            return ( EDirection )Enum.GetValues( typeof( EDirection ) ).GetValue( Random.Range( 2, Enum.GetValues( typeof( EDirection ) ).Length ) );
        }

        #endregion

        #region collision
        private void OnTriggerEnter( Collider other )
        {
            //destroy this enemy on collsion with bomb explosion
            Destroy( gameObject );
        }
        #endregion
    }

    public enum EDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    public enum EAxis
    {
        Vertical,
        Horizontal
    }
}