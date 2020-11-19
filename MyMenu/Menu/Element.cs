using System;

//
//  OwO
//
//TODO add color and backgound modification to every element
//TODO make element events

namespace MyMenu
{
    public class Element : IElement
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

        /**
         * Create element with Content text and handler function
         */
        public Element(string Content, Action Function)
        {
            this.Content = Content;
            this.Function = Function;
        }
        /**
         * Create element with Heading and Content text, handler function
         */
        public Element(string Content, Action Function, string Heading)
        {
            this.Content = Content;
            this.Function = Function;
            this.Heading = Heading;
        }

        /**
         * Check if element is valid to display
         */
        public bool isValid()
        {
            //If element's content, function or is not set
            if (string.IsNullOrWhiteSpace(Content) || Function == null)
                return false;
            else
                return true;
        }

        public string GetView()
        {
            if (string.IsNullOrEmpty(Heading))
                return Content;
            else
                return $"{Heading}\n\t{Content}";
        }

        /**
         * Run the handler of element
         */
        public void Execute()
        {
            Function();
        }
    }
}
