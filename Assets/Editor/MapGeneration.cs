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

            var mapData = Common.GetMapData( SceneManager.GetActiveScene().name );
            var worldParent = new GameObject( "_World" );
            worldParent.tag = "World";

            var index = 0;
            for ( var j = 0; j < mapData.height; j++ )
            {
                for ( var i = 0; i < mapData.width; i++ )
                {
                    var id = mapData.data[i, j];
                    if ( id != 0 )
                    {
                        var go = GetAsset( id );
                        if ( go != null )
                        {
                            var tile = Instantiate( go, new Vector3( i /*- mapData.height / 2*/, 0, -j /*+ mapData.width / 2*/ ), Quaternion.identity, id < 3 ? worldParent.transform : null ) as GameObject;
                            tile.name = $"_{go.name}_{index++}";
                        }
                    }
                }
            }

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene( SceneManager.GetActiveScene() );
        }

        private static GameObject GetAsset( int index )
        {
            if ( index == 1 )
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