using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Core;

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
            m_game = new Game();

            PitOpacity = new int[World.NROWS * World.NCOLS];
            StenchOpacity = new int[World.NROWS * World.NCOLS];
            BreezeOpacity = new int[World.NROWS * World.NCOLS];

            for (int row = 0; row < World.NROWS; ++row) {       // temporary
                for (int col = 0; col < World.NCOLS; ++col) {
                    if (World.IsBreeze(row, col))
                        BreezeOpacity[GetOpacityIndex(row, col)] = 1;
                }
            }
            for (int row = 0; row < World.NROWS; ++row) {       // temporary
                for (int col = 0; col < World.NCOLS; ++col) {
                    if (World.IsStench(row, col))
                        StenchOpacity[GetOpacityIndex(row, col)] = 1;
                }
            }
            for (int row = 0; row < World.NROWS; ++row) {       // temporary
                for (int col = 0; col < World.NCOLS; ++col) {
                    if (World.IsPit(row, col))
                        PitOpacity[GetOpacityIndex(row, col)] = 1;
                }
            }

            BreezeImage = @"..\..\breeze.png";
            PitImage = @"..\..\pit.png";
            StenchImage = @"..\..\stench.png";

            PlayerImage = @"..\..\player_right.png";

            WumpusOpacity = 1;
            WumpusImage = @"..\..\wumpus_live.png";

            GoldOpacity = 1;
            GoldImage = @"..\..\gold.png";


            //--------------------------------------
            OnUpPressed = new RelayCommand(InternalUpPressed);
            OnRightPressed = new RelayCommand(InternalRightPressed);
            OnLeftPressed = new RelayCommand(InternalLeftPressed);
            OnSpacePressed = new RelayCommand(InternalSpacePressed);
            OnEnterPressed = new RelayCommand(InternalEnterPressed);

            World.WumpusKilled += () => {
                WumpusImage = @"..\..\wumpus_dead.png";
                WumpusOpacity = 1;
                InternalPropertyChanged(nameof(WumpusImage));
                InternalPropertyChanged(nameof(WumpusOpacity));
            };
            World.GoldGrabbed += () => {
                GoldOpacity = 0;
                InternalPropertyChanged(nameof(GoldOpacity));
            };
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
            m_game.Player.Move();
            InternalPropertyChanged(nameof(PlayerCol));
            InternalPropertyChanged(nameof(PlayerRow));
            InternalPropertyChanged(nameof(PlayerImage));
            InternalPropertyChanged(nameof(ScoreText));
        }

        public ICommand OnRightPressed
        { get; private set; }
        void InternalRightPressed(object o)
        {
            m_game.Player.TurnRight();
            InternalUpdateFacing(m_game.Player.Facing);
        }

        public ICommand OnLeftPressed
        { get; private set; }
        void InternalLeftPressed(object o)
        {
            m_game.Player.TurnLeft();
            InternalUpdateFacing(m_game.Player.Facing);
        }
        void InternalUpdateFacing(Dirn dirn)
        {
            switch (dirn) {
                case Dirn.Down:
                    PlayerImage = @"..\..\player_down.png";
                    break;
                case Dirn.Up:
                    PlayerImage = @"..\..\player_up.png";
                    break;
                case Dirn.Left:
                    PlayerImage = @"..\..\player_left.png";
                    break;
                case Dirn.Right:
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
            m_game.Player.Shoot();

            InternalPropertyChanged(nameof(StenchOpacity));
            InternalPropertyChanged(nameof(BreezeOpacity));
            InternalPropertyChanged(nameof(PitOpacity));
            InternalPropertyChanged(nameof(WumpusOpacity));
        }

        public ICommand OnEnterPressed
        { get; private set; }
        void InternalEnterPressed(object o)
        {
            m_game.Player.Grab();
            InternalPropertyChanged(nameof(ScoreText));
        }

        //=================================================================================================================

        public int PlayerRow
        {
            get { return (3 - m_game.Player.Row) + 1; }
        }
        public int PlayerCol
        {
            get { return m_game.Player.Col + 1; }
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

        //=================================================================================================================

        public int WumpusRow
        {
            get { return (3 - World.WumpusRow) + 1; }
        }
        public int WumpusCol
        {
            get { return World.WumpusCol + 1; }
        }
        public int WumpusOpacity
        { get; private set; }
        public string WumpusImage
        { get; private set; }

        //=================================================================================================================

        public int GoldRow
        {
            get { return (3 - World.GoldRow) + 1; }
        }
        public int GoldCol
        {
            get { return World.GoldCol + 1; }
        }
        public int GoldOpacity
        { get; private set; }
        public string GoldImage
        { get; private set; }

        //=================================================================================================================
        
        public string ScoreText
        {
            get { return $"Score: {m_game.Score.Points}"; }
        }


        static int GetOpacityIndex(int row, int col)
        {
            return World.NCOLS * row + col;
        }
        Game m_game;
    }
}
