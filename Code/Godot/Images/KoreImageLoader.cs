
// KoreImageLoader:
// - A throttling point to load only a certain level of images, keep a count of "pixels loaded" to contain memory usage.
// - A Godot specific class, we can't waste time loading images in SkiaSharp of similar, then transferring across to Godot framework.

using System.Collections.Generic;
using Godot;
using KoreCommon;
using KoreSim;

#nullable enable

// ------------------------------------------------------------------------------------------------

public class KoreImageDetails
{
    public string ImagePath { get; set; }
    public Texture2D ImageTexture { get; set; }
    public int PixelCount { get; set; }

    public KoreImageDetails(string imagePath, Texture2D newTexture, int pixelCount)
    {
        ImagePath = imagePath;
        ImageTexture = newTexture;
        PixelCount = pixelCount;
    }
}

// ------------------------------------------------------------------------------------------------

public class KoreImageLoader
{
    // --------------------------------------------------------------------------------------------
    // MARK: Attribs
    // --------------------------------------------------------------------------------------------

    // Examples:
    // - 128x128 = 16,384 pixels
    // - 512x512 = 262,144 pixels
    // - 1024x1024 = 1,048,576 pixels
    // We reject the loading of images that would exceed the max pixel count.

    private List<KoreImageDetails> LoadedImages = new List<KoreImageDetails>();

    private static int _maxPixels = 10000000; // 10 million pixels
    private static int _currentPixels = 0;

    public static int MaxPixels
    {
        get => _maxPixels;
        set => _maxPixels = value;
    }

    public static int CurrentPixels => _currentPixels;

    // --------------------------------------------------------------------------------------------
    // MARK: Load / Get
    // --------------------------------------------------------------------------------------------

    private static Image? LoadDirectWebp(string filePath)
    {
        // Load image synchronously
        var image = new Image();
        var err = image.Load(filePath);

        if (err != Error.Ok) return null;
        if (image.IsEmpty()) return null;

        return image;
    }

    // --------------------------------------------------------------------------------------------

    public KoreImageDetails? LoadImage(string filePath)
    {
        // Load the image directly
        var image = LoadDirectWebp(filePath);
        if (image == null) return null;

        // Check if we have the capacity for the image
        int newImagePixels = image.GetWidth() * image.GetHeight();
        if (_currentPixels + newImagePixels > _maxPixels)
        {
            // Not enough room for this image
            GD.Print($"KoreImageLoader: Not enough room for image {filePath}. Current pixels: {_currentPixels}, New image pixels: {newImagePixels}, Max pixels: {_maxPixels}");
            return null;
        }

        // Create the Texture2D
        var texture = new ImageTexture();
        texture.SetImage(image);

        // Free CPU memory used by the image, it was only ever a vehicle for creating the texture
        image.Dispose();

        // Create a KoreImageDetails object
        KoreImageDetails newImgDetails = new KoreImageDetails(filePath, texture, newImagePixels);

        // Add to the loaded images list
        LoadedImages.Add(newImgDetails);

        // Add to the total pixel count
        _currentPixels += newImagePixels;

        return newImgDetails;
    }

    // --------------------------------------------------------------------------------------------

    public KoreImageDetails? GetLoadedImage(string filePath)
    {
        // Find the image in the loaded images list
        foreach (var imgDetails in LoadedImages)
        {
            if (imgDetails.ImagePath == filePath)
            {
                return imgDetails;
            }
        }

        // If not found, return null
        return null;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Remove
    // --------------------------------------------------------------------------------------------

    public void RemoveImage(KoreImageDetails imageDetails)
    {
        if (imageDetails == null) return;

        // Remove from the loaded images list
        LoadedImages.Remove(imageDetails);

        // Subtract from the total pixel count
        _currentPixels -= imageDetails.PixelCount;
    }

    // --------------------------------------------------------------------------------------------

    private int CountLoadedPixels()
    {
        int totalPixels = 0;
        foreach (var imgDetails in LoadedImages)
        {
            totalPixels += imgDetails.PixelCount;
        }
        return totalPixels;
    }

}