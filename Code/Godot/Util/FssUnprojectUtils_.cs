using Godot;

public static class FssUnprojectUtils
{
    public static bool IsPointInFrontOfCameraPlane(Vector3 gePosition, Camera3D camera)
    {
        // Calculate vector from camera position to the point
        Vector3 cameraToPoint = gePosition - camera.GlobalTransform.Origin;

        // Dot product with the negative Z basis to determine if the point is in front
        float dotProduct = cameraToPoint.Dot(-camera.GlobalTransform.Basis.Z);

        // If the dot product is positive, the point is in front of the camera
        return dotProduct > 0;
    }

    public static (Vector2 position, bool success) UnprojectPoint(Vector3 worldPosition, Camera3D camera, Viewport viewport)
    {
        // Check if the point is behind the camera using the dot product with the camera's forward vector
        if (worldPosition.Dot(camera.GlobalTransform.Basis.Z) > 0)
        {
            return (Vector2.Zero, false); // The point is behind the camera
        }

        // Project the 3D point to 2D screen coordinates
        Vector2 screenPosition = camera.UnprojectPosition(worldPosition);

        // Check if the point is within the screen bounds
        Vector2 screenSize = viewport.GetVisibleRect().Size;

        // Define a buffer margin around the screen size to allow for slightly off-screen points
        // This buffer extends one screen width in both horizontal directions and one screen height in both vertical directions.
        // float bufferX = screenSize.X * 2;
        // float bufferY = screenSize.Y * 2;

        bool success = true; //screenPosition.X >= -bufferX && screenPosition.X <= screenSize.X + bufferX &&
                       //screenPosition.Y >= -bufferY && screenPosition.Y <= screenSize.Y + bufferY;

        return (screenPosition, success);
    }
}


// Viewport viewport = Engine.GetMainLoop() is SceneTree sceneTree ? sceneTree.Root : null;
// return UnprojectPoint(worldPosition, camera, viewport);


