using System;
using System.Collections.Generic;

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
        public Element(string Content,Action Function)
        {
            this.Content = Content;
            this.Function = Function;
        }
        public Element(string Content,Action Function,string Heading)
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
            public Action onDrawMenuStart;//
            public Action onDrawMenuEnd;//

            public Action onUserChoose;

            public Events()
            {
                onStart = () => { };
                onStartCycle = () => { };
            }
        }

        //Defining variables
        public Settings menuSettings;
        public Events menuEvents;
        private List<Element> elements;

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
            Element temp = new Element(Function:function,Content:content);
            elements.Add(temp);
        }
        public void Add(string content, string heading, Action function)
        {
            Element temp = new Element(Function: function, Content: content,Heading:heading);
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

        public void Start()
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

    }
}
