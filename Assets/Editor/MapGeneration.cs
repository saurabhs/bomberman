using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bomberman
{
    public class MapGeneration : Editor
    {
        [MenuItem( "Tools/Generate Map" )]
        private static void GenerateMap()
        {
            //delete old world prefab
            var oldWorld = GameObject.FindGameObjectWithTag( "World" );
            if ( oldWorld != null )
            {
                DestroyImmediate( oldWorld );
            }

            var mapData = Common.GetMapData( SceneManager.GetActiveScene().name );
            var worldParent = new GameObject( "_World" );
            worldParent.tag = "World";

            var wallBlocksDict = new Dictionary<int, GameObject>();
            var wallBlocksMapper = new List<BlockMapper>();

            var index = 0;
            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    var id = mapData.data[i, j];
                    if ( id != 0 )
                    {
                        var go = GetAsset( id );

                        //set ground height to -1
                        var tile = Instantiate( go, new Vector3( i, 0, -j ), Quaternion.identity, worldParent.transform ) as GameObject;
                        tile.name = $"_{go.name}_{index}";

                        //add only dest wall blocks
                        if ( id == Constants.DESTRUCTABLE_WALL_ID )
                        {
                            wallBlocksMapper.Add( new BlockMapper( index, tile ) );
                        }
                    }

                    index++;
                }
            }

            //delete old gameController
            var oldGameController = FindObjectOfType<GameController>();
            if ( oldGameController != null )
            {
                DestroyImmediate( oldGameController.gameObject );
            }

            //create new gameController object
            var gameController = Instantiate( Resources.Load( @"Prefabs\GameController" ) ) as GameObject;
            gameController.name = "_gameController";

            //delete old player prefabs
            var playerControllers = FindObjectsOfType<PlayerController>();
            if ( playerControllers != null && playerControllers.Length > 0 )
            {
                for ( var i = playerControllers.Length - 1; i >= 0; i-- )
                {
                    DestroyImmediate( playerControllers[i].gameObject );
                }
            }

            //create players
            for ( var i = 0; i < mapData.players.Count; i++ )
            {
                var point = mapData.players[i];

                var playerGO = GetAsset( Constants.PLAYER1_ID + i );
                var player = Instantiate( playerGO, new Vector3( point.x, 0, -point.y ), Quaternion.identity ) as GameObject;
                player.name = $"_{playerGO.name}_{index}";
                player.GetComponent<PlayerController>().SetWallBlocks( wallBlocksMapper );

                gameController.GetComponent<GameController>().AddPlayerController( player );
            }

            //delete old enemies
            var enemies = FindObjectsOfType<AIController>();
            if ( enemies != null && enemies.Length > 0 )
            {
                for ( var i = enemies.Length - 1; i >= 0; i-- )
                {
                    DestroyImmediate( enemies[i].gameObject );
                }
            }

            //create new enemies
            foreach ( var point in mapData.enemies )
            {
                var enemyGO = GetAsset( Constants.ENEMY_ID );
                var enemy = Instantiate( enemyGO, new Vector3( point.x, 0, -point.y ), Quaternion.identity ) as GameObject;
                enemy.name = $"_{enemyGO.name}_{index}";
                enemy.GetComponent<AIController>().SetWallBlocks( wallBlocksMapper );
            }

            //save scene 
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene( SceneManager.GetActiveScene() );
        }

        private static GameObject GetAsset( int index )
        {
            if ( index == Constants.GROUND_ID )
                return Resources.Load( @"Prefabs\Ground" ) as GameObject;
            else if ( index == Constants.DESTRUCTABLE_WALL_ID )
                return Resources.Load( @"Prefabs\DestructableWall" ) as GameObject;
            else if ( index == Constants.INDESTRUCTABLE_WALL_ID )
                return Resources.Load( @"Prefabs\IndestructableWall" ) as GameObject;
            else if ( index == Constants.PLAYER1_ID )
                return Resources.Load( @"Prefabs\Ying" ) as GameObject;
            else if ( index == Constants.PLAYER2_ID )
                return Resources.Load( @"Prefabs\Yang" ) as GameObject;
            else if ( index == Constants.ENEMY_ID )
                return Resources.Load( @"Prefabs\Enemy" ) as GameObject;

            return null;
        }
    }
}