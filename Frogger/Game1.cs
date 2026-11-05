// "using" le dice a C# qué librerías vamos a usar en este archivo
using System;                           // Para AppContext (saber dónde está el ejecutable)
using System.IO;                        // Para leer archivos (File, Path)
using Microsoft.Xna.Framework;          // Clases base de MonoGame (Game, Vector2, Color, GameTime...)
using Microsoft.Xna.Framework.Graphics; // Para dibujar (Texture2D, SpriteBatch)
using Microsoft.Xna.Framework.Input;    // Para leer el teclado (Keyboard, Keys)

namespace Frogger
{  // Tiene que coincidir con el que usás en Program.cs (new Frogger.Game1())
    
    // Variable enum la cual va a guardar los distintos tipos de pantallas
    public enum PantallasJuego { MenuPrincipal }

    
    public class Game1 : Game
    {
        // ---------- VARIABLES DEL JUEGO ----------
        // Pantalla en la que empieza el juego
        PantallasJuego PantallaActual = PantallasJuego.MenuPrincipal;
        public Game1()
        {

            Content.RootDirectory = "Content"; // Carpeta del Content Pipeline (no la usamos para las imágenes)
            IsMouseVisible = true;             // Muestra el cursor del mouse sobre la ventana
        }

        // ---------- INITIALIZE ----------
        // Se ejecuta una vez, antes de empezar. Acá se prepara el estado inicial.
        protected override void Initialize()
        {


        }

        // ---------- LOADCONTENT ----------
        // Se ejecuta una vez, y es donde se cargan imágenes, sonidos, etc.
        protected override void LoadContent()
        {



        }

        // Función ayudante: carga una imagen de la carpeta Assets y la devuelve como textura.
        // Así no repetimos el mismo código para cada imagen.


        // ---------- UPDATE ----------
        // Se ejecuta unas 60 veces por segundo. Acá va la LÓGICA del juego (mover, colisiones, etc.)
        protected override void Update(GameTime gameTime)
        {

        }

        // ---------- DRAW ----------
        // Se ejecuta unas 60 veces por segundo, después de Update. Acá solo se DIBUJA, sin lógica.
        protected override void Draw(GameTime gameTime)
        {



            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
