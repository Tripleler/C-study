   M S B A M L   `   `   `  ����C  ?Practice, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:PracticePractice  5   	       Practice.MainWindow   5   	   a ]DevExpress.Xpf.Core.v21.2, Version=21.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Practice     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)    � 	Resources 5	       �� 
TargetType �� Property �� Value �� 
StartPoint �� EndPoint � Color � Offset	 �� 
TargetType
 ��Name �� CornerRadius �� RelativeSource �� Triggers P� Property P� Value �� 
TargetName��%�  '��       '�� 2    �� 5
   
   " ��6   .�5      ��  � ��$ 14(�6)   �� 5       � ��$
 Bold"�6+   �� 5       � �� 5      �� 5      $ 0,0.5&�6*   $ 1,0.5&�6=   .��5      � $ Green��6'   $	 0.0Q�65   � 5      $ White��6'   $	 0.9Q�65   5      5      5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      "	 ��6&   .R�5      �� $
 Border��6!   $ 20��5   !   $��1q�5   !   
���   ���5      �� 5   "   $ 0,0.5&�66   $ 1,0.5&�6I   .��5    &   � $	 0.0Q�5"   3    5    3   �� *Background.Color+ �� *TemplatedParent+5"   3   � 5#   &   $	 Red��63   $	 0.9Q�6?   5$   #   ._�5&      �� $��2q�6/   $��Center=�5'   /   $��Center=�5(   /   ��. 5)   /   5*       5+      P� 5,       � �� . . �5-   "   �� $ Border��6)    � �� 5.   &   �� 5/   *   $ 0,0.5&�6>   $ 1,0.5&�6Q   .��50   .   � $	 0.0Q�52   ;    50   ;   �� *Background.Color+ �� *TemplatedParent+52   ;   � 53   .   $ DarkSlateGray��6;   $	 0.9Q�6Q   54   +   56   #   57      5E      5G      5H      5I      .��5J      �  � RowDefinitions 5K   
   �� 5L      $��30 �6   �� 5M      $��30 �6   �� 5N      $��30 �6   �� 5O       � ColumnDefinitions 5Q   
   �� 5R      �� 5S      .+�5V   
   �� .��6   Enter your first name:5X      �� 5Y   
   -   5        $��	firstName��5Z      $��0��5Y      $��1��6   $��0,5,10,5q�5Z      �� 5\   
   $��1��6   .��6   Enter your last name:5^      �� 5_   
   -   5        $��lastName��5`      $��1��5_      $��1��6   $��0,5,10,5q�5`      �� 5b   
   -   5        $��submit��5c      $��2��5b      $��0��6   $��2q�5c      $��14(�5d      $��Light"�6   
����  �$��Submit��5f   &   5n      �� 5p   
   -   5        $��Clear��5q      $��2��5p      $��1��6   $��2q�5q      .��67   
Clear Name5s      5t      5u      