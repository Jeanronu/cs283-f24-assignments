using System;
using System.Drawing;
using System.Windows.Forms;

public class Game
{
    private Player player; // Player object
    private Obstacle obstacle; // Obstacle object
    private Sock sock;  // Sock object
    private bool isGameOver = false; // Did the player lose?
    private float elapsedTime = 0f; // Total time elapsed since the game started
    private float sockVisibleTime = 4f; // Time for which the sock remains visible
    private float sockAppearanceDelay = 20f; // Sock appears after 20 seconds of gameplay
    private float sockTimer = 0f; // Timer for controlling sock visibility duration
    private float sockCooldownTimer = 0f; // Timer for cooldown between sock appearances
    private float sockCooldownDuration = 5f; // Cooldown duration of 5 seconds
    private bool isSockVisible = false;
    private bool isInCooldown = false; // Flag for sock cooldown

    public void Setup()
    {
        player = new Player(400, Window.height / 2);
        obstacle = new Obstacle();
        sock = new Sock(); 
    }

    public void Update(float dt)
    {
        if (!isGameOver)
        {
            player.Update(dt);
            obstacle.Update(dt);
            elapsedTime += dt;  // Update the total elapsed time

            // Sock appearance logic: Only after 20 seconds and when not in cooldown
            if (elapsedTime >= sockAppearanceDelay && !isSockVisible && !isInCooldown)
            {
                sock.Spawn();  // Spawn the sock
                isSockVisible = true;
                sockTimer = 0f;  // Reset sock visibility timer
            }

            // If the sock is visible, update its timer and check if the player took it
            if (isSockVisible)
            {
                sockTimer += dt;

                // Sock disappears after being visible for 4 seconds
                if (sockTimer >= sockVisibleTime)
                {
                    sock.Disappear();  // Sock disappears if not taken
                    isSockVisible = false;  // Reset visibility
                    isInCooldown = true;  // Enter cooldown period
                    sockCooldownTimer = 0f;  // Reset cooldown timer
                }
                else if (sock.CheckCollision(player))  // Player took the sock
                {
                    sock.Collect();  // Sock is collected
                    isSockVisible = false;  // Hide the sock after being taken
                    isInCooldown = true;  // Enter cooldown period
                    sockCooldownTimer = 0f;  // Reset cooldown timer
                    obstacle.SlowDown(3);  // Slow down obstacles for 3 seconds
                }
            }

            // Cooldown logic: Handle sock cooldown after disappearance
            if (isInCooldown)
            {
                sockCooldownTimer += dt;
                if (sockCooldownTimer >= sockCooldownDuration)
                {
                    isInCooldown = false;  // Cooldown complete, ready for the sock to reappear
                }
            }

            // Check for collisions between player and obstacles
            if (obstacle.CheckCollision(player))
            {
                isGameOver = true;
            }
        }
    }

    public void Draw(Graphics g)
    {
        if (isGameOver)
        {
            g.DrawString("Game Over!", new Font("Arial", 24), Brushes.Black, Window.width / 2 - 100, Window.height / 2 - 40);

            // Draw the total time achieved
            string timerText = $"Total Time: {elapsedTime:F2} seconds";
            using (Font font = new Font("Arial", 18))
            {
                g.DrawString(timerText, font, Brushes.Black, Window.width / 2 - 150, Window.height / 2);
            }
        }
        else
        {
            player.Draw(g);
            obstacle.Draw(g);

            // Draw the sock if it is visible
            if (isSockVisible)
            {
                sock.Draw(g);
            }

            // Draw the timer in the top-left corner
            string timerText = $"Time: {elapsedTime:F2}";
            using (Font font = new Font("Arial", 12))
            {
                g.DrawString(timerText, font, Brushes.Black, new PointF(10, 10));
            }

            // Draw the box in the bottom-left corner with name and class year
            Rectangle boxRect = new Rectangle(10, Window.height - 50 - 10, 150, 50);
            g.FillRectangle(Brushes.LightGray, boxRect);  // Fill box with light gray color

            using (Font font = new Font("Arial", 12)) // put the text on the box
            {
                g.DrawString("Jean Rojas", font, Brushes.Black, new PointF(10 + 10, Window.height - 50 - 10 + 5));
                g.DrawString("Class of 2026", font, Brushes.Black, new PointF(10 + 10, Window.height - 50 - 10 + 25));
            }
        }
    }

    public void MouseClick(MouseEventArgs mouse)
    {
        if (mouse.Button == MouseButtons.Left)
        {
            System.Console.WriteLine(mouse.Location.X + ", " + mouse.Location.Y);
        }
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (key.KeyCode == Keys.Up || key.KeyCode == Keys.W)
        {
            player.Y -= player.Speed;  // Move up
        }
        else if (key.KeyCode == Keys.Down || key.KeyCode == Keys.S)
        {
            player.Y += player.Speed;  // Move down
        }
        else if (key.KeyCode == Keys.Left || key.KeyCode == Keys.A)
        {
            player.X -= player.Speed;  // Move left
        }
        else if (key.KeyCode == Keys.Right || key.KeyCode == Keys.D)
        {
            player.X += player.Speed;  // Move right
        }
    }
}