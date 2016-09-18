using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Flags]
    enum Cell
    {
        None    = 0,
        Wumpus  = (1 << 1),
        Gold    = (1 << 2),
        Pit     = (1 << 3),
        Stench  = (1 << 4),
        Breeze  = (1 << 5)
    }
    public enum Dirn
    {
        Up = 0, Right = 1, Down = 2, Left = 3
    }

    public static class World
    {
        public const int NROWS = 4;
        public const int NCOLS = 4;

        public static event Action WumpusKilled;
        public static event Action GoldGrabbed;
        static World()
        {
            do {
                WumpusCol = m_rand.Next(0, NCOLS);
                WumpusRow = m_rand.Next(0, NROWS);
            } while (WumpusRow == 0 && WumpusCol == 0);
            m_cells[WumpusRow, WumpusCol] |= Cell.Wumpus;
            FillAdjacent(WumpusRow, WumpusCol, Cell.Stench);

            
            do {
                GoldCol = m_rand.Next(0, NCOLS);
                GoldRow = m_rand.Next(0, NROWS);
            } while (GoldRow == 0 && GoldCol == 0);
            m_cells[GoldRow, GoldCol] |= Cell.Gold;

            int rand;
            for (int row = 0; row < NROWS; ++row) {
                for (int col = 0; col < NCOLS; ++col) {
                    rand = m_rand.Next(0, 5);
                    if (rand == 4) {
                        m_cells[row, col] |= Cell.Pit;
                        FillAdjacent(row, col, Cell.Breeze);
                    }
                }
            }
        }
        public static int WumpusRow
        { get; private set; }
        public static int WumpusCol
        { get; private set; }
        public static int GoldRow
        { get; private set; }
        public static int GoldCol
        { get; private set; }
        public static bool IsWumpus(int row, int col)
        {
            return Is(Cell.Wumpus, row, col);
        }
        public static bool IsGold(int row, int col)
        {
            return Is(Cell.Gold, row, col);
        }
        public static bool IsPit(int row, int col)
        {
            return Is(Cell.Pit, row, col);
        }
        public static bool IsStench(int row, int col)
        {
            return Is(Cell.Stench, row, col);
        }
        public static bool IsBreeze(int row, int col)
        {
            return Is(Cell.Breeze, row, col);
        }
        public static bool TryKillWumpus(int row, int col)
        {
            if (!IsWumpus(row, col))
                return false;
            m_cells[row, col] ^= Cell.Wumpus;
            WumpusKilled?.Invoke();
            return true;
        }
        public static bool TryGrabGold(int row, int col)
        {
            if (!IsGold(row, col))
                return false;
            m_cells[row, col] ^= Cell.Gold;
            GoldGrabbed?.Invoke();
            return true;
        }

        private static bool Is(Cell what, int row, int col)
        {
            if (row >= 0 && col >= 0 && row < NROWS && col < NCOLS)
                return (m_cells[row, col] & what) == what;
            return false;
        }
        private static void FillAdjacent(int row, int col, Cell value)
        {
            if (row != 0) m_cells[row - 1, col] |= value;
            if (row+1 != NROWS) m_cells[row + 1, col] |= value;
            if (col != 0) m_cells[row, col - 1] |= value;
            if (col+1 != NCOLS) m_cells[row, col + 1] |= value;
        }
        static Cell[,] m_cells     = new Cell[NROWS, NCOLS];
        static Random m_rand       = new Random();
    }

    public class Agent
    {
        public event Action ActionTaken;
        public event Action ArrowUsed;
        public Agent()
        {
            Row = Col = 0;
            Facing = Dirn.Right;
            HasArrow = true;
        }
        public Dirn Facing
        { get; private set; }
        public int Row
        { get; private set; }
        public int Col
        { get; private set; }
        public bool HasArrow
        { get; private set; }
        public void Move()
        {
            int colDist = 0;
            if (Facing == Dirn.Right && Col != World.NCOLS - 1)
                colDist = 1;
            else if (Facing == Dirn.Left && Col != 0)
                colDist = -1;

            int rowDist = 0;
            if (Facing == Dirn.Up && Row != World.NROWS - 1)
                rowDist = 1;
            else if (Facing == Dirn.Down && Row != 0)
                rowDist = -1;

            Col += colDist;
            Row += rowDist;
            ActionTaken?.Invoke();
        }
        public void TurnRight()
        {
            Facing = (Dirn)(((int)Facing + 1) % 4);
            ActionTaken?.Invoke();
        }
        public void TurnLeft()
        {
            Facing = (Dirn)(((int)Facing + 3) % 4);
            ActionTaken?.Invoke();
        }
        public void Shoot()
        {
            if (HasArrow) {
                bool killed = false;
                if (Facing == Dirn.Up) {
                    for (int row = Row + 1; !killed && row < World.NROWS; ++row)
                        killed = World.TryKillWumpus(row, Col);
                }
                else if (Facing == Dirn.Down) {
                    for (int row = Row - 1; !killed && row >= 0; --row)
                        killed = World.TryKillWumpus(row, Col);
                }
                else if (Facing == Dirn.Right) {
                    for (int col = Col + 1; !killed && col < World.NCOLS; ++col)
                        killed = World.TryKillWumpus(Row, col);
                }
                else {
                    for (int col = Col - 1; !killed && col >= 0; --col)
                        killed = World.TryKillWumpus(Row, col);
                }
                HasArrow = false;
                ArrowUsed?.Invoke();
            }
        }
        public bool Grab()
        {
            ActionTaken?.Invoke();
            return World.TryGrabGold(Row, Col);
        }
    }

    public class ScoreTeller
    {
        public int Points
        { get; private set; } = 0;

        public void OnEatenOrFallen()
        {
            Points -= 1000;
        }
        public void OnSuccess()
        {
            Points += 1000;
        }
        public void OnAction()
        {
            Points--;
        }
        public void OnArrow()
        {
            Points -= 10;
        }
    }

    public class Game
    {
        public event Action Gameover;
        public Game()
        {
            m_goldGrabbed = false;
            Score = new ScoreTeller();
            Player = new Agent();
            Player.ActionTaken += Score.OnAction;
            Player.ArrowUsed += Score.OnArrow;

            World.GoldGrabbed += () => m_goldGrabbed = true;
            Gameover += Score.OnEatenOrFallen;

            Player.ActionTaken += () => {
                if(m_goldGrabbed && Player.Row == 0 && Player.Col == 0) {
                    Score.OnSuccess();
                }
                else if (World.IsPit(Player.Row, Player.Col) || World.IsWumpus(Player.Row, Player.Col)) {
                    Gameover?.Invoke();
                }
            };
        }
        public ScoreTeller Score
        { get; }
        public Agent Player
        { get; }

        bool m_goldGrabbed;
    }
}

