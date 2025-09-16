using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Fyora_sprint3.Models
{
    /// <summary>
    /// Representa um usuário do sistema, identificado por Nickname e Email.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Nickname { get; set; } = string.Empty;

        [Required, MaxLength(200), EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Data/hora de criação do registro.
        /// Observação: também há default no banco (CURRENT_TIMESTAMP); este valor em C# é um fallback.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<ProgressLog> ProgressLogs { get; set; } = new();
    }
}
