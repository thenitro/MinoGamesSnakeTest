using TymurKoshel.Snake.Controller;
using TymurKoshel.Snake.Controllers.Base;

namespace TymurKoshel.Snake.Controllers.Inputs
{
    /*
     * We could inherit this interface for ReplayInputController or AiInputController and implement the movement of player/ai
     */
    public interface IPlayerInputController
    {
        Direction GetDirection();
    }
}