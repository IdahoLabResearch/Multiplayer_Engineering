# Digital Engineering Multiplayer

Welcome to the Digital Engineering Multiplayer framework.

This `XRAI` submodule is intended to be used across various web and mixed reality multiplayer applications.

### Namespaces

There are five directories and corresponding namespaces in this framework.

1. Interactions
2. React
3. Scene
4. Serval
5. Utility

**Interactions**
The interactions namespace holds scripts used to control the scene and gameobjects based on user input.

In general, if you need to interact with a gameobject, you should place the script in this directory.

Most of these interaction scripts have documentation inside of them. You may need to read this documentation. If something is missing, please contact `Nathan Woodruff`.

1. Camera Orbit - The camera orbits the origin of a scene
2. Draggable - You can click and drag a gameobject in the scene
3. Tooltip - When you hover your mouse over a gameobject, a custom tooltip is rendered
4. TooltipManager - Used in conjunction with the Tooltip.cs script
5. Highlight - This functionality is located elsewhere, in the `Serval` namespace

**React**
The react namespace holds functions that React uses to talk to Unity. If you want to interact with a gameobject in Unity, but _from React_, place it here. You can expose them and React will be able to see the listeners in the DOM.

Read the [docs](https://react-unity-webgl.dev/docs/api/send-message) for the library we use.

**Scene**
The scene namespace holds all of the master/manager scripts for the build.

This is where we dynamically attach scripts to specific gameobjects at runtime, collect gameobjects intended for mapping into DeepLynx, and information about the scenes.

**Serval**
The serval namespace is a framework on its own.

Serval has a master script called `Handler.cs`. This is where Unity broadcasts and receives game state from Serval.

It has three sets of classes.

1. Objects - Builds state information for gameobjects in the scene
2. Players - Builds state information for players in the scene
3. State - Helper classes used to craft object and player state.

**Utility**
The utility namespace is used for high-level game functions.

It holds the `CustomTags.cs` script, used to add tags to gameobjects in the scene. When a tag is added to a gameobject using this script, it is queried at runtime in the `Scene` namespace, and `Interactions` are added to it.
