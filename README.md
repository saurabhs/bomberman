**Bomberman Game**

Recreation of the classic Bomberman game

**How to Play**

* Ying (or P1) starts at bottom and is controlled using arrow keys and Right Control for dropping bombs
* Yang (or P2) starts at top and is controlled using WASD keys and Left Control for dropping bombs
* Use drop key (right & left control key) to activate remote bomb

**How to Develop**

* Unity 2017.1 with .net 4.6
* Visual Studio 2017 Community Edition
* Tiled Map Editor for creating maps

**How to generate maps in Unity**

1. Create a map in Tiled Map Editor (or any other map editor, though the map generation code is tied with the format Tiled Map Editor saves to)
2. Name it "map_XX" where XX is the map number, eg., map_05 for fifth level
3. Make sure the map has a "Map" tilemap layer with walls tiles and "Player" layer with players
4. Save the map in Assets/Resources/Tilemaps/ as a .tmx file
5. Create a new scene in Unity and save it with same name as the map, map_05.unity for fifth level
6. Execute map generation using Tools/Generate map in Unity
7. ???
8. Profit

**Next Steps**

* ~~The current version is coop only mode with no enemies, so the players fight between each other~~
* From code perspective, finding next available blocks for explosion in Bomb class can be improved
* The grid is instantiated from origin to negative z vertically and finding the grid value requires calculating its absoulute value
* ~~Power ups!~~