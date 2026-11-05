using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Sprite
{
    public Texture2D textura;
    public Vector2 posicion;
    public float rotacion;
    public Sprite(Texture2D textura, Vector2 posicion,float rotacion)
    {
        this.textura = textura;
        this.posicion = posicion;
        this.rotacion = rotacion;
    }
    
    public virtual void Update(GameTime gameTime) 
    { 
    }
    //hola
    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }
}
