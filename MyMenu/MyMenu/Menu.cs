using System;

//
//  Made bu mutado.ml
//
//
//  OwO
//

namespace MyMenu
{

    public class Element
    {
        //TODO add color and backgound modification
        public string Heading;
        public string Content;
        public Action Function;

        public Element()
        {
            Heading = "";
            Content = "";
            Function = null;
        }

        public bool isValid()
        {
            if (Content != "" && Function != null)
                return true;
            else
                return false;
        }
    }

    public class Menu
    {
        public struct MenuSettings
        {
            public bool clearOnStartMenu;
            public bool useReadKeyInput;
            public bool isBackVisible;
            public string backText;
            public bool isHeaderVisible;
            public string headerText;
            public bool showWrongSymbolException;
        }
        public MenuSettings menuSettings;

        private Element[] elements;
        public Menu()
        {
            elements = new Element[0];
            menuSettings.clearOnStartMenu = true;
            menuSettings.useReadKeyInput = true;
            menuSettings.isBackVisible = true;
            menuSettings.backText = "Back";
            menuSettings.isHeaderVisible = true;
            menuSettings.showWrongSymbolException = true;
            menuSettings.headerText = "Header";
        }
        public void AddElement(string content, Action function)
        {
            Array.Resize(ref elements, elements.Length + 1);
            elements[elements.Length - 1] = new Element();
            elements[elements.Length - 1].Content = content;
            elements[elements.Length - 1].Function = function;
        }

        public void AddElement(string content, string heading, Action function)
        {
            AddElement(content, function);
            elements[elements.Length - 1].Heading = heading;
        }

        public string GetCompletedView()
        {
            string view = "";
            if (menuSettings.isHeaderVisible)
                view += $"\n\t  {spaces(elements.Length.ToString().Length - 1)}{menuSettings.headerText}\n\n";
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].isValid())
                    view += getView(i) + "\n";
            }
            if (menuSettings.isBackVisible)
                view += $"0.{spaces(elements.Length.ToString().Length - 1)} {menuSettings.backText}\n";
            return view;
        }
        public string getView(int index)
        {
            if (!elements[index].isValid())
                return "";
            string view = $"{index + 1}.{spaces(elements.Length.ToString().Length - (index + 1).ToString().Length)}";
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
        private string spaces(int num)
        {
            string val = "";
            for (int i = 0; i < num; i++)
            {
                val += " ";
            }
            return val;
        }

        public void StartMenu()
        {
            int choose;
            do
            {
                if (menuSettings.clearOnStartMenu)
                    Console.Clear();

                Console.WriteLine(GetCompletedView());
                choose = 0;
                try
                {
                    if (menuSettings.useReadKeyInput)
                        choose = Convert.ToUInt16(Console.ReadKey(true).KeyChar.ToString());
                    else
                    {
                        //TODO make tryparse instead of convert
                        choose = Convert.ToInt32(Console.ReadLine());

                    }

                    if (choose != 0)
                    {
                        elements[choose - 1].Function();
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
            } while (choose != 0);
        }

    }
}
