// --------------------------------------------------------------------------------------
// Fyora Admin Console
// Camada de apresentação (Console UI) com navegação por menus
// - Garante criação do banco na primeira execução
// - Opera CRUD de usuários e logs de progresso
// - Centraliza Import/Export de dados via FileService (raiz do projeto)
// - UX aprimorada com mensagens e cores
// --------------------------------------------------------------------------------------

using Fyora_sprint3.Data;
using Fyora_sprint3.Models;
using Fyora_sprint3.Repositories;
using Fyora_sprint3.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Cria o banco na primeira execução (sem migrations).
using (var context = new FyoraContext())
{
    context.Database.EnsureCreated();
}

// Repositórios/serviços de longa-vida para a sessão do console.
var userRepository = new UserRepository();
var fileService = new FileService();

bool running = true;
while (running)
{
    Ui.Clear();
    Ui.Header("Fyora Admin Console");

    Ui.Option("1", "Gerenciar Usuários");
    Ui.Option("2", "Gerenciar Logs de Progresso");
    Ui.Option("3", "Importar/Exportar Dados");
    Ui.Option("0", "Sair");
    Console.WriteLine();

    var choice = Ui.Prompt("Escolha uma opção");
    switch (choice)
    {
        case "1":
            ManageUsers();
            break;
        case "2":
            ManageProgressLogs();
            break;
        case "3":
            ManageFiles();
            break;
        case "0":
            running = false;
            break;
        default:
            Ui.Warn("Opção inválida.");
            Ui.Continue();
            break;
    }
}

/// <summary>
/// Fluxo de gerenciamento de usuários (CRUD + detalhes)
/// </summary>
void ManageUsers()
{
    Ui.Clear();
    Ui.Header("Gerenciar Usuários");
    Ui.Option("1", "Adicionar Usuário");
    Ui.Option("2", "Listar todos os Usuários");
    Ui.Option("3", "Ver detalhes de um Usuário");
    Ui.Option("4", "Atualizar Usuário");
    Ui.Option("5", "Deletar Usuário");
    Console.WriteLine();

    switch (Ui.Prompt("Escolha uma opção"))
    {
        case "1": // Adicionar
            {
                var nickname = Ui.Prompt("Digite o Nickname");
                var email = Ui.Prompt("Digite o E-mail");

                if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(email))
                {
                    Ui.Warn("Nickname e E-mail não podem ser vazios.");
                    break;
                }

                try
                {
                    userRepository.AddUser(new User { Nickname = nickname, Email = email });
                    Ui.Success("Usuário adicionado com sucesso!");
                }
                catch (InvalidOperationException ex)
                {
                    // Retorna mensagens amigáveis para duplicidade (nickname/email).
                    Ui.Warn(ex.Message);
                }
                catch
                {
                    Ui.Warn("Erro ao adicionar usuário. Verifique os dados e tente novamente.");
                }
                break;
            }

        case "2": // Listar
            {
                var users = userRepository.GetAllUsers();
                Ui.SubHeader("Lista de Usuários");
                if (users.Count == 0)
                {
                    Ui.Info("Nenhum usuário cadastrado.");
                }
                else
                {
                    foreach (var u in users)
                        Console.WriteLine($"• ID: {u.Id,-4} Nickname: {u.Nickname,-20} Email: {u.Email}");
                }
                break;
            }

        case "3": // Detalhes
            {
                if (int.TryParse(Ui.Prompt("Digite o ID do Usuário"), out int detailId))
                {
                    var user = userRepository.GetUserById(detailId);
                    if (user != null)
                    {
                        Ui.SubHeader($"Detalhes de {user.Nickname} (ID: {user.Id})");
                        Console.WriteLine($"Email: {user.Email}");
                        Console.WriteLine("Logs de Progresso:");
                        if (user.ProgressLogs.Any())
                        {
                            foreach (var log in user.ProgressLogs.OrderByDescending(l => l.LogDate))
                            {
                                Console.WriteLine($"  - [{log.LogDate:dd/MM/yyyy}] Dias sem jogar: {log.DaysWithoutGambling}, Conquista: {log.Achievement}");
                            }
                        }
                        else
                        {
                            Ui.Info("Nenhum log encontrado.");
                        }
                    }
                    else
                    {
                        Ui.Warn("Usuário não encontrado.");
                    }
                }
                else
                {
                    Ui.Warn("ID inválido.");
                }
                break;
            }

        case "4": // Atualizar
            {
                if (!int.TryParse(Ui.Prompt("Digite o ID do Usuário para atualizar"), out int updateId))
                {
                    Ui.Warn("ID inválido.");
                    break;
                }

                var newNickname = Ui.Prompt("Digite o novo Nickname");
                var changeEmail = Ui.Prompt("Deseja alterar o E-mail? (s/N)").Trim().ToLower() == "s";
                string? newEmail = changeEmail ? Ui.Prompt("Digite o novo E-mail") : null;

                if (string.IsNullOrWhiteSpace(newNickname))
                {
                    Ui.Warn("Nickname não pode ser vazio.");
                    break;
                }

                try
                {
                    userRepository.UpdateUser(updateId, newNickname, newEmail);
                    Ui.Success("Usuário atualizado com sucesso!");
                }
                catch (InvalidOperationException ex)
                {
                    Ui.Warn(ex.Message);
                }
                catch
                {
                    Ui.Warn("Erro ao atualizar usuário. Verifique os dados e tente novamente.");
                }
                break;
            }

        case "5": // Deletar
            {
                if (int.TryParse(Ui.Prompt("Digite o ID do Usuário para deletar"), out int deleteId))
                {
                    userRepository.DeleteUser(deleteId);
                    Ui.Success("Usuário deletado com sucesso!");
                }
                else
                {
                    Ui.Warn("ID inválido.");
                }
                break;
            }
    }

    Ui.Continue();
}

/// <summary>
/// Fluxo de criação de logs de progresso para um usuário específico.
/// </summary>
void ManageProgressLogs()
{
    Ui.Clear();
    Ui.Header("Adicionar Log de Progresso");

    if (int.TryParse(Ui.Prompt("Digite o ID do Usuário"), out int userId))
    {
        var user = userRepository.GetUserById(userId);
        if (user != null)
        {
            if (int.TryParse(Ui.Prompt($"Digite os dias sem jogar para {user.Nickname}"), out int days))
            {
                var achievement = Ui.Prompt("Digite a conquista (ex: 'Completou desafio semanal')");
                if (!string.IsNullOrWhiteSpace(achievement))
                {
                    userRepository.AddProgressLog(userId, new ProgressLog { DaysWithoutGambling = days, Achievement = achievement });
                    Ui.Success("Log de progresso adicionado com sucesso!");
                }
                else
                {
                    Ui.Warn("Conquista não pode ser vazia.");
                }
            }
            else
            {
                Ui.Warn("Número de dias inválido.");
            }
        }
        else
        {
            Ui.Warn("Usuário não encontrado.");
        }
    }
    else
    {
        Ui.Warn("ID inválido.");
    }

    Ui.Continue();
}

/// <summary>
/// Fluxo de Import/Export (JSON/TXT) com paths resolvidos para a raiz do projeto.
/// </summary>
void ManageFiles()
{
    Ui.Clear();
    Ui.Header("Importar/Exportar Dados");
    Ui.Option("1", "Exportar todos os dados para JSON (raiz/fyora_export)");
    Ui.Option("2", "Importar dados de JSON (raiz/fyora_import.json)");
    Ui.Option("3", "Gerar Relatório Resumo em TXT (raiz/fyora_summary)");
    Console.WriteLine();

    switch (Ui.Prompt("Escolha uma opção"))
    {
        case "1":
            {
                var allUsers = userRepository.GetAllUsers();
                string outputPath = fileService.ExportUsersToJson(allUsers, "fyora_export.json");
                Ui.Success($"Dados de {allUsers.Count} usuário(s) exportados: {outputPath}");
                break;
            }
        case "2":
            {
                string fileName = Ui.Prompt($"Arquivo de import (ENTER para padrão '{Path.GetFileName(fileService.DefaultImportPath)}')").Trim();
                if (string.IsNullOrEmpty(fileName))
                    fileName = Path.GetFileName(fileService.DefaultImportPath);

                var importedUsers = fileService.ImportUsersFromJson(fileName);
                if (importedUsers.Any())
                {
                    int inserted = 0;
                    var existing = userRepository.GetAllUsers();

                    // Insere apenas novos usuários (evita duplicidade visual + repositório já valida).
                    foreach (var user in importedUsers)
                    {
                        bool exists = existing.Any(u =>
                            string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(u.Nickname, user.Nickname, StringComparison.OrdinalIgnoreCase));

                        if (!exists)
                        {
                            try
                            {
                                userRepository.AddUser(user);
                                inserted++;
                            }
                            catch (InvalidOperationException ex)
                            {
                                Ui.Warn(ex.Message);
                            }
                        }
                    }

                    Ui.Success($"{importedUsers.Count} usuário(s) lido(s). {inserted} novo(s) importado(s).");
                }
                else
                {
                    Ui.Warn($"Arquivo '{fileName}' não encontrado na raiz ou está vazio.");
                }
                break;
            }
        case "3":
            {
                var users = userRepository.GetAllUsers();
                int logCount = users.Sum(u => u.ProgressLogs.Count);
                string outputPath = fileService.ExportSummaryToTxt(users.Count, logCount, "fyora_summary.txt");
                Ui.Success($"Relatório gerado com sucesso: {outputPath}");
                break;
            }
        default:
            Ui.Warn("Opção inválida.");
            break;
    }

    Ui.Continue();
}

/// <summary>
/// Utilitários de UI para Console (cores, molduras, prompts e toasts).
/// </summary>
static class Ui
{
    /// <summary>Limpa a tela do console.</summary>
    public static void Clear() => Console.Clear();

    /// <summary>Imprime cabeçalho estilizado.</summary>
    public static void Header(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"┌────────────────────────────────────────────┐");
        Console.WriteLine($"│ {title.PadRight(42)} │");
        Console.WriteLine($"└────────────────────────────────────────────┘");
        Console.ResetColor();
    }

    /// <summary>Imprime subcabeçalho para seções internas.</summary>
    public static void SubHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"\n► {title}");
        Console.ResetColor();
    }

    /// <summary>Mostra uma opção de menu com hotkey.</summary>
    public static void Option(string key, string text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"[{key}] ");
        Console.ResetColor();
        Console.WriteLine(text);
    }

    /// <summary>Lê entrada textual com rótulo destacado.</summary>
    public static string Prompt(string label)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"→ {label}: ");
        Console.ResetColor();
        return Console.ReadLine() ?? string.Empty;
    }

    /// <summary>Mensagem de sucesso (verde, com check).</summary>
    public static void Success(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✔ {msg}");
        Console.ResetColor();
    }

    /// <summary>Mensagem de aviso/erro (amarelo escuro, com ícone).</summary>
    public static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"⚠ {msg}");
        Console.ResetColor();
    }

    /// <summary>Mensagem informativa (cinza, bullet).</summary>
    public static void Info(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"• {msg}");
        Console.ResetColor();
    }

    /// <summary>Pausa de retorno ao menu principal.</summary>
    public static void Continue()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Pressione qualquer tecla para voltar ao menu...");
        Console.ResetColor();
        Console.ReadKey(true);
    }
}
