// --------------------------------------------------------------------------------------
// Entidade ProgressLog
// - Registro de progresso (dias sem jogar, conquistas, data do log)
// - Relacionado a User (FK obrigatória) com cascata na exclusão
// --------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace Fyora_sprint3.Models
{
    /// <summary>
    /// Representa um registro de progresso associado a um usuário.
    /// </summary>
    public class ProgressLog
    {
        /// <summary>Chave primária.</summary>
        public int Id { get; set; }

        /// <summary>Chave estrangeira para o usuário.</summary>
        public int UserId { get; set; }

        /// <summary>Quantidade de dias sem jogar reportada nesse log.</summary>
        [Range(0, int.MaxValue)]
        public int DaysWithoutGambling { get; set; }

        /// <summary>Pequena descrição da conquista/etapa atingida.</summary>
        [MaxLength(300)]
        public string? Achievement { get; set; }

        /// <summary>Data/hora do registro do log.</summary>
        public DateTime LogDate { get; set; } = DateTime.Now;

        /// <summary>Navegação para o usuário.</summary>
        public User? User { get; set; }
    }
}
