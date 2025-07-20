using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using System.IO;
using System.Text;
using System;
using Avalonia;


namespace A_Tua_Biblioteca
{
    public partial class MainWindow : Window
    {
        

        private UsuarioManager _usuarioManager;
        private LivroManager _livroManager;
        private EmprestimoManager _emprestimoManager;

        public MainWindow()
        {
            InitializeComponent();
            _livroManager = new LivroManager(MainContent, null);

            // Depois o UsuarioManager
            _usuarioManager = new UsuarioManager(MainContent);

            // Agora cria o EmprestimoManager com as dependências
            _emprestimoManager = new EmprestimoManager(MainContent, _livroManager, _usuarioManager);

            // Finalmente atualiza o LivroManager com a referência ao EmprestimoManager
            _livroManager.SetEmprestimoManager(_emprestimoManager);

        }

        public void CriarUsuariosClick(object sender, RoutedEventArgs e)
        {
            _usuarioManager.CriarUsuarioUI();
        }

        public void ListarUsuariosClick(object sender, RoutedEventArgs e)
        {
            _usuarioManager.ListarUsuariosUI();
        }
        public void ListarLivroClick(object sender, RoutedEventArgs e) 
        {
            _livroManager.ListarLivrosUI();
        
        }
        public void CriarLivroClick(object sender, RoutedEventArgs e)
        {
            _livroManager.CriarLivroUI();

        }
        public void ListarEmprestimosClick(object sender, RoutedEventArgs e)
        {
            _emprestimoManager.ListarEmprestimosUI();
        }

        public void CriarEmprestimoClick(object sender, RoutedEventArgs e)
        {
            _emprestimoManager.CriarEmprestimoUI();
        }

        public void AtrasosClick(object sender, RoutedEventArgs e)
        {
            _emprestimoManager.ListarAtrasosUI();
        }

        public void ListarLivrosLivresClick(object sender, RoutedEventArgs e)
        {
            _livroManager.ListarLivrosLivresUI();
        }





    }
        
        


}