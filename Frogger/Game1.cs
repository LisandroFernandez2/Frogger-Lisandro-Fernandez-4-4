// "using" le dice a C# qué librerías vamos a usar en este archivo
using System;                           // Para AppContext (saber dónde está el ejecutable)
using System.IO;                        // Para leer archivos (File, Path)
using Microsoft.Xna.Framework;          // Clases base de MonoGame (Game, Vector2, Color, GameTime...)
using Microsoft.Xna.Framework.Graphics; // Para dibujar (Texture2D, SpriteBatch)
using Microsoft.Xna.Framework.Input;    // Para leer el teclado (Keyboard, Keys)

namespace Frogger; // Tiene que coincidir con el que usás en Program.cs (new Frogger.Game1())

public class Game1 : Game
{
    // ---------- VARIABLES DEL JUEGO ----------

    // Administra la ventana y la placa de video
    private GraphicsDeviceManager _graphics;

    // Es la "brocha" que usamos para dibujar imágenes en pantalla
    private SpriteBatch _spriteBatch;

    // Las dos imágenes de la rana
    private Texture2D _ranaNormal;    // Rana quieta
    private Texture2D _ranaSaltando;  // Rana en el aire

    // Posición actual de la rana en la pantalla (X = horizontal, Y = vertical)
    // (0,0) es la esquina superior izquierda
    private Vector2 _posicion;

    // Ángulo de rotación de la rana, en radianes.
    // 0 = mirando arriba, PiOver2 = derecha, Pi = abajo, -PiOver2 = izquierda.
    // Se guarda para que, al quedar quieta, siga mirando hacia donde saltó por última vez.
    private float _rotacion = 0f;

    // Ajuste por si tus imágenes originales NO miran hacia arriba:
    //   Mira a la derecha:  -MathHelper.PiOver2
    //   Mira a la izquierda: MathHelper.PiOver2
    //   Mira hacia abajo:    MathHelper.Pi
    //   Mira hacia arriba:   0f
    private const float AjusteImagen = 0f;

    // Guarda cómo estaba el teclado en el frame anterior.
    // Sirve para detectar el momento exacto en que se APRIETA una tecla
    // (y no que se mantenga apretada).
    private KeyboardState _tecladoAnterior;

    // Cuánto tiempo falta para que termine el salto actual (en segundos)
    private float _tiempoSalto;

    // Cuánto dura el salto, o sea, cuánto tiempo se ve la imagen "saltando"
    private const float DuracionSalto = 0.15f;

    // Cuántos píxeles avanza la rana en cada salto
    private const int Casilla = 48;

    // Vale true mientras la rana está en pleno salto
    private bool _seMueve;

    // ---------- CONSTRUCTOR ----------
    // Se ejecuta una sola vez, cuando se crea el juego
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content"; // Carpeta del Content Pipeline (no la usamos para las imágenes)
        IsMouseVisible = true;             // Muestra el cursor del mouse sobre la ventana
    }

    // ---------- INITIALIZE ----------
    // Se ejecuta una vez, antes de empezar. Acá se prepara el estado inicial.
    protected override void Initialize()
    {
        // La rana arranca en esta posición (X=400, Y=400)
        _posicion = new Vector2(400, 400);

        base.Initialize();
    }

    // ---------- LOADCONTENT ----------
    // Se ejecuta una vez, y es donde se cargan imágenes, sonidos, etc.
    protected override void LoadContent()
    {
        // Creamos el SpriteBatch (necesita el GraphicsDevice, por eso va acá)
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Cargamos las dos imágenes desde la carpeta Assets.
        // Los nombres tienen que coincidir EXACTO con los archivos (mayúsculas incluidas).
        _ranaNormal = CargarTextura("Rana_normal.png");
        _ranaSaltando = CargarTextura("Rana_saltando.png");
    }

    // Función ayudante: carga una imagen de la carpeta Assets y la devuelve como textura.
    // Así no repetimos el mismo código para cada imagen.
    private Texture2D CargarTextura(string nombre)
    {
        // AppContext.BaseDirectory = carpeta donde está el .exe (bin/Debug/net9.0/)
        // Path.Combine arma la ruta completa: ...\Assets\nombre
        string ruta = Path.Combine(AppContext.BaseDirectory, "Assets", nombre);

        // Abrimos el archivo. "using" lo cierra solo cuando terminamos de leerlo.
        using var stream = File.OpenRead(ruta);

        // Convertimos el archivo en una textura que MonoGame puede dibujar
        return Texture2D.FromStream(GraphicsDevice, stream);
    }

    // ---------- UPDATE ----------
    // Se ejecuta unas 60 veces por segundo. Acá va la LÓGICA del juego (mover, colisiones, etc.)
    protected override void Update(GameTime gameTime)
    {
        // Leemos qué teclas están apretadas ahora mismo
        var teclado = Keyboard.GetState();

        // Escape cierra el juego
        if (teclado.IsKeyDown(Keys.Escape))
            Exit();

        // dt = segundos que pasaron desde el frame anterior (ej: 0.016).
        // Lo usamos para medir tiempos sin depender de qué tan rápida sea la PC.
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Si la rana está saltando, vamos restando tiempo al contador del salto
        if (_tiempoSalto > 0)
            _tiempoSalto -= dt;

        // Función local: devuelve true solo en el instante en que se APRIETA la tecla.
        // Es decir: ahora está apretada Y en el frame anterior no lo estaba.
        // Así, mantener la tecla apretada no hace saltar a la rana una y otra vez.
        bool Presionada(Keys tecla) =>
            teclado.IsKeyDown(tecla) && _tecladoAnterior.IsKeyUp(tecla);

        // Solo aceptamos un nuevo salto si ya terminó el anterior
        if (_tiempoSalto <= 0)
        {
            if (Presionada(Keys.Up) || Presionada(Keys.W))
            {
                _posicion.Y -= Casilla;            // Arriba: el eje Y disminuye
                _rotacion = 0f;                    // Sin rotar
                _tiempoSalto = DuracionSalto;      // Arranca el contador del salto
            }
            else if (Presionada(Keys.Down) || Presionada(Keys.S))
            {
                _posicion.Y += Casilla;            // Abajo: el eje Y aumenta
                _rotacion = MathHelper.Pi;         // Girada 180°
                _tiempoSalto = DuracionSalto;
            }
            else if (Presionada(Keys.Left) || Presionada(Keys.A))
            {
                _posicion.X -= Casilla;            // Izquierda: el eje X disminuye
                _rotacion = -MathHelper.PiOver2;   // Girada 90° a la izquierda
                _tiempoSalto = DuracionSalto;
            }
            else if (Presionada(Keys.Right) || Presionada(Keys.D))
            {
                _posicion.X += Casilla;            // Derecha: el eje X aumenta
                _rotacion = MathHelper.PiOver2;    // Girada 90° a la derecha
                _tiempoSalto = DuracionSalto;
            }
        }

        // La rana "se mueve" (usa la imagen saltando) mientras queda tiempo de salto
        _seMueve = _tiempoSalto > 0;

        // Evitamos que la rana se salga de la ventana.
        // Clamp limita un valor entre un mínimo y un máximo.
        // Restamos el tamaño de la imagen para que tampoco se pase por el borde derecho/inferior.
        _posicion.X = MathHelper.Clamp(_posicion.X, 0, GraphicsDevice.Viewport.Width - _ranaNormal.Width);
        _posicion.Y = MathHelper.Clamp(_posicion.Y, 0, GraphicsDevice.Viewport.Height - _ranaNormal.Height);

        // Guardamos el estado del teclado para compararlo en el próximo frame
        _tecladoAnterior = teclado;

        base.Update(gameTime);
    }

    // ---------- DRAW ----------
    // Se ejecuta unas 60 veces por segundo, después de Update. Acá solo se DIBUJA, sin lógica.
    protected override void Draw(GameTime gameTime)
    {
        // Pinta toda la pantalla de un color (borra lo del frame anterior)
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Elegimos qué imagen mostrar:
        // si _seMueve es true -> imagen saltando, si es false -> imagen normal
        Texture2D ranaActual = _seMueve ? _ranaSaltando : _ranaNormal;

        // Punto sobre el que gira la imagen: su centro.
        // Si no lo definimos, giraría alrededor de la esquina superior izquierda y se movería de lugar.
        Vector2 origen = new Vector2(ranaActual.Width / 2f, ranaActual.Height / 2f);

        // Como el "origen" ahora es el centro de la imagen, sumamos la mitad
        // del tamaño a la posición para que la rana quede en el mismo lugar.
        Vector2 posicionDibujo = _posicion + origen;

        // Begin avisa que vamos a empezar a dibujar.
        // PointClamp hace que las imágenes pixeladas no se vean borrosas al escalarlas.
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(
            ranaActual,                 // Imagen a dibujar
            posicionDibujo,             // Dónde dibujarla
            null,                       // Usar la imagen completa (sin recortar)
            Color.White,                // Sin cambiar colores
            _rotacion + AjusteImagen,   // Ángulo de rotación (según la dirección del salto)
            origen,                     // Centro de rotación
            1f,                         // Escala (1 = tamaño original)
            SpriteEffects.None,         // Sin espejar
            0f                          // Capa de dibujo
        );

        // End termina el dibujado y manda todo a la pantalla
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
