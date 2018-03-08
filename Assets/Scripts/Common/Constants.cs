using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Bomberman
{
    [System.Serializable]
    public struct Point
    {
        public int x;
        public int y;

        private int absX;
        private int absY;

        public int AbsX { get { return absX; } }
        public int AbsY { get { return absY; } }

        public Point( int x, int y )
        {
            this.x = x;
            this.y = y;

            absX = Mathf.Abs( x );
            absY = Mathf.Abs( y );
        }

        public Point( Vector3 position )
        {
            x = ( int )position.x;
            y = ( int )position.z;

            absX = Mathf.Abs( x );
            absY = Mathf.Abs( y );
        }

        public Point GetAbs()
        {
            return new Point( absX, absY );
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    [System.Serializable]
    public struct TilesetData
    {
        public string prefix;
        public int firstgid;
        public int tilecount;
    }

    [System.Serializable]
    public class MapData
    {
        public int height;
        public int width;
        public int[,] data;
        public List<Point> players;
        public List<Point> enemies;
    }

    /// <summary>
    /// helper class because untiy cant 
    /// update values from editor script 
    /// unless the variable is exposed
    /// </summary>
    [System.Serializable]
    public struct BlockMapper
    {
        [SerializeField] public int index;
        [SerializeField] public GameObject tile;

        public BlockMapper( int index, GameObject tile )
        {
            this.index = index;
            this.tile = tile;
        }
    }

    public class Constants
    {
        public static int GROUND_ID = 0;
        public static int DESTRUCTABLE_WALL_ID = 1;
        public static int INDESTRUCTABLE_WALL_ID = 2;
        public static int PLAYER1_ID = 3;
        public static int PLAYER2_ID = 4;
        public static int ENEMY_ID = 5;

        public static int SINGLEPLAYER_ID = 100;
        public static int COOP_ID = 101;

        public static string GAME_TYPE = "GAME_TYPE";
        public static string GAME_RESULT = "GAME_RESULT";

        public static string GAME_DRAW = "DRAW!!!";
        public static string GAME_YANG = "YANG WINS!!!";
        public static string GAME_YING = "YING WINS!!!";
    }

    public class Common
    {
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

            mapData.data = new int[mapData.width, mapData.height];
            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    mapData.data[i, j] = int.Parse( tiles[index++].Trim() );
                }
            }

            //players layer
            dataNode = xmlDocument.DocumentElement.SelectSingleNode( "/map/layer[@name='Players']/data" );
            tiles = dataNode.InnerText.Split( ',' );
            index = 0;

            mapData.players = new List<Point>();
            mapData.players.Add( new Point() );
            mapData.players.Add( new Point() );

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
            var filePath = Application.dataPath + @"/Resources/Tilemaps/" + filename + @".tmx";

            if ( !System.IO.File.Exists( filePath ) )
                return null;

            var xmlDocument = new XmlDocument();
            xmlDocument.Load( filePath );

            return xmlDocument;
        }
    }
}