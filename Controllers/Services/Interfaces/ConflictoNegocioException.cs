// PROPUESTA del integrante 3 según contratos §4B; pendiente de aprobación del integrante 2
namespace personapi_dotnet.Controllers.Services.Interfaces
{
    /// <summary>Tipos de error de negocio que un servicio puede reportar.</summary>
    public enum TipoConflicto
    {
        /// <summary>Ya existe un registro con la misma llave (PK repetida).</summary>
        LlaveDuplicada,

        /// <summary>El registro relacionado no existe (persona, profesión o dueño: FK inexistente).</summary>
        ReferenciaInexistente,

        /// <summary>El registro tiene dependientes y no se borra (FK con NO ACTION, contratos §2.2).</summary>
        BorradoConReferencias
    }

    /// <summary>
    /// Mecanismo único de errores de negocio de los servicios (contratos §4B).
    /// Los servicios la lanzan en lugar de dejar escapar excepciones de EF Core o SQL;
    /// los controladores de vista la convierten en un error de ModelState y los API Controller en 409.
    /// "No existe" no es un conflicto: se informa con null (consultas) o false (actualizar y eliminar).
    /// </summary>
    public class ConflictoNegocioException : Exception
    {
        public ConflictoNegocioException(TipoConflicto tipo, string mensaje, Exception? causa = null)
            : base(mensaje, causa)
        {
            Tipo = tipo;
        }

        public TipoConflicto Tipo { get; }
    }
}
