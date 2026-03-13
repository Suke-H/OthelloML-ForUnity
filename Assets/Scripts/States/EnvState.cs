namespace Othello.States
{
    public enum Player { Black = 0, White = 1 }

    public sealed class EnvState
    {
        public const int Size = 8;

        public readonly int[,]  Board;
        public readonly Player  CurrentTurn;
        public readonly bool[,] LegalMoves;

        public EnvState(int[,] board, Player currentTurn, bool[,] legalMoves)
        {
            Board       = board;
            CurrentTurn = currentTurn;
            LegalMoves  = legalMoves;
        }

        public EnvState Clone()
        {
            var newBoard = (int[,])Board.Clone();
            return new EnvState(newBoard, CurrentTurn, LegalMoves);
        }
    }
}
