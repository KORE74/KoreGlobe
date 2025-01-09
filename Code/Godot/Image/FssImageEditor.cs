// A static class holding utility routines around editing images.
// Uses the godot engine's Image class to perform operations on images: https://docs.godotengine.org/en/stable/classes/class_image.html

using Godot;

public static class FssImageEditor
{
    // --------------------------------------------------------------------------------------------
    // MARK: Basic Loading and Saving
    // --------------------------------------------------------------------------------------------

    public static Image LoadImage(string path)
    {
        Image img = new Image();
        Error err = img.Load(path);
        if (err != Error.Ok)
        {
            FssCentralLog.AddEntry($"Error loading image: {err}");
            return null;
        }
        return img;
    }

    public static void SaveImageWebp(Image img, string path)
    {
        Error err = img.SaveWebp(path);
        if (err != Error.Ok)
        {
            FssCentralLog.AddEntry($"Error saving image: {err}");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Basic Editing
    // --------------------------------------------------------------------------------------------

    public static Image ResizeImage(Image img, int newwidth, int newheight)
    {
        // Create a new image to return
        Image newSizeImage = new Image();
        newSizeImage.CopyFrom(img);

        // Resize and return
        newSizeImage.Resize(newwidth, newheight);
        return newSizeImage;
    }

    public static Image CropImage(Image img, int topleftx, int toplefty, int width, int height)
    {
        // Create a new image to return
        Image newSizeImage = new Image();
        newSizeImage.CopyFrom(img);

        // Check the bounds
        topleftx = Mathf.Clamp(topleftx, 0, img.GetWidth());
        toplefty = Mathf.Clamp(toplefty, 0, img.GetHeight());
        width    = Mathf.Clamp(width, 0, img.GetWidth() - topleftx);
        height   = Mathf.Clamp(height, 0, img.GetHeight() - toplefty);

        Rect2I rect = new Rect2I(topleftx, toplefty, width, height);

        // Get the region of the image to crop
        Image newimg = img.GetRegion(rect);
        return newimg;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Advanced Editing
    // --------------------------------------------------------------------------------------------

    public static Image OverlaySubImage(Image mainimage, Image subimage, Fss2DGridPos subimagePos)
    {
        // Calculate the position of the subimage in the main image
        int x = (int)(subimagePos.LeftEdgeFraction   * mainimage.GetWidth());
        int y = (int)(subimagePos.TopEdgeFraction    * mainimage.GetHeight());
        int w = (int)(subimagePos.RightEdgeFraction  * mainimage.GetWidth()) - x;
        int h = (int)(subimagePos.BottomEdgeFraction * mainimage.GetHeight()) - y;

        // Create a new image to return
        Image retMainImg = new Image();
        retMainImg.CopyFrom(mainimage);

        // Create the variables for the subimage call
        Rect2I subimagerect = new Rect2I(0, 0, w, h);
        Vector2I subimagepos = new Vector2I(x, y);

        // Copy the main image to the new image
        retMainImg.BlitRect(subimage, subimagerect, subimagepos);

        return retMainImg;
    }

}