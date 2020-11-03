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
            modelo = new ModeloTekla();
            if (!modelo.modeloAberto)
            {
                MessageBox.Show("Nenhuma instância do Tekla encontrada. \nAbra um modelo no Tekla para poder iniciar.");
                Environment.Exit(0);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           

        }
        public ModeloTekla modelo { get; set; }
        private void selecionar_etapa(object sender, RoutedEventArgs e)
        {
            
            var etapas = modelo.GetEtapas();
            var sel = Conexoes.Utilz.SelecionarObjeto(etapas,null,"Selecione");
            var pcs = modelo.GetMarcas(sel);
            modelo.ResetVisuais();
            //modelo.SetVisivel(modelo.GetMarcas(), false);
            //modelo.SetVisivel(pcs, false);
            //modelo.SetCor(pcs,TeklaMedabilAPIs.Funcoes.GetCor());

            this.lista.ItemsSource = null;
            this.lista.ItemsSource = pcs;
          
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (modelo.modeloAberto)
            {
                modelo.ResetVisuais();
            }
                Environment.Exit(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch pp = new Stopwatch();
            pp.Start();
           
            var pos = modelo.GetPosicoes();
            pp.Stop();
            MessageBox.Show($" {pos.Count} Parts encontrados.\n"  + pp.Elapsed.ToString());

            pp.Reset();
            pp.Start();
           var ss = modelo.GetMarcas();
            pp.Stop();
            MessageBox.Show($"{ss.Count} Assemblies encontrados.\n" + pp.Elapsed.ToString());

        }
    }
}
