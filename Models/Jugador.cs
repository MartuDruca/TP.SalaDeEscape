public class Jugador
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int SalaActual { get; set; }

    public Jugador(string nombre)
    {   
        Nombre = nombre;
        SalaActual = 1;
    }

    public Jugador() { }
}