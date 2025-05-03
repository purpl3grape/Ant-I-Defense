**Ant-I-Defense**

- Project Structure is split up first by type of file.

Then the folder/subcategories  structure follows a Parent-Child like relationship, where if a Grid contains a Cell within, Cell would be a subcategory of the Grid Folder.

- Naming Conventions brief:
	private fields: leading underscore i.e.) private int _someVariable
	public field i.e.) public int SomeVariable


## Project Architechture Overview
As per the guidelines, the implementation of a Generic Grid<TCell> data structure allows us to decouple Grid Logic, such as Unit positions as Cell related Data, from handling Unit Visual Updates. The visual updates are called on each given unit's FixedUpdate. I think this grid system would be highly compatible with A* Pathfinding, and I would definitely consider this route for AI decision making in future expansion.

## Final Thoughts
So if further expansion of this grid system to include more CellTypes, it is possible. Currently the celltypes are defined by enum, but this can be modifed to have different CellTypes (where they exhibit different behavior) -> The Poison-Effect CellType I briefly introduced as something to experiment with, I think can and should be moved into its separate class to expand out its behavior in future, again to separate out logic better.

- Scriptable Objects for Settings for Game / Player
As per the guidelines, I appreciate the decision to separate our settings into scriptable objects, so we can test out different values without having to get into the scripts to change these values. This will be of some help in future testing and gameplay balancing, in conjunction with other tools that can be leverage.

- Managers
The game logic is split out into their respective Managers.
GridManager and UnitManager are the core ones that drive the game. GridManager handles the GridLogic and UnitManager handles Moving Units from their Current to Next Grid coordinates.

The MainMenuManager currently drives the Canavas and UI logic, and CameraManager handles camera behaviors.