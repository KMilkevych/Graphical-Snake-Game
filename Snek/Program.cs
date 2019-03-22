using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Snek
{
    static class Program
    {
        /// <summary>
        /// The game runs natively at 400x400 resolution with cell size 20x20.
        /// Coordinates are calculated normally and then upscaled when drawn.
        /// </summary>
        
        static string[] themes = new string[] { "Light", "Dark" }; //Creates an array to store theme names
        static int currentThemeIndex = 0; //Defines a number that indicates current theme index in themes (themes[currentThemeIndex])

        static Color snekBody; //Declares the diffirent colors used in the game for easier change depending on the theme
        static Color snekHead;
        static Color apple;
        static Color bg;

        static int cellSize = 20; //Defines the cellsize
        static int levelWidth = 20; //Level Width in cells
        static int levelHeight = 20; //Level Height in cells

        static int snekTime = 100; //Time before snek body is moved in ms

        static List<Vector2i> bodyParts; //Declares a global list for containing the body parts coordinates
        static int snekDirection = 2; //Defines a global snake head direction

        static Vector2i appleCoordinates = new Vector2i(11, 10); //Defines the applecoordinates;

        static void Main()
        {

            bodyParts = new List<Vector2i>(); //Defines the global list for snake body parts
            bodyParts.Add(new Vector2i(10, 10)); //Adds three bodyparts for the snake, with the first one being the head
            bodyParts.Add(new Vector2i(9, 10));
            bodyParts.Add(new Vector2i(8, 10));


            RenderWindow window = new RenderWindow(new VideoMode(Convert.ToUInt32(levelWidth*cellSize), Convert.ToUInt32(levelHeight*cellSize)), "Snek9", Styles.Close); //Initalizes the window
            window.SetFramerateLimit(60);
            window.Closed += new EventHandler(OnClose); //Initializes the eventhandlers
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPress);

            //Defines a clock for gametime
            Clock gameClock = new Clock();

            //Defines a Random entity to take care of all randomizes calculations
            Random rnd = new Random();

            setColors(); //Sets the colors

            while (window.IsOpen) //The game loop
            {
                window.DispatchEvents();

                //All game logic starts from here
                if (gameClock.ElapsedTime.AsMilliseconds() >= snekTime) //Check if it should move snek
                {
                    int elNum = bodyParts.Count();
                    for (int i = 0; i < (elNum - 1); i++) //Move each and every bodypart except snekhead to the place of the bodypart before them
                    {
                        bodyParts[elNum - i - 1] = bodyParts[elNum - (i + 1) - 1];
                    }

                    int moveNum = 0;
                    if (snekDirection % 2 != 0) //Add moveNum to Y coordinate
                    {
                        moveNum = snekDirection - 2;
                        bodyParts[0] = new Vector2i(bodyParts[0].X, bodyParts[0].Y + moveNum);
                    }
                    else //Add moveNum to X coordinate
                    {
                        moveNum = snekDirection - 1;
                        bodyParts[0] = new Vector2i(bodyParts[0].X + moveNum, bodyParts[0].Y);
                    }

                    gameClock.Restart(); //Restarts the timer
                }

                if (bodyParts[0] == appleCoordinates) //Checks if snekhead has touched apple and acts accordingly
                {
                    //Moves the apple to new random coordinates
                    appleCoordinates = new Vector2i(rnd.Next(0, levelWidth), rnd.Next(0, levelHeight));

                    //Adds a new bodypart to snekbody at the coordinates of the last bodypart
                    int elNum = bodyParts.Count();
                    bodyParts.Add(new Vector2i(bodyParts[elNum - 1].X, bodyParts[elNum - 1].Y));
                }
                
                //Clears screen to white
                window.Clear(bg);

                RectangleShape cell = new RectangleShape(new Vector2f(cellSize, cellSize)); //Defines a shape for the cell

                //Draws every bodypart
                foreach (Vector2i bodyPart in bodyParts)
                {
                    cell.Position = new Vector2f(bodyPart.X * cellSize, bodyPart.Y * cellSize);
                    if (bodyParts[0] == bodyPart)
                    {
                        cell.FillColor = snekHead;
                    } else
                    {
                        cell.FillColor = snekBody;
                    }
                    window.Draw(cell);
                }

                //Draws the apple
                cell.Position = new Vector2f(appleCoordinates.X*cellSize, appleCoordinates.Y*cellSize);
                cell.FillColor = apple;
                window.Draw(cell);

                //Displayes the backbuffer
                window.Display();
            }
        }

        static void setColors() //Sets the game colors depending on the theme
        {
            if (themes[currentThemeIndex] == "Light")
            {
                snekBody = Color.Black;
                snekHead = Color.Red;
                apple = Color.Green;
                bg = Color.White;
            }
            else if (themes[currentThemeIndex] == "Dark")
            {
                snekBody = Color.White;
                snekHead = Color.Red;
                apple = Color.Green;
                bg = Color.Black;
            }
        }

        //Defines the events
        static void OnClose(object sender, EventArgs e) //The window closing event
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void OnKeyPress(object sender, KeyEventArgs e) //The keypress event
        {
            //Check for which key was pressed and acts accordingly
            if (e.Code == Keyboard.Key.Escape)
            {
                RenderWindow window = (RenderWindow)sender;
                window.Close();
            }

            if (e.Code == Keyboard.Key.Left)
            {
                snekDirection = 0;
            }

            if (e.Code == Keyboard.Key.Up)
            {
                snekDirection = 1;
            }

            if (e.Code == Keyboard.Key.Right)
            {
                snekDirection = 2;
            }

            if (e.Code == Keyboard.Key.Down)
            {
                snekDirection = 3;
            }

            if (e.Code == Keyboard.Key.Q)
            {
                int elNum = themes.Length;
                if (currentThemeIndex >= (elNum - 1))
                {
                    currentThemeIndex = 0;
                } else
                {
                    currentThemeIndex++;
                }

                setColors();
            }
        }
    }
}
