using System.Collections.Generic;
using System.Xml;
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
                            //wallBlocksDict.Add( index, tile );
                            wallBlocksMapper.Add( new BlockMapper( index, tile ) );
                        }
                    }

                    index++;
                }
            }

            //delete old player prefabs
            var playerControllers = FindObjectsOfType<PlayerController>();
            if ( playerControllers != null && playerControllers.Length > 0 )
            {
                DestroyImmediate( playerControllers[0].gameObject );
                DestroyImmediate( playerControllers[1].gameObject );
            }

            //create players
            var playerGO = GetAsset( Constants.PLAYER1_ID );
            var player1 = Instantiate( playerGO, new Vector3( mapData.player1.x, 0, -mapData.player1.y ), Quaternion.identity ) as GameObject;
            player1.name = $"_{playerGO.name}_{index}";

            playerGO = GetAsset( Constants.PLAYER2_ID );
            var player2 = Instantiate( playerGO, new Vector3( mapData.player2.x, 0, -mapData.player2.y ), Quaternion.identity ) as GameObject;
            player2.name = $"_{playerGO.name}_{index}";

            //delete old gameController
            var oldGameController = FindObjectOfType<GameController>();
            if ( oldGameController != null )
            {
                DestroyImmediate( oldGameController.gameObject );
            }

            //create new gameController object
            var gameController = Instantiate( Resources.Load( @"Prefabs\GameController" ) ) as GameObject;
            gameController.name = "_gameController";
            gameController.GetComponent<GameController>().SetWallBlocks( wallBlocksMapper );
            gameController.GetComponent<GameController>().SetPlayerControllers( player1, player2 );

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
            else
                return Resources.Load( @"Prefabs\Yang" ) as GameObject;
        }
    }
}