   M S B A M L   `   `   `  ����K  GTeklaMedabilAPIs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:TeklaMedabilAPIsTeklaMedabilAPIs  5   	   !    TeklaMedabilAPIs.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:TeklaMedabilAPIs     5   	   -   5          � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $��25 �6   6*   �� 5      $��* �6   6)   .+�5   
   o� $��Center=�6   .@�5      m� $��Ferramentas��6   .@�5      m� -   5        $"��Importar Status de Peças...��5      6\   5      m� 5      $��Testes��6   .@�5      m� -   5        $��Selecionar Etapa...��5      6O   5      5      &   System.Windows.Controls.DataGrid  5   
   -   5        $��lista��5   -      CanUserDeleteRows .  $��2��6:      
IsReadOnly .    CanUserAddRows .     AutoGenerateColumns .     SelectionMode$ Extended=�6�      HeadersVisibility$ Column=�6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Marca��6&   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 65   �� *nome+  5      $	 
Quantidade��6&   
 6:   �� *
quantidade+  5      $
	 Peso��6&   
 64   �� *peso+5       5"      5#      