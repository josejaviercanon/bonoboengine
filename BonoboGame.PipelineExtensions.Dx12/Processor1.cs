using Microsoft.Xna.Framework.Content.Pipeline;

using TInput = System.String;
using TOutput = System.String;

namespace BonoboGame.PipelineExtensions;

[ContentProcessor(DisplayName = "Processor1")]
public class Processor1 : ContentProcessor<TInput, TOutput>
{
    public override TOutput Process(TInput input, ContentProcessorContext context)
    {
        return default(TOutput);
    }
}
