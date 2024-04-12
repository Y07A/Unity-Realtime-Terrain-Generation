# Realtime Terrain Generation

 A simple realtime-endless terrain generation for Unity 2020.

![Screenshot](/Assets/Files/Screenshots/Screenshot2.jpg?raw=true "Screenshot")

Noise library: [https://github.com/Scrawk/Procedural-Noise]


## Introduction

Finding no simple solution to generate realtime and performant terrains in Unity, I created my own with caveman brain.

It's a pretty rudimentary way, which generates chunks of terrains, with props (no grass for now).

Terrains are shaped trough different layers of noise and have different LODs. 

A shader in the project allows to draw a different texture on steep areas.

There is multiple examples in the project, with differents use cases and styles.

![Screenshot](/Assets/Files/Screenshots/Screenshot6.jpg?raw=true "Screenshot")

I didn't tested it on mobile, but it might works pretty well.

Everything is fully customizable.

Feel free to use it in your games !


## How to use

A few steps:
- Create a new GameObject
- Add the TerrainMeshGenerator component to it
- Set the tilePrefab to the Files/Prefabs/Tiles/TileLod prefab
- Tweak the settings as you want
- Add and setup AmbientSettings to control fog and skybox (Additional)


## Changing Terrain resolution

Generated chunks of terrains are in fact instances of a prefab that contains every resolutions of the terrain. 
If you want to modify the resolution of it, you will need to use a 3d modeling software like Blender (free) and achieve few steps.

For Blender:
- Import the TileLod.fbx into blender
- Add/modify the resolution by subdividing/unsubdividing the planes
- Rotate the view like this:

![Blender View](/Assets/Files/Screenshots/BlenderView.JPG?raw=true "Blender View")
- In edit mode, go to "Mesh/Sort Elements../View Z Axis"
- Still in edit mode, to verify vertices orders, check "Edit/Preferences/Interface/Developer Extras", next check enable "Indices" in this window

![Blender Viewport Overlays window](/Assets/Files/Screenshots/BlenderWindow.png?raw=true "Blender Viewport Overlays window")

- Vertices must have this order:

![Blender Vertices Order](/Assets/Files/Screenshots/VerticesOrder.jpg?raw=true "Blender Vertices Order")
  
- Now you can export your planes as FBX.
- In your model import settings in unity, make sure to set "Mesh Compression" to "Off"



