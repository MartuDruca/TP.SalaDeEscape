public class Jugador
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int SalaActual { get; set; }

    Jugador(string nombre)
    {   
        Nombre = nombre;
        SalaActual = 1;
    }
    Jugador(){}
}