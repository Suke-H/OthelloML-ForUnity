using Othello.States;

namespace Othello.Systems
{
    public static class UpdateSystem
    {
        public static ViewState CreateViewState(EnvState env, InputState input)
        {
            return new ViewState(env, env.LegalMoves, input);
        }
    }
}
