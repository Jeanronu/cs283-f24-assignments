using System;
using System.Drawing;

public class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Speed { get; set; } = 5;
    private float timeElapsed = 0f;  // Time elapsed since the start
    private float speedIncreaseRate = 1f; // Increase speed every 1 second
    private int maxSpeed = 30; // Max speed limit
    Image mImg = Image.FromFile("Mike.png");

    public Player(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Update(float dt)
    {
        timeElapsed += dt; // Time passed

        // Increase player speed over time
        if (timeElapsed >= speedIncreaseRate)
        {
            if (Speed < maxSpeed)
            {
                Speed++;  // Increase speed
            }
            timeElapsed = 0f;  // Reset time to track for the next speed increase
        }
      
        // Ensure player doesn't go out of window
        X = Math.Max(0, Math.Min(X, Window.width - 50));
        Y = Math.Max(0, Math.Min(Y, Window.height - 50));
    }

    public void Draw(Graphics g)
    {
        // Draw the player as an image
        g.DrawImage(mImg, X, Y, 50, 50);
    }
}