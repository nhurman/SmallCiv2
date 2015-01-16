#pragma once
#include <vector>

enum FactionType
{
	None = -1,
	Elves,
	Dwarves,
	Orcs
};

enum TileType
{
	Invalid = -1,
	Desert,
	Forest,
	Field,
	Mountain,
};

#ifdef EXPORT_LIB
	#define DLLSPEC __declspec(dllexport)
	
#else
	#define DLLSPEC __declspec(dllimport)
#endif

#define EXTERNC extern "C"

class DLLSPEC NativeMapBackend
{
public:
	NativeMapBackend(int size);
	~NativeMapBackend();

	void generate();
	TileType tileType(int x, int y);
	bool canMoveTo(FactionType faction, int srcX, int srcY, int dstX, int dstY);
	void startTile(int playerId, int& x, int& y);
	int distanceTo(int srcX, int srcY, int dstX, int dstY);

private:
	int m_size;
	std::vector<std::vector<TileType>>* m_tiles;

	void offsetToCube(int x, int y, int& cx, int& cy, int& cz);
	void cubeToOffset(int cx, int cy, int cz, int& x, int& y);
};


EXTERNC NativeMapBackend* NativeMapBackend_new(int size);
EXTERNC void NativeMapBackend_delete(NativeMapBackend* self);
EXTERNC void NativeMapBackend_generate(NativeMapBackend* self);
EXTERNC TileType NativeMapBackend_tileType(NativeMapBackend* self, int x, int y);
EXTERNC bool NativeMapBackend_canMoveTo(NativeMapBackend* self, FactionType faction, int srcX, int srcY, int dstX, int dstY);
EXTERNC void NativeMapBackend_startTile(NativeMapBackend* self, int playerId, int& x, int& y);
EXTERNC int NativeMapBackend_distanceTo(NativeMapBackend* self, int srcX, int srcY, int dstX, int dstY);