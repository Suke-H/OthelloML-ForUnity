namespace Othello.Core
{
    public enum Player { Black = 0, White = 1 }

    public sealed class EnvState
    {
        public const int Size = 8;

        public readonly int[,] Board;
        public readonly Player CurrentTurn;

        public EnvState(int[,] board, Player currentTurn)
        {
            Board = board;
            CurrentTurn = currentTurn;
        }

        public static EnvState CreateInitial()
        {
            var board = new int[Size, Size];
            board[3, 3] = 2; board[4, 4] = 2;
            board[3, 4] = 1; board[4, 3] = 1;
            return new EnvState(board, Player.Black);
        }

        public EnvState Clone()
        {
            var newBoard = (int[,])Board.Clone();
            return new EnvState(newBoard, CurrentTurn);
        }
    }
}
