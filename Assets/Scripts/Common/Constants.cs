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

        public int AbsX { get { return Mathf.Abs( x ); } }
        public int AbsY { get { return Mathf.Abs( y ); } }

        public Point( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        public Point( Vector3 position )
        {
            x = ( int )position.x;
            y = ( int )position.z;
        }

        public Point GetAbs()
        {
            return new Point( AbsX, AbsY );
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    [System.Serializable]
    public class TileDataMapper
    {
        [SerializeField] public int i;
        [SerializeField] public int j;
        [SerializeField] public int value;

        public TileDataMapper( int i, int j, int value )
        {
            this.i = i;
            this.j = j;
            this.value = value;
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
        public List<TileDataMapper> tiledata;
        public List<Point> players;
        public List<Point> enemies;

        //public void Set2DArrayFromTileDataMapper()
        //{
        //    data = new int[width, height];
        //    foreach ( var item in tiledata )
        //    {
        //        data[item.i, item.j] = item.value;
        //    }
        //}

        public void SetValue( int i, int j, int value )
        {
            var index = i + j * width;
            tiledata[index] = new TileDataMapper( i, j, value );
        }

        public int GetValue( int i, int j )
        {
            var index = i + j * width;
            return tiledata[index].value;
        }
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

        public static byte SINGLEPLAYER_ID = 100;
        public static byte COOP_ID = 101;

        public static string GAME_TYPE = "GAME_TYPE";
        public static string GAME_RESULT = "GAME_RESULT";

        public static string GAME_DRAW = "DRAW!!!";
        public static string GAME_YANG = "YANG WINS!!!";
        public static string GAME_YING = "YING WINS!!!";
        public static string GAME_OVER = "GAME OVER!!!";    //for single player

        public static int MAX_DETONATION_DELAY = 10;

        public static LayerMask LAYER_DEFAULT = LayerMask.NameToLayer( "Default" );
        public static LayerMask LAYER_ENEMY = LayerMask.NameToLayer( "Enemy" );
        public static LayerMask LAYER_EXPLOSION = LayerMask.NameToLayer( "Explosion" );
        public static LayerMask LAYER_POWERUP = LayerMask.NameToLayer( "Powerup" );
    }
}