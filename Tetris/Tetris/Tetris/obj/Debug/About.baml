   M S B A M L   `   `   `  ����A  =Tetris, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:TetrisTetris  5   	       Tetris.Window1   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Tetris     5   	   -   5          �Name$  About��5   	    � Title$ About��5   	   $	��350��6   $	��350��6$    � 
ResizeMode$ NoResize=�60    � Icon$ Resources/video-game.png��6F   .��5	      �  � RowDefinitions 5
   
   �� 5      $	��15* �6   6+   �� 5      $	��53* �6   6+    � ColumnDefinitions 5   
   �� 5      6!   �� 5      6!   .+�5   
   �� $��
5, 5, 5, 5q�6   $��Center=�6$   $��Center=�6A   $��0��6\   $��0��6i   $��Resources/video-game.png��6y   6�   �� 5   
   $��Center=�6   $��Center=�61   $��0��6L   $��1��6Y   $��36(�6i   $��44��6w   $��12.667,6,12.667,10q�6�   $	��122��6�   TETRIS6�   �� 5   
   -   5         ��Name$ 	highScore��5      $��Center=�6'   $��Bottom=�6D   $��0��6_   $��1��6l   $��10(�6|   6�   �� 5   
   2  ,System.Windows.Markup.XmlAttributeProperties  XmlSpace preserve6�   $��5,0,0,0q�6   $
��Left=�6%   $	��Top=�6@   $��1��6X   $��2��6e   $��Calibri)�6y   ��Tetris
Version 1.0
Authors: Courtney Grover, Bragdon Bester
            
Credits: 
    Tetris: Alexey Pajitnov
    Icon: Roundicons
    Sounds and Background: The Legend of Zelda: Wind Waker 
            
Details:
This is our version of the popular game tetris.
To play the game you must rotate and shift blocks left
and right as they fall at increasing speeds. The goal is to
clear as many horizontal lines as possible by completing 
horizontal rows with no empty spaces. If there is no more
room for new pieces the game is over!!!
        5&      5'      5(      