using SkiaSharp;
using System.Text.Json;
using atx2img.Data;
using atx2img.Utils;

namespace atx2img;

public class AtxConverter(string inputFilePath, string outputDirectory)
{
    // Fields for character sprite handling
    private bool _isChara = false;
    private string _charaBase = "";
    private int _charaBaseX = 0;
    private int _charaBaseY = 0;

    public void Convert()
    {
        using var zipReader = new ZipFileReader(inputFilePath);
        // Read and parse atlas.json
        var atlasEntryStream = zipReader.GetEntryStream("atlas.json");
        if (atlasEntryStream == null)
        {
            throw new FileNotFoundException("atlas.json not found in the ATX file.");
        }

        AtlasData? atlasData;
        try
        {
            atlasData = JsonSerializer.Deserialize<AtlasData>(atlasEntryStream);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Error parsing atlas.json: {ex.Message}", ex);
        }

        if (atlasData?.Blocks == null)
        {
            Console.WriteLine("No blocks found in atlas.json. Nothing to convert.");
            return;
        }

        // Process each block
        foreach (var block in atlasData.Blocks)
        {
            if (block.Width <= 0 || block.Height <= 0)
            {
                Console.WriteLine($"Skipping block '{block.Filename}' due to invalid dimensions ({block.Width}x{block.Height}).");
                continue;
            }

            // Create output image for the block
            using SKBitmap outimg = ImageProcessor.CreateBlankImage((int)block.Width, (int)block.Height);
            if (block.Mesh == null)
            {
                Console.WriteLine("Empty Mesh block. Nothing to convert.");
                return;
            }

            // Process each mesh within the block
            foreach (var mesh in block.Mesh)
            {
                string texFileNamePng = $"tex{mesh.TexNo}.png";
                string texFileNameWebp = $"tex{mesh.TexNo}.webp";
                string texFileName = "";

                // Determine texture file name (prefer png, then webp)
                if (zipReader.GetEntryStream(texFileNamePng) != null)
                {
                    texFileName = texFileNamePng;
                }
                else if (zipReader.GetEntryStream(texFileNameWebp) != null)
                {
                    texFileName = texFileNameWebp;
                }
                else
                {
                    Console.WriteLine($"Warning: Texture file tex{mesh.TexNo}.png or tex{mesh.TexNo}.webp not found for block '{block.Filename}'. Skipping mesh.");
                    continue;
                }

                // Read texture image from zip
                using var texPicStream = zipReader.GetEntryStream(texFileName);
                if (texPicStream == null) continue;
                try
                {
                    using var memoryStream = new MemoryStream();
                    texPicStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin); // Reset stream position to the beginning
                    using SKBitmap texpic = ImageProcessor.LoadImageFromStream(memoryStream);
                    if (texpic == null)
                    {
                        Console.WriteLine($"Warning: Failed to load texture image from '{texFileName}' for block '{block.Filename}'. Skipping mesh.");
                        continue;
                    }


                    // Crop the mesh piece from the texture
                    // Ensure crop dimensions are within texture bounds
                    int cropX = (int)mesh.ViewX;
                    int cropY = (int)mesh.ViewY;
                    int cropWidth = (int)mesh.Width;
                    int cropHeight = (int)mesh.Height;

                    if (cropX < 0 || cropY < 0 || cropX + cropWidth > texpic.Width || cropY + cropHeight > texpic.Height)
                    {
                        Console.WriteLine($"Warning: Mesh crop area for block '{block.Filename}' is outside texture bounds. Skipping mesh.");
                        continue;
                    }

                    using SKBitmap meshPiece = ImageProcessor.CropImage(texpic, cropX, cropY, cropWidth, cropHeight);


                    // Paste the mesh piece onto the output image
                    ImageProcessor.PasteImage(outimg, meshPiece, (int)mesh.SrcOffsetX, (int)mesh.SrcOffsetY);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing mesh for block '{block.Filename}' from '{texFileName}': {ex.Message}");
                    // print stack trace
                    Console.WriteLine(ex.StackTrace);
                    continue; // Continue with next mesh/block
                }
            }

            // Character sprite handling logic
            if (!_isChara)
            {
                if (block.Filename != block.FilenameOld)
                {
                    _isChara = true;
                    _charaBase = block.FilenameOld ?? ""; // Use ?? "" to handle potential null
                    _charaBaseX = (int)block.OffsetX;
                    _charaBaseY = (int)block.OffsetY;
                }
            }

            if (_isChara)
            {
                if (_charaBase == block.FilenameOld)
                {
                    // Save the base image
                    string outputFileName = Path.Combine(outputDirectory, $"{block.FilenameOld}.png");
                    try
                    {
                        ImageProcessor.SaveImage(outimg, outputFileName);
                        Console.WriteLine($"Saved '{outputFileName}'");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving base image for block '{block.Filename}': {ex.Message}");
                    }
                }
                else
                {
                    // This is a derived character sprite, overlay on the base image
                    string baseImagePath = Path.Combine(outputDirectory, $"{_charaBase}.png");
                    if (File.Exists(baseImagePath))
                    {
                        try
                        {
                            using SKBitmap baseImg = ImageProcessor.LoadImageFromFile(baseImagePath);
                            int pasteX = (int)block.OffsetX - _charaBaseX;
                            int pasteY = (int)block.OffsetY - _charaBaseY;
                            ImageProcessor.PasteImage(baseImg, outimg, pasteX, pasteY);

                            // Save the combined image, overwriting if necessary (following dec.py logic)
                            string outputFileName = Path.Combine(outputDirectory, $"{block.FilenameOld}.png");
                            ImageProcessor.SaveImage(baseImg, outputFileName);
                            Console.WriteLine($"Saved derived character sprite to '{outputFileName}'");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing derived character sprite for block '{block.FilenameOld}': {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Base image '{_charaBase}.png' not found for block '{block.FilenameOld}'. Cannot overlay.");
                        // If base image not found, save the current block image using non-character logic
                        string outputFileName = Path.Combine(outputDirectory, $"{block.Filename}.png");
                        if (File.Exists(outputFileName))
                        {
                            outputFileName = Path.Combine(outputDirectory, $"{block.Filename}_{block.Priority}.png");
                        }
                        try
                        {
                            ImageProcessor.SaveImage(outimg, outputFileName);
                            Console.WriteLine($"Saved '{outputFileName}'");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error saving image for block '{block.Filename}': {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                // Original saving logic for non-character sprites
                string outputFileName = Path.Combine(outputDirectory, $"{block.Filename}.png");
                if (File.Exists(outputFileName))
                {
                    outputFileName = Path.Combine(outputDirectory, $"{block.Filename}_{block.Priority}.png");
                }

                try
                {
                    ImageProcessor.SaveImage(outimg, outputFileName);
                    Console.WriteLine($"Saved '{outputFileName}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving image for block '{block.Filename}': {ex.Message}");
                }
            }
        }
    }
}
