using System.Collections.Generic;
using System.Drawing;
using System;

public class Obstacle
{
    private List<Rectangle> obstacles = new List<Rectangle>();
    private float spawnTimer = 0f;
    private float spawnRate = 2f; // Time between obstacles
    private float minSpawnRate = 0.3f; // Minimum time between obstacles
    private int obstacleSpeed = 2;
    private float timeElapsed = 0f;  // Time elapsed since the start
    private float IncreaseRate = 0.9f; // Increase by seconds
    private int maxSpeed = 25; // Max speed
    private int originalSpeed; // Store the original speed for slow-down effect
    private float slowDownTimer = 0f; // Timer for the slow-down effect
    private bool isSlowedDown = false; // See if obstacles are currently slowed down
    private float slowDownDuration = 3f; // Duration of the slow-down effect
    private Image obstacleImage = Image.FromFile("Bu.png"); // Load obstacle image

    public void Update(float dt)
    {
        spawnTimer += dt;
        timeElapsed += dt; // time passed

        // If obstacles are slowed down, update the slow-down timer
        if (isSlowedDown)
        {
            slowDownTimer += dt;

            // Check if slow-down effect is over
            if (slowDownTimer >= slowDownDuration)
            {
                obstacleSpeed = originalSpeed; // Restore original speed
                isSlowedDown = false; // Stop the slow-down effect
                slowDownTimer = 0f; // Reset the timer
            }
        }

        // Increase obstacle speed over time until maximum speed is reached
        if (timeElapsed >= IncreaseRate && !isSlowedDown) // Only increase speed if not slowed down
        {
            if (obstacleSpeed < maxSpeed)
            {
                obstacleSpeed++;  // Increase speed
            }

            // Decrease spawn rate over time, but not below the minimum spawn rate
            if (spawnRate > minSpawnRate)
            {
                spawnRate -= 0.05f;  // Reduce spawn rate
            }

            timeElapsed = 0f;  // Reset the elapsed time for speed and spawn adjustments
        }

        // Spawn new obstacle if enough time has passed
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0;
            SpawnObstacle();
        }

        // Move obstacles to the left across the screen
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            obstacles[i] = new Rectangle(obstacles[i].X - obstacleSpeed, obstacles[i].Y, 50, 50);
            if (obstacles[i].X < 0)
            {
                obstacles.RemoveAt(i);
            }
        }
    }

    public void Draw(Graphics g)
    {
        // Draw obstacles as images
        foreach (var obstacle in obstacles)
        {
            g.DrawImage(obstacleImage, obstacle.X, obstacle.Y, obstacle.Width, obstacle.Height); // Draw image at obstacle's position
        }
    }

    private void SpawnObstacle()
    {
        int y = Window.RandomRange(0, Window.height - 50); // Random y position
        obstacles.Add(new Rectangle(Window.width, y, 50, 50)); // Obstacle appears on the right
    }

    public bool CheckCollision(Player player)
    {
        // Check if the player's rectangle intersects with any obstacle
        Rectangle playerRect = new Rectangle(player.X, player.Y, 50, 50);
        foreach (var obstacle in obstacles)
        {
            if (playerRect.IntersectsWith(obstacle))
            {
                return true;
            }
        }
        return false;
    }

    // Method to slow down the obstacle speed for a given duration
    public void SlowDown(float duration)
    {
        if (!isSlowedDown) // Only slow down if not already slowed
        {
            originalSpeed = obstacleSpeed; // Store the original speed
            obstacleSpeed = Math.Max(1, obstacleSpeed / 2); // Reduce speed by half, but ensure it's at least 1
            isSlowedDown = true; // Activate the slow-down effect
            slowDownDuration = duration; // Set the duration of the slow-down effect
        }
    }
}