   M S B A M L   `   `   `  ����F  BChattingApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:ChattingAppChattingApp  5          ChattingApp.ucReceiveFile   5      a ]DevExpress.Xpf.Core.v21.2, Version=21.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      +localclr-namespace:ChattingApp     5      .��5	      � $
��Left=�6   $	��Top=�6&   $��0, 0, 50, 0q�6>     � RowDefinitions  5
   
   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6    � ColumnDefinitions 5   
   �� 5      �� 5      $
��auto �6   .+�5   
   �� -   5         ��Name$ txt_user��5      $��0��6&   $��2��63   $
��Left=�6G   $��Bottom=�6b   $��20, 10, 10, 0q�6}   $��14(�6�   $y�Visible=�6�   �� 5   
   -   5        $ txt_time��5      $��1��6&   $��2��63   $��Right=�6C   $��Bottom=�6_   $��10, 10, 10, 12q�6z   �� 5   
   -   5         ��Name$ border��5      $��1��6!   $��0��6.    �� CornerRadius$ 10��6>   
�������$
��Left=�6g   $	��Top=�6�   $��10, 10, 0, 10q�6�   ._�5      �  5      �� 5      �� 5      .+�5      &   DevExpress.Xpf.Core.SimpleButton  -   5          Name$ btn_download��5   "   $��0��68      CornerRadius$ 10��6H   
������ $��Center=�6s   $��Center=�6�   $��10, 10, 5, 10q�5   "   $��1q�69   .��5      �� $��./icon/download.png��6   $��15��69   $��30��6D   5      �� 5      -   5        $ txt_msg��5   ,   $��1��6   $
��Wrap=�6=   $��10q�6Q   $��14(�6]   $��Center=�6k   5       5!      � 5"   
   -   5         �Name$ pb_file��5"      $��2��6'   $��0��64    � Maximum$	 100Q�6D   	 � Minimum$	 0Q�6R   $��	10,0,0,10q�6^   $��20��6q   
������$y�	Collapsed=�6�   5#      5$      