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

        static bool gamePaused = false; //If true game is not going to update player position

        static string[] themes = new string[] { "Light", "Dark" }; //Creates an array to store theme names
        static int currentThemeIndex = 0; //Defines a number that indicates current theme index in themes (themes[currentThemeIndex])

        static Color snekBodyColor; //Declares the diffirent colors used in the game for easier change depending on the theme
        static Color snekHeadColor;
        static Color appleColor;
        static Color backgroundColor;

        static int cellSize = 20; //Defines the cellsize
        static int levelWidth = 20; //Level Width in cells
        static int levelHeight = 20; //Level Height in cells

        static int snekTime = 100; //Time before snek body is moved in ms

        static List<Vector2i> bodyParts; //Declares a global list for containing the body parts coordinates
        static int snekDirection = 2; //Defines a global snake head direction

        static Vector2i appleCoordinates; //Declares the apples coordinates;

        static void Main()
        {
            //Defines a Random entity to take care of all randomizes calculations
            Random rnd = new Random();

            bodyParts = new List<Vector2i>(); //Defines the global list for snake body parts
            bodyParts.Add(new Vector2i(10, 10)); //Adds three bodyparts for the snake, with the first one being the head
            bodyParts.Add(new Vector2i(9, 10));
            bodyParts.Add(new Vector2i(8, 10));

            appleCoordinates = new Vector2i(rnd.Next(0, levelWidth), rnd.Next(0, levelHeight)); //Sets random apple coordinates

            RenderWindow window = new RenderWindow(new VideoMode(Convert.ToUInt32(levelWidth*cellSize), Convert.ToUInt32(levelHeight*cellSize)), "Snek9", Styles.Close); //Initalizes the window
            window.SetFramerateLimit(60);
            window.Closed += new EventHandler(OnClose); //Initializes the eventhandlers
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPress);

            //Defines a clock for gametime
            Clock gameClock = new Clock();            

            setColors(); //Sets the colors

            while (window.IsOpen) //The game loop
            {
                window.DispatchEvents();

                //All game logic starts from here
                if (!gamePaused) //Only update and check collissions if game not paused
                {
                    //Defines variables to store past head coordinates so that the snake can be moved a step back when it dies (looks prettier)
                    Vector2i previousHeadCoordinates = new Vector2i(0, 0);


                    if (gameClock.ElapsedTime.AsMilliseconds() >= snekTime) //Check if it should move snek
                    {

                        int elNum = bodyParts.Count();
                        for (int i = 0; i < (elNum - 1); i++) //Move each and every bodypart except snekhead to the place of the bodypart before them
                        {
                            bodyParts[elNum - i - 1] = bodyParts[elNum - (i + 1) - 1];
                        }

                        //Sets the previousHeadCoordinates variable before head's coordinates are changed
                        previousHeadCoordinates = new Vector2i(bodyParts[0].X, bodyParts[0].Y);


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

                    //Check for collissions with walls
                    if (bodyParts[0].X < 0 || bodyParts[0].X > 19 || bodyParts[0].Y < 0 || bodyParts[0].Y > 19)
                    {
                        //Pause game
                        gamePaused = true;

                        //Move snek head a step back
                        bodyParts[0] = previousHeadCoordinates;
                    }

                    //Check for collissions with bodyparts
                    for (int i = 1; i < bodyParts.Count; i++)
                    {
                        if (bodyParts[0].X == bodyParts[i].X && bodyParts[0].Y == bodyParts[i].Y)
                        {
                            //Pause game
                            gamePaused = true;

                            //Move snek head a step back
                            bodyParts[0] = previousHeadCoordinates;
                        }
                    }
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
                window.Clear(backgroundColor);

                RectangleShape cell = new RectangleShape(new Vector2f(cellSize, cellSize)); //Defines a shape for the cell

                //Draws every bodypart
                foreach (Vector2i bodyPart in bodyParts)
                {
                    cell.Position = new Vector2f(bodyPart.X * cellSize, bodyPart.Y * cellSize);
                    if (bodyParts[0] == bodyPart)
                    {
                        cell.FillColor = snekHeadColor;
                    } else
                    {
                        cell.FillColor = snekBodyColor;
                    }
                    window.Draw(cell);
                }

                //Draws the apple
                cell.Position = new Vector2f(appleCoordinates.X*cellSize, appleCoordinates.Y*cellSize);
                cell.FillColor = appleColor;
                window.Draw(cell);

                //Displayes the backbuffer
                window.Display();
            }
        }

        static void setColors() //Sets the game colors depending on the theme
        {
            if (themes[currentThemeIndex] == "Light")
            {
                snekBodyColor = Color.Black;
                snekHeadColor = Color.Red;
                appleColor = Color.Green;
                backgroundColor = Color.White;
            }
            else if (themes[currentThemeIndex] == "Dark")
            {
                snekBodyColor = Color.White;
                snekHeadColor = Color.Red;
                appleColor = Color.Green;
                backgroundColor = Color.Black;
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
