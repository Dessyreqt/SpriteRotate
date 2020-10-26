namespace SpriteRotate.Features.Hello
{
    using CommandLine;
    using MediatR;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Drawing.Processing;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    [Verb("Clockwise", HelpText = "Creates a rotation of sprites clockwise below existing sprites")]
    public class Request : IRequest
    {
        [Value(0, Default = "", HelpText = "The subject to whom you are saying \"Hello\"")]
        public string Filename { get; set; }

        [Option('r', "rows", Required = true, HelpText = "The number of rows of sprites.")]
        public int Rows { get; set; }

        [Option('c', "columns", Required = true, HelpText = "The number of columns in each row.")]
        public int Columns { get; set; }

        [Option('s', "size", Required = true, HelpText = "The width and height in pixels of each sprite. Sprites must be square.")]
        public int Size { get; set; }

        [Option('p', "padding", Required = true, HelpText = "The amount of padding between sprites.")]
        public int Padding { get; set; }

        [Option('o', "output", Default = "output.png")]
        public string Output { get; set; }
    }

    public class Handler : RequestHandler<Request>
    {
        protected override void Handle(Request request)
        {
            int width = request.Columns * (request.Size + request.Padding) + request.Padding;
            int height = request.Rows * (request.Size + request.Padding) + request.Padding;
            using var inputImage = Image.Load(request.Filename);
            using var outputImage = new Image<Rgba32>(width, height * 2);
            outputImage.Mutate(ctx => ctx.DrawImage(inputImage, new Point(0, 0), 1));

            for (int row = 0; row < request.Rows; row++)
            for (int column = 0; column < request.Columns; column++)
            {
                var x = column * (request.Size + request.Padding) + request.Padding;
                var y = row * (request.Size + request.Padding) + request.Padding;
                var sprite = inputImage.Clone(ctx => ctx.Crop(new Rectangle(x, y, request.Size, request.Size)).Rotate(RotateMode.Rotate90));
                var newY = y + request.Rows * (request.Size + request.Padding);

                outputImage.Mutate(ctx => ctx.DrawImage(sprite, new Point(x, newY), 1));
            }

            outputImage.SaveAsPng(request.Output);
        }
    }
}
