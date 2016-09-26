#pragma once

#include <SDKDDKVer.h>

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers

#include <windows.h>

#ifdef CPPCORE_EXPORTS
# define CPPCORE_API __declspec(dllexport)
#else
# define CPPCORE_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C"
{
#endif
    CPPCORE_API void GameInit();

    CPPCORE_API bool IsWumpus(unsigned int row, unsigned int col);

    CPPCORE_API bool IsGold(unsigned int row, unsigned int col);

    CPPCORE_API bool IsStench(unsigned int row, unsigned int col);

    CPPCORE_API bool IsBreeze(unsigned int row, unsigned int col);

    CPPCORE_API bool IsPit(unsigned int row, unsigned int col);

    CPPCORE_API unsigned int WumpusRow();

    CPPCORE_API unsigned int WumpusCol();

    CPPCORE_API unsigned int GoldRow();

    CPPCORE_API unsigned int GoldCol();

    CPPCORE_API int IsWumpusKilled();

    CPPCORE_API int IsGoldGrabbed();

    // ----------------------------------------------------------------------

    CPPCORE_API void AgentMove();

    CPPCORE_API void AgentShoot();

    CPPCORE_API void AgentGrab();

    CPPCORE_API void AgentDie();

    CPPCORE_API void AgentTurnRight();

    CPPCORE_API void AgentTurnLeft();

    CPPCORE_API unsigned int AgentRow();

    CPPCORE_API unsigned int AgentCol();

    CPPCORE_API unsigned int AgentFacing();

    CPPCORE_API long long GetScore();

#ifdef __cplusplus
}
#endif