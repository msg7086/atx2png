using SkiaSharp;

namespace atx2img.Utils;

public static class ImageProcessor
{
    /// <summary>
    /// Creates a new blank image with the specified dimensions and transparency.
    /// </summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <returns>A new blank Image object.</returns>
    public static SKBitmap CreateBlankImage(int width, int height)
    {
        var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);
        return bitmap;
    }

    /// <summary>
    /// Loads an image from a stream.
    /// </summary>
    /// <param name="imageStream">The stream containing the image data.</param>
    /// <returns>An Image object loaded from the stream.</returns>
    public static SKBitmap LoadImageFromStream(Stream imageStream)
    {
        // Use SKCodec for more control over decoding
        using var codec = SKCodec.Create(imageStream);
        if (codec == null)
        {
            return null; // Failed to create codec
        }

        // Get image info and create bitmap with desired color type
        var info = codec.Info.WithColorType(SKColorType.Rgba8888);
        var bitmap = new SKBitmap(info);

        // Read pixels into the bitmap
        var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());

        if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
        {
            // Convert to Premul alpha type if necessary
            // Convert to Premul alpha type if necessary
            if (bitmap.AlphaType != SKAlphaType.Premul)
            {
                var premulInfo = info.WithAlphaType(SKAlphaType.Premul);
                var premulBitmap = new SKBitmap(premulInfo);
                using (var canvas = new SKCanvas(premulBitmap))
                {
                    canvas.DrawBitmap(bitmap, 0, 0);
                }
                bitmap.Dispose(); // Dispose the original bitmap
                return premulBitmap;
            }
            return bitmap;
        }
        else
        {
            bitmap.Dispose(); // Dispose the partially decoded bitmap
            return null; // Decoding failed
        }
    }

    /// <summary>
    /// Crops a portion of an image.
    /// </summary>
    /// <param name="sourceImage">The image to crop from.</param>
    /// <param name="x">The x-coordinate of the top-left corner of the crop area.</param>
    /// <param name="y">The y-coordinate of the top-left corner of the crop area.</param>
    /// <param name="width">The width of the crop area.</param>
    /// <param name="height">The height of the crop area.</param>
    /// <returns>A new Image object containing the cropped portion.</returns>
    public static SKBitmap CropImage(SKBitmap sourceImage, int x, int y, int width, int height)
    {
        // SkiaSharp cropping using SKCanvas.DrawBitmap
        var croppedBitmap = new SKBitmap(width, height, sourceImage.ColorType, sourceImage.AlphaType);
        using var canvas = new SKCanvas(croppedBitmap);
        // Define the source rectangle (area to crop from)
        var sourceRect = new SKRectI(x, y, x + width, y + height);

        // Define the destination rectangle (area to draw onto the new bitmap)
        var destRect = new SKRectI(0, 0, width, height);

        // Ensure the source rectangle is within the source image bounds
        if (sourceRect.Left < 0 || sourceRect.Top < 0 || sourceRect.Right > sourceImage.Width || sourceRect.Bottom > sourceImage.Height)
        {
            // Handle error or adjust sourceRect if necessary
            // For simplicity, we'll just return null or throw an exception
            return null; // Or throw new ArgumentOutOfRangeException("Crop area is outside source image bounds.");
        }

        // Draw the specified subset of the source image onto the new bitmap
        canvas.DrawBitmap(sourceImage, sourceRect, destRect);

        return croppedBitmap;
    }

    /// <summary>
    /// Pastes a source image onto a destination image at a specified location, using the source image's alpha channel as a mask.
    /// </summary>
    /// <param name="destinationImage">The image to paste onto.</param>
    /// <param name="sourceImage">The image to paste.</param>
    /// <param name="x">The x-coordinate on the destination image to paste at.</param>
    /// <param name="y">The y-coordinate on the destination image to paste at.</param>
    public static void PasteImage(SKBitmap destinationImage, SKBitmap sourceImage, int x, int y)
    {
        // SkiaSharp handles alpha blending automatically when drawing bitmaps
        using var canvas = new SKCanvas(destinationImage);
        canvas.DrawBitmap(sourceImage, new SKPoint(x, y));
    }

    /// <summary>
    /// Saves an image to a file.
    /// </summary>
    /// <param name="image">The image to save.</param>
    /// <param name="filePath">The path to save the image to.</param>
    public static void SaveImage(SKBitmap image, string filePath)
    {
        using var stream = File.OpenWrite(filePath);
        // Convert to Unpremul alpha type before encoding if necessary
        if (image.AlphaType != SKAlphaType.Unpremul)
        {
            var unpremulInfo = image.Info.WithAlphaType(SKAlphaType.Unpremul);
            using var unpremulBitmap = new SKBitmap(unpremulInfo);
            using (var canvas = new SKCanvas(unpremulBitmap))
            {
                canvas.DrawBitmap(image, 0, 0);
            }
            unpremulBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
        }
        else
        {
            image.Encode(stream, SKEncodedImageFormat.Png, 100);
        }
    }

    /// <summary>
    /// Loads an image from a file path.
    /// </summary>
    /// <param name="filePath">The path to the image file.</param>
    /// <returns>An Image object loaded from the file.</returns>
    public static SKBitmap LoadImageFromFile(string filePath)
    {
        return SKBitmap.Decode(filePath);
    }
}
