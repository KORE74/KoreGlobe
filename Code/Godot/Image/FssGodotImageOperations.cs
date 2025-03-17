
using System;
using System.IO;

using Godot;

#nullable enable

// FssGodotImageOperations: Image operations for Godot, using the Godot Image class.

public static class FssGodotImageOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK: Load Save
    // --------------------------------------------------------------------------------------------

    public static Image? LoadImage(string filepath)
    {
        try
        {
            // Ensure the path has the right / separator characters
            filepath = FssFileOperations.StandardizePath(filepath);

            // Check the file exists
            if (!File.Exists(filepath))
                return null;

            // Load the image
            Image image = new Image();
            Error err = image.Load(filepath);
            if (err != Error.Ok)
                return null;

            return image;
        }
        catch
        {
            return null;
        }
    }

    // --------------------------------------------------------------------------------------------

    // Save an image to a file.

    // Usage example: FssGodotImageOperations.SaveImage(image, "C:/Users/User/Documents/image.png");
    public static void SaveImagePng(Image image, string filepath)
    {
        // Ensure the path has the right / separator characters
        filepath = FssFileOperations.StandardizePath(filepath);

        // Save the image
        image.SavePng(filepath);
    }

    // --------------------------------------------------------------------------------------------

    // Save an image to a file: Webp format.

    // Usage example: FssGodotImageOperations.SaveImageWebp(image, "C:/Users/User/Documents/image.webp");

    public static void SaveImageWebp(Image image, string filepath)
    {
        // Ensure the path has the right / separator characters
        filepath = FssFileOperations.StandardizePath(filepath);

        // Save the image
        image.SaveWebp(filepath);
    }

    // --------------------------------------------------------------------------------------------

    public static void PngToWebp(string pngFilepath, string webpFilepath)
    {
        // Load the image - using clause ensures the image is disposed of after use
        using (Image? image = LoadImage(pngFilepath))
        {
            if (image == null)
                return;

            // Save the image
            SaveImageWebp(image, webpFilepath);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Resize
    // --------------------------------------------------------------------------------------------

    // Resize an image to a new size.

    // Usage example: FssGodotImageOperations.ResizeImage(image, 100, 100);
    public static Image ResizeImage(Image srcImage, int newWidth, int newHeight)
    {
        // Create a new image
        Image copyImage = new Image();
        copyImage.CopyFrom(srcImage);

        // Resize the image
        copyImage.Resize(newWidth, newHeight);

        return copyImage;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Texture
    // --------------------------------------------------------------------------------------------

    public static ImageTexture? LoadToTexture(string filepath)
    {
        ImageTexture? newTexture = null;

        try
        {
            // Load the image
            Image? image = LoadImage(filepath);
            if (image == null)
                return null;

            // Create a texture from the loaded image
            newTexture = ImageTexture.CreateFromImage(image);
            image.Dispose(); // Release the image resource
        }
        catch
        {
            return null;
        }

        return newTexture;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Material
    // --------------------------------------------------------------------------------------------

    // Usage example: Material? material = FssGodotImageOperations.LoadToMaterial("C:/Users/User/Documents/image.png");

    public static Material? LoadToMaterial(string filepath)
    {
        // Load the image
        Image? image = LoadImage(filepath);
        if (image == null)
            return null;

        // Create a texture from the loaded image
        ImageTexture? newTexture = ImageTexture.CreateFromImage(image);
        image.Dispose(); // Release the image resource

        if (newTexture == null)
            return null;

        // Create a material from the texture
        var material = new StandardMaterial3D
        {
            AlbedoTexture = newTexture,
            ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded
        };
        return material;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Dispose
    // --------------------------------------------------------------------------------------------

    public static void Dispose(Image image)
    {
        image.Dispose();
    }

    public static void Dispose(ImageTexture tex)
    {
        tex.Dispose();
    }

    public static void Dispose(Material mat)
    {
        mat.Free();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Collate
    // --------------------------------------------------------------------------------------------

    // Collate a 2D array of images into a single image.

    // Usage example: Image newImg = FssGodotImageOperations.CollateImages(images);

    public static Image CollateImages(Image[,] srcImages, int outputWidth, int outputHeight)
    {
        // Get the size of the images
        int srcArrayWidth  = srcImages.GetLength(0);
        int srcArrayHeight = srcImages.GetLength(1);

        // Get the size of the images
        int srcImageWidth  = srcImages[0, 0].GetWidth();
        int srcImageHeight = srcImages[0, 0].GetHeight();

        // Create a new image to hold the collated images
        int combinedWidth   = srcArrayWidth  * srcImageWidth;
        int combinedHeight  = srcArrayHeight * srcArrayHeight;
        Image combinedImage = Image.CreateEmpty(combinedWidth, combinedHeight, false, Image.Format.Rgba8);

        // loop through the images and copy them into the combined image
        for (int x=0; x<srcArrayWidth; x++)
        {
            for (int y=0; y<srcArrayHeight; y++)
            {
                Rect2I srcRect = new Rect2I(0, 0, srcImageWidth, srcImageHeight);

                int destX = x * srcImageWidth;
                int destY = y * srcImageHeight;
                Vector2I destPos = new Vector2I(destX, destY);

                combinedImage.BlitRect(srcImages[x, y], srcRect, destPos);
            }
        }

        // resize the final image
        combinedImage.Resize(outputWidth, outputHeight);

        return combinedImage;
    }

    // --------------------------------------------------------------------------------------------

    // Sparse collate image: Have a backdrop and check each element of the source array for a potentially
    // null image, which we ignore.

    public static Image CollateSparseImages(Image backdropImage, Image[,] sparseSrcImages, int outputWidth, int outputHeight)
    {
        // Get the size of the images
        int srcArrayWidth  = sparseSrcImages.GetLength(0);
        int srcArrayHeight = sparseSrcImages.GetLength(1);

        // Get the size of the images
        int srcImageWidth  = backdropImage.GetWidth();
        int srcImageHeight = backdropImage.GetHeight();

        // Create a new image (from backdrop) to hold the collated images
        int combinedWidth   = srcArrayWidth  * srcImageWidth;
        int combinedHeight  = srcArrayHeight * srcArrayHeight;
        Image combinedImage = ResizeImage(backdropImage, combinedWidth, combinedHeight);

        // loop through the images and copy them into the combined image
        for (int x = 0; x<srcArrayWidth; x++)
        {
            for (int y = 0; y<srcArrayHeight; y++)
            {
                if (sparseSrcImages[x, y] == null)
                    continue;

                Rect2I srcRect = new Rect2I(0, 0, srcImageWidth, srcImageHeight);

                int destX = x * srcImageWidth;
                int destY = y * srcImageHeight;
                Vector2I destPos = new Vector2I(destX, destY);

                combinedImage.BlitRect(sparseSrcImages[x, y], srcRect, destPos);
            }
        }

        // resize the final image
        combinedImage.Resize(outputWidth, outputHeight);

        return combinedImage;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Divide
    // --------------------------------------------------------------------------------------------

    // Divide an input image into an array of images.
    // First we resize the source image to a multiple of the output sizes and counts, then copy out the individual images.
    // Aims to maintain the highest resolution through the operation.

    // Usage example: Image[,] images = FssGodotImageOperations.DivideImage(image, 3, 3, 100, 100, 300);

    public static Image[,] DivideImage(Image image, int arrayWidth, int arrayHeight, int imageWidth, int imageHeight)
    {
        // Copy and resize the image to a multiple of the output sizes and counts
        int resizedWidth  = imageWidth  * arrayWidth;
        int resizedHeight = imageHeight * arrayHeight;
        Image resizedImage = Image.CreateEmpty(resizedWidth, resizedHeight, false, Image.Format.Rgba8);
        resizedImage.CopyFrom(image);
        resizedImage.Resize(resizedWidth, resizedHeight);

        // Create the output array
        Image[,] outputImages = new Image[arrayWidth, arrayHeight];

        for (int x=0; x<arrayWidth; x++)
        {
            for (int y=0; y<arrayHeight; y++)
            {
                int srcMinX = x * imageWidth;
                int srcMinY = y * imageHeight;
                Rect2I srcRect = new Rect2I(srcMinX, srcMinY, imageWidth, imageHeight);

                outputImages[x, y] = resizedImage.GetRegion(srcRect);
            }
        }

        return outputImages;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Brightness
    // --------------------------------------------------------------------------------------------

    public static (float averageBrightness, float averageSaturation) GetAverageBrightnessAndSaturation(Image image)
    {
        int width  = image.GetWidth();
        int height = image.GetHeight();
        long totalBrightness = 0;
        long totalSaturation = 0;
        int pixelCount = width * height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                float r = pixel.R;
                float g = pixel.G;
                float b = pixel.B;

                // Calculate brightness (luminance)
                float luminance = 0.2126f * r + 0.7152f * g + 0.0722f * b;
                totalBrightness += (long)(luminance * 255);

                // Calculate saturation
                float maxChannel = FssNumericUtils<float>.Max(r, g, b);
                float minChannel = FssNumericUtils<float>.Min(r, g, b);
                float saturation = maxChannel == 0 ? 0 : (maxChannel - minChannel) / maxChannel;
                totalSaturation += (long)(saturation * 255);
            }
        }

        float averageBrightness = totalBrightness / (float)pixelCount;
        float averageSaturation = totalSaturation / (float)pixelCount;
        return (averageBrightness, averageSaturation);
    }

    // --------------------------------------------------------------------------------------------

    public static void ApplyAverageValuesToImage(Image image, float targetBrightness, float targetSaturation)
    {
        int width  = image.GetWidth();
        int height = image.GetHeight();

        // Analyze the current image to get its average brightness and saturation
        var (currentBrightness, currentSaturation) = GetAverageBrightnessAndSaturation(image);

        // Calculate deltas for adjustment
        float brightnessDelta = (targetBrightness - currentBrightness) / 255.0f; // Normalize to [0, 1]
        float saturationDelta = (targetSaturation - currentSaturation) / 255.0f; // Normalize to [0, 1]

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = image.GetPixel(x, y);

                // Adjust brightness and saturation in HSV
                float hue        = pixel.H; // Preserve hue
                float saturation = FssNumericUtils<float>.Clamp(pixel.S + saturationDelta, 0, 1);
                float value      = FssNumericUtils<float>.Clamp(pixel.V + brightnessDelta, 0, 1);

                // Convert back to RGB
                Color adjustedPixel = Color.FromHsv(hue, saturation, value);
                image.SetPixel(x, y, adjustedPixel);
            }
        }
    }


}