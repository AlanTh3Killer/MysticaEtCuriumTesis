using UnityEngine;

[CreateAssetMenu(fileName = "GrimorioEntry", menuName = "Grimorio/Entry")]
public class GrimorioEntrySO : ScriptableObject
{
    // Identificador unico, ejemplo "cristal_luz"
    public string id;

    // Titulo que se mostrara en la pagina
    public string titulo;

    // Categoria simple: Vendible, Contenible, Destruible o General
    public string categoria;

    // Descripcion larga
    [TextArea(3, 8)]
    public string descripcion;

    // Si quieres, una descripcion corta para resumen
    public string descripcionCorta;

    // Icono o mini imagen (opcional)
    public Sprite icono;
}
