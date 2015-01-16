#ifndef __WRAPPER__
#define __WRAPPER__

#include "../SCvNativeLib/NativeMapBackend.h"
#pragma comment(lib, "../Debug/SCvNativeLib.lib")

using namespace System;
namespace Wrapper
{
	public ref class MapBackend
	{
	private:
		NativeMapBackend* _mapBackend;
	public:
		MapBackend(int size, int seed)
		{
			_mapBackend = NativeMapBackend_new(size, seed);
		}

		~MapBackend()
		{
			NativeMapBackend_delete(_mapBackend);
		}

		void Generate()
		{
			NativeMapBackend_generate(_mapBackend);
		}

		int GetTileType(int x, int y)
		{
			return NativeMapBackend_tileType(_mapBackend, x, y);
		}

		double MoveCost(int faction, int srcX, int srcY, int dstX, int dstY)
		{
			return NativeMapBackend_moveCost(_mapBackend, (FactionType)faction, srcX, srcY, dstX, dstY);
		}

		int StartTileX(int playerId)
		{
			int x, y;
			NativeMapBackend_startTile(_mapBackend, playerId, x, y);
			return x;
		}

		int StartTileY(int playerId)
		{
			int x, y;
			NativeMapBackend_startTile(_mapBackend, playerId, x, y);
			return y;
		}

		int DistanceTo(int srcX, int srcY, int dstX, int dstY)
		{
			return NativeMapBackend_distanceTo(_mapBackend, srcX, srcY, dstX, dstY);
		}
	};
}
#endif