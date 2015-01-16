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
		MapBackend(int size)
		{
			_mapBackend = NativeMapBackend_new(size);
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

		bool CanMoveTo(int faction, int srcX, int srcY, int dstX, int dstY)
		{
			return NativeMapBackend_canMoveTo(_mapBackend, (FactionType)faction, srcX, srcY, dstX, dstY);
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