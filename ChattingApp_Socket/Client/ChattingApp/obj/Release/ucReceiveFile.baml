   M S B A M L   `   `   `  ���� F  BChattingApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:ChattingAppChattingApp      ChattingApp.ucReceiveFile   a ]DevExpress.Xpf.Core.v21.2, Version=21.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     +localclr-namespace:ChattingApp     .��� $
��Left=�$	��Top=�$��0, 0, 50, 0q�  � RowDefinitions  �� $
��auto ��� �� $
��auto � � ColumnDefinitions �� �� $
��auto �.+��� -    ��Name$ txt_user��$��0��$��2��$
��Left=�$��Bottom=�$��20, 10, 10, 0q�$��14(�$y�Visible=��� -   $ txt_time��$��1��$��2��$��Right=�$��Bottom=�$��10, 10, 10, 12q��� -    ��Name$ border��$��1��$��0�� �� CornerRadius$ 10��
�������$
��Left=�$	��Top=�$��10, 10, 0, 10q�._��  �� �� .+�&   DevExpress.Xpf.Core.SimpleButton  -     Name$ btn_download��$��0��   CornerRadius$ 10��
������ $��Center=�$��Center=�$��10, 10, 5, 10q�$��1q�.���� $��./icon/download.png��$��15��$��30���� -   $ txt_msg��$��1��$
��Wrap=�$��10q�$��14(�$��Center=�� -    �Name$ pb_file��$��2��$��0�� � Maximum$	 100Q�	 � Minimum$	 0Q�$��	10,0,0,10q�$��20��
������$y�	Collapsed=�