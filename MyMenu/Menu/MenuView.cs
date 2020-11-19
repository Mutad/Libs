using System;

namespace MyMenu
{
    /// <summary>
    /// <c>MenuView</c> is the class that simplifies menu creation
    /// </summary>
    public class MenuView : IView
    {
        protected Menu menu;

        public MenuView()
        {
            menu = new Menu();
        }

        public void Close()
        {
            menu.Close();
        }

        public void Open()
        {
            menu.Open();
        }
    }
}
