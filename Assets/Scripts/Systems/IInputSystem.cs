using Othello.States;

namespace Othello.Systems
{
    public interface IInputSystem
    {
        void Tick(InputState state);
    }
}
