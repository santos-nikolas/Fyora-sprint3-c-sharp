// --------------------------------------------------------------------------------------
// FileService
// - Resolve caminhos relativos à RAIZ do projeto (onde está o .csproj)
// - Importa usuários de JSON (default: {raiz}/fyora_import.json)
// - Exporta JSON e TXT para pastas na raiz: fyora_export/ e fyora_summary/
// - Evita gravações em bin/Debug/... quando o usuário fornece apenas o nome do arquivo
// --------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Fyora_sprint3.Models;

namespace Fyora_sprint3.Services
{
    /// <summary>
    /// Serviço de I/O para importação e exportação de dados em arquivos.
    /// Resolve caminhos a partir da raiz do projeto para padronizar a organização.
    /// </summary>
    public class FileService
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        private readonly string _projectRoot;

        /// <summary>
        /// Inicializa o serviço localizando a raiz do projeto (diretório que contém o .csproj).
        /// </summary>
        /// <exception cref="InvalidOperationException">Quando a raiz do projeto não é encontrada.</exception>
        public FileService()
        {
            _projectRoot = FindProjectRoot()
                ?? throw new InvalidOperationException("Não foi possível localizar a raiz do projeto (arquivo .csproj).");
        }

        /// <summary>
        /// Caminho padrão de importação: {raiz}/fyora_import.json
        /// </summary>
        public string DefaultImportPath => Path.Combine(_projectRoot, "fyora_import.json");

        /// <summary>
        /// Exporta uma lista de usuários para JSON.
        /// Se for passado apenas o nome do arquivo, salva em {raiz}/fyora_export/{nome}.
        /// </summary>
        /// <param name="users">Lista de usuários.</param>
        /// <param name="fileNameOrPath">Nome do arquivo ou caminho absoluto.</param>
        /// <returns>Caminho final do arquivo gravado.</returns>
        public string ExportUsersToJson(List<User> users, string fileNameOrPath)
        {
            string path = ResolveExportPath(fileNameOrPath, "fyora_export");
            EnsureDirectory(path);

            string jsonString = JsonSerializer.Serialize(users, _options);
            File.WriteAllText(path, jsonString);

            return path;
        }

        /// <summary>
        /// Importa usuários a partir de um JSON.
        /// Tenta, nesta ordem:
        /// 1) Caminho absoluto informado (se existir)
        /// 2) {raiz}/{arquivo}, quando receber apenas o nome
        /// 3) {raiz}/fyora_import.json (padrão)
        /// 4) Pasta de execução (bin/...) como fallback
        /// </summary>
        /// <param name="fileNameOrPath">Nome do arquivo (ex.: "fyora_import.json") ou caminho absoluto.</param>
        /// <returns>Lista de usuários desserializada (vazia se arquivo não existir).</returns>
        public List<User> ImportUsersFromJson(string fileNameOrPath)
        {
            // 1) Caminho absoluto
            if (Path.IsPathRooted(fileNameOrPath) && File.Exists(fileNameOrPath))
                return ReadUsers(fileNameOrPath);

            // 2) Nome simples na raiz
            string inRoot = Path.Combine(_projectRoot, fileNameOrPath);
            if (File.Exists(inRoot))
                return ReadUsers(inRoot);

            // 3) Caminho padrão
            if (File.Exists(DefaultImportPath))
                return ReadUsers(DefaultImportPath);

            // 4) Fallback: pasta de execução (compatibilidade)
            string baseDirCandidate = Path.Combine(AppContext.BaseDirectory, fileNameOrPath);
            if (File.Exists(baseDirCandidate))
                return ReadUsers(baseDirCandidate);

            return new List<User>();
        }

        /// <summary>
        /// Gera um relatório TXT simples com contagens agregadas.
        /// Se for passado apenas o nome do arquivo, salva em {raiz}/fyora_summary/{nome}.
        /// </summary>
        /// <param name="userCount">Quantidade total de usuários.</param>
        /// <param name="logCount">Quantidade total de logs.</param>
        /// <param name="fileNameOrPath">Nome do arquivo ou caminho absoluto.</param>
        /// <returns>Caminho final do arquivo gerado.</returns>
        public string ExportSummaryToTxt(int userCount, int logCount, string fileNameOrPath)
        {
            string path = ResolveExportPath(fileNameOrPath, "fyora_summary");
            EnsureDirectory(path);

            string reportContent =
                $"--- Relatório Resumo Fyora Admin ---{Environment.NewLine}" +
                $"Data do Relatório: {DateTime.Now}{Environment.NewLine}" +
                $"Total de Usuários no Sistema: {userCount}{Environment.NewLine}" +
                $"Total de Logs de Progresso Registrados: {logCount}{Environment.NewLine}";

            File.WriteAllText(path, reportContent);
            return path;
        }

        // -------------------- Helpers internos --------------------

        /// <summary>Lê e desserializa usuários a partir de um arquivo JSON.</summary>
        private static List<User> ReadUsers(string path)
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<User>>(jsonString, new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            }) ?? new List<User>();
        }

        /// <summary>
        /// Resolve um caminho final de export ao receber apenas um nome de arquivo:
        /// grava em {raiz}/{defaultFolder}/{nome}. Se já vier absoluto, respeita-o.
        /// </summary>
        private string ResolveExportPath(string fileNameOrPath, string defaultFolder)
        {
            if (Path.IsPathRooted(fileNameOrPath))
                return fileNameOrPath;

            string folder = Path.Combine(_projectRoot, defaultFolder);
            return Path.Combine(folder, fileNameOrPath);
        }

        /// <summary>Garante que o diretório do caminho exista (cria se necessário).</summary>
        private static void EnsureDirectory(string finalPath)
        {
            string? dir = Path.GetDirectoryName(finalPath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Sobe diretórios a partir de AppContext.BaseDirectory até encontrar um *.csproj.
        /// </summary>
        /// <returns>Diretório raiz do projeto ou null se não encontrar.</returns>
        private static string? FindProjectRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current is not null)
            {
                bool hasCsproj = current.EnumerateFiles("*.csproj", SearchOption.TopDirectoryOnly).Any();
                if (hasCsproj)
                    return current.FullName;

                current = current.Parent;
            }
            return null;
        }
    }
}
