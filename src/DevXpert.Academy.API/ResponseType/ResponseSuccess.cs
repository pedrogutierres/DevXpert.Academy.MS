using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DevXpert.Academy.API.ResponseType
{
    public class ResponseSuccess
    {
        /// <summary>
        /// ID do dado que foi salvo, alterado, consultado, excluido ou etc
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Geralmente quando um novo código identificador é gerado pelo database
        /// </summary>
        public long? Codigo { get; set; }

        /// <summary>
        /// Notificações que indicam um ponto de atenção ou possível problema futuro
        /// </summary>
        [JsonPropertyName("warnings")]
        public IEnumerable<string> Warnings { get; set; }
    }

    public class ResponseSuccess<T> : ResponseSuccess
    {
        public T Data { get; set; }
    }
}
