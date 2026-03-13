namespace Othello.States
{
    public enum Player { Black = 0, White = 1 }

    public sealed class EnvState
    {
        public const int Size = 8;

        public readonly bool    IsActionSuccess;
        public readonly int[,]  Board;
        public readonly Player  CurrentTurn;
        public readonly bool[,] LegalMoves;

        public EnvState(bool isActionSuccess, int[,] board, Player currentTurn, bool[,] legalMoves)
        {
            IsActionSuccess = isActionSuccess;
            Board           = board;
            CurrentTurn     = currentTurn;
            LegalMoves      = legalMoves;
        }

        public EnvState Clone()
        {
            var newBoard = (int[,])Board.Clone();
            return new EnvState(IsActionSuccess, newBoard, CurrentTurn, LegalMoves);
        }
    }
}
