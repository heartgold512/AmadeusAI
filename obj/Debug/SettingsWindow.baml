   M S B A M L   `   `   `  ����D  @AmadeusAI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:AmadeusAI	AmadeusAI  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	       AmadeusAI.SettingsWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:AmadeusAI     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	   -   5          � Title$  AmadeusAI settings��5	   	   $��375.581��6$   $��478.385��65    � WindowStartupLocation$ CenterScreen=�6E    � 
ResizeMode$ NoResize=�6j   .��5
      � .+�5   
   �� $
��Left=�6   $
��Wrap=�6/   $��What should I call you?��6C   $	��Top=�6b   $��	10,12,0,0q�6z   �� 5   
   -   5         ��Name$ textBoxName��5   p   $��22��6   $��	184,9,0,0q�6    �� TextWrapping$
 Wrap=�61   $��DefaultName��6E   $	��Top=�6X   $
��Left=�6�   $	��274��6�   �� 5   
   -   5         ��Name$ checkBoxPotatoPC��5      $��Potato PC mode��6.   $
��Left=�6G   $	��Top=�6b   $��
10,194,0,0q�6z    � ToolTip$3 -I don't blame you, as long as you're with me~��6�   �� 5   
   $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��	10,65,0,0q�6[   �� 6o   $"�Which screen should I be on?��6s   6�   �� 5   
   -   5         ��Name$ comboBoxScreen��5      $��
184,63,0,0q�6+   $��22��6?   $	��Top=�6K   $
��Left=�6c   $	��274��6~   6�   �� 5   
   -   5         ��Name$ buttonOk��5      $	��Ok!��6#   $
��Left=�61   $	��Top=�6L   $��75��6d   $��383,298,0,0q�6o   	 �� 	IsDefault	 . 6�   �� 5   
   -   5        $ checkBoxAutoUpdate��5      $$��Automatically update AmadeusAI��60   $
��Left=�6Y   $	��Top=�6t   $��
10,174,0,0q�6�   $( "This will connect to the internet!��6�   �� 5   
   $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��
10,220,0,0q�6[   �� 6p   $"�Open Settings (this window):��6t   6�   �� 5   
   $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��
10,240,0,0q�6[   �� 6p   $�Hide/Re-Show AmadeusAI:��6t   6�   �� 5   
   $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��
10,262,0,0q�6[   �� 6p   $�Exit AmadeusAI:��6t   6�   �� 5   
   -   5        
 ��Name$
 txtSettings��5      $
��Left=�6)   $
��Wrap=�6D   $��CTRL-SHIFT-F10��6X   $	��Top=�6n   $��265,220,0,0q�6�   �� 5   
   -   5        $
 txtHide��5      $
��Left=�6%   $
��Wrap=�6@   $��CTRL-SHIFT-F11��6T   $	��Top=�6j   $��265,240,0,0q�6�   �� 5   
   -	   5        $
 txtExit��5      $
��Left=�6%   $
��Wrap=�6@   $��CTRL-SHIFT-F12��6T   $	��Top=�6j   $��265,262,0,0q�6�   �� 5   
   -
   5        $	��Set��5      $
��Left=�6   $	��Top=�6:   $��68��6R   $��184,218,0,0q�6]   6r   �� 5   
   -   5        $	��Set��5      $
��Left=�6   $	��Top=�6:   $��68��6R   $��184,238,0,0q�6]   6r   �� 5   
   -   5        $	��Set��5      $
��Left=�6   $	��Top=�6:   $��68��6R   $��184,260,0,0q�6]   6r   	� 5   
   -   5         	�Name$ 
radioRight��5      $��Right��6*   $
��Left=�6:   $	��Top=�6U   $��
411,92,0,0q�6m   6�   	� 5   
   -   5        $ 	radioLeft��5      $
��Left��6)   $
��Left=�68   $	��Top=�6S   $��
299,92,0,0q�6k   $��42��6�   �� 5   
   -   5        $��Reset��5      $
��Left=�6!   $	��Top=�6<   $��75��6T   $��303,298,0,0q�6_   6t   �� 5   
   $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��
10,114,0,0q�6[   �� 6p   $*�$Make me bigger/smaller, if you want:��6t   6�   �� 5   
   -   5         ��Name$ sliderScale��5      $
��Left=�6&   $	��Top=�6A   $��234,114,0,0q�6Y   $	��224��6n    � Maximum$
 2.75Q�6z    � Value$ 1Q�6�    � Minimum$
 0.25Q�6�    � SmallChange$
 0.05Q�6�    � LargeChange$
 0.25Q�6�   6�   �� 5    
   -   5        $ buttonAutostart��5       $��Start with Windows!��6*   $
��Left=�6H   $	��Top=�6c   $	��214��6{   $��
10,298,0,0q�6�   6�   �� 5!   
   $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��
10,144,0,0q�6[   �� 5"      $&� Wait time between idle dialogue:��6   5#      �� 5$   
   $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��234,176,0,0q�6[   �� 6q   $�Night color mode:��6u   6�   �� 5%   
   -   5        $ comboBoxIdle��5%      $��234,142,0,0q�6)   $��22��6>   $	��Top=�6J   $
��Left=�6b   $	��224��6}   .@�5&      ��Very Long (240-600s)��62   ��5'      Long (180-480s)��6-   ��5(      Regular (120-300s)��60   ��5)      Short (60-180s)��6-   ��5*      Very Short (30-120s)��62   ��5+      Off��6!   5,      	� 5-   
   -   5        $ radioManual��5-      $��Manual��6+   $
��Left=�6<   $	��Top=�6W   $��
346,92,0,0q�6o   $��60��6�   �� 5.   
   -   5        $ comboBoxNightMode��5.      $
��Left=�6.   $	��Top=�6I   $	��112��6a   $��346,174,0,0q�6m   .@�5/      ��Auto��6"   ��50      Day��6!   ��51      	Night��6#   52      �� 53   
   -   5        $
 textBlockVersion��53      $
��Left=�6.   $
��Wrap=�6I   $	��Top=�6]   $��10,323,0,-1q�6u   $��Invalid Version!��6�   6�   �� 54   
   -   5        $ comboBoxLanguage��54      $��
184,36,0,0q�6-   $��22��6A   $	��Top=�6M   $
��Left=�6e   $	��274��6�   .@�55      ��English��6%   ��56      Russian��6%   ��57      
French��6$   ��58      Japanese��6&   59      �� 5:   
   $
��Left=�6   $
��Wrap=�6/   $"��Which language should I use?��6C   $	��Top=�6g   $��	10,39,0,0q�6   5;      5<      