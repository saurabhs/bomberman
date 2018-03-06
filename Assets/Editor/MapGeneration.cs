using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bomberman
{
    public class MapGeneration : Editor
    {
        [MenuItem( "Design/Generate Map" )]
        private static void GenerateMap()
        {
            //delete old world prefab
            var oldWorld = GameObject.FindGameObjectWithTag( "World" );
            if ( oldWorld != null )
            {
                DestroyImmediate( oldWorld );
            }

            //delete old player prefabs
            var playerControllers = FindObjectsOfType<PlayerController>();
            if ( playerControllers != null && playerControllers.Length > 0 )
            {
                DestroyImmediate( playerControllers[0].gameObject );
                //DestroyImmediate( playerControllers[1] );
            }

            var mapData = Common.GetMapData( SceneManager.GetActiveScene().name );
            var worldParent = new GameObject( "_World" );
            worldParent.tag = "World";

            var wallBlocks = new List<GameObject>();

            var index = 0;
            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    var id = mapData.data[i, j];
                    var go = GetAsset( id );

                    //set ground height to -1
                    var tile = Instantiate( go, new Vector3( i, id == Constants.GROUND_ID ? -1 : 0, -j ), Quaternion.identity, worldParent.transform ) as GameObject;
                    tile.name = $"_{go.name}_{index}";

                    //add wall blocks and ground
                    wallBlocks.Add( tile );

                    index++;
                }
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
            gameController.GetComponent<GameController>().SetWallBlocks( wallBlocks );

            //save scene 
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene( SceneManager.GetActiveScene() );
        }

        private static GameObject GetAsset( int index )
        {
            if ( index == 0 )
                return Resources.Load( @"Prefabs\Ground" ) as GameObject;
            else if ( index == 1 )
                return Resources.Load( @"Prefabs\DestructableWall" ) as GameObject;
            else if ( index == 2 )
                return Resources.Load( @"Prefabs\IndestructableWall" ) as GameObject;
            else if ( index == 3 )
                return Resources.Load( @"Prefabs\Ying" ) as GameObject;
            else
                return Resources.Load( @"Prefabs\Yang" ) as GameObject;
        }
    }
}