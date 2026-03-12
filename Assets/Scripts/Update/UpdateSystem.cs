using Othello.Core;
using UnityEngine;

namespace Othello.Update
{
    public static class UpdateSystem
    {
        private static readonly (int dx, int dy)[] Directions =
        {
            (-1,-1),( 0,-1),( 1,-1),
            (-1, 0),        ( 1, 0),
            (-1, 1),( 0, 1),( 1, 1)
        };

        public static EnvState Apply(EnvState state, int x, int y)
        {
            if (!IsLegal(state, x, y)) return null;

            var next  = state.Clone();
            int stone = (int)state.CurrentTurn + 1;

            next.Board[x, y] = stone;
            Flip(next.Board, x, y, stone);

            next = new EnvState(next.Board,
                state.CurrentTurn == Player.Black ? Player.White : Player.Black);

            LogTurn(next, x, y);
            return next;
        }

        public static bool[,] GetLegalMoves(EnvState state)
        {
            var legal = new bool[EnvState.Size, EnvState.Size];
            for (int x = 0; x < EnvState.Size; x++)
            for (int y = 0; y < EnvState.Size; y++)
                legal[x, y] = IsLegal(state, x, y);
            return legal;
        }

        private static bool IsLegal(EnvState s, int x, int y)
        {
            if (s.Board[x, y] != 0) return false;
            int stone = (int)s.CurrentTurn + 1;
            foreach (var (dx, dy) in Directions)
                if (CountFlippable(s.Board, x, y, dx, dy, stone) > 0) return true;
            return false;
        }

        private static void Flip(int[,] board, int x, int y, int stone)
        {
            foreach (var (dx, dy) in Directions)
            {
                int count = CountFlippable(board, x, y, dx, dy, stone);
                for (int i = 1; i <= count; i++)
                    board[x + dx * i, y + dy * i] = stone;
            }
        }

        private static int CountFlippable(int[,] board, int x, int y,
                                          int dx, int dy, int stone)
        {
            int opp   = stone == 1 ? 2 : 1;
            int count = 0;
            int nx    = x + dx;
            int ny    = y + dy;

            while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && board[nx, ny] == opp)
            {
                count++;
                nx += dx;
                ny += dy;
            }

            if (count == 0 || nx < 0 || nx >= 8 || ny < 0 || ny >= 8) return 0;
            return board[nx, ny] == stone ? count : 0;
        }

        private static void LogTurn(EnvState state, int actionX, int actionY)
        {
            Debug.Log($"[Turn確定] action({actionX},{actionY})");
            Debug.Log($"[EnvState] Turn={state.CurrentTurn}");
            for (int y = 7; y >= 0; y--)
            {
                var row = "";
                for (int x = 0; x < 8; x++)
                    row += state.Board[x, y] switch { 1 => "●", 2 => "○", _ => "・" };
                Debug.Log(row);
            }
        }
    }
}
