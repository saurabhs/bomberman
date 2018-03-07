using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman
{
    public class Bomb : MonoBehaviour
    {
        #region globals
        /// <summary>
        /// the player who detonated the bomb
        /// </summary>
        public PlayerController parent;

        public float cooldown = 2f;

        /// <summary>
        /// detonate time
        /// </summary>
        public float detonateTime = 2f;

        /// <summary>
        /// bomb range in grid
        /// </summary>
        public int range = 4;

        /// <summary>
        /// raw grid data
        /// </summary>
        private MapData mapData;

        /// <summary>
        /// list of wall gameobject
        /// </summary>
        private List<BlockMapper> wallBlocks;

        /// <summary>
        /// objects created for explosion
        /// </summary>
        List<GameObject> explosionsEffect = new List<GameObject>();
        #endregion

        #region combat
        /// <summary>
        /// called from player after instantiation
        /// </summary>
        public void StarDetonation( PlayerController parent, MapData mapData, List<BlockMapper> wallBlocks )
        {
            this.parent = parent;
            this.mapData = mapData;
            this.wallBlocks = wallBlocks;

            StartCoroutine( OnBombDetonate() );
        }

        private void Explosion()
        {
            var currentGrid = new Point( Mathf.Abs( ( int )transform.position.x ), Mathf.Abs( ( int )transform.position.z ) );

            //check for grid extrems or if the next block is indestructible
            var canMoveUp = (currentGrid.y - 1) >= 0 && mapData.data[currentGrid.x, currentGrid.y - 1] != 2;
            var canMoveDown = (currentGrid.y + 1) < mapData.height && mapData.data[currentGrid.x, currentGrid.y + 1] != 2;
            var canMoveRight = (currentGrid.x + 1) < mapData.width && mapData.data[currentGrid.x + 1, currentGrid.y] != 2;
            var canMoveLeft = (currentGrid.x - 1) >= 0 && mapData.data[currentGrid.x - 1, currentGrid.y] != 2;

            //add grids to be used for explosin and update for next block
            for ( var i = 1; i <= range; i++ )
            {
                //up
                if ( canMoveUp )
                {
                    var point = new Point( ( int )transform.position.x, ( int )transform.position.z + i );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, Mathf.Abs( point.y )] == 1;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveUp = point.y + 1 <= 0 &&
                                !isCurrentBlockDestructibleWall &&
                                mapData.data[point.x, Mathf.Abs( point.y ) - 1] != 2;
                }
                //down
                if ( canMoveDown )
                {
                    var point = new Point( ( int )transform.position.x, ( int )transform.position.z - i );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, Mathf.Abs( point.y )] == 1;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );


                    //check if next point is valid
                    canMoveDown = Mathf.Abs( point.y ) + 1 < mapData.height &&
                                    !isCurrentBlockDestructibleWall &&
                                    mapData.data[point.x, Mathf.Abs( point.y ) + 1] != 2;
                }
                //right
                if ( canMoveRight )
                {
                    var point = new Point( ( int )transform.position.x + i, ( int )transform.position.z );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, Mathf.Abs( point.y )] == 1;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveRight = point.x + 1 < mapData.width &&
                                    !isCurrentBlockDestructibleWall &&
                                    mapData.data[point.x + 1, Mathf.Abs( point.y )] != 2;
                }
                //left
                if ( canMoveLeft )
                {
                    var point = new Point( ( int )transform.position.x - i, ( int )transform.position.z );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, Mathf.Abs( point.y )] == 1;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveLeft = point.x - 1 >= 0 &&
                                !isCurrentBlockDestructibleWall &&
                                mapData.data[point.x - 1, Mathf.Abs( point.y )] != 2;
                }
            }
        }

        private void DestroyCurrentBlockAndUpdateMapData( bool isCurrentBlockDesWall, Point point )
        {
            if ( isCurrentBlockDesWall )
            {
                mapData.data[point.x, Mathf.Abs( point.y )] = 0;
                var index = point.x + (Mathf.Abs( point.y ) * mapData.width);
                Destroy( wallBlocks[index] );
            }
        }

        public GameObject go;

        /// <summary>
        /// Response on player detonate the bomb
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnBombDetonate()
        {
            yield return new WaitForSeconds( detonateTime );

            parent.OnBombDetonateStart();
            Explosion();

            yield return new WaitForSeconds( cooldown );

            parent.OnBombDetonateEnd();
            Destroy( gameObject );
        }
        #endregion
    }
}