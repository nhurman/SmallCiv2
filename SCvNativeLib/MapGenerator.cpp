#include "MapGenerator.h"
#include <iostream>
#include <exception>
#include <vector>
#include <ctime>

MapGenerator::~MapGenerator()
{
	m_storage.~vector();
	m_probas.~vector();
	m_modified.~vector();
}

/**
* Map generation methods
*/
/*
* This function sets an equal number of each tile type randomly.
* The coordinates go from -(size/2) to (size/2)-1, with the origin (0,0) being at the center of the map (axial coordinates).
* The reason to do this instead of putting the origin in the uppter left corner is to have easier to write function late on, like pathfinding.
* By example on the demo map 6x6, x and y go from -3 to 2.
*/
void MapGenerator::buildMap(int size)
{
	m_size = size;
	int nbTiles = m_size*m_size;
	int nbDesertLeft = nbTiles / 4;
	int nbPlainLeft = nbTiles / 4;
	int nbForestLeft = nbTiles / 4;
	int nbMountainLeft = nbTiles / 4;
	initializeModifiedToFalse();
	initializeProbasTo25();
	setProbas();
	setStartTiles();
	srand(time(0));
	int rd, type;
	Point tmp;
	m_storage.resize(m_size);
	for (int x = 0; x < m_size; x++)
	{
		m_storage[x].resize(m_size);
		for (int y = 0; y < m_size; y++)
		{
			rd = rand()%100;
			type = selectType(rd, Point{ x, y });
			switch (type)
			{
			case DESERT:
				if (nbDesertLeft == 0)
				{
					if (m_probas[x][y][DESERT] == 100) { setProbas(tmp, DESERT, 0); }
					y--;
					continue;
				}
				else
				{
					m_storage[x][y] = DESERT;
					nbDesertLeft--;
				}
				break;
			case FOREST:
				if (nbForestLeft == 0)
				{
					if (m_probas[x][y][FOREST] == 100) { setProbas(tmp, FOREST, 0); }
					y--;
					continue;
				}
				else
				{
					m_storage[x][y] = FOREST;
					nbForestLeft--;
				}
				break;
			case MOUNTAIN:
				if (nbMountainLeft == 0)
				{
					if (m_probas[x][y][MOUNTAIN] == 100) { setProbas(tmp, MOUNTAIN, 0); }
					y--;
					continue;
				}
				else
				{
					m_storage[x][y] = MOUNTAIN;
					nbMountainLeft--;
				}
				break;
			case PLAIN:
				if (nbPlainLeft == 0)
				{
					if (m_probas[x][y][PLAIN] == 100) { setProbas(tmp, PLAIN, 0); }
					y--;
					continue;
				}
				else
				{
					m_storage[x][y] = PLAIN;
					nbPlainLeft--;
				}
				break;
			default:
				break;
			}
		}
	}
}
/**
* Once the initial state is set, we modify the probabilites of each tile to be of a certain type
* In a first time we select a random tile and check if it has already been modified or not
* If it hasn't, we affect it a 100% proba to be of a given type, and we raise the probability of the nearest tiles to be of the same type (the proba greadually decreases with distance)
* Repeat until all tiles have been modified 
*/
void MapGenerator::setProbas()
{
	srand(time(0));
	int x, y;
	int type;
	//Point coord;
	while (m_leftToModify > 0)
	{
		x = rand() % m_size;
		y = rand() % m_size;
		Point coord = { x, y };
		if (!m_modified[x][y])
		{
			type = rand() % 4;
			setProbas(coord, type, 100);
			m_modified[x][y] = true;
			m_leftToModify--;
			expandProbaToNeighbors(coord, 80, type);
		}
	}
}
/**
* Initialy, all tiles are set to unmodified (modified = false)
*/
void MapGenerator::initializeModifiedToFalse()
{
	m_modified.resize(m_size);
	for (int x = 0; x < m_size; x++)
	{
		m_modified[x].resize(m_size);
		for (int y = 0; y < m_size ; y++)
		{
			m_modified[x].push_back(false);
		}
	}
	m_leftToModify = m_size*m_size;
}
/**
* Initialy, all tiles have the same probability to be of each type 
*/
void MapGenerator::initializeProbasTo25()
{
	int tmp[4];
	m_probas.resize(m_size);
	for (int i = 0; i < 4; i++) { tmp[i] = 25; }
	for (int x =0; x < m_size  ; x++)
	{
		m_probas[x].resize(m_size);
		for (int y = 0 ; y < m_size ; y++)
		{
			m_probas[x][y].resize(4);
			for (int i = 0; i < 4; i++) { m_probas[x][y][i] = 25; }
		}
	}
}
/**
* Sets the proba of the tiles sourrounding the origin to be of "type" at the given value
* Marks all these tiles as modified
* Then calls itself on every modified tile with a decreased proba until reaching a 40% proba
*/
void MapGenerator::expandProbaToNeighbors(Point origin, int proba, int type)
{
	int a, b;
	for (int y = -1; y < 2; y++)
	{
		for (int x = -1; x < 2; x++)
		{
			a = origin.m_x + x;
			b = origin.m_y + y;
			Point tmp = { a, b };
			if (a>=0 && a<m_size && b>=0 && b<m_size && !m_modified[a][b])
			{
				m_modified[origin.m_x + x][ origin.m_y + y] = true;
				setProbas(tmp, type, proba);
				m_leftToModify--;
				if(proba>=60) expandProbaToNeighbors(tmp, proba - 20, type);
			}
		}
	}
}
/**
* Given roll being a number randomly generated between 0 and 99, returns the type that will be affected at the tile at position Pos
*/
int MapGenerator::selectType(int roll, Point pos)
{
	int cumulativeProbas[4];
	cumulativeProbas[0] = m_probas[pos.m_x][pos.m_y][0];
	cumulativeProbas[1] = cumulativeProbas[0] + m_probas[pos.m_x][pos.m_y][1];
	cumulativeProbas[2] = cumulativeProbas[1] + m_probas[pos.m_x][pos.m_y][2];
	cumulativeProbas[3] = cumulativeProbas[2] + m_probas[pos.m_x][pos.m_y][3];
	for (int i = 0; i < 4; i++)
	{
		if (roll < cumulativeProbas[i] - 1)
		{
			return i;
		}
	}
	return -1;
}

//getters
int MapGenerator::getTileType(int x, int y) const
{
	if (x<-(m_size / 2) || x>(m_size / 2) - 1 || y<-(m_size / 2) || y>(m_size / 2) - 1) {return INVALID;}
	return m_storage[x][y];
}
Point MapGenerator::getStartTileA() const
{
	return m_startTileA;
}
Point MapGenerator::getStartTileB() const
{
	return m_startTileB;
}

//setters
void MapGenerator::setStartTiles()
{
	int xA, yA;
	int xB, yB;
	srand(0);
	int rdX = rand() % 2;
	int rdY = rand() % 2;
	switch (rdX)
	{
	case 0:
		xA = 0;
		xB = m_size - 1;
		break;
	case 1:
		xA = m_size - 1;
		xB = 0;
		break;
	default:
		break;
	}
	switch (rdY)
	{
	case 0:
		yA = 0;
		yB = m_size - 1;
		break;
	case 1:
		yA = m_size - 1;
		yB = 0;
		break;
	default:
		break;
	}
	m_startTileA = { xA, yA };
	m_startTileB = { xB, yB };
}
void MapGenerator::setProbas(Point p, int type, int proba)
{
	int count = 0;
	for (int i = 0; i < 4; i++) 
	{
		if (i == type) m_probas[p.m_x][p.m_y][i] = proba;
		else m_probas[p.m_x][p.m_y][i] = (100 - proba) / 3;
		count += m_probas[p.m_x][p.m_y][i];
	}
	if (count < 100)
	{
		for (int i = 0; i < 4; i++)
		{
			if (m_probas[p.m_x][p.m_y][i] != 0) { m_probas[p.m_x][p.m_y][i] += (100 - count); }
		}
	}
}

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
bool MapGenerator::reachableThanksToSpecialMovment(int x1, int y1, int x2, int y2, int expectedTileType) const
{
	for (int i = -1; i < 2; i++)
	{
		for (int j = -1; i < 2; i++)
		{
			if (neighbors(x1 + j, y1 + i, x2, y2) && m_storage[x1 + j][y1 + i] == expectedTileType && m_storage[x2][y2] == expectedTileType) { return true; }
		}
	}
	return false;
}
bool MapGenerator::canMoveTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const
{
	if (movePoint == 0) { return false; }
	if (unitFaction1 == ELVES && m_storage[ x1][ y1 ] == DESERT) { return false; }
	if (neighbors(x1, y1, x2, y2)) { return true; }
	switch (unitFaction1)
	{
	case DWARVES:
		if (m_storage[x1][y1] == MOUNTAIN && m_storage[ x2][y2] == MOUNTAIN && unitFaction2 == NONE) { return true; }
		if (movePoint == 1) { return reachableThanksToSpecialMovment(x1, y1, x2, y2, PLAIN); }
		break;
	case ELVES:
		if (movePoint == 1) { return reachableThanksToSpecialMovment(x1, y1, x2, y2, FOREST); }
		break;
	case ORCS:
		if (movePoint == 1) { return reachableThanksToSpecialMovment(x1, y1, x2, y2, PLAIN); }
		break;
	default:
		break;
	}
	return false;
}
bool MapGenerator::canAttackTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const
{
	if (m_storage[x2][y2] == NONE) { return false; }
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
		if (movePoint == 1) { return reachableThanksToSpecialMovment(x1, y1, x2, y2, PLAIN); }
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
	delete gen;
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
Point externGetStartTileA(){ return externGetStartTileA(); }
Point externGetStartTileB(){ return externGetStartTileB(); }

