using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using System.IO;
using System.Collections.Generic;
using Avalonia;
using System;
using System.Linq;
using Avalonia.Data;

namespace A_Tua_Biblioteca
{
    public class EmprestimoManager
    {
        // Campos para os controles de UI
        private ComboBox _livroComboBox;
        private ComboBox _usuarioComboBox;
        private DatePicker _dataEmprestimo;
        private DatePicker _dataDevolucao;
        private ContentControl _mainContent;
        private LivroManager _livroManager;
        private UsuarioManager _usuarioManager;

        public EmprestimoManager(ContentControl mainContent, LivroManager livroManager, UsuarioManager usuarioManager)
        {
            _mainContent = mainContent;
            _livroManager = livroManager;
            _usuarioManager = usuarioManager;
        }

        // Modelo de dados para empréstimo
        public class Emprestimo
        {
            public string Livro { get; set; }
            public string Usuario { get; set; }
            public DateTime DataEmprestimo { get; set; }
            public DateTime DataDevolucao { get; set; }
            public bool Devolvido { get; set; }
        }

        // Método para criar a interface de empréstimo
        public void CriarEmprestimoUI()
        {
            
                var panel = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(40, 100, 40, 40)
                };

                // Título
                panel.Children.Add(new TextBlock
                {
                    Text = "Criar Empréstimo",
                    FontSize = 32,
                    Margin = new Thickness(0, 0, 0, 30),
                    HorizontalAlignment = HorizontalAlignment.Center
                });

                // Campo Livro (ComboBox)
                panel.Children.Add(new TextBlock { Text = "Livro:", Margin = new Thickness(0, 0, 0, 5) });

                // Verifica se _livroManager não é null
                if (_livroManager != null)
                {
                    _livroComboBox = new ComboBox
                    {
                        ItemsSource = _livroManager.CarregarLivros(),
                        DisplayMemberBinding = new Binding("Titulo"),
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    panel.Children.Add(_livroComboBox);
                // Campo Usuário (ComboBox)
                panel.Children.Add(new TextBlock { Text = "Usuário:", Margin = new Thickness(0, 0, 0, 5) });
                _usuarioComboBox = new ComboBox
                {
                    ItemsSource = _usuarioManager.CarregarUsuarios(),
                    DisplayMemberBinding = new Binding("Nome"),
                    Margin = new Thickness(0, 0, 0, 15)
                };
                panel.Children.Add(_usuarioComboBox);

                // Campo Data Empréstimo
                panel.Children.Add(new TextBlock { Text = "Data de Empréstimo:", Margin = new Thickness(0, 0, 0, 5) });
                _dataEmprestimo = new DatePicker
                {
                    SelectedDate = DateTimeOffset.Now,
                    Margin = new Thickness(0, 0, 0, 15)
                };
                panel.Children.Add(_dataEmprestimo);

                // Campo Data Devolução
                panel.Children.Add(new TextBlock { Text = "Data de Devolução:", Margin = new Thickness(0, 0, 0, 5) });
                _dataDevolucao = new DatePicker
                {
                    SelectedDate = DateTimeOffset.Now.AddDays(14),
                    Margin = new Thickness(0, 0, 0, 15)
                };
                panel.Children.Add(_dataDevolucao);
            }
                else
                {
                    panel.Children.Add(new TextBlock
                    {
                        Text = "Erro: LivroManager não inicializado",
                        Foreground = Brushes.Red
                    });
                }


                // Botão Submeter
                var submeter = new Button
            {
                Content = "Submeter",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            submeter.Click += GuardarEmprestimo;
            panel.Children.Add(submeter);

            _mainContent.Content = panel;
        }

        // Método para guardar um empréstimo
        private void GuardarEmprestimo(object sender, RoutedEventArgs e)
        {
            if (ValidarCamposEmprestimo())
            {
                var emprestimo = new Emprestimo
                {
                    Livro = (_livroComboBox.SelectedItem as LivroManager.Livro)?.Titulo ?? string.Empty,
                    Usuario = (_usuarioComboBox.SelectedItem as UsuarioManager.Usuario)?.Nome ?? string.Empty,
                    DataEmprestimo = _dataEmprestimo.SelectedDate?.DateTime ?? DateTime.Today,
                    DataDevolucao = _dataDevolucao.SelectedDate?.DateTime ?? DateTime.Today.AddDays(14),
                    Devolvido = false
                };

                SalvarEmprestimoEmArquivo(emprestimo);
                MostrarMensagemSucesso();
            }
            else
            {
                MostrarMensagemErro();
            }
        }

        // Validação dos campos
        private bool ValidarCamposEmprestimo()
        {
            if (_livroComboBox == null || _usuarioComboBox == null ||
        _dataEmprestimo == null || _dataDevolucao == null)
            {
                MostrarMensagemErro();
                return false;
            }

            return _livroComboBox.SelectedItem != null &&
                   _usuarioComboBox.SelectedItem != null &&
                   _dataEmprestimo.SelectedDate != null &&
                   _dataDevolucao.SelectedDate != null &&
                   _dataDevolucao.SelectedDate > _dataEmprestimo.SelectedDate;

            return _livroComboBox.SelectedItem != null &&
                   _usuarioComboBox.SelectedItem != null &&
                   _dataEmprestimo.SelectedDate != null &&
                   _dataDevolucao.SelectedDate != null &&
                   _dataDevolucao.SelectedDate > _dataEmprestimo.SelectedDate;
        }

        // Persistência em arquivo
        private void SalvarEmprestimoEmArquivo(Emprestimo emprestimo)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDir = Path.Combine(appDataPath, "A_Tua_Biblioteca");
            Directory.CreateDirectory(appDir);

            string filePath = Path.Combine(appDir, "emprestimos.txt");

            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine(emprestimo.Livro);
                writer.WriteLine(emprestimo.Usuario);
                writer.WriteLine(emprestimo.DataEmprestimo.ToString("yyyy-MM-dd"));
                writer.WriteLine(emprestimo.DataDevolucao.ToString("yyyy-MM-dd"));
                writer.WriteLine(emprestimo.Devolvido);
                writer.WriteLine("-----------------");
            }
        }

        // Método para listar empréstimos
        public void ListarEmprestimosUI()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Lista de Empréstimos",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            List<Emprestimo> emprestimos = CarregarEmprestimos();

            if (emprestimos.Count > 0)
            {
                var emprestimosPanel = new StackPanel();

                foreach (var emprestimo in emprestimos)
                {
                    emprestimosPanel.Children.Add(CriarPainelEmprestimo(emprestimo));
                }

                panel.Children.Add(emprestimosPanel);
            }
            else
            {
                panel.Children.Add(new TextBlock
                {
                    Text = "Nenhum empréstimo registrado ainda.",
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.Gray
                });
            }

            _mainContent.Content = panel;
        }

        // Carrega empréstimos do arquivo
        public List<Emprestimo> CarregarEmprestimos()
        {
            var emprestimos = new List<Emprestimo>();
            string filePath = GetEmprestimoFilePath();

            if (File.Exists(filePath))
            {
                string[] allLines = File.ReadAllLines(filePath);

                for (int i = 0; i < allLines.Length; i += 6)
                {
                    if (!string.IsNullOrWhiteSpace(allLines[i]) && allLines[i] != "-----------------")
                    {
                        emprestimos.Add(new Emprestimo
                        {
                            Livro = allLines[i].Trim(),
                            Usuario = allLines[i + 1].Trim(),
                            DataEmprestimo = DateTime.Parse(allLines[i + 2].Trim()),
                            DataDevolucao = DateTime.Parse(allLines[i + 3].Trim()),
                            Devolvido = bool.Parse(allLines[i + 4].Trim())
                        });
                    }
                }
            }

            return emprestimos;
        }

        // Cria o painel de visualização de um empréstimo
        private StackPanel CriarPainelEmprestimo(Emprestimo emprestimo)
        {
            var emprestimoPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            emprestimoPanel.Children.Add(new TextBlock
            {
                Text = $"Livro: {emprestimo.Livro}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            emprestimoPanel.Children.Add(new TextBlock
            {
                Text = $"Usuário: {emprestimo.Usuario}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            emprestimoPanel.Children.Add(new TextBlock
            {
                Text = $"Data de Empréstimo: {emprestimo.DataEmprestimo:dd/MM/yyyy}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            emprestimoPanel.Children.Add(new TextBlock
            {
                Text = $"Data de Devolução: {emprestimo.DataDevolucao:dd/MM/yyyy}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            emprestimoPanel.Children.Add(new TextBlock
            {
                Text = $"Status: {(emprestimo.Devolvido ? "Devolvido" : "Pendente")}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = emprestimo.Devolvido ? Brushes.Green : Brushes.Red
            });

            // Add Edit button if not returned
            if (!emprestimo.Devolvido)
            {
                if (!emprestimo.Devolvido)
                {
                    var devolverButton = new Button
                    {
                        Content = "Devolver",
                        Width = 100,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 10)
                    };

                    devolverButton.Click += (sender, e) =>
                    {
                        DevolverEmprestimo(emprestimo);
                    };
                    emprestimoPanel.Children.Add(devolverButton);
                }
            }

            return emprestimoPanel;
        }

        // Método para editar empréstimo
        public void EditarEmprestimoUI(Emprestimo emprestimo)
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Editar Empréstimo",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // Livro field (readonly)
            panel.Children.Add(new TextBlock { Text = "Livro:", Margin = new Thickness(0, 0, 0, 5) });
            panel.Children.Add(new TextBlock
            {
                Text = emprestimo.Livro,
                Margin = new Thickness(0, 0, 0, 15)
            });

            // Usuário field (readonly)
            panel.Children.Add(new TextBlock { Text = "Usuário:", Margin = new Thickness(0, 0, 0, 5) });
            panel.Children.Add(new TextBlock
            {
                Text = emprestimo.Usuario,
                Margin = new Thickness(0, 0, 0, 15)
            });

            // Data Empréstimo field
            panel.Children.Add(new TextBlock { Text = "Data de Empréstimo:", Margin = new Thickness(0, 0, 0, 5) });
            _dataEmprestimo = new DatePicker
            {
                SelectedDate = new DateTimeOffset(emprestimo.DataEmprestimo),
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_dataEmprestimo);

            // Data Devolução field
            panel.Children.Add(new TextBlock { Text = "Data de Devolução:", Margin = new Thickness(0, 0, 0, 5) });
            _dataDevolucao = new DatePicker
            {
                SelectedDate = new DateTimeOffset(emprestimo.DataDevolucao),
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_dataDevolucao);

            // Save button
            var salvarBtn = new Button
            {
                Content = "Salvar Alterações",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };

            salvarBtn.Click += (sender, e) =>
            {
                if (ValidarCamposEmprestimo())
                {
                    var emprestimoAtualizado = new Emprestimo
                    {
                        Livro = emprestimo.Livro,
                        Usuario = emprestimo.Usuario,
                        DataEmprestimo = _dataEmprestimo.SelectedDate?.DateTime ?? DateTime.Today,
                        DataDevolucao = _dataDevolucao.SelectedDate?.DateTime ?? DateTime.Today.AddDays(14),
                        Devolvido = emprestimo.Devolvido
                    };

                    if (AtualizarEmprestimo(emprestimo, emprestimoAtualizado))
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

        private bool AtualizarEmprestimo(Emprestimo emprestimoAntigo, Emprestimo emprestimoNovo)
        {
            try
            {
                string filePath = GetEmprestimoFilePath();
                var linhas = new List<string>(File.ReadAllLines(filePath));

                for (int i = 0; i < linhas.Count; i += 6)
                {
                    if (i + 4 < linhas.Count &&
                        linhas[i].Trim() == emprestimoAntigo.Livro &&
                        linhas[i + 1].Trim() == emprestimoAntigo.Usuario &&
                        linhas[i + 2].Trim() == emprestimoAntigo.DataEmprestimo.ToString("yyyy-MM-dd") &&
                        linhas[i + 3].Trim() == emprestimoAntigo.DataDevolucao.ToString("yyyy-MM-dd") &&
                        linhas[i + 4].Trim() == emprestimoAntigo.Devolvido.ToString())
                    {
                        linhas[i + 2] = emprestimoNovo.DataEmprestimo.ToString("yyyy-MM-dd");
                        linhas[i + 3] = emprestimoNovo.DataDevolucao.ToString("yyyy-MM-dd");
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

        // Método para listar empréstimos em atraso
        public void ListarAtrasosUI()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Empréstimos em Atraso",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            List<Emprestimo> emprestimos = CarregarEmprestimos()
                .Where(e => !e.Devolvido && e.DataDevolucao < DateTime.Today)
                .ToList();

            if (emprestimos.Count > 0)
            {
                var atrasosPanel = new StackPanel();

                foreach (var emprestimo in emprestimos)
                {
                    atrasosPanel.Children.Add(CriarPainelEmprestimo(emprestimo));
                }

                panel.Children.Add(atrasosPanel);
            }
            else
            {
                panel.Children.Add(new TextBlock
                {
                    Text = "Nenhum empréstimo em atraso.",
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.Gray
                });
            }

            _mainContent.Content = panel;
        }

        // Método para devolver empréstimo
        private void DevolverEmprestimo(Emprestimo emprestimo)
        {
            try
            {
                string filePath = GetEmprestimoFilePath();
                var linhas = new List<string>(File.ReadAllLines(filePath));

                bool encontrado = false;

                for (int i = 0; i < linhas.Count; i += 6)
                {
                    if (i + 4 < linhas.Count &&
                        linhas[i].Trim() == emprestimo.Livro &&
                        linhas[i + 1].Trim() == emprestimo.Usuario &&
                        linhas[i + 2].Trim() == emprestimo.DataEmprestimo.ToString("yyyy-MM-dd") &&
                        linhas[i + 3].Trim() == emprestimo.DataDevolucao.ToString("yyyy-MM-dd") &&
                        linhas[i + 4].Trim() == "False") // Only mark as returned if currently not returned
                    {
                        // Found the matching loan - mark as returned
                        linhas[i + 4] = "True";
                        encontrado = true;
                        break;
                    }
                }

                if (encontrado)
                {
                    File.WriteAllLines(filePath, linhas);
                    MostrarMensagemSucessoDevolucao();
                    // Refresh the loans list to show updated status
                    ListarEmprestimosUI();
                }
                else
                {
                    MostrarMensagemErroDevolucao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                MostrarMensagemErroDevolucao();
            }
        }

        // Helper method para obter o caminho do arquivo
        private string GetEmprestimoFilePath()
        {
            string info = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(info, "A_Tua_Biblioteca");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "emprestimos.txt");
        }

        // Mensagens de feedback
        private void MostrarMensagemSucesso()
        {
            var sucesso = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            sucesso.Children.Add(new TextBlock
            {
                Text = "Empréstimo Criado com Sucesso!",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Green
            });

            var voltar = new Button
            {
                Content = "Voltar ao Formulário",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            voltar.Click += (sender, args) => CriarEmprestimoUI();
            sucesso.Children.Add(voltar);

            _mainContent.Content = sucesso;
        }

        private void MostrarMensagemErro()
        {
            var falha = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            falha.Children.Add(new TextBlock
            {
                Text = "Por favor preencha as informações corretamente!",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Red
            });

            var voltar = new Button
            {
                Content = "Voltar",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            voltar.Click += (sender, args) => CriarEmprestimoUI();
            falha.Children.Add(voltar);

            _mainContent.Content = falha;
        }

        private void MostrarMensagemSucessoEdicao()
        {
            var sucesso = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            sucesso.Children.Add(new TextBlock
            {
                Text = "Empréstimo Atualizado com Sucesso!",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Green
            });

            var voltar = new Button
            {
                Content = "Voltar à Lista",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            voltar.Click += (sender, args) => ListarEmprestimosUI();
            sucesso.Children.Add(voltar);

            _mainContent.Content = sucesso;
        }

        private void MostrarMensagemErroEdicao()
        {
            var falha = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            falha.Children.Add(new TextBlock
            {
                Text = "Erro ao atualizar empréstimo!",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Red
            });

            var voltar = new Button
            {
                Content = "Voltar",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            voltar.Click += (sender, args) => ListarEmprestimosUI();
            falha.Children.Add(voltar);

            _mainContent.Content = falha;
        }

        private void MostrarMensagemSucessoDevolucao()
        {
            var sucesso = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            sucesso.Children.Add(new TextBlock
            {
                Text = "Devolução Registrada com Sucesso!",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Green
            });

            var voltar = new Button
            {
                Content = "Voltar à Lista",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            voltar.Click += (sender, args) => ListarEmprestimosUI();
            sucesso.Children.Add(voltar);

            _mainContent.Content = sucesso;
        }

        private void MostrarMensagemErroDevolucao()
        {
            var falha = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            falha.Children.Add(new TextBlock
            {
                Text = "Erro ao registrar devolução!",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Red
            });

            var voltar = new Button
            {
                Content = "Voltar",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            voltar.Click += (sender, args) => ListarEmprestimosUI();
            falha.Children.Add(voltar);

            _mainContent.Content = falha;
        }
    }
}