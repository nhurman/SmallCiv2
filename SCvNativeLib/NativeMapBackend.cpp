#include "NativeMapBackend.h"

NativeMapBackend::NativeMapBackend(int size, int seed) : m_size(size), m_seed(seed)
{
	srand(m_seed);

	m_storage = new std::vector<std::vector<TileType>>();
	m_probas = new std::vector<std::vector<vector<int>>>();
	m_modified = new std::vector<std::vector<bool>>();
}

NativeMapBackend::~NativeMapBackend()
{
	delete m_storage;
	delete m_probas;
	delete m_modified;
}


void NativeMapBackend::generate()
{
	int nbTiles = m_size*m_size;
	int nbDesertLeft = nbTiles / 4;
	int nbPlainLeft = nbTiles / 4;
	int nbForestLeft = nbTiles / 4;
	int nbMountainLeft = nbTiles / 4;
	initializeModifiedToFalse();
	initializeProbasTo25();
	setProbas();
	setStartTiles();
	int rd, type;
	int px = 0, py = 0;
	m_storage->resize(m_size);
	for (int y = 0; y < m_size; y++)
	{
		(*m_storage)[y].resize(m_size);
		for (int x = 0; x < m_size; x++)
		{
			rd = rand() % 100;
			type = selectType(rd, x, y);
			switch (type)
			{
			case TileType::Desert:
				if (nbDesertLeft == 0)
				{
					if ((*m_probas)[y][x][TileType::Desert] == 100) { setProbas(px, py, TileType::Desert, 0); }
					x--;
					continue;
				}
				else
				{
					(*m_storage)[y][x] = TileType::Desert;
					nbDesertLeft--;
				}
				break;
			case TileType::Forest:
				if (nbForestLeft == 0)
				{
					if ((*m_probas)[y][x][TileType::Forest] == 100) { setProbas(px, py, TileType::Forest, 0); }
					x--;
					continue;
				}
				else
				{
					(*m_storage)[y][x] = TileType::Forest;
					nbForestLeft--;
				}
				break;
			case TileType::Mountain:
				if (nbMountainLeft == 0)
				{
					if ((*m_probas)[y][x][TileType::Mountain] == 100) { setProbas(px, py, TileType::Mountain, 0); }
					x--;
					continue;
				}
				else
				{
					(*m_storage)[y][x] = TileType::Mountain;
					nbMountainLeft--;
				}
				break;
			case TileType::Field:
				if (nbPlainLeft == 0)
				{
					if ((*m_probas)[y][x][TileType::Field] == 100) { setProbas(px, py, TileType::Field, 0); }
					x--;
					continue;
				}
				else
				{
					(*m_storage)[y][x] = TileType::Field;
					nbPlainLeft--;
				}
				break;
			default:
				break;
			}
		}
	}

}

TileType NativeMapBackend::tileType(int x, int y)
{
	return (*m_storage)[y][x];
}

float NativeMapBackend::moveCost(FactionType faction, int srcX, int srcY, int dstX, int dstY)
{
	int distance = distanceTo(srcX, srcY, dstX, dstY);

	if (faction == FactionType::Dwarves && tileType(srcX, srcY) == TileType::Mountain && tileType(dstX, dstY) == TileType::Mountain)
		return 0;

	if (distance > 1) { return 1000; } // FIXME Pathfinding

	if (faction == FactionType::Elves && tileType(dstX, dstY) == TileType::Forest)
		return 0.5;

	if (faction == FactionType::Elves && tileType(dstX, dstY) == TileType::Desert)
		return 2;

	if (faction == FactionType::Orcs && tileType(dstX, dstY) == TileType::Field)
		return 0.5;

	if (faction == FactionType::Dwarves && tileType(dstX, dstY) == TileType::Field)
		return 0.5;

	return 1.;
}

void NativeMapBackend::startTile(int playerId, int& x, int& y)
{
	if (playerId == 0) {
		x = m_startTileAX;
		y = m_startTileAY;
	}
	else {
		x = m_startTileBX;
		y = m_startTileBY;
	}
}

int NativeMapBackend::distanceTo(int srcX, int srcY, int dstX, int dstY)
{
	int sX, sY, sZ, dX, dY, dZ;
	offsetToCube(srcX, srcY, sX, sY, sZ);
	offsetToCube(dstX, dstY, dX, dY, dZ);

	return (abs(sX - dX) + abs(sY - dY) + abs(sZ - dZ)) / 2;
}


void NativeMapBackend::offsetToCube(int x, int y, int& cx, int& cy, int& cz)
{
	cx = x;
	cz = y - (x - (x & 1)) / 2;
	cy = -cx - cz;
}

/**
* Once the initial state is set, we modify the probabilites of each tile to be of a certain type
* In a first time we select a random tile and check if it has already been modified or not
* If it hasn't, we affect it a 100% proba to be of a given type, and we raise the probability of the
* nearest tiles to be of the same type (the proba greadually decreases with distance)
* Repeat until all tiles have been modified
*/
void NativeMapBackend::setProbas()
{
	int x, y;
	int type;

	while (m_leftToModify > 0)
	{
		x = rand() % m_size;
		y = rand() % m_size;
		if (!(*m_modified)[y][x])
		{
			type = rand() % 4;
			setProbas(x, y, type, 100);
			(*m_modified)[y][x] = true;
			m_leftToModify--;
			expandProbaToNeighbors(x, y, 80, type);
		}
	}
}

/**
* Initialy, all tiles are set to unmodified (modified = false)
*/
void NativeMapBackend::initializeModifiedToFalse()
{
	m_modified->resize(m_size);
	for (int y = 0; y < m_size; y++)
	{
		(*m_modified)[y].resize(m_size);
		for (int x = 0; x < m_size; x++)
		{
			(*m_modified)[y].push_back(false);
		}
	}
	m_leftToModify = m_size*m_size;
}

/**
* Initialy, all tiles have the same probability to be of each type
*/
void NativeMapBackend::initializeProbasTo25()
{
	int tmp[4];
	m_probas->resize(m_size);
	for (int i = 0; i < 4; i++) { tmp[i] = 25; }
	for (int y = 0; y < m_size; y++)
	{
		(*m_probas)[y].resize(m_size);
		for (int x = 0; x < m_size; x++)
		{
			(*m_probas)[y][x].resize(4);
			for (int i = 0; i < 4; i++) { (*m_probas)[y][x][i] = 25; }
		}
	}
}
/**
* Sets the proba of the tiles sourrounding the origin to be of "type" at the given value
* Marks all these tiles as modified
* Then calls itself on every modified tile with a decreased proba until reaching a 40% proba
*/
void NativeMapBackend::expandProbaToNeighbors(int px, int py, int proba, int type)
{
	int a, b;
	for (int y = -1; y < 2; y++)
	{
		for (int x = -1; x < 2; x++)
		{
			a = px + x;
			b = py + y;
			int tx = 0, ty = 0;
			if (a >= 0 && a<m_size && b >= 0 && b<m_size && !(*m_modified)[b][a])
			{
				(*m_modified)[py + y][px + x] = true;
				setProbas(tx, ty, type, proba);
				m_leftToModify--;
				if (proba >= 60) expandProbaToNeighbors(tx, ty, proba - 20, type);
			}
		}
	}
}
/**
* Given roll being a number randomly generated between 0 and 99, returns the type that will be affected at the tile at position Pos
*/
int NativeMapBackend::selectType(int roll, int px, int py)
{
	int cumulativeProbas[4];
	cumulativeProbas[0] = (*m_probas)[py][px][0];
	cumulativeProbas[1] = cumulativeProbas[0] + (*m_probas)[py][px][1];
	cumulativeProbas[2] = cumulativeProbas[1] + (*m_probas)[py][px][2];
	cumulativeProbas[3] = cumulativeProbas[2] + (*m_probas)[py][px][3];
	for (int i = 0; i < 4; i++)
	{
		if (roll < cumulativeProbas[i] - 1)
		{
			return i;
		}
	}
	return -1;
}

void NativeMapBackend::setStartTiles()
{
	int xA, yA;
	int xB, yB;
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
	m_startTileAX = xA; m_startTileAY = yA;
	m_startTileBX = xB; m_startTileBY = yB;
}

void NativeMapBackend::setProbas(int px, int py, int type, int proba)
{
	int count = 0;
	for (int i = 0; i < 4; i++)
	{
		if (i == type) (*m_probas)[py][px][i] = proba;
		else (*m_probas)[py][px][i] = (100 - proba) / 3;
		count += (*m_probas)[py][px][i];
	}
	if (count < 100)
	{
		for (int i = 0; i < 4; i++)
		{
			if ((*m_probas)[py][px][i] != 0) { (*m_probas)[py][px][i] += (100 - count); }
		}
	}
}


//////////////////

NativeMapBackend* NativeMapBackend_new(int size, int seed)
{
	return new NativeMapBackend(size, seed);
}

void NativeMapBackend_delete(NativeMapBackend* self)
{
	delete self;
}

void NativeMapBackend_generate(NativeMapBackend* self)
{
	return self->generate();
}

TileType NativeMapBackend_tileType(NativeMapBackend* self, int x, int y)
{
	return self->tileType(x, y);
}

double NativeMapBackend_moveCost(NativeMapBackend* self, FactionType faction, int srcX, int srcY, int dstX, int dstY)
{
	return self->moveCost(faction, srcX, srcY, dstX, dstY);
}

void NativeMapBackend_startTile(NativeMapBackend* self, int playerId, int& x, int& y)
{
	return self->startTile(playerId, x, y);
}

int NativeMapBackend_distanceTo(NativeMapBackend* self, int srcX, int srcY, int dstX, int dstY)
{
	return self->distanceTo(srcX, srcY, dstX, dstY);
}