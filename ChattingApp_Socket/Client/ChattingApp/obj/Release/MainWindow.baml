   M S B A M L   `   `   `  ���� F  BChattingApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:ChattingAppChattingApp      ChattingApp.MainWindow   a ]DevExpress.Xpf.Core.v21.2, Version=21.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  d `DevExpress.Xpf.Docking.v21.2, Version=21.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a@dxdo5http://schemas.devexpress.com/winfx/2008/xaml/docking  ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:ChattingApp     -     � Title$  ChatMate��$	��700��$	��450��.���  � RowDefinitions �� $��* ��� $
��auto �.+�� $��0�� �� $��50 ��� $��* ��� $��70 �.+�� $��0��.+��� -    ��Name$ 
btn_Logout��$
��Left=�$��7q�
������ $��0q�.���� $��./icon/exit.png�� � 	Resources  �� 
TargetType �� Property �� Value��%:   '��       �� " ��.���  �@��CornerRadius$ 10���� -    ��Name$ lbl_ID��$��Center=�$��25(�$
��Bold"�
��� � �� $��1��
�������.+��� -    ��Name$ bd_chat��$��0q�	 �� CornerRadius$	 20��
����ح�$��10q�
 J� 	AllowDrop
 . ._��� -    ��Name$ scroll��$
��Auto=�.���� -    ��Name$ 
stpnl_chat��� $��2��$��Bottom=� � ColumnDefinitions �� $��* ��� $��70 �.+��� $	��1.3q�
��� � �$��40��$	 13�� �� Padding$ 5,5,5,5q�$��10q�._��� -    ��Name$ txt_send��$��0q�$��15(�.�� ��%:   '��       �� " ��.���  �@��CornerRadius$ 10��.���� -   $ btn_send��$��10q�$��1��$��0q�
������ $��0q� � ToolTip$ Ctrl + Enter��.���� $��./icon/send.png�� ��%:   '��       �� " ��.���  �@��CornerRadius$ 10���� -	   $ 	lay_Login��$��10,10,10,10q�$y�Visible=�$��0��$��0q�$	 10��
������._��� $��Hidden=�.����  �� Orientation$ Vertical=�$��
40,35,40,5q�.+��� 
��� � �$��35(�$��ChatMate��$��Center=��� $��ID��
��� � �$
��Bold"�$��0,40,0,5q��� $	��1.3q�
��� � �$��40��$	 13��$ 5,5,5,5q�._�%  DevExpress.Xpf.Editors.TextEdit  -
     Name$ txt_ID��$��15(�%  DevExpress.Xpf.Editors.BaseEdit   CornerRadius$ 10��$��Center=�
������ �� $��PW��
��� � �$
��Bold"�$��0,20,0,5q��� $	��1.3q�
��� � �$��40��$	 13��$ 5,5,5,5q�._�,  &DevExpress.Xpf.Editors.PasswordBoxEdit  -     Name$ txt_PW��$ 10��
�������
������ $��Center=�$��15(�&   DevExpress.Xpf.Core.SimpleButton  -     Name$ 	btn_Login��$��0,40,0,0q�$��20(�$��	로그인�����#22c922$��35��   CornerRadius$ 10��  -   $ btn_Register_Login��$��0,40,0,0q�$��20(�$��회원가입�����#22c922$��35��$ 10���� -   $ lay_Register��$y�	Collapsed=�$��10,10,10,10q�$��0��$��0q�$	 10��
�������._��� $��Hidden=�.���� $ Vertical=�$��
40,35,40,5q�.+��� 
��� � �$��35(�$��ChatMate��$��Center=�$��20q��� 
��� � �$��25(�$��회원가입��$��Center=�$��20q��� $��	아이디��
��� � �$
��Bold"�$��0,40,0,5q��� $	��1.3q�
��� � �$��40��$	 13��$ 5,5,5,5q�._�  -   $ txt_ID_Register��$��0q�$��15(�$ 10��$��Center=�
������ 
������ �� $��비밀번호��
��� � �$
��Bold"�$��0,20,0,5q��� $	��1.3q�
��� � �$��40��$	 13��$ 5,5,5,5q�._�  -   $ txt_PW_Register��$ 10��
������ 
������ $��Center=�$��15(��� $��비밀번호 확인��
��� � �$
��Bold"�$��0,20,0,5q��� $	��1.3q�
��� � �$��40��$	 13��$ 5,5,5,5q�._�  -   $ txt_PW_Register_Check��$ 10��
������ 
������ $��Center=�$��15(�  -   $ btn_Register��$��0,40,0,0q�$��20(�$��가입하기�����#22c922
�������$��35��$ 10��
������   -   $ btn_Back��$��0,40,0,0q�$��20(�$��돌아가기�����#019d01
�������$��35��$ 10��
������ � -    �Name$ 
lay_Status��$��1��$y�	Collapsed=�.+��� -    ��Name$ 
txt_Status��$��10q� �� 
IsReadOnly . " �� VerticalScrollBarVisibility$ Visible=�$	��200��$	��200��$��0q�$
��Auto=�