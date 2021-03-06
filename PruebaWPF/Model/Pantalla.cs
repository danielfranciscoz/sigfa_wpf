//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PruebaWPF.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pantalla
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pantalla()
        {
            this.AccesoDirectoPerfil = new HashSet<AccesoDirectoPerfil>();
            this.AccesoDirectoUsuario = new HashSet<AccesoDirectoUsuario>();
            this.Permiso = new HashSet<Permiso>();
        }
    
        public int IdPantalla { get; set; }
        public Nullable<int> IdPadre { get; set; }
        public string Titulo { get; set; }
        public string Uid { get; set; }
        public string Descripcion { get; set; }
        public bool isMenu { get; set; }
        public string Tipo { get; set; }
        public string URL { get; set; }
        public byte Orden { get; set; }
        public string Abreviacion { get; set; }
        public string Icon { get; set; }
        public bool isWeb { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public bool regAnulado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccesoDirectoPerfil> AccesoDirectoPerfil { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccesoDirectoUsuario> AccesoDirectoUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Permiso> Permiso { get; set; }
    }
}
