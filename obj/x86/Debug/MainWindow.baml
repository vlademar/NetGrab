   M S B A M L   `   `   `  ����B  >NetGrab, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:NetGrabNetGrab  5   	       NetGrab.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   'localclr-namespace:NetGrab     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  
MainWindow��5	   	   $	��430��6   $	��351��6)    � 	Resources 5
         BoolToVisibility  NetGrab.DBG  Dbg��%:   &	        &	      �� 5   
   6'     5   
   6   5      .��5      � $��5q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��ListView��5       $��0��6   ��60   �� *
TaskHost+ �� 	Converter# [  �� Mode$ OneWay=� �� View 5      �� 5      .P�5      �� $��Id��6%    �� Width$ 60��6�    �� DisplayMemberBinding 61   �� *
LoaderId+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5      $��State��6%   $ 80��6�    64   �� *State+$ PropertyChanged=�6�   �� 5      $��Description��6%    6:   �� *Description+$ PropertyChanged=�5      5      �� 5   
   $��1��6   	 �� Orientation$	 
Horizontal=�6"   $��Right=�6;   y�5      �� *TaskHost.Running+# [ .+�5       �� -   5        $��TextBox��5       $��60��6%   $��2q�60   �� 5!      -   5        $��Right=�5!      $��GO��61   $��60��6T   $��2q�6_   5"      5#      5$      