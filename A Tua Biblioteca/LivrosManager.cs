using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using System.IO;
using System.Collections.Generic;
using Avalonia;
using System;
using System.Linq;

namespace A_Tua_Biblioteca
{
    public class LivroManager
    {
        private TextBox _titulo;
        private TextBox _autor;
        private TextBox _idadeMinima;
        private ContentControl _mainContent;
        private EmprestimoManager _emprestimoManager;

        public LivroManager(ContentControl mainContent, EmprestimoManager emprestimoManager)
        {
            _mainContent = mainContent;
            _emprestimoManager = emprestimoManager;
        }

        public class Livro
        {
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public int IdadeMinima { get; set; }
        }

        public void CriarLivroUI()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Criar Livro",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock { Text = "Título:", Margin = new Thickness(0, 0, 0, 5) });
            _titulo = new TextBox
            {
                Watermark = "Título do livro",
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_titulo);

            panel.Children.Add(new TextBlock { Text = "Autor:", Margin = new Thickness(0, 0, 0, 5) });
            _autor = new TextBox
            {
                Watermark = "Nome do autor",
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_autor);

            panel.Children.Add(new TextBlock { Text = "Idade Mínima:", Margin = new Thickness(0, 0, 0, 5) });
            _idadeMinima = new TextBox
            {
                Watermark = "Ex: 12",
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_idadeMinima);

            var submeter = new Button
            {
                Content = "Submeter",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            submeter.Click += GuardarLivro;
            panel.Children.Add(submeter);

            _mainContent.Content = panel;
        }

        private void GuardarLivro(object sender, RoutedEventArgs e)
        {
            if (ValidarCamposLivro())
            {
                var livro = new Livro
                {
                    Titulo = _titulo.Text,
                    Autor = _autor.Text,
                    IdadeMinima = int.Parse(_idadeMinima.Text)
                };

                SalvarLivroEmArquivo(livro);
                MostrarMensagemSucesso();
            }
            else
            {
                MostrarMensagemErro();
            }
        }

        private bool ValidarCamposLivro()
        {
            return !string.IsNullOrWhiteSpace(_titulo.Text) &&
                   !string.IsNullOrWhiteSpace(_autor.Text) &&
                   int.TryParse(_idadeMinima.Text, out _);
        }

        private void SalvarLivroEmArquivo(Livro livro)
        {
            string path = GetLivroFilePath();
            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine(livro.Titulo);
                writer.WriteLine(livro.Autor);
                writer.WriteLine(livro.IdadeMinima);
                writer.WriteLine("-----------------");
            }
        }

        public void ListarLivrosUI(bool apenasLivres = false)
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = apenasLivres ? "Lista de Livros Livres" : "Lista de Todos os Livros",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            try
            {
                List<Livro> livros = apenasLivres ? ObterLivrosLivres() : CarregarLivros();

                if (livros.Count > 0)
                {
                    var livrosPanel = new StackPanel();
                    foreach (var livro in livros)
                    {
                        livrosPanel.Children.Add(CriarPainelLivro(livro));
                    }
                    panel.Children.Add(livrosPanel);
                }
                else
                {
                    panel.Children.Add(new TextBlock
                    {
                        Text = apenasLivres ?
                            "Nenhum livro disponível no momento." :
                            "Nenhum livro cadastrado no sistema.",
                        FontSize = 16,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                }
            }
            catch (Exception ex)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = $"Erro ao carregar livros: {ex.Message}",
                    Foreground = Brushes.Red,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }

            _mainContent.Content = panel;
        }

        public List<Livro> ObterLivrosLivres()
        {
            // Verifica se o _emprestimoManager foi inicializado
            if (_emprestimoManager == null)
            {
                throw new InvalidOperationException("EmprestimoManager não foi inicializado");
            }

            var todosLivros = CarregarLivros();
            if (todosLivros.Count == 0) return todosLivros;

            var livrosEmprestados = _emprestimoManager.CarregarEmprestimos()
                .Where(e => !e.Devolvido)
                .Select(e => e.Livro)
                .ToList();

            return todosLivros
                .Where(l => !livrosEmprestados.Contains(l.Titulo))
                .ToList();
        }

        public List<Livro> CarregarLivros()
        {
            try
            {
                string filePath = GetLivroFilePath();

                if (!File.Exists(filePath))
                    return new List<Livro>();

                string[] allLines = File.ReadAllLines(filePath);
                var livros = new List<Livro>();

                for (int i = 0; i < allLines.Length; i += 4)
                {
                    if (!string.IsNullOrWhiteSpace(allLines[i]) && allLines[i] != "-----------------")
                    {
                        livros.Add(new Livro
                        {
                            Titulo = allLines[i].Trim(),
                            Autor = allLines[i + 1].Trim(),
                            IdadeMinima = int.Parse(allLines[i + 2].Trim())
                        });
                    }
                }
                return livros;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar livros: {ex.Message}");
                return new List<Livro>();
            }
        }

        private StackPanel CriarPainelLivro(Livro livro)
        {
            var livroPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            livroPanel.Children.Add(new TextBlock
            {
                Text = $"Título: {livro.Titulo}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            livroPanel.Children.Add(new TextBlock
            {
                Text = $"Autor: {livro.Autor}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            livroPanel.Children.Add(new TextBlock
            {
                Text = $"Idade Mínima: {livro.IdadeMinima}+",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var editarBtn = new Button
            {
                Content = "Editar",
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };

            editarBtn.Click += (s, e) => EditarLivroUI(livro);
            livroPanel.Children.Add(editarBtn);

            return livroPanel;
        }

        public void EditarLivroUI(Livro livro)
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Editar Livro",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock { Text = "Título:", Margin = new Thickness(0, 0, 0, 5) });
            _titulo = new TextBox
            {
                Text = livro.Titulo,
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_titulo);

            panel.Children.Add(new TextBlock { Text = "Autor:", Margin = new Thickness(0, 0, 0, 5) });
            _autor = new TextBox
            {
                Text = livro.Autor,
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_autor);

            panel.Children.Add(new TextBlock { Text = "Idade Mínima:", Margin = new Thickness(0, 0, 0, 5) });
            _idadeMinima = new TextBox
            {
                Text = livro.IdadeMinima.ToString(),
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_idadeMinima);

            var salvarBtn = new Button
            {
                Content = "Salvar Alterações",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };

            salvarBtn.Click += (sender, e) =>
            {
                if (ValidarCamposLivro())
                {
                    var livroEditado = new Livro
                    {
                        Titulo = _titulo.Text,
                        Autor = _autor.Text,
                        IdadeMinima = int.Parse(_idadeMinima.Text)
                    };

                    if (AtualizarLivro(livro, livroEditado))
                    {
                        MostrarMensagemSucessoEdicao();
                    }
                    else
                    {
                        MostrarMensagemErroEdicao();
                    }
                }
                else
                {
                    MostrarMensagemErro();
                }
            };

            panel.Children.Add(salvarBtn);
            _mainContent.Content = panel;
        }

        private bool AtualizarLivro(Livro livroAntigo, Livro livroNovo)
        {
            try
            {
                string filePath = GetLivroFilePath();
                var linhas = new List<string>(File.ReadAllLines(filePath));

                for (int i = 0; i < linhas.Count; i += 4)
                {
                    if (i + 2 < linhas.Count &&
                        linhas[i].Trim() == livroAntigo.Titulo &&
                        linhas[i + 1].Trim() == livroAntigo.Autor &&
                        linhas[i + 2].Trim() == livroAntigo.IdadeMinima.ToString())
                    {
                        linhas[i] = livroNovo.Titulo;
                        linhas[i + 1] = livroNovo.Autor;
                        linhas[i + 2] = livroNovo.IdadeMinima.ToString();
                        break;
                    }
                }

                File.WriteAllLines(filePath, linhas);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void MostrarMensagemSucesso()
        {
            var sucesso = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            sucesso.Children.Add(new TextBlock
            {
                Text = "Livro Criado com Sucesso!",
                FontSize = 32,
                Foreground = Brushes.Green,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            });

            var voltar = new Button
            {
                Content = "Voltar ao Formulário",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            voltar.Click += (s, e) => CriarLivroUI();
            sucesso.Children.Add(voltar);

            _mainContent.Content = sucesso;
        }

        private void MostrarMensagemErro()
        {
            var erro = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            erro.Children.Add(new TextBlock
            {
                Text = "Por favor preencha todos os campos corretamente!",
                FontSize = 32,
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            });

            var voltar = new Button
            {
                Content = "Voltar",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            voltar.Click += (s, e) => CriarLivroUI();
            erro.Children.Add(voltar);

            _mainContent.Content = erro;
        }

        private void MostrarMensagemSucessoEdicao()
        {
            var sucesso = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            sucesso.Children.Add(new TextBlock
            {
                Text = "Livro Atualizado com Sucesso!",
                FontSize = 32,
                Foreground = Brushes.Green,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            });

            var voltar = new Button
            {
                Content = "Voltar à Lista",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            voltar.Click += (s, e) => ListarLivrosUI();
            sucesso.Children.Add(voltar);

            _mainContent.Content = sucesso;
        }

        private void MostrarMensagemErroEdicao()
        {
            var erro = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            erro.Children.Add(new TextBlock
            {
                Text = "Erro ao atualizar livro!",
                FontSize = 32,
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            });

            var voltar = new Button
            {
                Content = "Voltar",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            voltar.Click += (s, e) => ListarLivrosUI();
            erro.Children.Add(voltar);

            _mainContent.Content = erro;
        }

        private string GetLivroFilePath()
        {
            string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(baseDir, "A_Tua_Biblioteca");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "livros.txt");
        }

        public void ListarLivrosLivresUI()
        {
            ListarLivrosUI(apenasLivres: true);
        }
        public void SetEmprestimoManager(EmprestimoManager emprestimoManager)
        {
            _emprestimoManager = emprestimoManager;
        }
    }
}