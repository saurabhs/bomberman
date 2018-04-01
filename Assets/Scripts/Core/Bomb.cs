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

        /// <summary>
        /// list of powerup to spawn from
        /// </summary>
        public List<GameObject> powerups;

        /// <summary>
        /// flag to spawn only one 
        /// powerup for each bomb
        /// </summary>
        private bool hasSpawnedAPowerup = false;

        /// <summary>
        /// 
        /// </summary>
        private Coroutine bombExplosionRoutine = null;

        #endregion

        #region trigger response
        private void OnTriggerEnter( Collider collider )
        {
            if ( collider.gameObject.layer == Constants.LAYER_EXPLOSION )
            {
                //remove layer to avoid generating multiple explosions
                gameObject.layer = Constants.LAYER_DEFAULT;
                collider.gameObject.layer = Constants.LAYER_DEFAULT;
                ;

                //is hit by explosion effect
                StopCoroutine( bombExplosionRoutine );
                StartCoroutine( Explosion( 0.1f ) );
            }
        }
        #endregion

        #region combat
        /// <summary>
        /// called from player after instantiation
        /// </summary>
        public Coroutine StartDetonation( PlayerController parent, MapData mapData, List<BlockMapper> wallBlocks, float rangeMultiplier = 1f, bool hasRemoteBomb = false )
        {
            this.parent = parent;
            this.mapData = mapData;
            this.wallBlocks = wallBlocks;
            range = ( int )(range * rangeMultiplier);

            //increase max time to detonate to 10s
            //incase the player doesn't activate manually
            if ( hasRemoteBomb )
                detonateTime = Constants.MAX_DETONATION_DELAY;

            bombExplosionRoutine = StartCoroutine( OnBombDetonate() );
            return bombExplosionRoutine;
        }

        public IEnumerator Explosion( float delay = 0.5f )
        {
            //create explosion at origin
            CreateExplosion( new Point( ( int )transform.position.x, ( int )transform.position.z ) );

            var currentGrid = new Point( Mathf.Abs( ( int )transform.position.x ), Mathf.Abs( ( int )transform.position.z ) );

            //check for grid extrems or if the next block is indestructible
            var canMoveUp = (currentGrid.y - 1) >= 0 && mapData.GetValue( currentGrid.x, currentGrid.y - 1 ) != Constants.INDESTRUCTABLE_WALL_ID;
            var canMoveDown = (currentGrid.y + 1) < mapData.height && mapData.GetValue( currentGrid.x, currentGrid.y + 1 ) != Constants.INDESTRUCTABLE_WALL_ID;
            var canMoveRight = (currentGrid.x + 1) < mapData.width && mapData.GetValue( currentGrid.x + 1, currentGrid.y ) != Constants.INDESTRUCTABLE_WALL_ID;
            var canMoveLeft = (currentGrid.x - 1) >= 0 && mapData.GetValue( currentGrid.x - 1, currentGrid.y ) != Constants.INDESTRUCTABLE_WALL_ID;

            //add grids to be used for explosin and update for next block
            for ( var i = 1; i <= range; i++ )
            {
                //up
                if ( canMoveUp )
                {
                    var point = new Point( ( int )transform.position.x, ( int )transform.position.z + i );

                    var isCurrentBlockDestructibleWall = mapData.GetValue( point.x, point.AbsY ) == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveUp = point.y + 1 <= 0 &&
                                !isCurrentBlockDestructibleWall &&
                                mapData.GetValue( point.x, point.AbsY - 1 ) != Constants.INDESTRUCTABLE_WALL_ID;
                }
                //down
                if ( canMoveDown )
                {
                    var point = new Point( ( int )transform.position.x, ( int )transform.position.z - i );

                    var isCurrentBlockDestructibleWall = mapData.GetValue( point.x, point.AbsY ) == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );


                    //check if next point is valid
                    canMoveDown = point.AbsY + 1 < mapData.height &&
                                    !isCurrentBlockDestructibleWall &&
                                    mapData.GetValue( point.x, point.AbsY + 1 ) != Constants.INDESTRUCTABLE_WALL_ID;
                }
                //right
                if ( canMoveRight )
                {
                    var point = new Point( ( int )transform.position.x + i, ( int )transform.position.z );

                    var isCurrentBlockDestructibleWall = mapData.GetValue( point.x, point.AbsY ) == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveRight = point.x + 1 < mapData.width &&
                                    !isCurrentBlockDestructibleWall &&
                                    mapData.GetValue( point.x + 1, point.AbsY ) != Constants.INDESTRUCTABLE_WALL_ID;
                }
                //left
                if ( canMoveLeft )
                {
                    var point = new Point( ( int )transform.position.x - i, ( int )transform.position.z );

                    var isCurrentBlockDestructibleWall = mapData.GetValue( point.x, point.AbsY ) == Constants.DESTRUCTABLE_WALL_ID;
                    DestroyCurrentBlockAndUpdateMapData( isCurrentBlockDestructibleWall, point );

                    //check if next point is valid
                    canMoveLeft = point.x - 1 >= 0 &&
                                !isCurrentBlockDestructibleWall &&
                                mapData.GetValue( point.x - 1, point.AbsY ) != Constants.INDESTRUCTABLE_WALL_ID;
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
                mapData.SetValue( point.x, point.AbsY, Constants.GROUND_ID );
                var index = point.x + (point.AbsY * mapData.width);

                var listIndex = wallBlocks.FindIndex( obj => obj.index == index );
                if ( listIndex != -1 )
                {
                    //destroy destructible wall
                    Destroy( wallBlocks[listIndex].tile.gameObject );
                    //remove from list
                    wallBlocks.RemoveAt( listIndex );

                    SpawnPowerup( point );
                }
            }
        }

        /// <summary>
        /// spawns powerup if none has
        /// spawn yet at 33% spawn rate
        /// </summary>
        private void SpawnPowerup( Point point )
        {
            //33% chances of a destructible wall spawning a powerup
            if ( Random.Range( 0, 100 ) % 3 == 0 && !hasSpawnedAPowerup )
            {
                hasSpawnedAPowerup = true;

                //get a random powerup
                var powerupGO = powerups[Random.Range( 0, powerups.Count )];

                //Instantiate and place at the broken wall
                var powerup = Instantiate( powerupGO, new Vector3( point.x, 0, point.y ), powerupGO.transform.rotation );
                powerup.name = $"{powerupGO.name}";

                //destroy if the powerup is not
                //picked up in the given time window
                Destroy( powerup, powerupGO.GetComponent<Powerup>().lifetime );
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