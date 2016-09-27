using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GUI
{
    public static class World
    {
        public static bool IsWumpus(int row, int col)
        {
            return IsWumpus((uint)row, (uint)col) != 0;
        }
        public static bool IsGold(int row, int col)
        {
            return IsGold((uint)row, (uint)col) != 0;
        }
        public static bool IsStench(int row, int col)
        {
            return IsStench((uint)row, (uint)col) != 0;
        }
        public static bool IsBreeze(int row, int col)
        {
            return IsBreeze((uint)row, (uint)col) != 0;
        }
        public static bool IsPit(int row, int col)
        {
            return IsPit((uint)row, (uint)col) != 0;
        }


        public static int WumpusRow
        {
            get { return (int)WRow(); }
        }
        public static int WumpusCol
        {
            get { return (int)WCol(); }
        }
        public static int GoldRow
        {
            get { return (int)GRow(); }
        }
        public static int GoldCol
        {
            get { return (int)GCol(); }
        }

        public static bool IsWumpusKilled()
        {
            return IsWKilled() != 0;
        }
        public static bool IsGoldGrabbed()
        {
            return IsGGrabbed() != 0;
        }
        
        public static readonly int NROWS;
        public static readonly int NCOLS;

        static World()
        {
            GameInit();
            NROWS = (int)RowCount();
            NCOLS = (int)ColCount();
        }

        // ------------------------------------------------------------------------------------------

        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void GameInit();

        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint RowCount();
        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint ColCount();

        [DllImport("CppCore.dll", EntryPoint = "WumpusRow", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint WRow();
        [DllImport("CppCore.dll", EntryPoint = "WumpusCol", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint WCol();

        [DllImport("CppCore.dll", EntryPoint = "GoldRow", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint GRow();
        [DllImport("CppCore.dll", EntryPoint = "GoldCol", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint GCol();

        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int IsWumpus(uint row, uint col);
        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int IsGold(uint row, uint col);
        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int IsStench(uint row, uint col);
        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int IsBreeze(uint row, uint col);
        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static int IsPit(uint row, uint col);

        [DllImport("CppCore.dll", EntryPoint = "IsGoldGrabbed", CallingConvention = CallingConvention.Cdecl)]
        extern static int IsGGrabbed();
        [DllImport("CppCore.dll", EntryPoint = "IsWumpusKilled", CallingConvention = CallingConvention.Cdecl)]
        extern static int IsWKilled();
    }

    public static class Player
    {
        [DllImport("CppCore.dll", EntryPoint = "AgentMove", CallingConvention = CallingConvention.Cdecl)]
        public extern static void Move();

        [DllImport("CppCore.dll", EntryPoint = "AgentShoot", CallingConvention = CallingConvention.Cdecl)]
        public extern static void Shoot();

        [DllImport("CppCore.dll", EntryPoint = "AgentGrab", CallingConvention = CallingConvention.Cdecl)]
        public extern static void Grab();

        [DllImport("CppCore.dll", EntryPoint = "AgentDie", CallingConvention = CallingConvention.Cdecl)]
        public extern static void Die();

        [DllImport("CppCore.dll", EntryPoint = "AgentTurnRight")]
        public extern static void TurnRight();

        [DllImport("CppCore.dll", EntryPoint = "AgentTurnLeft", CallingConvention = CallingConvention.Cdecl)]
        public extern static void TurnLeft();

        public static int Row
        {
            get { return (int)AgentRow(); }
        }
        public static int Col
        {
            get { return (int)AgentCol(); }
        }
        
        public const int DIRN_UP    = 0;
        public const int DIRN_RIGHT = 1;
        public const int DIRN_DOWN  = 2;
        public const int DIRN_LEFT  = 3;

        public static int Facing
        {
            get { return (int)AgentFacing(); }
        }
        public static long Score
        {
            get { return GetScore(); }
        }
        public static bool HasArrow
        {
            get { return AgentHasArrow() != 0; }
        }



        [DllImport("CppCore.dll", EntryPoint = "AgentRow", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint AgentRow();

        [DllImport("CppCore.dll", EntryPoint = "AgentCol", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint AgentCol();

        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static uint AgentFacing();

        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static long GetScore();

        [DllImport("CppCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int AgentHasArrow();
    }
}
