#include "NativeMapBackend.h"

NativeMapBackend::NativeMapBackend(int size, int seed) : m_size(size), m_seed(seed)
{
	m_tiles = new std::vector<std::vector<TileType>>();
	(*m_tiles).resize(m_size);
	for (int i = 0; i < m_size; ++i) {
		(*m_tiles)[i].resize(m_size);
	}
}

NativeMapBackend::~NativeMapBackend()
{
	delete m_tiles;
}


void NativeMapBackend::generate()
{
	for (int y = 0; y < m_size; ++y) {
		for (int x = 0; x < m_size; ++x) {
			(*m_tiles)[y][x] = TileType::Field;
		}
	}

	for (int x = 0; x < m_size; ++x) {
		(*m_tiles)[2][x] = TileType::Desert;
	}
}

TileType NativeMapBackend::tileType(int x, int y)
{
	return (*m_tiles)[y][x];
}

float NativeMapBackend::moveCost(FactionType faction, int srcX, int srcY, int dstX, int dstY)
{
	int distance = distanceTo(srcX, srcY, dstX, dstY);

	if (faction == FactionType::Dwarves && tileType(srcX, srcY) == TileType::Mountain && tileType(dstX, dstY) == TileType::Mountain)
		return 0;

	if (distance > 1) { return 1000; } // FIXME Pathfinding

	if (faction == FactionType::Elves && tileType(srcX, srcY) == TileType::Forest)
		return 0.5;

	if (faction == FactionType::Elves && tileType(srcX, srcY) == TileType::Desert)
		return 2;

	if (faction == FactionType::Orcs && tileType(srcX, srcY) == TileType::Field)
		return 0.5;

	if (faction == FactionType::Dwarves && tileType(srcX, srcY) == TileType::Field)
		return 0.5;

	return 1.;
}

void NativeMapBackend::startTile(int playerId, int& x, int& y)
{
	if (playerId == 0) {
		x = 4;
		y = 0;
	}
	else {
		x = 0;
		y = 4;
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

void NativeMapBackend::cubeToOffset(int cx, int cy, int cz, int& x, int& y)
{
	x = cx;
	y = cz + (cx - (cx & 1)) / 2;
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