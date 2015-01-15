#ifndef __WRAPPER__
#define __WRAPPER__

#include "../SCvNativeLib/MapGenerator.h"

#pragma comment(lib, "nativeLib.lib")

using namespace System;

namespace Wrapper
{
	public ref class WrapperMapGenerator{
	private:
		MapGenerator* gen;
	public:
		WrapperMapGenerator(){ gen = createMapGenerator(); }
		~WrapperMapGenerator(){ delMapGenerator(gen); }
		bool externCanMoveTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint) { return gen->canMoveTo(x1, y1, x2, y2, unitFaction1, unitFaction2, movePoint); }
		bool externCanAttackTo(int x1, int y1, int x2, int y2, int unitFaction1, int unitFaction2, float movePoint){ return gen->canAttackTo(x1, y1, x2, y2, unitFaction1, unitFaction2, movePoint); }
		void externBuildMap(int size){ gen->buildMap(size); }
	};
}
#endif