using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FighterScript : MonoBehaviour {

    public GameObject playerOne;
    public GameObject playerTwo;

    public int totalPartidas = 10;

    private bool start;
    private string estadoTablero;

    private Dictionary<string, float> dictionaryQ1;
    private Dictionary<string, float> dictionaryQ2;


    private const char guion = '-';
    private const char equis = 'x';
    private const char laO = 'o';

    private int ganadosJ1=0;
    private int ganadosJ2=0;
    private int empate=0;



    // Use this for initialization
    void Start () {
        start = false;
        NuevaPartida();
        StartCoroutine(WaitingForOthers());
        
	}
	
	// Update is called once per frame
	void Update () {

        if (start)
        {
            HacerPartida();
            if (totalPartidas<=0) {

                start = false;
                print("SeCABO");
                print("GanadosJ1: "+ganadosJ1);
                print("GanadosJ2: " + ganadosJ2);
                print("Empate: " + empate);



            }

        }
	}

    private int[] PosiblesJugadas(string estadoActual)
    {
        int queCutre = 0;
        for (int y = 0; y < estadoActual.Length; y++)
        {
            if (estadoActual[y].Equals(guion))
            {
                queCutre++;
            }
        }

        int[] listaPosiblesJugadas = new int[queCutre];

        int contador = 0;

        for (int p = 0; p < estadoActual.Length; p++)
        {
            if (estadoActual[p].Equals(guion))
            {
                listaPosiblesJugadas[contador] = p;
                contador++;
            }
        }

        return listaPosiblesJugadas;
    }

    private void NuevaPartida()
    {

        estadoTablero = "---------";

        //jugada random J1
        int[] posiblesJugadas = PosiblesJugadas(estadoTablero);
        int jugadaRandom = posiblesJugadas[Random.Range(0, posiblesJugadas.Length - 1)];

        StringBuilder builder = new StringBuilder(estadoTablero);
        builder[jugadaRandom] = equis;
        estadoTablero = builder.ToString();

        //jugada random J2
        posiblesJugadas = PosiblesJugadas(estadoTablero);
        jugadaRandom = posiblesJugadas[Random.Range(0, posiblesJugadas.Length - 1)];

        StringBuilder builder2 = new StringBuilder(estadoTablero);
        builder2[jugadaRandom] = laO;
        estadoTablero = builder2.ToString();
        //print("INICIO: " + estadoTablero);

    }

    IEnumerator WaitingForOthers()
    {
        yield return new WaitUntil(() => playerOne.GetComponent<QLearning>().finAprendizaje == true);
        print("OKEY");
        yield return new WaitUntil(() => playerTwo.GetComponent<QLearningJ2>().finAprendizaje == true);
        print("OKEY2");

        dictionaryQ1 = playerOne.GetComponent<QLearning>().GetDictionary();
        dictionaryQ2 = playerTwo.GetComponent<QLearningJ2>().GetDictionary();
/*
        print("DICTIONARY 1");

        foreach (KeyValuePair<string, float> kvp in dictionaryQ1)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            print(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        }

        print("DICTIONARY 2");

        foreach (KeyValuePair<string, float> kvp in dictionaryQ2)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            print(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        }
*/

        start = true;

    }

    private void HacerPartida()
    {
        //jugada J1

        float maximo;

        float value;
        int[] posiblesJugadas = PosiblesJugadas(estadoTablero);
        int mejorJugada = posiblesJugadas[0];

        dictionaryQ1.TryGetValue((estadoTablero + (posiblesJugadas[0] + 1)), out maximo);

        for (int i = 0; i < posiblesJugadas.Length; i++)
        {
            if (dictionaryQ1.TryGetValue((estadoTablero + (posiblesJugadas[i] + 1)), out value))
            {
                if (value > maximo)
                {
                    maximo = value;
                    mejorJugada = i;
                }
            }
        }

        StringBuilder builder = new StringBuilder(estadoTablero);
        builder[mejorJugada] = equis;
        estadoTablero = builder.ToString();

        //print("J1: " + estadoTablero);

        if (Ganado(estadoTablero, equis))
        {
            ganadosJ1++;
            totalPartidas--;
            NuevaPartida();

        }
        else if (Empate(estadoTablero, guion))
        {

            empate++;
            totalPartidas--;
            NuevaPartida();

        }
        else 
        {
            //JUGADA j2

            posiblesJugadas = PosiblesJugadas(estadoTablero);
            mejorJugada = posiblesJugadas[0];

            dictionaryQ2.TryGetValue((estadoTablero + (posiblesJugadas[0] + 1)), out maximo);

            for (int i = 0; i < posiblesJugadas.Length; i++)
            {
                if (dictionaryQ2.TryGetValue((estadoTablero + (posiblesJugadas[i] + 1)), out value))
                {
                    if (value > maximo)
                    {
                        maximo = value;
                        mejorJugada = i;
                    }
                }
            }

            StringBuilder builder2 = new StringBuilder(estadoTablero);
            builder2[mejorJugada] = laO;
            estadoTablero = builder2.ToString();

            //print("J2: " + estadoTablero);


            if (Ganado(estadoTablero, laO))
            {
                ganadosJ2++;
                totalPartidas--;
                NuevaPartida();

            }
        }

    }

    private bool Ganado(string jugada, char ficha)
    {
        char[,] tablero = new char[3, 3];
        int contador = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tablero[i, j] = jugada[contador];
                contador++;
            }
        }

        for (int x = 0; x < 3; x++)
        { // miramos si hay victoria horizontal y vertical
            if (tablero[x, 0].Equals(ficha) && tablero[x, 0].Equals(tablero[x, 1]) && tablero[x, 0].Equals(tablero[x, 2]))
            {
                //print("GANADO: FILA");
                return true;

            }
            else if (tablero[0, x].Equals(ficha) && tablero[0, x].Equals(tablero[1, x]) && tablero[0, x].Equals(tablero[2, x]))
            {
                //print("GANADO: COLUMNA");
                return true;

            }
        }

        if (tablero[0, 0].Equals(ficha) && tablero[0, 0].Equals(tablero[1, 1]) && tablero[0, 0].Equals(tablero[2, 2]))
        { //  si es \
            //print("GANADO: \\");
            return true;

        }

        if (tablero[2, 0].Equals(ficha) && tablero[2, 0].Equals(tablero[1, 1]) && tablero[2, 0].Equals(tablero[0, 2]))
        { //  si es /
            //print("GANADO: //");
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
        //print("EMPATE");
        return true;


    }


}
