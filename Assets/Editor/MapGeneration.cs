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

            var mapData = GetMapData( SceneManager.GetActiveScene().name );
            var worldParent = new GameObject( "_World" );
            worldParent.tag = "World";

            var wallBlocksDict = new Dictionary<int, GameObject>();
            var wallBlocksMapper = new List<BlockMapper>();

            var index = 0;
            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    var id = mapData.GetValue( i, j );
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
                var playerController = player.GetComponent<PlayerController>();
                playerController.SetWallBlocks( wallBlocksMapper );
                playerController.SetMapData( mapData );

                gameController.GetComponent<GameController>().AddPlayerController( player.GetComponent<PlayerController>() );
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
                var aiController = enemy.GetComponent<AIController>();
                aiController.SetWallBlocks( wallBlocksMapper );
                aiController.SetMapData( mapData );
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

        [MenuItem( "Tools/Game Mode/Play Single player" )]
        private static void SetSinglePlayerGameMode()
        {
            PlayerPrefs.SetInt( Constants.GAME_TYPE, Constants.SINGLEPLAYER_ID );
        }

        [MenuItem( "Tools/Game Mode/Play COOP" )]
        private static void SetCoopGameMode()
        {
            PlayerPrefs.SetInt( Constants.GAME_TYPE, Constants.COOP_ID );
        }

        public static MapData GetMapData( string filePath )
        {
            return ReadMapDataFromTMXFile( GetXMLDocument( filePath ) );
        }

        private static MapData ReadMapDataFromTMXFile( XmlDocument xmlDocument )
        {
            var mapData = new MapData();

            //read metadata
            var xmlData = xmlDocument.DocumentElement.SelectSingleNode( "/map" );
            if ( xmlData == null )
                throw new System.Exception( "Invalid XML Data..." );

            if ( !int.TryParse( xmlData.Attributes["width"].Value.Trim(), out mapData.width ) )
                throw new System.Exception( "Invalid Width value..." );

            if ( !int.TryParse( xmlData.Attributes["height"].Value.Trim(), out mapData.height ) )
                throw new System.Exception( "Invalid Height value..." );

            //read tile map data
            var dataNode = xmlDocument.DocumentElement.SelectSingleNode( "/map/layer[@name='Map']/data" );
            var tiles = dataNode.InnerText.Split( ',' );
            var index = 0;

            mapData.tiledata = new List<TileDataMapper>();
            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    mapData.tiledata.Add( new TileDataMapper( i, j, int.Parse( tiles[index++].Trim() ) ) );
                }
            }

            //players layer
            dataNode = xmlDocument.DocumentElement.SelectSingleNode( "/map/layer[@name='Players']/data" );
            tiles = dataNode.InnerText.Split( ',' );
            index = 0;

            mapData.players = new List<Point>
            {
                new Point(),
                new Point()
            };

            mapData.enemies = new List<Point>();

            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    if ( int.Parse( tiles[index].Trim() ) == Constants.PLAYER1_ID )
                    {
                        mapData.players[0] = (new Point( i, j ));
                    }
                    else if ( int.Parse( tiles[index].Trim() ) == Constants.PLAYER2_ID )
                    {
                        mapData.players[1] = new Point( i, j );
                    }
                    else if ( int.Parse( tiles[index].Trim() ) == Constants.ENEMY_ID )
                    {
                        mapData.enemies.Add( new Point( i, j ) );
                    }

                    index++;
                }
            }

            return mapData;
        }

        private static XmlDocument GetXMLDocument( string filename )
        {
            var filePath = Application.dataPath + @"/Resources/Tilemaps/" + filename + @".xml";
            if ( !System.IO.File.Exists( filePath ) )
                return null;

            var xmlDocument = new XmlDocument();
            xmlDocument.Load( filePath );

            return xmlDocument;
        }
    }
}