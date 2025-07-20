using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using System.IO;
using System.Collections.Generic;
using Avalonia;
using System;

namespace A_Tua_Biblioteca
{
    public class UsuarioManager
    {
        // Campos para os controles de UI
        private TextBox _nome;
        private TextBox _email;
        private TextBox _password;
        private ContentControl _mainContent;

        public UsuarioManager(ContentControl mainContent)
        {
            _mainContent = mainContent;
        }

        // Modelo de dados para usuário
        public class Usuario
        {
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // Método para criar a interface de usuário
        public void CriarUsuarioUI()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            // Título
            panel.Children.Add(new TextBlock
            {
                Text = "Criar Usuário",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // Campo Nome
            panel.Children.Add(new TextBlock { Text = "Nome:", Margin = new Thickness(0, 0, 0, 5) });
            _nome = new TextBox
            {
                Watermark = "Nome Completo",
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_nome);

            // Campo Email
            panel.Children.Add(new TextBlock { Text = "Email:", Margin = new Thickness(0, 0, 0, 5) });
            _email = new TextBox
            {
                Watermark = "Insira o seu email",
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_email);

            // Campo Password
            panel.Children.Add(new TextBlock { Text = "Password:", Margin = new Thickness(0, 0, 0, 5) });
            _password = new TextBox
            {
                PasswordChar = '*',
                Watermark = "Insira a sua password",
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_password);

            // Botão Submeter
            var submeter = new Button
            {
                Content = "Submeter",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };
            submeter.Click += GuardarUsuario;
            panel.Children.Add(submeter);

            _mainContent.Content = panel;
        }

        // Método para guardar um usuário
        private void GuardarUsuario(object sender, RoutedEventArgs e)
        {
            if (ValidarCamposUsuario())
            {
                var usuario = new Usuario
                {
                    Nome = _nome.Text,
                    Email = _email.Text,
                    Password = _password.Text
                };

                SalvarUsuarioEmArquivo(usuario);
                MostrarMensagemSucesso();
            }
            else
            {
                MostrarMensagemErro();
            }
        }

        // Validação dos campos
        private bool ValidarCamposUsuario()
        {
            return !string.IsNullOrWhiteSpace(_nome.Text) &&
                   !string.IsNullOrWhiteSpace(_email.Text) &&
                   !string.IsNullOrWhiteSpace(_password.Text);
        }

        // Persistência em arquivo
        private void SalvarUsuarioEmArquivo(Usuario usuario)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDir = Path.Combine(appDataPath, "A_Tua_Biblioteca");
            Directory.CreateDirectory(appDir);

            string filePath = Path.Combine(appDir, "usuarios.txt");

            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine(usuario.Nome);
                writer.WriteLine(usuario.Email);
                writer.WriteLine(usuario.Password);
                writer.WriteLine("-----------------");
            }
        }

        // Método para listar usuários
        public void ListarUsuariosUI()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Lista de Usuários",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            List<Usuario> usuarios = CarregarUsuarios();

            if (usuarios.Count > 0)
            {
                var usersPanel = new StackPanel();

                foreach (var usuario in usuarios)
                {
                    usersPanel.Children.Add(CriarPainelUsuario(usuario));
                }

                panel.Children.Add(usersPanel);
            }
            else
            {
                panel.Children.Add(new TextBlock
                {
                    Text = "Nenhum usuário cadastrado ainda.",
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.Gray
                });
            }

           
            _mainContent.Content = panel;
        }

        // Carrega usuários do arquivo
        public List<Usuario> CarregarUsuarios()
        {
            var usuarios = new List<Usuario>();
            string filePath = GetUsuarioFilePath();

            if (File.Exists(filePath))
            {
                string[] allLines = File.ReadAllLines(filePath);

                for (int i = 0; i < allLines.Length; i += 4)
                {
                    if (!string.IsNullOrWhiteSpace(allLines[i]) && allLines[i] != "-----------------")
                    {
                        usuarios.Add(new Usuario
                        {
                            Nome = allLines[i].Trim(),
                            Email = allLines[i + 1].Trim(),
                            Password = allLines[i + 2].Trim()
                        });
                    }
                }
            }

            return usuarios;
        }

        // Cria o painel de visualização de um usuário
        private StackPanel CriarPainelUsuario(Usuario usuario)
        {
            var userPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            userPanel.Children.Add(new TextBlock
            {
                Text = $"Nome: {usuario.Nome}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            userPanel.Children.Add(new TextBlock
            {
                Text = $"Email: {usuario.Email}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            userPanel.Children.Add(new TextBlock
            {
                Text = $"Password: {new string('*', usuario.Password.Length)}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Add Edit button
            var editButton = new Button
            {
                Content = "Editar",
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };

            editButton.Click += (sender, e) => EditarUsuarioUI(usuario);
            userPanel.Children.Add(editButton);

            return userPanel;
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
                Text = "Usuário Criado com Sucesso!",
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
            voltar.Click += (sender, args) => CriarUsuarioUI();
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
                Text = "Por favor preencha as informações!",
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
            voltar.Click += (sender, args) => CriarUsuarioUI();
            falha.Children.Add(voltar);

            _mainContent.Content = falha;
        }

        // Helper method para obter o caminho do arquivo
        private string GetUsuarioFilePath()
        {
            string info = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(info, "A_Tua_Biblioteca");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "usuarios.txt");
        }

        public void EditarUsuarioUI(Usuario usuario)
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Editar Usuário",
                FontSize = 32,
                Margin = new Thickness(0, 0, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // Nome field
            panel.Children.Add(new TextBlock { Text = "Nome:", Margin = new Thickness(0, 0, 0, 5) });
            _nome = new TextBox
            {
                Text = usuario.Nome,
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_nome);

            // Email field
            panel.Children.Add(new TextBlock { Text = "Email:", Margin = new Thickness(0, 0, 0, 5) });
            _email = new TextBox
            {
                Text = usuario.Email,
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_email);

            // Password field
            panel.Children.Add(new TextBlock { Text = "Password:", Margin = new Thickness(0, 0, 0, 5) });
            _password = new TextBox
            {
                Text = usuario.Password,
                PasswordChar = '*',
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(_password);

            // Save button
            var salvarBtn = new Button
            {
                Content = "Salvar Alterações",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10)
            };

            salvarBtn.Click += (sender, e) =>
            {
                if (ValidarCamposUsuario())
                {
                    var usuarioAtualizado = new Usuario
                    {
                        Nome = _nome.Text,
                        Email = _email.Text,
                        Password = _password.Text
                    };

                    if (AtualizarUsuario(usuario, usuarioAtualizado))
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

        private bool AtualizarUsuario(Usuario usuarioAntigo, Usuario usuarioNovo)
        {
            try
            {
                string filePath = GetUsuarioFilePath();
                var linhas = new List<string>(File.ReadAllLines(filePath));

                for (int i = 0; i < linhas.Count; i += 4)
                {
                    if (i + 2 < linhas.Count &&
                        linhas[i].Trim() == usuarioAntigo.Nome &&
                        linhas[i + 1].Trim() == usuarioAntigo.Email &&
                        linhas[i + 2].Trim() == usuarioAntigo.Password)
                    {
                        linhas[i] = usuarioNovo.Nome;
                        linhas[i + 1] = usuarioNovo.Email;
                        linhas[i + 2] = usuarioNovo.Password;
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

        private void MostrarMensagemSucessoEdicao()
        {
            var sucesso = new StackPanel
            {
                Margin = new Thickness(40, 100, 40, 40)
            };

            sucesso.Children.Add(new TextBlock
            {
                Text = "Usuário Atualizado com Sucesso!",
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
            voltar.Click += (sender, args) => ListarUsuariosUI();
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
                Text = "Erro ao atualizar usuário!",
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
            voltar.Click += (sender, args) => ListarUsuariosUI();
            falha.Children.Add(voltar);

            _mainContent.Content = falha;
        }

        
        

    }
}