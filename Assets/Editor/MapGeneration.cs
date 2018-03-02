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
            //delete old prefab
            var oldWorld = GameObject.FindGameObjectWithTag( "World" );
            if ( oldWorld != null )
            {
                DestroyImmediate( oldWorld );
            }

            //var currentScene = SceneManager.GetActiveScene();
            var mapData = ReadMapDataFromTMXFile( GetXMLDocument( SceneManager.GetActiveScene().name ) );
            var worldParent = new GameObject( "_World" );
            worldParent.tag = "World";

            var index = 0;
            var tilesetData = mapData.tilesetData;

            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    var id = mapData.data[j][i];
                    if ( id == 0 || id > 2 )
                    {
                        continue;
                    }

                    //id -= tilesetData[tilesetDataIndex].firstgid - 1;

                    var go = GetAsset( id );
                    if ( go == null )
                        continue;

                    var tile = Instantiate( go, new Vector3( i - mapData.height / 2, 0, -j + mapData.width / 2 ), Quaternion.identity, worldParent.transform ) as GameObject;
                    tile.name = $"_{go.name}_{index++}";
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene( SceneManager.GetActiveScene() );
        }

        private static GameObject GetAsset( int index )
        {
            if ( index == 1 )
                return Resources.Load( @"Models\DestructableWall" ) as GameObject;
            else if ( index == 2 )
                return Resources.Load( @"Models\IndestructableWall" ) as GameObject;

            return null;
        }

        /// <summary>
        /// Extract the XML from the TMX file
        /// </summary>
        /// <param name="filename">TMX file</param>
        /// <returns></returns>
        private static XmlDocument GetXMLDocument( string filename )
        {
            var filePath = Application.dataPath + @"/Resources/Tilemaps/" + filename + @".tmx";

            if ( !System.IO.File.Exists( filePath ) )
                return null;

            var xmlDocument = new XmlDocument();
            xmlDocument.Load( filePath );

            ////save file as xml for Unity Resource.Load to use at the start of a new game
            //filePath = Application.dataPath + @"/Resources/Tilemaps/" + filename + @".xml";
            //xmlDocument.Save( filePath );

            return xmlDocument;
        }

        /// <summary>
        /// Read the Tiled Map Editor's TMX file for level design
        /// </summary>
        /// <param name="xmlDocument">raw TMX file</param>
        /// <returns></returns>
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
                    mapData.data[j][i] = int.Parse( tiles[index].Trim() );
                    index++;
                }
            }

            return mapData;
        }
    }
}