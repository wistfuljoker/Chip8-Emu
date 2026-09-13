using Chip8_Emu;
using SDL3;

var chip = new Chip8();

chip.Initializer();

var screen = new Chip8Display();
var display = new Display();

display.Render(new byte[64 * 32]);

bool running = true;

Dictionary<byte, SDL.Scancode> ReveseKeys = chip.Keys.ToDictionary(kv => kv.Value, kv => kv.Key);

while (running)
{
  var SDLKeys = SDL.GetKeyboardState(out int numkeys);
  for (byte i = 0; i < 16; i++)
  {
    chip.keypad[i] = SDLKeys[(int)ReveseKeys[i]];
  }
  while (SDL.PollEvent(out SDL.Event @event))
  {
    if (@event.Type == (uint)SDL.EventType.Quit)
      Environment.Exit(1);
    else if (@event.Type == (uint)SDL.EventType.KeyDown)
    {
      try
      {
        chip.ResolveKey(chip.Keys[@event.Key.Scancode]);
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.Message);
      }
    }

  }

  if (!chip.waitforkey)
    chip.EmulateCycle();

  if (chip.Drawflag)
  {

    //screen.Render(chip.gfx);
    display.Render(chip.gfx);
    chip.Drawflag = false;
  }

  // update timers
  chip.UpdateTimer();

}

display.Cleanup();