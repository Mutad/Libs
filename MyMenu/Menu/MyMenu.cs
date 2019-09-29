using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

//
//  OwO
//
//TODO add color and backgound modification to every element

namespace MyMenu
{

    public class Element
    {

        //  Heading text
        //      Content text

        //Title text of element
        public string Heading;

        //Content text of element
        public string Content;

        //Function that executes when user select this element
        public Action Function;

        //Default constructor for Element
        public Element()
        {
            Heading = "";
            Content = "";
            Function = null;
        }
        public Element(string Content, Action Function)
        {
            this.Content = Content;
            this.Function = Function;
        }
        public Element(string Content, Action Function, string Heading)
        {
            this.Content = Content;
            this.Function = Function;
            this.Heading = Heading;
        }

        //Validate the element
        public bool isValid()
        {
            //If element's content, function or is not set
            if (Content == "" || Function == null)
                return false;
            else
                return true;
        }

        public void Execute()
        {
            Function();
        }
    }

    public class Menu
    {
        public class GlobalSettings
        {
            [JsonIgnore]
            public string filename = "menuGlobalSettings.json";
            //colors
            public ConsoleColor foreColor { get; private set; }
            public ConsoleColor backColor { get; private set; }
            public ConsoleColor foreColorChoosed { get; private set; }
            public ConsoleColor backColorChoosed { get; private set; }

            public void SetColors(ConsoleColor foreColor, ConsoleColor backColor)
            {
                this.foreColor = foreColor;
                this.backColor = backColor;
                this.foreColorChoosed = backColor;
                this.backColorChoosed = foreColor;
                Console.ForegroundColor = this.foreColor;
                Console.BackgroundColor = this.backColor;
            }

            public void SetColors(ConsoleColor foreColor, ConsoleColor backColor, ConsoleColor foreColorChoosed, ConsoleColor backColorChoosed)
            {
                SetColors(foreColor, backColor);
                this.foreColorChoosed = foreColorChoosed;
                this.backColorChoosed = backColorChoosed;
            }

            public GlobalSettings()
            {
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

                        SetColors(sets.foreColor, sets.backColor, sets.foreColorChoosed, sets.backColorChoosed);
                    }
                }
                else
                {
                    foreColor = ConsoleColor.White;
                    backColor = ConsoleColor.Black;
                    foreColorChoosed = ConsoleColor.Black;
                    backColorChoosed = ConsoleColor.White;
                }
            }

            [JsonConstructor]
            public GlobalSettings(ConsoleColor foreColor, ConsoleColor backColor, ConsoleColor foreColorChoosed, ConsoleColor backColorChoosed)

            {
                this.foreColor = foreColor;
                this.backColor = backColor;
                this.foreColorChoosed = foreColorChoosed;
                this.backColorChoosed = backColorChoosed;
            }

            ~GlobalSettings()
            {
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
        public class Settings
        {
            //clear console onStart
            public bool clearOnStartMenu;

            public bool useReadKeyInput;

            //visibility of back key
            public bool isBackVisible;

            //visibility of header
            public bool isHeaderVisible;

            //back text
            public string backText;

            //header text
            public string headerText;


            public bool showWrongSymbolException;


            public bool waitForReadKey;

            //colors
            public ConsoleColor foreColor;
            public ConsoleColor backColor;
            public ConsoleColor foreColorChoosed;
            public ConsoleColor backColorChoosed;

            public bool usingNewMenu;

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
        public class Events
        {
            //executes function when menu start
            public Action onStart;
            public Action onEnd;

            //executes function every before menu cycle 
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

        //Defining variables
        public Settings menuSettings;
        public Events menuEvents;
        public List<Element> elements { get; private set; }
        public static GlobalSettings globalMenuSettings = new GlobalSettings();

        public void ClearElements()
        {
            elements.Clear();
        }

        //Default constructor for Menu
        public Menu()
        {
            elements = new List<Element>();
            menuSettings = new Settings();
            menuEvents = new Events();
        }

        //Add new Menu element to the list
        public void Add(string content, Action function)
        {
            Element temp = new Element(Function: function, Content: content);
            elements.Add(temp);
        }
        public void Add(string content, string heading, Action function)
        {
            Element temp = new Element(Function: function, Content: content, Heading: heading);
            elements.Add(temp);
        }

        //Get view of menu
        public string GetCompletedView()
        {
            string view = "";

            //Add header to menu
            if (menuSettings.isHeaderVisible)
                view += $"\n\t  {spaces(elements.Count.ToString().Length - 1)}{menuSettings.headerText}\n\n";

            //Add view of every element to menu view
            for (int i = 0; i < elements.Count; i++)
            {
                //Check if element is valid
                if (elements[i].isValid())
                    //Add element to menu view
                    view += getView(i) + "\n";
            }

            //Add back to menu view
            if (menuSettings.isBackVisible)
                view += $"0.{spaces(elements.Count.ToString().Length - 1)} {menuSettings.backText}\n";


            return view;
        }
        public string getView(int index)
        {
            string view = $"{index + 1}.{spaces(elements.Count.ToString().Length - (index + 1).ToString().Length)}";

            if (elements[index].Heading != "")
            {
                view += $" {elements[index].Heading}\n";
                for (int i = 0; i < index.ToString().Length; i++)
                    view += " ";
                view += "  ";
            }

            view += $" {elements[index].Content}";
            return view;
        }

        private static string spaces(int num)
        {
            string val = "";
            for (int i = 0; i < num; i++)
            {
                val += " ";
            }
            return val;
        }

        private void OldMenuStart()
        {
            int choose;

            menuEvents.onStart();
            do
            {
                //start menu cycle
                menuEvents.onStartCycle();

                //Clear menu
                if (menuSettings.clearOnStartMenu)
                    Console.Clear();


                menuEvents.onDrawMenuStart();
                //Draw menu
                Console.WriteLine(GetCompletedView());
                menuEvents.onDrawMenuEnd();


                choose = 0;
                try
                {

                    //wait for user to input his choose
                    if (menuSettings.useReadKeyInput)
                        choose = Convert.ToUInt16(Console.ReadKey(true).KeyChar.ToString());
                    else
                        //TODO make tryparse instead of convert
                        choose = Convert.ToInt32(Console.ReadLine());

                    menuEvents.onUserChoose();


                    if (choose != 0)
                    {
                        elements[choose - 1].Execute();

                        if (menuSettings.waitForReadKey)
                            Console.ReadKey(true);
                    }
                }
                catch (FormatException)
                {
                    if (menuSettings.showWrongSymbolException)
                    {
                        Console.WriteLine("You entered wrong symbol");
                        choose = 1;
                        System.Threading.Thread.Sleep(800);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("something went wrong");
                }

                menuEvents.onEndCycle();

            } while (choose != 0);

            menuEvents.onEnd();
        }


        private void Draw(int choosedMenuItem)
        {
            //Add header to menu
            if (menuSettings.isHeaderVisible)
                Console.WriteLine("\t\t" + menuSettings.headerText);

            //Add view of every element to menu view
            for (int i = 0; i < elements.Count; i++)
            {
                //Check if element is valid
                if (elements[i].isValid())
                {
                    //Check if element is choosen
                    if (i == choosedMenuItem)
                    {
                        //Set console colors to choosen colors
                        Console.BackgroundColor = globalMenuSettings.backColorChoosed;
                        Console.ForegroundColor = globalMenuSettings.foreColorChoosed;
                    }


                    if (elements[i].Heading == "")
                    {
                        Console.WriteLine(elements[i].Heading);
                        Console.WriteLine("\t" + elements[i].Content);
                    }
                    else
                    {
                        Console.WriteLine(elements[i].Content);
                    }

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
                //Draw menu
                Draw(choosedMenuItem);
                menuEvents.onDrawMenuEnd();


                try
                {
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.UpArrow:
                            choosedMenuItem = choosedMenuItem == 0 ? choosedMenuItem : choosedMenuItem - 1;
                            break;
                        case ConsoleKey.DownArrow:
                            choosedMenuItem = choosedMenuItem == (elements.Count - 1) ? choosedMenuItem : choosedMenuItem + 1;
                            break;
                        case ConsoleKey.Enter:
                            menuEvents.onUserChoose();
                            elements[choosedMenuItem].Execute();
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
                    Console.WriteLine("something went wrong\n"+ex.Message);
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

        public  void CloseNow()
        {
            cycle = false;
            this.menuSettings.waitForReadKey = false;
        }
        public void Start()
        {
            if (elements.Count > 0)
            {
                if (menuSettings.usingNewMenu)
                    NewMenuStart();
                else
                    OldMenuStart();
            }
        }
    }
}
