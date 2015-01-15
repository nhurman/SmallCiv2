#ifdef WANTDLLEXP
#define DLL _declspec(dllexport)
#define EXTERNC extern "C"
#else
#define DLL
#define EXTERNC
#endif
#include<map>

enum Faction { NONE = -1, ELVES, ORCS, DWARVES };
enum TileType { INVALID = -1, DESERT, MOUTAIN, PLAIN, FOREST };

struct Point
{
public:
	Point() : x(0), y(0) {}
	Point(int inX, int inY) : x(inX), y(inY) {}
	int x, y;
};

typedef std::map<Point, int> mapStorage;

class DLL MapGenerator
{
	mapStorage storage;
	int size;
	/*Point startTileA;
	Point startTileB;*/
public:
	~MapGenerator();

	void buildMap(int size/*, int rndSeed*/);

	//setters
	/*void setStartTileA();
	void setStartTileB();
	void setStartTiles();*/

	//getters
	int getTileType(int x, int y) const;
	//Point getStartTileA(/*int playerId*/) const;
	//Point getStartTileB(/*int playerId*/) const;

	//action functions
	bool neighbors(int x1, int y1, int x2, int y2) const;
	bool pathLength2(int x1, int y1, int x2, int y2, int tileType) const;
	bool canMoveTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const;
	bool canAttackTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const;
};

EXTERNC DLL MapGenerator* createMapGenerator();
EXTERNC DLL void delMapGenerator(MapGenerator* gen);
EXTERNC DLL bool externCanMoveTo(MapGenerator* gen, int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint);
EXTERNC DLL bool externCanAttackTo(MapGenerator* gen, int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint);
EXTERNC DLL void externBuildMap(MapGenerator* gen, int size);