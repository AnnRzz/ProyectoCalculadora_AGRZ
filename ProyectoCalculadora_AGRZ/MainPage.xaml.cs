using System.Globalization;

namespace ProyectoCalculadora_AGRZ;

public partial class MainPage : ContentPage
{
    string expresion = "";          // Lo que el usuario va escribiendo
    bool resultadoMostrado = false;

    public MainPage()
    {
        InitializeComponent();
    }

    // NUMEROS DEL 0-9
    void OnNumero(object sender, EventArgs e)
    {
        if (resultadoMostrado) { expresion = ""; resultadoMostrado = false; }
        if (expresion.EndsWith("%")) return;

        expresion += ((Button)sender).Text;
        Pantalla.Text = expresion;
    }

    // EL PUNTO DECIMAL
    async void OnPunto(object sender, EventArgs e)
    {
        if (resultadoMostrado) { expresion = ""; resultadoMostrado = false; }
        if (expresion.EndsWith("%")) return;

        // Ultimo numero escrito lo que esta despues del ultimo operador
        string ultimoNumero = expresion.Split('+', '-', '×', '÷').Last();

        if (ultimoNumero.Contains('.'))
        {

        }

        expresion += ultimoNumero == "" ? "0." : ".";
        Pantalla.Text = expresion;
    }

    // OPERADORES
    void OnOperador(object sender, EventArgs e)
    {
        if (expresion == "") return;           // Para no empezar con operador
        resultadoMostrado = false;

        string operador = ((Button)sender).Text;
        string ultimo = expresion.Substring(expresion.Length - 1);

        if ("+-×÷".Contains(ultimo))
            expresion = expresion.Substring(0, expresion.Length - 1) + operador; // Cambio del operador
        else
            expresion += operador;

        Pantalla.Text = expresion;
    }

    // PORCENTAJE
    void OnPorcentaje(object sender, EventArgs e)
    {
        if (expresion == "") return;
        resultadoMostrado = false;

        string ultimo = expresion.Substring(expresion.Length - 1);
        if (char.IsDigit(ultimo[0]))           // Solo se pone despues de un numero
        {
            expresion += "%";
            Pantalla.Text = expresion;
        }
    }

    // BORRAR EL ULTIMO CARACTER
    void OnBorrar(object sender, EventArgs e)
    {
        resultadoMostrado = false;
        if (expresion.Length > 0)
            expresion = expresion.Substring(0, expresion.Length - 1);

        Pantalla.Text = expresion == "" ? "0" : expresion;
    }

    // LIMPIAR PANTALLA
    void OnLimpiar(object sender, EventArgs e)
    {
        expresion = "";
        resultadoMostrado = false;
        Pantalla.Text = "0";
    }

    // EL IGUAL
    async void OnIgual(object sender, EventArgs e)
    {
        // Quita operadores o punto sobrantes
        while (expresion.Length > 0 && "+-×÷.".Contains(expresion[expresion.Length - 1]))
            expresion = expresion.Substring(0, expresion.Length - 1);

        if (expresion == "") { Pantalla.Text = "0"; return; }

        try
        {
            double resultado = Calcular(expresion);
            expresion = Math.Round(resultado, 8).ToString(CultureInfo.InvariantCulture);
            Pantalla.Text = expresion;
            resultadoMostrado = true;
        }
        catch (DivideByZeroException)
        {
            expresion = "";
            Pantalla.Text = "0";
        }
    }

    // CALCULAR
    double Calcular(string texto)
    {
        List<double> numeros = new List<double>();
        List<char> operadores = new List<char>();
        string actual = "";

        // Separar numeros y operadores
        for (int i = 0; i < texto.Length; i++)
        {
            char c = texto[i];
            bool esOperador = "+-×÷".Contains(c);
            bool esSigno = (c == '-' && i == 0);   // Muestra el - 

            if (esOperador && !esSigno)
            {
                numeros.Add(ConvertirNumero(actual));
                operadores.Add(c);
                actual = "";
            }
            else
            {
                actual += c;
            }
        }
        numeros.Add(ConvertirNumero(actual));

        // Operadores × y ÷
        int pos = 0;
        while (pos < operadores.Count)
        {
            if (operadores[pos] == '×' || operadores[pos] == '÷')
            {
                if (operadores[pos] == '÷' && numeros[pos + 1] == 0)
                    throw new DivideByZeroException();

                if (operadores[pos] == '×')
                    numeros[pos] = numeros[pos] * numeros[pos + 1];
                else
                    numeros[pos] = numeros[pos] / numeros[pos + 1];

                numeros.RemoveAt(pos + 1);
                operadores.RemoveAt(pos);
            }
            else
            {
                pos++;
            }
        }

        // Operadores + y -
        double total = numeros[0];
        for (int i = 0; i < operadores.Count; i++)
        {
            if (operadores[i] == '+')
                total = total + numeros[i + 1];
            else
                total = total - numeros[i + 1];
        }
        return total;
    }

    // CONVERTIR NUMERO A PORCENTAJE
    double ConvertirNumero(string texto)
    {
        bool llevaPorcentaje = texto.EndsWith("%");
        if (llevaPorcentaje)
            texto = texto.Substring(0, texto.Length - 1);

        double valor = double.Parse(texto, CultureInfo.InvariantCulture);
        return llevaPorcentaje ? valor / 100 : valor;
    }
}