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
        /// explsion placehodler,
        /// pink cube for now
        /// </summary>
        public GameObject explosionGO;

        /// <summary>
        /// objects created for explosion
        /// </summary>
        private List<GameObject> explosionsEffect = new List<GameObject>();
        #endregion

        #region combat
        /// <summary>
        /// called from player after instantiation
        /// </summary>
        public void StartDetonation( PlayerController parent, MapData mapData, List<BlockMapper> wallBlocks )
        {
            this.parent = parent;
            this.mapData = mapData;
            this.wallBlocks = wallBlocks;

            StartCoroutine( OnBombDetonate() );
        }

        private IEnumerator Explosion( float delay = 0.5f )
        {
            //create explosion at origin
            var origin = new Point( ( int )transform.position.x, ( int )transform.position.z );
            CreateExplosion( origin );

            var currentGrid = origin.GetAbs();

            //check for grid extrems or if the next block is indestructible
            var canMoveUp = (currentGrid.y - 1) >= 0 && mapData.data[currentGrid.x, currentGrid.y - 1] != Constants.INDESTRUCTABLE_WALL_ID;
            var canMoveDown = (currentGrid.y + 1) < mapData.height && mapData.data[currentGrid.x, currentGrid.y + 1] != Constants.INDESTRUCTABLE_WALL_ID;
            var canMoveRight = (currentGrid.x + 1) < mapData.width && mapData.data[currentGrid.x + 1, currentGrid.y] != Constants.INDESTRUCTABLE_WALL_ID;
            var canMoveLeft = (currentGrid.x - 1) >= 0 && mapData.data[currentGrid.x - 1, currentGrid.y] != Constants.INDESTRUCTABLE_WALL_ID;

            //add grids to be used for explosin and update for next block
            for ( var i = 1; i <= range; i++ )
            {
                //up
                if ( canMoveUp )
                {
                    var point = new Point( ( int )transform.position.x, ( int )transform.position.z + i );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, point.AbsY] == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveUp = point.y + 1 <= 0 &&
                                !isCurrentBlockDestructibleWall &&
                                mapData.data[point.x, point.AbsY - 1] != Constants.INDESTRUCTABLE_WALL_ID;
                }
                //down
                if ( canMoveDown )
                {
                    var point = new Point( ( int )transform.position.x, ( int )transform.position.z - i );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, point.AbsY] == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );


                    //check if next point is valid
                    canMoveDown = point.AbsY + 1 < mapData.height &&
                                    !isCurrentBlockDestructibleWall &&
                                    mapData.data[point.x, point.AbsY + 1] != Constants.INDESTRUCTABLE_WALL_ID;
                }
                //right
                if ( canMoveRight )
                {
                    var point = new Point( ( int )transform.position.x + i, ( int )transform.position.z );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, point.AbsY] == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveRight = point.x + 1 < mapData.width &&
                                    !isCurrentBlockDestructibleWall &&
                                    mapData.data[point.x + 1, point.AbsY] != Constants.INDESTRUCTABLE_WALL_ID;
                }
                //left
                if ( canMoveLeft )
                {
                    var point = new Point( ( int )transform.position.x - i, ( int )transform.position.z );

                    var isCurrentBlockDestructibleWall = mapData.data[point.x, point.AbsY] == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveLeft = point.x - 1 >= 0 &&
                                !isCurrentBlockDestructibleWall &&
                                mapData.data[point.x - 1, point.AbsY] != Constants.INDESTRUCTABLE_WALL_ID;
                }

                yield return new WaitForSeconds( delay );
            }

            //post explosion cleanup
            //explosion over, clear the grid
            DestroyExplosionEffect();

            parent.OnBombDetonateEnd();
            Destroy( gameObject );
        }

        private void DestroyCurrentBlockAndUpdateMapData( bool isCurrentBlockDesWall, Point point )
        {
            //create new explosion
            CreateExplosion( point );

            if ( isCurrentBlockDesWall )
            {
                mapData.data[point.x, point.AbsY] = Constants.GROUND_ID;
                var index = point.x + (point.AbsY * mapData.width);

                var listIndex = wallBlocks.FindIndex( obj => obj.index == index );
                if ( listIndex != -1 )
                {
                    //destroy destructible wall
                    Destroy( wallBlocks[listIndex].tile.gameObject );
                    //remove from list
                    wallBlocks.RemoveAt( listIndex );
                }
            }
        }

        /// <summary>
        /// instantiate explosion
        /// </summary>
        private void CreateExplosion( Point point )
        {
            var explosionEffect = Instantiate( explosionGO, new Vector3( point.x, 0, point.y ), Quaternion.identity );
            explosionEffect.name = "_explosion";
            explosionsEffect.Add( explosionEffect );
        }

        /// <summary>
        /// clear explosion objects
        /// </summary>
        private void DestroyExplosionEffect()
        {
            for ( int i = 0; i < explosionsEffect.Count; i++ )
            {
                Destroy( explosionsEffect[i].gameObject );
            }

            explosionsEffect.Clear();
        }

        /// <summary>
        /// Response on player detonate the bomb
        /// </summary>
        private IEnumerator OnBombDetonate()
        {
            parent.OnBombDetonateStart();

            yield return new WaitForSeconds( detonateTime );

            StartCoroutine( Explosion( 0.1f ) );
        }
        #endregion
    }
}