using Othello.Core;
using Othello.InputSystem;

namespace Othello.Update
{
    public static class UpdateSystem
    {
        public static ViewState CreateViewState(EnvState env, InputState input)
        {
            return new ViewState(env, env.LegalMoves, input);
        }
    }
}
