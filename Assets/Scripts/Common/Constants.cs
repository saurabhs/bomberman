using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Bomberman
{
    [System.Serializable]
    public struct TilesetData
    {
        public string prefix;
        public int firstgid;
        public int tilecount;
    }

    [System.Serializable]
    public struct MapData
    {
        public int height;
        public int width;
        public int[][] data;
        public List<TilesetData> tilesetData;
    }

    public class Constants
    {
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

            var tilesetsXMLData = xmlDocument.DocumentElement.SelectNodes( "/map/tileset" );
            for ( var i = 0; i < tilesetsXMLData.Count; i++ )
            {
                if ( mapData.tilesetData == null )
                    mapData.tilesetData = new List<TilesetData>();

                var tilesetData = new TilesetData();
                tilesetData.firstgid = int.Parse( tilesetsXMLData[i].Attributes["firstgid"].Value.Trim() );
                mapData.tilesetData.Add( tilesetData );
            }

            //read tile map data
            var dataNode = xmlDocument.DocumentElement.SelectSingleNode( "/map/layer[@name='Map']/data" );
            var tiles = dataNode.InnerText.Split( ',' );
            var index = 0;

            mapData.data = new int[mapData.width][];
            for ( var j = 0; j < mapData.height; j++ )
            {
                mapData.data[j] = new int[mapData.width];
                for ( var i = 0; i < mapData.width; i++ )
                {
                    mapData.data[j][i] = int.Parse( tiles[index++].Trim() );
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