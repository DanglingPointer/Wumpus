#pragma once
#include <mutex>
#include <memory>
#include "Util.h"

enum class Square
{
    EMPTY   = 0,
    WUMPUS  = 1 << 0,
    GOLD    = 1 << 1,
    PIT     = 1 << 2,
    STENCH  = 1 << 3,
    BREEZE  = 1 << 4
};

enum class Dirn
{
    UP      = 0,
    RIGHT   = 1,
    DOWN    = 2,
    LEFT    = 3
};

template<uint NROWS, uint NCOLS> class World
{
public:
    World() noexcept
        : m_wrow(0), m_wcol(0), m_grow(0), m_gcol(0)
    {
        for (int row = 0; row < NROWS; ++row) {
            for (int col = 0; col < NCOLS; ++col)
                m_sqrs[row][col] = Square::EMPTY;
        }

        do {
            m_wcol = std::rand() % NCOLS;
            m_wrow = std::rand() % NROWS;
        } while (m_wcol == 0 && m_wrow == 0);
        m_sqrs[m_wrow][m_wcol] |= Square::WUMPUS;
        FillAdjacent(m_wrow, m_wcol, Square::STENCH);

        do {
            m_gcol = std::rand() % NCOLS;
            m_grow = std::rand() % NROWS;
        } while (m_gcol == 0 && m_grow == 0);
        m_sqrs[m_grow][m_gcol] |= Square::GOLD;

        int rand;
        for (uint row = 0; row < NROWS; ++row) {
            for (uint col = 0; col < NCOLS; ++col) {
                if (!(row == m_grow && col == m_gcol) && !(row == 0 && col == 0)) {
                    rand = std::rand() % 5;
                    if (rand == 4) {
                        m_sqrs[row][col] |= Square::PIT;
                        FillAdjacent(row, col, Square::BREEZE);
                    }
                }
            }
        }
    }
    uint WumpusRow() const noexcept
    {
        return m_wrow;
    }
    uint WumpusCol() const noexcept
    {
        return m_wcol;
    }
    uint GoldRow() const noexcept
    {
        return m_grow;
    }
    uint GoldCol() const noexcept
    {
        return m_gcol;
    }
    bool IsWumpusKilled() const noexcept
    {
        std::lock_guard<std::mutex> lg(m_lock);
        return !IsWumpus(m_wrow, m_wcol);
    }
    bool IsGoldGrabbed() const noexcept
    {
        std::lock_guard<std::mutex> lg(m_lock);
        return !IsGold(m_grow, m_gcol);
    }
    bool IsWumpus(uint row, uint col) const noexcept
    {
        return Is(Square::WUMPUS, row, col);
    }
    bool IsGold(uint row, uint col) const noexcept
    {
        return Is(Square::GOLD, row, col);
    }
    bool IsPit(uint row, uint col) const noexcept
    {
        return Is(Square::PIT, row, col);
    }
    bool IsStench(uint row, uint col) const noexcept
    {
        return Is(Square::STENCH, row, col);
    }
    bool IsBreeze(uint row, uint col) const noexcept
    {
        return Is(Square::BREEZE, row, col);
    }
    bool TryKillWumpus(uint row, uint col)
    {
        if (!IsWumpus(row, col))
            return false;
        std::lock_guard<std::mutex> lg(m_lock);
        m_sqrs[row][col] ^= Square::WUMPUS;
        return true;
    }
    bool TryGrabGold(uint row, uint col)
    {
        if (!IsGold(row, col))
            return false;
        std::lock_guard<std::mutex> lg(m_lock);
        m_sqrs[row][col] ^= Square::GOLD;
        return true;
    }
private:
    void FillAdjacent(uint row, uint col, Square value)
    {
        if (row != 0)           m_sqrs[row - 1][col] |= value;
        if (row + 1 != NROWS)   m_sqrs[row + 1][col] |= value;
        if (col != 0)           m_sqrs[row][col - 1] |= value;
        if (col + 1 != NCOLS)   m_sqrs[row][col + 1] |= value;
    }
    bool Is(Square what, uint row, uint col) const noexcept
    {
        if (row < NROWS && col < NCOLS)
            return (m_sqrs[row][col] & what) == what;
        return false;
    }
    Square m_sqrs[NROWS][NCOLS];
    uint m_wrow, m_wcol, m_grow, m_gcol;
    mutable std::mutex m_lock;
};

template <class TWorld> class Agent;

template <uint NROWS, uint NCOLS>
class Agent<World<NROWS, NCOLS>>
{
    typedef World<NROWS, NCOLS> World_t;
public:
    explicit Agent(std::shared_ptr<World_t> pworld) noexcept
        :m_pworld(std::move(pworld)), m_row(0), m_col(0), m_hasArr(true), 
        m_facing(Dirn::RIGHT), m_pts(0)
    { }
    Dirn Facing() const noexcept { return m_facing; }
    uint Row() const noexcept { return m_row; }
    uint Col() const noexcept { return m_col; }
    bool HasArrow() const noexcept { return m_hasArr; }
    void Move() noexcept
    {
        int colDist = 0;
        if (m_facing == Dirn::RIGHT && m_col != NCOLS - 1)
            colDist = 1;
        else if (m_facing == Dirn::LEFT && m_col != 0)
            colDist = -1;

        int rowDist = 0;
        if (m_facing == Dirn::UP && m_row != NROWS - 1)
            rowDist = 1;
        else if (m_facing == Dirn::DOWN && m_row != 0)
            rowDist = -1;

        m_col += colDist;
        m_row += rowDist;

        --m_pts;
        if (m_col == 0 && m_row == 0 && m_pworld->IsGoldGrabbed())
            m_pts += 1000;
    }
    void TurnRight() noexcept 
    { 
        m_facing = (Dirn)(((int)m_facing + 1) % 4);
        --m_pts;
    }
    void TurnLeft() noexcept
    {
        m_facing = (Dirn)(((int)m_facing + 3) % 4);
        --m_pts;
    }
    void Shoot()
    {
        if (m_hasArr) {
            bool killed = false;
            if (m_facing == Dirn::UP) {
                for (int row = m_row + 1; !killed && row < NROWS; ++row)
                    killed = m_pworld->TryKillWumpus(row, m_col);
            }
            else if (m_facing == Dirn::DOWN) {
                for (int row = m_row - 1; !killed && row >= 0; --row)
                    killed = m_pworld->TryKillWumpus(row, m_col);
            }
            else if (m_facing == Dirn::RIGHT) {
                for (int col = m_col + 1; !killed && col < NCOLS; ++col)
                    killed = m_pworld->TryKillWumpus(m_row, col);
            }
            else {
                for (int col = m_col - 1; !killed && col >= 0; --col)
                    killed = m_pworld->TryKillWumpus(m_row, col);
            }
            m_hasArr = false;
            m_pts -= 10;
        }
    }
    bool Grab() 
    { 
        --m_pts;
        return m_pworld->TryGrabGold(m_row, m_col); 
    }
    void Die() noexcept
    {
        m_row = m_col = 0;
        m_hasArr = false;
        m_facing = Dirn::RIGHT;
        m_pts = 0;
    }
    std::int64_t Score() const noexcept
    {
        return m_pts;
    }

private:
    std::shared_ptr<World_t> m_pworld;
    uint m_row, m_col;
    bool m_hasArr;
    Dirn m_facing;
    std::int64_t m_pts;

};