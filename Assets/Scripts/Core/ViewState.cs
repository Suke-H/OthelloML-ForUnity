using Othello.InputSystem;

namespace Othello.Core
{
    public sealed class ViewState
    {
        public readonly int[,]  Board;
        public readonly Player  CurrentTurn;
        public readonly bool[,] LegalMoves;
        public readonly float   CursorWorldX;
        public readonly float   CursorWorldY;

        public ViewState(EnvState env, bool[,] legalMoves, InputState input)
        {
            Board        = (int[,])env.Board.Clone();
            CurrentTurn  = env.CurrentTurn;
            LegalMoves   = legalMoves;
            CursorWorldX = input.DragWorldX;
            CursorWorldY = input.DragWorldY;
        }
    }
}
