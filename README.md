# Libs
C# libraries

## Menu
Easy-to-use menu for c#

### Simple menu
```c#
Menu menu = new Menu();

menu.Add("Element 1", () => { Console.WriteLine("Element 1 is choosen"); });

menu.Start();
```

### Events
```c#
menu.menuEvents.onStart = () =>
{
  // executed when menu is started
}
```

### Global Parameters
```c#
Menu.globalMenuSettings.SetColors(ConsoleColor.Black,ConsoleColor.White);
```

### Customization
```c#
menu.menuSettings.isHeaderVisible = true;
menu.menuSettings.isBackVisible = true;
```
