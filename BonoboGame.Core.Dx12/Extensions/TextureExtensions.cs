using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BonoboGame.Core.Dx12.Extensions;

public static class TextureExtensions
{
    /// <summary>
    ///     Creates a square texture and returns it.
    /// </summary>
    /// <param name="graphicsDevice"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Texture2D CreateSquareTexture(GraphicsDevice graphicsDevice, int size)
    {
        var texture = new Texture2D(graphicsDevice, size, size);
        var data = new Color[size * size];
        for (var i = 0; i < data.Length; ++i) data[i] = Color.White;
        texture.SetData(data);

        return texture;
    }
}
