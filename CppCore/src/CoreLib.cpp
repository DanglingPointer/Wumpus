// CppCore.cpp : Defines the exported functions for the DLL application.
//
#include <SDKDDKVer.h>
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#include <windows.h>


#include "WumpusCore.h"
#include "CoreLib.h"

#define ROWCOUNT 4
#define COLCOUNT 4


BOOL APIENTRY DllMain(HMODULE hModule,
                      DWORD  ul_reason_for_call,
                      LPVOID lpReserved
)
{
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

typedef World<ROWCOUNT, COLCOUNT> World_t;
typedef Agent<World<ROWCOUNT, COLCOUNT>> Agent_t;

std::shared_ptr<World_t> pWorld;
std::shared_ptr<Agent_t> pAgent;


CPPCORE_API void GameInit()
{
    pWorld = std::make_shared<World_t>();
    pAgent = std::make_shared<Agent_t>(pWorld);
}

CPPCORE_API int IsWumpus(unsigned int row, unsigned int col)
{
    return pWorld->IsWumpus(row, col);
}

CPPCORE_API int IsGold(unsigned int row, unsigned int col)
{
    return pWorld->IsGold(row, col);
}

CPPCORE_API int IsStench(unsigned int row, unsigned int col)
{
    return pWorld->IsStench(row, col);
}

CPPCORE_API int IsBreeze(unsigned int row, unsigned int col)
{
    return pWorld->IsBreeze(row, col);
}

CPPCORE_API int IsPit(unsigned int row, unsigned int col)
{
    return pWorld->IsPit(row, col);
}

CPPCORE_API unsigned int WumpusRow()
{
    return pWorld->WumpusRow();
}

CPPCORE_API unsigned int WumpusCol()
{
    return pWorld->WumpusCol();
}

CPPCORE_API unsigned int GoldRow()
{
    return pWorld->GoldRow();
}

CPPCORE_API unsigned int GoldCol()
{
    return pWorld->GoldCol();
}

CPPCORE_API int IsWumpusKilled()
{
    return pWorld->IsWumpusKilled();
}

CPPCORE_API int IsGoldGrabbed()
{
    return pWorld->IsGoldGrabbed();
}

CPPCORE_API void AgentMove()
{
    pAgent->Move();
}

CPPCORE_API void AgentShoot()
{
    pAgent->Shoot();
}

CPPCORE_API void AgentGrab()
{
    pAgent->Grab();
}

CPPCORE_API void AgentDie()
{
    pAgent->Die();
}

CPPCORE_API void AgentTurnRight()
{
    pAgent->TurnRight();
}

CPPCORE_API void AgentTurnLeft()
{
    pAgent->TurnLeft();
}

CPPCORE_API unsigned int AgentRow()
{
    return pAgent->Row();
}

CPPCORE_API unsigned int AgentCol()
{
    return pAgent->Col();
}

CPPCORE_API unsigned int AgentFacing()
{
    return (uint)pAgent->Facing();
}

CPPCORE_API int AgentHasArrow()
{
    return pAgent->HasArrow();
}

CPPCORE_API long long GetScore()
{
    return pAgent->Score();
}


CPPCORE_API unsigned int RowCount()
{
    return ROWCOUNT;
}

CPPCORE_API unsigned int ColCount()
{
    return COLCOUNT;
}
