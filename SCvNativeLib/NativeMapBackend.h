#pragma once
#include <vector>
using namespace std;

enum FactionType
{
	None = -1,
	Elves = 0,
	Dwarves = 1,
	Orcs = 2
};

enum TileType
{
	Invalid = -1,
	Desert = 0,
	Forest = 1,
	Field = 2,
	Mountain = 3,
};

#ifdef EXPORT_LIB
	#define DLLSPEC __declspec(dllexport)
#else
	#define DLLSPEC __declspec(dllimport)
#endif

struct Point { int x; int y; Point(int a=0, int b=0) :x(a), y(b) {} };

class DLLSPEC NativeMapBackend
{
public:
	NativeMapBackend(int size, int seed);
	~NativeMapBackend();

	void generate();
	TileType tileType(int x, int y);
	float moveCost(FactionType faction, int srcX, int srcY, int dstX, int dstY);
	void startTile(int playerId, int& x, int& y);
	int distanceTo(int srcX, int srcY, int dstX, int dstY);

private:
	int m_size;
	int m_seed;

	vector<vector<TileType>>* m_storage;
	vector<vector<vector<int>>>* m_probas;
	vector<vector<bool>>* m_modified;

	Point m_startTileA;
	Point m_startTileB;

	int m_leftToModify;

	void offsetToCube(int x, int y, int& cx, int& cy, int& cz);

	void setProbas();
	void setProbas(Point p, int type, int proba);
	void initializeModifiedToFalse();
	void initializeProbasTo25();
	void setStartTiles();
	int selectType(int roll, Point pos);
	void expandProbaToNeighbors(Point origine, int proba, int type);
};

extern "C"
{
	NativeMapBackend* NativeMapBackend_new(int size, int seed);
	void NativeMapBackend_delete(NativeMapBackend* self);
	void NativeMapBackend_generate(NativeMapBackend* self);
	TileType NativeMapBackend_tileType(NativeMapBackend* self, int x, int y);
	double NativeMapBackend_moveCost(NativeMapBackend* self, FactionType faction, int srcX, int srcY, int dstX, int dstY);
	void NativeMapBackend_startTile(NativeMapBackend* self, int playerId, int& x, int& y);
	int NativeMapBackend_distanceTo(NativeMapBackend* self, int srcX, int srcY, int dstX, int dstY);
}