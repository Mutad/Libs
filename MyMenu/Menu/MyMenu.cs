using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

//
//  OwO
//
//TODO add color and backgound modification to every element
//TODO make element events

namespace MyMenu
{

    namespace Utils
    {
        /**
         * Global settings for the menu
         */
        public class GlobalSettings
        {

            [JsonIgnore]
            public string filename = "menuGlobalSettings.json";

            //colors
            public ConsoleColor foreColor { get; private set; }
            public ConsoleColor backColor { get; private set; }
            public ConsoleColor foreColorSelected { get; private set; }
            public ConsoleColor backColorSelected { get; private set; }

            /**
             * Set colors to the element and inverted for selected element
             */
            public void SetColors(ConsoleColor foreColor, ConsoleColor backColor)
            {
                SetColors(foreColor, backColor, backColor, foreColor);
            }

            /**
             * Set colors to the element and to the selected element
             */
            public void SetColors(ConsoleColor foreColor, ConsoleColor backColor, ConsoleColor foreColorSelected, ConsoleColor backColorSelected)
            {
                this.foreColor = foreColor;
                this.backColor = backColor;
                this.foreColorSelected = foreColorSelected;
                this.backColorSelected = backColorSelected;
                Console.ForegroundColor = this.foreColor;
                Console.BackgroundColor = this.backColor;
            }

            /**
             * Load Global settings from file
             */
            public GlobalSettings()
            {
                Console.WriteLine("Global settings created");
                System.Threading.Thread.Sleep(1000);


                if (File.Exists(filename))
                {
                    using (FileStream fstream = File.OpenRead(filename))
                    {
                        // преобразуем строку в байты
                        byte[] array = new byte[fstream.Length];
                        // считываем данные
                        fstream.Read(array, 0, array.Length);
                        // декодируем байты в строку
                        string textFromFile = System.Text.Encoding.Default.GetString(array);
                        GlobalSettings sets = JsonConvert.DeserializeObject<GlobalSettings>(textFromFile);

                        SetColors(sets.foreColor, sets.backColor, sets.foreColorSelected, sets.backColorSelected);
                    }
                }
                else
                {
                    foreColor = ConsoleColor.White;
                    backColor = ConsoleColor.Black;
                    foreColorSelected = ConsoleColor.Black;
                    backColorSelected = ConsoleColor.White;
                }
            }

            /**
             * Constructor for NewtonSoftJson
             */
            [JsonConstructor]
            public GlobalSettings(ConsoleColor foreColor, ConsoleColor backColor, ConsoleColor foreColorSelected, ConsoleColor backColorSelected)

            {
                SetColors(foreColor, backColor, foreColorSelected, backColorSelected);
            }

            /**
             * Save settings to file before destroying
             */
            ~GlobalSettings()
            {
                Console.WriteLine("Saved");
                System.Threading.Thread.Sleep(1000);

                string text = JsonConvert.SerializeObject(this);
                using (FileStream fstream = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    // преобразуем строку в байты
                    byte[] array = System.Text.Encoding.Default.GetBytes(text);
                    // запись массива байтов в файл
                    fstream.Write(array, 0, array.Length);
                }
            }
        }

        /**
         * Settings for current menu instance
         */
        public class Settings
        {
            //clear console onStart
            public bool clearOnStartMenu;

            // control menu by pressing numbers instead of arrows
            public bool useReadKeyInput;

            //visibility of back button
            public bool isBackVisible;

            //visibility of header
            public bool isHeaderVisible;

            //back text
            public string backText;

            //header text
            public string headerText;

            //show error when wrong symbol is pressed
            public bool showWrongSymbolException;

            //
            public bool waitForReadKey;

            //colors
            public ConsoleColor foreColor;
            public ConsoleColor backColor;
            public ConsoleColor foreColorChoosed;
            public ConsoleColor backColorChoosed;


            // use menu from 2.0 version
            public bool usingNewMenu;

            // defaults
            public Settings()
            {
                waitForReadKey = true;
                clearOnStartMenu = true;
                useReadKeyInput = true;
                isBackVisible = true;
                backText = "Back";
                isHeaderVisible = true;
                showWrongSymbolException = true;
                headerText = "Header";

                backColor = ConsoleColor.Black;
                foreColor = ConsoleColor.White;
                backColorChoosed = ConsoleColor.White;
                foreColorChoosed = ConsoleColor.Black;

                usingNewMenu = true;
            }
        }

        /**
         * Events for current menu instance
         */
        public class Events
        {
            /// <summary>
            /// executes function when menu start
            /// </summary>
            public Action onStart;
            public Action onEnd;

            //executes function every time before menu cycle 
            public Action onStartCycle;
            public Action onEndCycle;

            //executes function every time when menu start draw
            public Action onDrawMenuStart;
            public Action onDrawMenuEnd;

            public Action onUserChoose;

            public Events()
            {
                onStart = () => { };
                onStartCycle = () => { };
                onEnd = () => { };
                onEndCycle = () => { };
                onDrawMenuStart = () => { };
                onDrawMenuEnd = () => { };
                onUserChoose = () => { };
            }
        }

    }

    /**
     * Menu class for creating amazing menu
     */
    public class Menu
    {

        //Defining variables

        /// <summary>
        /// Property <c>Elements</c> represents a list of menu elements
        /// </summary>
        public List<IElement> Elements { get; private set; }

        /// <summary>
        ///Variable <c>menuSettings</c> represents the settings of current menu instance
        /// </summary>
        public Utils.Settings menuSettings;

        /// <summary>
        /// Variable <c>menuEvents</c> represents the events of current menu instance
        /// </summary>
        public Utils.Events menuEvents;

        /// <summary>
        /// Static Variable <c>globalMenuSettings</c> represents the settings of all menu instances
        /// </summary>
        public static Utils.GlobalSettings globalMenuSettings = new Utils.GlobalSettings();

        /// <summary>
        /// This constructor initializes menu elements, settings and eventse
        /// </summary>
        public Menu() : this(new List<IElement>())
        { }

        /// <summary>
        /// Create menu with <paramref name="elements"/>
        /// </summary>
        /// <param name="elements"><c>elements</c> is the list of menu elements</param>
        public Menu(List<IElement> elements)
        {
            this.Elements = elements;
            menuSettings = new Utils.Settings();
            menuEvents = new Utils.Events();
        }

        /// <summary>
        /// Clears all menu items
        /// </summary>
        public void ClearElements()
        {
            Elements.Clear();
        }


        /// <summary>
        /// Adds new menu element to the list
        /// </summary>
        /// <param name="content"><c>content</c> is the text of menu button</param>
        /// <param name="function"><c>function</c> is the callback of button</param>
        public void Add(string content, Action function)
        {
            Elements.Add(new Element(Function: function, Content: content));
        }

        /// <summary>
        /// Adds new menu element to the list
        /// </summary>
        /// <param name="content"><c>content</c> is the description of menu button</param>
        /// <param name="heading"><c>heading</c> is the heading text of menu button</param>
        /// <param name="function"><c>function</c> is the callback of button</param>
        public void Add(string content, string heading, Action function)
        {
            Elements.Add(new Element(Function: function, Content: content, Heading: heading));
        }

        /// <summary>
        /// Draws all elements to the screen
        /// </summary>
        /// <param name="choosedMenuItem"><c>choosedMenuItem</c> represents the current selected element index</param>
        private void Draw(int choosedMenuItem)
        {
            //Add header to menu
            if (menuSettings.isHeaderVisible)
                Console.WriteLine("\t\t" + menuSettings.headerText);

            //Iterate through each element in menu
            for (int i = 0; i < Elements.Count; i++)
            {
                //Check if element is valid
                if (Elements[i].isValid())
                {
                    //Check if element is choosen
                    if (i == choosedMenuItem)
                    {
                        //Set console colors to choosen colors
                        Console.BackgroundColor = globalMenuSettings.backColorSelected;
                        Console.ForegroundColor = globalMenuSettings.foreColorSelected;
                    }


                    Console.WriteLine(Elements[i].GetView());

                    if (i == choosedMenuItem)
                    {
                        //Set console colors to default colors
                        Console.BackgroundColor = globalMenuSettings.backColor;
                        Console.ForegroundColor = globalMenuSettings.foreColor;
                    }
                }
            }
        }

        public void BEEEE() { Console.Beep(); }

        private bool cycle = true;
        private void NewMenuStart()
        {
            cycle = true;
            int choosedMenuItem = 0;
            menuEvents.onStart();
            do
            {
                //start menu cycle
                menuEvents.onStartCycle();

                //Clear menu
                if (menuSettings.clearOnStartMenu)
                    Console.Clear();

                menuEvents.onDrawMenuStart();
                // Draw menu
                Draw(choosedMenuItem);
                menuEvents.onDrawMenuEnd();

                // Get input from user
                try
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.UpArrow:
                            choosedMenuItem = choosedMenuItem == 0 ? choosedMenuItem : choosedMenuItem - 1;
                            break;
                        case ConsoleKey.DownArrow:
                            choosedMenuItem = choosedMenuItem == (Elements.Count - 1) ? choosedMenuItem : choosedMenuItem + 1;
                            break;
                        case ConsoleKey.Enter:
                            menuEvents.onUserChoose();
                            Elements[choosedMenuItem].Execute();
                            if (menuSettings.waitForReadKey)
                                Console.ReadKey(true);

                            break;
                        case ConsoleKey.Escape:
                            menuEvents.onEnd();
                            return;
                    }
                }
                catch (FormatException)
                {
                    if (menuSettings.showWrongSymbolException)
                    {
                        Console.WriteLine("You entered wrong symbol");
                        System.Threading.Thread.Sleep(800);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("something went wrong\n" + ex.Message);
                    System.Threading.Thread.Sleep(800);
                }

                menuEvents.onEndCycle();

            } while (cycle);

            menuEvents.onEnd();
        }

        public void Close()
        {
            cycle = false;
        }

        public void CloseNow()
        {
            cycle = false;
            this.menuSettings.waitForReadKey = false;
        }

        /// <summary>
        /// Starts menu
        /// </summary>
        public void Start()
        {
            if (Elements.Count > 0)
            {
                NewMenuStart();
            }
        }
    }
}
