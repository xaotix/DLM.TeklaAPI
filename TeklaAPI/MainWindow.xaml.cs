using MedailTeklaAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tekla.Structures.Model;
using Tekla.Structures.ModelInternal;

namespace TeklaMedabilAPIs
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!Consultas.modelo.modeloAberto)
            {
                MessageBox.Show("Nenhuma instância do Tekla encontrada. \nAbra um modelo no Tekla para poder iniciar.");
                Environment.Exit(0);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           

        }
        private void selecionar_etapa(object sender, RoutedEventArgs e)
        {
            
            var pcs = Consultas.SelecionarEtapa();


            this.lista.ItemsSource = null;
            this.lista.ItemsSource = pcs;
          
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Consultas.modelo.modeloAberto)
            {
                Consultas.modelo.ResetVisuais();
            }
                Environment.Exit(0);
        }



        private void importar_st_pecas(object sender, RoutedEventArgs e)
        {
            Consultas.ImportarSAP();
           
        }
    }
}
