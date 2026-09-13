using SDL3;

namespace Chip8_Emu;

public class Input
{
  SDL.Event @event;
  Dictionary<SDL.Scancode, byte> Keys = new(){
    {SDL.Scancode.Alpha1, 0},
    {SDL.Scancode.Alpha2, 1},
    {SDL.Scancode.Alpha3, 2},
    {SDL.Scancode.Alpha4, 3},
    {SDL.Scancode.Q, 4},
    {SDL.Scancode.W, 5},
    {SDL.Scancode.E, 6},
    {SDL.Scancode.R, 7},
    {SDL.Scancode.A, 8},
    {SDL.Scancode.S, 9},
    {SDL.Scancode.D, 10},
    {SDL.Scancode.F, 11},
    {SDL.Scancode.Z, 12},
    {SDL.Scancode.X, 13},
    {SDL.Scancode.C, 14},
    {SDL.Scancode.V, 15}

  };

  internal Input() { }

  public bool IsKeyPressed(SDL.Scancode key)
  {
    var kb_state = SDL.GetKeyboardState(out int numpad);
    return kb_state[(int)key];
  }



}
