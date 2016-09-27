﻿using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace GUI
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            m_execute = execute;
            m_canExec = canExecute;
        }
        public RelayCommand(Action<object> execute)
        {
            m_execute = execute;
            m_canExec = (o) => true;
        }
        public void Execute(object o)
        {
            m_execute(o);
        }
        public bool CanExecute(object o)
        {
            return m_canExec(o);
        }
        public void ExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(sender, e);
        }
        Action<object> m_execute;
        Func<object, bool> m_canExec;
    }

    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            PitOpacity = new int[World.NROWS * World.NCOLS];
            StenchOpacity = new int[World.NROWS * World.NCOLS];
            BreezeOpacity = new int[World.NROWS * World.NCOLS];
            BorderOpacity = new double[World.NROWS * World.NCOLS];

            for (int i = 0; i < BorderOpacity.Length; ++i)
                BorderOpacity[i] = 0.5;

            //for (int row = 0; row < World.NROWS; ++row) {       // temporary
            //    for (int col = 0; col < World.NCOLS; ++col) {
            //        if (World.IsBreeze(row, col))
            //            BreezeOpacity[GetOpacityIndex(row, col)] = 1;
            //    }
            //}
            //for (int row = 0; row < World.NROWS; ++row) {       // temporary
            //    for (int col = 0; col < World.NCOLS; ++col) {
            //        if (World.IsStench(row, col))
            //            StenchOpacity[GetOpacityIndex(row, col)] = 1;
            //    }
            //}
            //for (int row = 0; row < World.NROWS; ++row) {       // temporary
            //    for (int col = 0; col < World.NCOLS; ++col) {
            //        if (World.IsPit(row, col))
            //            PitOpacity[GetOpacityIndex(row, col)] = 1;
            //    }
            //}

            BreezeImage = @"..\..\breeze.png";
            PitImage = @"..\..\pit.png";
            StenchImage = @"..\..\stench.png";

            PlayerImage = @"..\..\player_right.png";

            WumpusOpacity = 0;
            WumpusImage = @"..\..\wumpus_live.png";

            GoldOpacity = 0;
            GoldImage = @"..\..\gold.png";
            
            CurrentStenchOpacity = 0.5;            
            CurrentBreezeOpacity = 0.5;
            CurrentPitOpacity = 0.5;
            //--------------------------------------
            OnUpPressed = new RelayCommand(InternalUpPressed);
            OnRightPressed = new RelayCommand(InternalRightPressed);
            OnLeftPressed = new RelayCommand(InternalLeftPressed);
            OnSpacePressed = new RelayCommand(InternalSpacePressed);
            OnEnterPressed = new RelayCommand(InternalEnterPressed);

            Task.Run(() => {
                while (true) {
                    Thread.Sleep(100);
                    if (World.IsWumpusKilled()) {
                        WumpusImage = @"..\..\wumpus_dead.png";
                        WumpusOpacity = 1;
                        InternalPropertyChanged(nameof(WumpusImage));
                        InternalPropertyChanged(nameof(WumpusOpacity));
                        break;
                    }
                }
            });
            Task.Run(() => {
                while (true) {
                    Thread.Sleep(100);
                    if (World.IsGoldGrabbed()) {
                        GoldOpacity = 0;
                        InternalPropertyChanged(nameof(GoldOpacity));
                        break;
                    }
                }
            });
            InternalUpdateOpacities(false, Player.Row, Player.Col);
        }
        void InternalPropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        //=================================================================================================================

        public ICommand OnUpPressed
        { get; private set; }
        void InternalUpPressed(object o)
        {
            Player.Move();
            InternalPropertyChanged(nameof(PlayerCol));
            InternalPropertyChanged(nameof(PlayerRow));
            InternalPropertyChanged(nameof(ScoreText));

            InternalUpdateOpacities(true, Player.Row, Player.Col);

            if (World.IsPit(Player.Row, Player.Col) || World.IsWumpus(Player.Row, Player.Col)) {
                Thread.Sleep(100);
                MessageBox.Show("Condolences, you lost! (Fallen in a pit or eaten by the Wumpus)", "Gameover");
                //Application.Current.Shutdown();
            }
        }
        void InternalUpdateOpacities(bool notify, int row, int col)
        {
            BorderOpacity[GetOpacityIndex(row, col)] = 1;

            if (World.IsBreeze(row, col)) {
                BreezeOpacity[GetOpacityIndex(row, col)] = 1;
                CurrentBreezeOpacity = 1;
                if (notify)
                    InternalPropertyChanged(nameof(BreezeOpacity));
            }
            else
                CurrentBreezeOpacity = 0.5;

            if (World.IsStench(row, col)) {
                StenchOpacity[GetOpacityIndex(row, col)] = 1;
                CurrentStenchOpacity = 1;
                if (notify)
                    InternalPropertyChanged(nameof(StenchOpacity));
            }
            else 
                CurrentStenchOpacity = 0.5;
            
            if (World.IsPit(row, col)) {
                PitOpacity[GetOpacityIndex(row, col)] = 1;
                CurrentPitOpacity = 1;
                if (notify) 
                    InternalPropertyChanged(nameof(PitOpacity));
            }
            else 
                CurrentPitOpacity = 0.5;

            if (World.IsGold(row, col)) {
                GoldOpacity = 1;
                if (notify)
                    InternalPropertyChanged(nameof(GoldOpacity));
            }
            if (World.IsWumpus(row, col)) {
                WumpusOpacity = 1;
                if (notify)
                    InternalPropertyChanged(nameof(WumpusOpacity));
            }

            if (notify) {
                InternalPropertyChanged(nameof(CurrentPitOpacity));
                InternalPropertyChanged(nameof(CurrentBreezeOpacity));
                InternalPropertyChanged(nameof(CurrentStenchOpacity));
                InternalPropertyChanged(nameof(BorderOpacity));
            }
        }

        public ICommand OnRightPressed
        { get; private set; }
        void InternalRightPressed(object o)
        {
            Player.TurnRight();
            InternalUpdateFacing(Player.Facing);
        }

        public ICommand OnLeftPressed
        { get; private set; }
        void InternalLeftPressed(object o)
        {
            Player.TurnLeft();
            InternalUpdateFacing(Player.Facing);
        }
        void InternalUpdateFacing(int dirn)
        {
            switch (dirn) {
                case Player.DIRN_DOWN:
                    PlayerImage = @"..\..\player_down.png";
                    break;
                case Player.DIRN_UP:
                    PlayerImage = @"..\..\player_up.png";
                    break;
                case Player.DIRN_LEFT:
                    PlayerImage = @"..\..\player_left.png";
                    break;
                case Player.DIRN_RIGHT:
                    PlayerImage = @"..\..\player_right.png";
                    break;
            }
            InternalPropertyChanged(nameof(PlayerImage));
            InternalPropertyChanged(nameof(ScoreText));
        }

        public ICommand OnSpacePressed
        { get; private set; }
        void InternalSpacePressed(object o)
        {
            if (Player.HasArrow) {
                Task.Run(() => {
                    int facing = Player.Facing;

                    ArrowImage = (facing == Player.DIRN_UP || facing == Player.DIRN_DOWN) ? @"..\..\arrow_vertical.png" : @"..\..\arrow_horisontal.png";
                    m_arrowCol = Player.Col;
                    m_arrowRow = Player.Row;
                    ArrowOpacity = 1;
                    InternalPropertyChanged(nameof(ArrowCol));
                    InternalPropertyChanged(nameof(ArrowRow));
                    InternalPropertyChanged(nameof(ArrowOpacity));
                    InternalPropertyChanged(nameof(ArrowImage));

                    if (facing == Player.DIRN_UP || facing == Player.DIRN_DOWN) {
                        int dist = facing == Player.DIRN_UP ? 1 : -1;
                        while ((m_arrowRow + dist) < World.NROWS && (m_arrowRow + dist) >= 0) {
                            Thread.Sleep(200);
                            m_arrowRow += dist;
                            InternalPropertyChanged(nameof(ArrowRow));
                            InternalUpdateOpacities(true, m_arrowRow, m_arrowCol);
                        }
                    }
                    else {
                        int dist = facing == Player.DIRN_RIGHT ? 1 : -1;
                        while ((m_arrowCol + dist) < World.NCOLS && (m_arrowCol + dist) >= 0) {
                            Thread.Sleep(200);
                            m_arrowCol += dist;
                            InternalPropertyChanged(nameof(ArrowCol));
                            InternalUpdateOpacities(true, m_arrowRow, m_arrowCol);
                        }
                    }

                    ArrowOpacity = 0;
                    Thread.Sleep(500);
                    InternalPropertyChanged(nameof(ArrowOpacity));
                    Player.Shoot();
                    InternalPropertyChanged(nameof(ScoreText));
                });
            }
        }

        public ICommand OnEnterPressed
        { get; private set; }
        void InternalEnterPressed(object o)
        {
            Player.Grab();
            InternalPropertyChanged(nameof(ScoreText));
        }

        //=================================================================================================================
        public double CurrentStenchOpacity
        { get; private set; }
        
        public double CurrentBreezeOpacity
        { get; private set; }

        public double CurrentPitOpacity
        { get; private set; }
        //=================================================================================================================

        public int PlayerRow
        {
            get { return GetGuiRow(Player.Row); }
        }
        public int PlayerCol
        {
            get { return GetGuiCol(Player.Col); }
        }
        public string PlayerImage
        { get; private set; }

        //=================================================================================================================

        public int[] PitOpacity
        { get; }
        public string PitImage
        { get; private set; }

        public int[] StenchOpacity
        { get; }
        public string StenchImage
        { get; private set; }

        public int[] BreezeOpacity
        { get; }
        public string BreezeImage
        { get; private set; }

        public double[] BorderOpacity
        { get; }
        //=================================================================================================================

        public int WumpusRow
        {
            get { return GetGuiRow(World.WumpusRow); }
        }
        public int WumpusCol
        {
            get { return GetGuiCol(World.WumpusCol); }
        }
        public int WumpusOpacity
        { get; private set; }
        public string WumpusImage
        { get; private set; }

        //=================================================================================================================

        public int GoldRow
        {
            get { return GetGuiRow(World.GoldRow); }
        }
        public int GoldCol
        {
            get { return GetGuiCol(World.GoldCol); }
        }
        public int GoldOpacity
        { get; private set; }
        public string GoldImage
        { get; private set; }

        //=================================================================================================================

        public int ArrowRow
        {
            get { return GetGuiRow(m_arrowRow); }
        }
        public int ArrowCol
        {
            get { return GetGuiCol(m_arrowCol); }
        }
        public string ArrowImage
        { get; private set; }
        public double ArrowOpacity
        { get; private set; }

        //=================================================================================================================

        public string ScoreText
        {
            get { return $"Score: {Player.Score}"; }
        }

        static int GetGuiRow(int coreRow)
        {
            return (3 - coreRow) + 1;
        }
        static int GetGuiCol(int coreCol)
        {
            return coreCol + 1;
        }
        static int GetOpacityIndex(int row, int col)
        {
            return World.NCOLS * row + col;
        }

        int m_arrowRow;
        int m_arrowCol;
    }
}
