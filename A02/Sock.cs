using System.Drawing;

public class Sock
{
    private int x, y;
    private Image sockImg = Image.FromFile("Sock.png");  // Load the sock image
    public bool IsVisible { get; private set; } = false;  // Whether the sock is currently visible

    // Function to spawn the sock
    public void Spawn()
    {
        x = Window.RandomRange(0, Window.width - 50);  // Random X position
        y = Window.RandomRange(0, Window.height - 50);  // Random Y position
        IsVisible = true;  // Make the sock visible
    }

    // Function to collect the sock
    public void Collect()
    {
        IsVisible = false;  // Hide the sock after it's taken
    }

    // Function to make the sock disappear after time runs out
    public void Disappear()
    {
        IsVisible = false;  // Hide the sock after it expires
    }

    // Check if the player has collided with the sock
    public bool CheckCollision(Player player)
    {
        // Check if the player's rectangle intersects with the sock
        Rectangle playerRect = new Rectangle(player.X, player.Y, 50, 50);
        Rectangle sockRect = new Rectangle(x, y, 50, 50);

        return playerRect.IntersectsWith(sockRect);
    }

    // Draw the sock on the screen
    public void Draw(Graphics g)
    {
        if (IsVisible)
        {
            g.DrawImage(sockImg, x, y, 50, 50);  // Draw the sock if it's visible
        }
    }
}