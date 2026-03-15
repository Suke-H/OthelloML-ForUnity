using Othello.States;

namespace Othello.Systems
{
    public static class EnvSystem
    {
        private static readonly (int dx, int dy)[] Directions =
        {
            (-1,-1),( 0,-1),( 1,-1),
            (-1, 0),        ( 1, 0),
            (-1, 1),( 0, 1),( 1, 1)
        };

        public static EnvState CreateInitial()
        {
            var board = new int[EnvState.Size, EnvState.Size];
            board[3, 3] = 2; board[4, 4] = 2;
            board[3, 4] = 1; board[4, 3] = 1;
            var legal = ComputeLegalMoves(board, Player.Black);
            return new EnvState(isActionSuccess: true, board, Player.Black, legal);
        }

        public static EnvState Apply(EnvState state, int x, int y)
        {
            if (!IsLegal(state, x, y))
                return new EnvState(isActionSuccess: false, state.Board, state.CurrentTurn, state.LegalMoves);

            var next  = state.Clone();
            int stone = (int)state.CurrentTurn + 1;

            next.Board[x, y] = stone;
            Flip(next.Board, x, y, stone);

            var nextTurn  = state.CurrentTurn == Player.Black ? Player.White : Player.Black;
            var nextLegal = ComputeLegalMoves(next.Board, nextTurn);
            var nextState = new EnvState(isActionSuccess: true, next.Board, nextTurn, nextLegal);

            return nextState;
        }

        private static bool[,] ComputeLegalMoves(int[,] board, Player turn)
        {
            var legal = new bool[EnvState.Size, EnvState.Size];
            for (int x = 0; x < EnvState.Size; x++)
            for (int y = 0; y < EnvState.Size; y++)
                legal[x, y] = IsLegalRaw(board, x, y, turn);
            return legal;
        }

        public static bool IsLegal(EnvState s, int x, int y)
            => IsLegalRaw(s.Board, x, y, s.CurrentTurn);

        private static bool IsLegalRaw(int[,] board, int x, int y, Player turn)
        {
            if (board[x, y] != 0) return false;
            int stone = (int)turn + 1;
            foreach (var (dx, dy) in Directions)
                if (CountFlippable(board, x, y, dx, dy, stone) > 0) return true;
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

    }
}
