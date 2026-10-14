using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TicTacToe.Properties;

namespace TicTacToe
{
    /// <summary>
    /// Main window of the Tic-Tac-Toe game.
    /// Two players take turns on the same machine: Player 1 plays X, Player 2 plays O.
    /// </summary>
    public partial class MainForm : Form
    {
        private enum Player
        {
            Player1,
            Player2
        }

        private enum GameResult
        {
            InProgress,
            Player1Wins,
            Player2Wins,
            Draw
        }

        // Cell states are stored in each button's Tag property.
        private const string EmptyCell = "?";
        private const string MarkX = "X";
        private const string MarkO = "O";

        private const int TotalCells = 9;

        private static readonly Color WinningCellColor = Color.GreenYellow;

        // Board grid geometry (drawn manually in the Paint event).
        private const int GridLineWidth = 15;
        private const int GridLeft = 400;
        private const int GridRight = 1050;
        private const int GridTop = 140;
        private const int GridBottom = 620;
        private const int FirstRowLine = 300;
        private const int SecondRowLine = 460;
        private const int FirstColumnLine = 610;
        private const int SecondColumnLine = 840;

        private readonly Button[] _cells;
        private readonly Button[][] _winningLines;

        private Player _currentPlayer = Player.Player1;
        private GameResult _result = GameResult.InProgress;
        private int _moveCount;

        public MainForm()
        {
            InitializeComponent();

            _cells = new[]
            {
                btnCell1, btnCell2, btnCell3,
                btnCell4, btnCell5, btnCell6,
                btnCell7, btnCell8, btnCell9
            };

            _winningLines = new[]
            {
                // Rows
                new[] { btnCell1, btnCell2, btnCell3 },
                new[] { btnCell4, btnCell5, btnCell6 },
                new[] { btnCell7, btnCell8, btnCell9 },

                // Columns
                new[] { btnCell1, btnCell4, btnCell7 },
                new[] { btnCell2, btnCell5, btnCell8 },
                new[] { btnCell3, btnCell6, btnCell9 },

                // Diagonals
                new[] { btnCell1, btnCell5, btnCell9 },
                new[] { btnCell3, btnCell5, btnCell7 }
            };
        }

        /// <summary>
        /// Single click handler shared by all nine cells.
        /// </summary>
        private void Cell_Click(object sender, EventArgs e)
        {
            if (_result != GameResult.InProgress)
            {
                MessageBox.Show("The game is over. Press \"Restart Game\" to play again.",
                    "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Button cell = (Button)sender;

            if (!IsEmpty(cell))
            {
                MessageBox.Show("This cell is already taken. Please choose another one.",
                    "Invalid Move", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PlayMove(cell);
        }

        private void PlayMove(Button cell)
        {
            bool isPlayer1 = _currentPlayer == Player.Player1;

            cell.Image = isPlayer1 ? Resources.X : Resources.O;
            cell.Tag = isPlayer1 ? MarkX : MarkO;
            _moveCount++;

            GameResult result = CheckForWinner();

            if (result == GameResult.InProgress && _moveCount == TotalCells)
                result = GameResult.Draw;

            if (result != GameResult.InProgress)
            {
                EndGame(result);
                return;
            }

            SwitchTurn();
        }

        private void SwitchTurn()
        {
            _currentPlayer = _currentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
            lblTurn.Text = GetPlayerName(_currentPlayer);
        }

        /// <summary>
        /// Checks all eight winning lines. If one is complete, its cells are highlighted
        /// and the winning player is returned; otherwise the game is still in progress.
        /// </summary>
        private GameResult CheckForWinner()
        {
            foreach (Button[] line in _winningLines)
            {
                string first = GetMark(line[0]);

                if (first != EmptyCell && first == GetMark(line[1]) && first == GetMark(line[2]))
                {
                    foreach (Button cell in line)
                        cell.BackColor = WinningCellColor;

                    return first == MarkX ? GameResult.Player1Wins : GameResult.Player2Wins;
                }
            }

            return GameResult.InProgress;
        }

        private void EndGame(GameResult result)
        {
            _result = result;
            lblTurn.Text = "Game Over";

            string message;

            switch (result)
            {
                case GameResult.Player1Wins:
                    lblWinner.Text = GetPlayerName(Player.Player1);
                    message = "Player 1 (X) wins!";
                    break;

                case GameResult.Player2Wins:
                    lblWinner.Text = GetPlayerName(Player.Player2);
                    message = "Player 2 (O) wins!";
                    break;

                default:
                    lblWinner.Text = "Draw";
                    message = "It's a draw!";
                    break;
            }

            MessageBox.Show(message, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RestartGame()
        {
            foreach (Button cell in _cells)
                ResetCell(cell);

            _currentPlayer = Player.Player1;
            _result = GameResult.InProgress;
            _moveCount = 0;

            lblTurn.Text = GetPlayerName(_currentPlayer);
            lblWinner.Text = "In Progress";
        }

        private static void ResetCell(Button cell)
        {
            cell.Image = Resources.QuestionMark;
            cell.Tag = EmptyCell;
            cell.BackColor = Color.Transparent;
        }

        private static string GetMark(Button cell)
        {
            return cell.Tag as string ?? EmptyCell;
        }

        private static bool IsEmpty(Button cell)
        {
            return GetMark(cell) == EmptyCell;
        }

        private static string GetPlayerName(Player player)
        {
            return player == Player.Player1 ? "Player 1" : "Player 2";
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        /// <summary>
        /// Draws the white board grid with rounded line ends.
        /// </summary>
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(Color.White, GridLineWidth))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Horizontal lines
                e.Graphics.DrawLine(pen, GridLeft, FirstRowLine, GridRight, FirstRowLine);
                e.Graphics.DrawLine(pen, GridLeft, SecondRowLine, GridRight, SecondRowLine);

                // Vertical lines
                e.Graphics.DrawLine(pen, FirstColumnLine, GridTop, FirstColumnLine, GridBottom);
                e.Graphics.DrawLine(pen, SecondColumnLine, GridTop, SecondColumnLine, GridBottom);
            }
        }
    }
}
