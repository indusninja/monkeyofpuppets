# Monkey of Puppets
A game made at the 2010 Global Game Jam in XNA.

## Description
This game is a puzzle game where players have to sneak past the enemies, find the key and get to the door to escape the current level. And do it all over again in the next level.

Levels in the game are built using a 2D tile pattern, which denotes light green tiles (grass) as passable and dark green tiles (trees) as non passable. Enemies in the game are inactive unless they spot the player avatar (using a straight-forward line of sight logic). Players can either hide out of sight of the enemy, or deploy a dummy, in which case enemies will attack the closest one of the two. This can be used as a ploy by the player to distract the enemy while trying to sneak to a new hiding spot.

It has a basic state management system that helps in showing menus, keeping track of lives, managing state of the game and its levels, playing sound events for different game events, etc.

This game was initially developed during the Global Game Jam, over a duration of 48 hours. It also has an incomplete port in Flash, which has now been abandoned due to unpopularity of the platform.

![Monkey of Puppets Screen 2](images/MonkeyOfPuppets2.png?raw=true)

## Features
- 2D tile-based game level definition and rendering.
- Game entity management - Player, enemies, key, door, etc.
- Game state management system for handling game, menu and audio events.
- Bot AI allowing them line-of-sight and ability to track to a position.
- [Ogmo Editor](http://www.ogmoeditor.com/) 2D time level editor support.
- Bresenham’s line algorithm for AI's line of sight implementation (Flash version).
- Flood-fill algorithm for AI's pathfinding (Flash version).