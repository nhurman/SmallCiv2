#ifdef WANTDLLEXP
#define DLL _declspec(dllexport)
#define EXTERNC extern "C"
#else
#define DLL
#define EXTERNC
#endif
#include<vector>

using namespace std;

enum Faction { NONE = -1, ELVES, ORCS, DWARVES };
enum TileType { INVALID = -1, DESERT, MOUNTAIN, PLAIN, FOREST };

/**
* A position okn the map
*/
struct DLL Point
{
public:
	Point(int x=0, int y=0){ m_x = x; m_y = y; }
	int m_x = 0;
	int m_y = 0;
};
/**
* Associated with a tile on the map, it will set the probability for this tile to be of a given type
*/
struct ProbasSet
{
public:
	int m_D = 25;
	int m_M = 25;
	int m_P = 25;
	int m_F = 25;
};

class DLL MapGenerator
{
public:
	vector < vector<TileType> > m_storage;
	vector < vector<vector<int>> > m_probas;
	vector < vector<bool> > m_modified;
	int m_size;
	Point m_startTileA;
	Point m_startTileB;
	int m_leftToModify;
//public:
	~MapGenerator();

	//map generation functions
	void buildMap(int size);
	void setProbas();
	void initializeModifiedToFalse();
	void initializeProbasTo25();
	void expandProbaToNeighbors(Point origine, int proba, int type);
	int selectType(int roll, Point pos);

	//setters
	void setStartTiles();
	void setProbas(Point p, int type, int proba);

	//getters
	int getTileType(int x, int y) const;
	Point getStartTileA() const;
	Point getStartTileB() const;

	//action functions
	bool neighbors(int x1, int y1, int x2, int y2) const;
	bool reachableThanksToSpecialMovment(int x1, int y1, int x2, int y2, int tileType) const;
	bool canMoveTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const;
	bool canAttackTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) const;
};

EXTERNC DLL MapGenerator* createMapGenerator();
EXTERNC DLL void delMapGenerator(MapGenerator* gen);
EXTERNC DLL bool externCanMoveTo(MapGenerator* gen, int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint);
EXTERNC DLL bool externCanAttackTo(MapGenerator* gen, int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint);
EXTERNC DLL void externBuildMap(MapGenerator* gen, int size);
EXTERNC DLL Point externGetStartTileA();
EXTERNC DLL Point externGetStartTileB();