#include "MapGenerator.h"
#include "stdlib.h"
#include <exception>
#include <vector>
using namespace std;

MapGenerator::~MapGenerator()
{
	storage.clear();
	delete this;
}

/*
* This function sets an equal number of each tile type randomly.
* The coordinates go from -(size/2) to (size/2)-1, with the origin (0,0) being at the center of the map (axial coordinates).
* The reason to do this instead of putting the origin in the uppter left corner is to have easier to write function late on, like pathfinding.
* By example on the demo map 6x6, x and y go from -3 to 2.
*/
void MapGenerator::buildMap(int size/*, int rdnSeed*/)
{
	this->size = size;
	int nbTiles = size*size;
	int nbDesertLeft = nbTiles / 4;
	int nbPlainLeft = nbTiles / 4;
	int nbForestLeft = nbTiles / 4;
	int nbMountainLeft = nbTiles / 4;
	int rd;
	mapStorage map;
	for (int i = 0; i < size; i++)
	{
		for (int j = 0; j < size; j++)
		{
			rd = rand() % 4;
			switch (rd)
			{
			case DESERT:
				if (nbDesertLeft == 0)
				{
					rd = (rd + 1) % 4;
				}
				else
				{
					map[Point(j - (size / 2), i - (size / 2))] = DESERT;
					nbDesertLeft--;
				}
				break;
			case FOREST:
				if (nbForestLeft == 0)
				{
					rd = (rd + 1) % 4;
				}
				else
				{
					map[Point(j - (size / 2), i - (size / 2))] = FOREST;
					nbForestLeft--;
				}
				break;
			case MOUTAIN:
				if (nbMountainLeft == 0)
				{
					rd = (rd + 1) % 4;
				}
				else
				{
					map[Point(j - (size / 2), i - (size / 2))] = MOUTAIN;
					nbMountainLeft--;
				}
				break;
			case PLAIN:
				if (nbPlainLeft == 0)
				{
					rd = (rd + 1) % 4;
				}
				else
				{
					map[Point(j - (size / 2), i - (size / 2))] = PLAIN;
					nbPlainLeft--;
				}
				break;
			default:
				break;
			}
		}
	}
	storage = map;
}

//getters
int MapGenerator::getTileType(int x, int y) const
{
	Point a = Point(x, y);
	try{
		return storage.at(a);
	}
	catch (out_of_range& e)
	{
		return INVALID;
	}
}
/*Point MapGenerator::getStartTileA() const
{
return startTileA;
}
Point MapGenerator::getStartTileB() const
{
return startTileB;
}*/

//setters
/*void MapGenerator::setStartTileA()
{
int x, y;
int rdX = rand() % 2;
int rdY = rand() % 2;
switch (rdX)
{
case 0:
x = 0;
break;
case 1:
x = size - 1;
break;
default:
break;
}
switch (rdY)
{
case 0:
y = 0;
break;
case 1:
y = size - 1;
break;
default:
break;
}
startTileA = Point(x, y);
}
void MapGenerator::setStartTileB()
{
int x, y;
int s = size-1;
if (startTileA.x == 0 && startTileA.Y == 0)
getStartTileA();
switch (startTileA.x)
{
case 0:
x = size - 1;
break;
case s :
x = 0;
break;
default:
break;
}
switch (startTileA.y)
{
case 0:
y = size - 1;
break;
case s - 1:
y = 0;
break;
default:
break;
}
startTileB = Point(x, y);
}*/
/*void MapGenerator::setStartTiles()
{
int xA, yA;
int xB, yB;
int rdX = rand() % 2;
int rdY = rand() % 2;
switch (rdX)
{
case 0:
xA = 0;
xB = size - 1;
break;
case 1:
xA = size - 1;
xB = 0;
break;
default:
break;
}
switch (rdY)
{
case 0:
yA = 0;
yB = size - 1;
break;
case 1:
yA = size - 1;
yB = 0;
break;
default:
break;
}
startTileA = Point(xA, yA);
startTileB = Point(xB, yB);
}*/

//action functions
bool MapGenerator::neighbors(int x1, int y1, int x2, int y2) const
{
	if (x2 - x1 == 1)
	{
		if (y2 - y1 == 0 || y2 - y1 == -1) return true;
	}
	if (x2 - x1 == 0)
	{
		if (y2 - y1 == -1 || y2 - y1 == 1) return true;
	}
	if (x2 - x1 == -1)
	{
		if (y2 - y1 == 0 || y2 - y1 == 1) return true;
	}
	return false;
}
bool MapGenerator::pathLength2(int x1, int y1, int x2, int y2, int expectedTileType) const
{
	int tmpX, tmpY;
	for (int i = -1; i < 2; i++)
	{
		for (int j = -1; i < 2; i++)
		{
			if (neighbors(x1 + j, y1 + i, x2, y2) && storage.at(Point(x1 + j, y1 + i)) == expectedTileType && storage.at(Point(x2, y2)) == expectedTileType) { return true; }
		}
	}
}
bool MapGenerator::canMoveTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const
{
	if (movePoint == 0) { return false; }
	if (unitFaction1 == ELVES && storage.at(Point(x1, y1)) == DESERT) { return false; }
	if (neighbors(x1, y1, x2, y2)) { return true; }
	switch (unitFaction1)
	{
	case DWARVES:
		if (storage.at(Point(x1, y1)) == MOUTAIN && storage.at(Point(x2, y2)) == MOUTAIN && unitFaction2 == NONE) { return true; }
		if (movePoint == 1) { return pathLength2(x1, y1, x2, y2, PLAIN); }
		break;
	case ELVES:
		if (movePoint == 1) { return pathLength2(x1, y1, x2, y2, FOREST); }
		break;
	case ORCS:
		if (movePoint == 1) { return pathLength2(x1, y1, x2, y2, PLAIN); }
		break;
	default:
		break;
	}
	return false;
}
bool MapGenerator::canAttackTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const
{
	if (storage.at(Point(x2, y2)) == NONE) { return false; }
	//specific case for dwarves because they can't attack someone on a distant mountain, even if they are on a mountain ! 
	if (unitFaction1 != DWARVES)
	{
		//the dwarves case appart, the only difference between move or attack is that in the 1st case there isn't a target and in the 2nd, there is
		return canMoveTo(x1, y1, x2, y2, unitFaction1, unitFaction2, movePoint);
	}
	else
	{
		if (movePoint == 0) { return false; }
		if (neighbors(x1, y1, x2, y2)) { return true; }
		if (movePoint == 1) { return pathLength2(x1, y1, x2, y2, PLAIN); }
	}
	return false;
}

//external functions
MapGenerator* createMapGenerator()
{
	return new MapGenerator();
}
void delMapGenerator(MapGenerator* gen)
{
	gen->~MapGenerator();
}
bool externCanMoveTo(MapGenerator* gen, int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint)
{
	return gen->canMoveTo(x1, y1, x2, y2, unitFaction1, unitFaction2, movePoint);
}
bool externCanAttackTo(MapGenerator* gen, int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint)
{
	return gen->canAttackTo(x1, y1, x2, y2, unitFaction1, unitFaction2, movePoint);
}
void externBuildMap(MapGenerator* gen, int size)
{
	gen->buildMap(size);
}