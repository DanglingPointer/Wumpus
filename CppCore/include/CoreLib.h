#pragma once

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

    CPPCORE_API int IsWumpus(unsigned int row, unsigned int col);

    CPPCORE_API int IsGold(unsigned int row, unsigned int col);

    CPPCORE_API int IsStench(unsigned int row, unsigned int col);

    CPPCORE_API int IsBreeze(unsigned int row, unsigned int col);

    CPPCORE_API int IsPit(unsigned int row, unsigned int col);

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

    // UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3
    CPPCORE_API unsigned int AgentFacing();

    CPPCORE_API int AgentHasArrow();

    CPPCORE_API long long GetScore();

    // ----------------------------------------------------------------------

    CPPCORE_API unsigned int RowCount();

    CPPCORE_API unsigned int ColCount();

#ifdef __cplusplus
}
#endif