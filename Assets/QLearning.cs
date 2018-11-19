using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class QLearning : MonoBehaviour
{

    public int totalPartidasAprendizaje;

    private Dictionary<string, float> dictionaryQ1;
    //private Dictionary<string, float> dictionaryQ2;

    private string estadoActualJugador1;
    private string estadoDespuesJugador1;

    //private string estadoActualJugador2;

    private string jugadaJugador1;
    //private string jugadaJugador2;


    private float rewardCortoPlazoJugador1;
    //private float rewardJugador2;

    private float epsilon;
    private float velocidadAprendizaje; //0 significa que no aprendemos nada de una nueva experiencia, y 1 significa que olvidamos todo lo que sabíamos hasta ahora y nos fiamos completamente de la nueva experiencia
    private float factorDescuento; //0 significa que sólo nos importan los refuerzos inmediatos, y 1 significa que los refuerzos inmediatos no importan, sólo importa el largo plazo


    private int cantidadDePartidasJugadas;

    private const char guion = '-';
    private const char equis = 'x';
    private const char laO = 'o';

    private bool finAprendizaje = false;
    private bool finPartida = false;



    // Use this for initialization
    void Start()
    {
        velocidadAprendizaje = 0.8f; //algo entre 0.9 y 0.1
        factorDescuento = 0.8f;
        rewardCortoPlazoJugador1 = 0;
        cantidadDePartidasJugadas = 0;
        dictionaryQ1 = new Dictionary<string, float>();
        //dictionaryQ2 = new Dictionary<string, float>();

        epsilon = 0.8f;

        //estadoActualJugador1 = "---------";
        
        
        for(int i = 0 ; i < 9 ; i++){
            estadoActualJugador1 += guion;
		}
        //Testeando();
        //TurnoJugador1();
    }


    void Update()
    {
        /* //SI NO HAY REFERENCIA A LA KEY DEVUELVE 0
         bool printeado;
         float value;
         printeado = dictionaryQ1.TryGetValue("algo", out value);
         print(printeado);
         print(value);*/

        
        if (!finPartida)
        {
            TurnoJugador1();
        }
        else if (!finAprendizaje)
        {
            //print(dictionaryQ1.ToString()); //NOPE
            foreach (KeyValuePair<string, float> kvp in dictionaryQ1)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                print(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            }
            finAprendizaje = true;
        }
        
        /*
        if (!finAprendizaje)
        {
            
            string guiones = "---xxxooo";
            int[] pos = PosiblesJugadas("---------");

            print(guiones[1].Equals(guion));
            print(guiones[4].Equals(equis));

            print(guiones[7].Equals(laO));


            for (int i =0; i < pos.Length; i++)
            {
                print(pos[i]);
            }
            //print(pos.ToString());
            
            int[] pos = PosiblesJugadas("-x--o--x-");
            for (int i = 0; i < pos.Length; i++)
            {
                print(pos[i]);
            }
            
            //print(pos.ToString());
            finAprendizaje = true;
        }
        */

       



    }

    private void Testeando()
    {
        jugadaJugador1 = "---------2";
        rewardCortoPlazoJugador1 = -1;

        float valorJugada;
        dictionaryQ1.TryGetValue(jugadaJugador1, out valorJugada); //recuerda, si no existe, devuelve un 0

        //jugada = jugada + velocidad de aprendizaje * (recompensa corto plazo + factor de descuento * maximo de la posible futura jugada - jugada) //el estado inicial antes de hacer la jugada no cuenta para nada
        print(valorJugada);
        dictionaryQ1[jugadaJugador1] = valorJugada + velocidadAprendizaje * (rewardCortoPlazoJugador1 + (factorDescuento * 0) - valorJugada); //maximo futuro sera 0 porque no hay mas jugadas despues de ganar
        dictionaryQ1.TryGetValue(jugadaJugador1, out valorJugada);
        print(valorJugada);


    }

    private int[] PosiblesJugadas(string estadoActual)
    {
        int queCutre =  0;
        for (int i = 0; i < estadoActual.Length ; i++ )
        {
            if (estadoActual[i].Equals(guion))
            {
                queCutre++;
            }
        }

        int[] listaPosiblesJugadas = new int[queCutre];

        int contador = 0;

        for (int i = 0; i < estadoActual.Length; i++)
        {
            if (estadoActual[i].Equals(guion))
            {
                listaPosiblesJugadas[contador] = i;
                contador++;
            }
        }

        return listaPosiblesJugadas;
    }

    private void TurnoJugador1() //A VECES HACE LA MISMA JUGADA VARIAS VECES, MIRAR EN LO RANDOM! O SI SE ACTUALIZA MAL LA COSA!
    { //x
      //E-greedy y decidimos si lo hacemos al azar  (N<E) o sino miramos el mejor de la fila
      //Random rnd = new Random();
      //float randomFloat = rnd.Next(0, 1); // creates a number between 1 and 12

        float number = Random.Range(0.0f, 1.0f);

        
        if (number < epsilon)
        {
            //random
            int[] posiblesJugadas = PosiblesJugadas(estadoActualJugador1);
            int jugadaRandom = posiblesJugadas[Random.Range(0, posiblesJugadas.Length-1)];
            jugadaJugador1 = estadoActualJugador1 + jugadaRandom;

            StringBuilder builder = new StringBuilder(estadoActualJugador1);
            builder[jugadaRandom] = equis;
            estadoDespuesJugador1 = builder.ToString();

            //print("Actual: " + estadoActualJugador1);
            print("Jugada: " + jugadaRandom);
            print("Random J1: " + estadoDespuesJugador1);

        }
        else
        {
            //mejor jugada
            float maximo = -1.0f;
             
            float value;
            int[] posiblesJugadas = PosiblesJugadas(estadoActualJugador1);
            int mejorJugada = posiblesJugadas[0];
            for (int i = 1; i < posiblesJugadas.Length; i++)
            {
                if (dictionaryQ1.TryGetValue((estadoActualJugador1 + (posiblesJugadas[i]+1)), out value))
                {
                    if (value > maximo)
                    {
                        maximo = value;
                        mejorJugada = i;
                    }
                }
            }

            jugadaJugador1 = estadoActualJugador1 + mejorJugada;

            StringBuilder builder = new StringBuilder(estadoActualJugador1);
            builder[mejorJugada] = equis;
            estadoDespuesJugador1 = builder.ToString();

            //print("Actual: " + estadoActualJugador1);
            print("Jugada: " + mejorJugada);
            print("Mejor Jugada J1: " + estadoDespuesJugador1);

        }



        //mirar si con esa jugada gano

        if (Ganado(estadoDespuesJugador1, equis))
        {
            //recompensa corto plazo
            //R1t -> +1
            //R2t -> -1
            //Actualizar Q1(S1t, A1t) ->R1t
            //Actualizar Q2(S2t-1, A2t-1) ->R2t-1

            rewardCortoPlazoJugador1 = 1;

            float valorJugada;
            dictionaryQ1.TryGetValue(jugadaJugador1, out valorJugada); //recuerda, si no existe, devuelve un 0

            //jugada = jugada + velocidad de aprendizaje * (recompensa corto plazo + factor de descuento * maximo de la posible futura jugada - jugada) //el estado inicial antes de hacer la jugada no cuenta para nada
            
            dictionaryQ1[jugadaJugador1] = valorJugada + velocidadAprendizaje * (rewardCortoPlazoJugador1 + (factorDescuento * 0) - valorJugada) ; //maximo futuro sera 0 porque no hay mas jugadas despues de ganar

            FinPartida();


        }
        else if (Empate(estadoDespuesJugador1, equis)) //para el jugador 2 el empate es despues de la jugada del jugador 1
        {
            //si es fin partida supongo que recompensa =0
            //R2t-1 <- 0
            //Actualizar Q2

            rewardCortoPlazoJugador1 = 0;

            float valorJugada;
            dictionaryQ1.TryGetValue(jugadaJugador1, out valorJugada); //recuerda, si no existe, devuelve un 0
            dictionaryQ1[jugadaJugador1] = valorJugada + velocidadAprendizaje * (rewardCortoPlazoJugador1 + (factorDescuento * 0) - valorJugada); //maximo futuro sera 0 porque no hay mas jugadas despues de ganar

            FinPartida();
        }
        else //ni se empata ni se gana
        { //lo mismo que empate pero sin terminar partida

            TurnoJugador2();

            if (Ganado(estadoActualJugador1,laO)) //si el J1 ha perdido
            {
                rewardCortoPlazoJugador1 = -1;
                float valorJugada;
                dictionaryQ1.TryGetValue(jugadaJugador1, out valorJugada); //recuerda, si no existe, devuelve un 0
                dictionaryQ1[jugadaJugador1] = valorJugada + velocidadAprendizaje * (rewardCortoPlazoJugador1 + (factorDescuento * 0) - valorJugada); //maximo futuro sera 0 porque no hay mas jugadas despues de ganar

                FinPartida();
            }



        }

        //SI NO HAY FIN CONTINUAMOS POR AQUI

        //casi que mjeor si entreno con dos algoridmos distintos



    }

    private void TurnoJugador2()
    {
        //print("Actual: " + estadoDespuesJugador1);

        StringBuilder builder = new StringBuilder(estadoDespuesJugador1);

        int[] posiblesJugadasJ2 = PosiblesJugadas(estadoDespuesJugador1);
        int jugadaRandom = posiblesJugadasJ2[Random.Range(0, posiblesJugadasJ2.Length - 1)];

        builder[jugadaRandom] = laO;
        estadoActualJugador1 = builder.ToString();
        //estado despues se queda si actualizar, ya lo hara quando juegue J1

        print("Jugada: " + jugadaRandom);
        print("Jugada J2: " + estadoActualJugador1);
    }

    private void FinPartida()
    {

        cantidadDePartidasJugadas++;
        if (cantidadDePartidasJugadas>= totalPartidasAprendizaje)
        {
            finAprendizaje = true;
        }
        finPartida = true;
    }

    private bool Ganado(string jugada, char ficha) //HAY QUE PASARLE LA JUGADA ACTUALIZADA
    {
        char[,] tablero = new char[3,3];
		int contador = 0;
		for( int i=0 ; i<3 ; i++){
			for(int j=0; j<3 ; j++){
				tablero[i,j] = jugada[contador];
				contador++;
			}
		}

		for ( int x=0;x<3;x++){ // miramos si hay victoria horizontal y vertical
			if(tablero[x, 0].Equals(ficha) && tablero[x,0].Equals(tablero[x, 1]) && tablero[x, 0].Equals(tablero[x, 2])){
				return true;
			}
			else if(tablero[0, x].Equals(ficha) && tablero[0, x].Equals(tablero[1, x]) && tablero[0, x].Equals(tablero[2, x])){
				return true;
			}
		}

		if(tablero[0, 0].Equals(ficha) && tablero[0, 0].Equals(tablero[1, 1]) && tablero[0, 0].Equals(tablero[2,2])){ //  si es \
				return true;
		}

		if(tablero[2,0].Equals(ficha) && tablero[2,0].Equals(tablero[1,1]) && tablero[2,0].Equals(tablero[0,2])){ //  si es /
				return true;
		}

		return false;

    }
    private bool Empate(string jugada, char ficha)
    {
        for (int i = 0; i < jugada.Length; i++)
        {
            if (jugada[i].Equals(ficha))
            {
                return false;
            }
        }

        return true;

    }












}