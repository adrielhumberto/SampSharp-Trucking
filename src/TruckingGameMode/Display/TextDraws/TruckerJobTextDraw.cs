using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;

namespace TruckingGameMode.Display.TextDraws
{
    public class TruckerJobTextDraw : PlayerTextDraw
    {
        public TruckerJobTextDraw(BasePlayer owner) : base(owner)
        {
        }

        public TruckerJobTextDraw(BasePlayer owner, Vector2 position, string text) : base(owner, position, text)
        {
        }

        public TruckerJobTextDraw(BasePlayer owner, Vector2 position, string text, TextDrawFont font) : base(owner, position, text, font)
        {
        }

        public TruckerJobTextDraw(BasePlayer owner, Vector2 position, string text, TextDrawFont font, Color foreColor) : base(owner, position, text, font, foreColor)
        {
        }
    }
}
