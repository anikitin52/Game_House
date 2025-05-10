using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace Open_TK
{
    // Класс для работы с шейдерами
    public class Shader
    {
        public int shaderHandle;  // Идентификатор шейдерной программы 

        // Загрузка шейдера 
        public void LoadShader()
        {
            // Создание шейдерной программы 
            shaderHandle = GL.CreateProgram();

            // Загрузка вершинного шейдера
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource("shader.vert"));
            GL.CompileShader(vertexShader);

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success1);
            if (success1 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine(infoLog);
            }

            // Загрузка фрагментного шейдера
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource("shader.frag"));
            GL.CompileShader(fragmentShader);

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success2);
            if (success2 == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine(infoLog);
            }

            // Настройка шейдеров: Присоединение к основной программе и линковка 
            GL.AttachShader(shaderHandle, vertexShader);
            GL.AttachShader(shaderHandle, fragmentShader);
            GL.LinkProgram(shaderHandle);

            // Удаление ресурсов 
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        // Активация шейдерной программы 
        public void UseShader()
        {
            GL.UseProgram(shaderHandle);
        }

        // Удаление шейдерной программы
        public void DeleteShader()
        {
            GL.DeleteProgram(shaderHandle);
        }

        // Загрузка кода шейдера из файла 
        public static string LoadShaderSource(string filepath)
        {
            string shaderSource = "";
            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filepath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file: " + e.Message);
            }
            return shaderSource;
        }
    }

    // Основной класс игры 
    internal class Game : GameWindow
    {
        int width, height; // Размеры окна

        // Вершины
        List<Vector3> vertices = new List<Vector3>()
        {
        // Основной корпус
        // Передняя грань (стены)
        new Vector3(-0.75f, -1.0f, 0.5f),   // 0
        new Vector3(0.75f, -1.0f, 0.5f),    // 1
        new Vector3(0.75f, 1.0f, 0.5f),     // 2
        new Vector3(-0.75f, 1.0f, 0.5f),    // 3

        // Задняя грань
        new Vector3(-0.75f, -1.0f, -0.5f),  // 4
        new Vector3(0.75f, -1.0f, -0.5f),   // 5
        new Vector3(0.75f, 1.0f, -0.5f),    // 6
        new Vector3(-0.75f, 1.0f, -0.5f),   // 7

        // Левая грань
        new Vector3(-0.75f, -1.0f, -0.5f),  // 8
        new Vector3(-0.75f, -1.0f, 0.5f),   // 9
        new Vector3(-0.75f, 1.0f, 0.5f),    // 10
        new Vector3(-0.75f, 1.0f, -0.5f),   // 11

        // Правая грань
        new Vector3(0.75f, -1.0f, 0.5f),    // 12
        new Vector3(0.75f, -1.0f, -0.5f),   // 13
        new Vector3(0.75f, 1.0f, -0.5f),    // 14
        new Vector3(0.75f, 1.0f, 0.5f),     // 15

        // Пол (нижняя грань)
        new Vector3(-0.75f, -1.0f, 0.5f),   // 16
        new Vector3(0.75f, -1.0f, 0.5f),    // 17
        new Vector3(0.75f, -1.0f, -0.5f),   // 18
        new Vector3(-0.75f, -1.0f, -0.5f),  // 19

        // Потолок (верхняя грань)
        new Vector3(-0.75f, 1.0f, 0.5f),    // 20
        new Vector3(0.75f, 1.0f, 0.5f),     // 21
        new Vector3(0.75f, 1.0f, -0.5f),    // 22
        new Vector3(-0.75f, 1.0f, -0.5f),   // 23

        // Крыша (передний треугольник)
        new Vector3(-1.0f, 1.0f, 0.6f),     // 24
        new Vector3(1.0f, 1.0f, 0.6f),      // 25
        new Vector3(0.0f, 2.0f, 0.0f),      // 26 (верхушка)

        // Крыша (задний треугольник)
        new Vector3(-1.0f, 1.0f, -0.6f),    // 27
        new Vector3(1.0f, 1.0f, -0.6f),     // 28

        // Нижняя грань крыши (прямоугольник)
        new Vector3(-1.0f, 1.0f, 0.6f),     // 29 
        new Vector3(1.0f, 1.0f, 0.6f),      // 30 
        new Vector3(1.0f, 1.0f, -0.6f),     // 31 
        new Vector3(-1.0f, 1.0f, -0.6f),    // 32

        // Боковые грани крыши (левая и правая)
        new Vector3(-1.0f, 1.0f, 0.6f),     // 33
        new Vector3(-1.0f, 1.0f, -0.6f),    // 34 
        new Vector3(1.0f, 1.0f, 0.6f),      // 35
        new Vector3(1.0f, 1.0f, -0.6f),     // 36 


        // Дымоход
        new Vector3(0.3f, 1.0f, 0.2f),   // 37 (передняя нижняя правая)
        new Vector3(0.7f, 1.0f, 0.2f),    // 38 (передняя нижняя дальняя)
        new Vector3(0.7f, 1.8f, 0.2f),    // 39 (передняя верхняя дальняя)
        new Vector3(0.3f, 1.8f, 0.2f),   // 40 (передняя верхняя ближняя)
    
        new Vector3(0.3f, 1.0f, -0.2f),  // 41 (задняя нижняя ближняя)
        new Vector3(0.7f, 1.0f, -0.2f),   // 42 (задняя нижняя дальняя)
        new Vector3(0.7f, 1.8f, -0.2f),   // 43 (задняя верхняя дальняя)
        new Vector3(0.3f, 1.8f, -0.2f),  // 44 (задняя верхняя ближняя)
        };

        // Текстурные координаты
        List<Vector2> texCoords = new List<Vector2>()
        {
        // Основной корпус (6 граней)
        // Передняя, задняя, левая, правая
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),

        // Пол и потолок
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Пол
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Потолок

        // Передний треугольник крыши
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 1),

        // Задний треугольник крыши
        new Vector2(0, 0), new Vector2(1, 0),

        // Нижняя грань крыши
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),

        // Боковые грани
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 1),
        new Vector2(0, 0), new Vector2(1, 0),

        // Дымоход (6 граней)
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Перед
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Зад
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Верх
        };

        // Нормали
        List<Vector3> normals = new List<Vector3>()
        {
        // Основной корпус
        new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), // Перед
        new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), // Зад
        new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), // Левая
        new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), // Правая

        // Пол и потолок
        new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), // Пол (Y-)
        new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0),    // Потолок (Y+)

        // Крыша
        new Vector3(0, 0.6f, 0.8f), new Vector3(0, 0.6f, 0.8f), new Vector3(0, 0.6f, 0.8f), // Перед
        new Vector3(0, 0.6f, -0.8f), new Vector3(0, 0.6f, -0.8f), new Vector3(0, 0.6f, -0.8f), // Зад

        // Дымоход
        new Vector3(0,0,1), new Vector3(0,0,1), new Vector3(0,0,1), new Vector3(0,0,1),
        new Vector3(0,0,-1), new Vector3(0,0,-1), new Vector3(0,0,-1), new Vector3(0,0,-1),
        new Vector3(-1,0,0), new Vector3(-1,0,0), new Vector3(-1,0,0), new Vector3(-1,0,0),
        new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(1,0,0),
        new Vector3(0,1,0), new Vector3(0,1,0), new Vector3(0,1,0), new Vector3(0,1,0),
        new Vector3(0,-1,0), new Vector3(0,-1,0), new Vector3(0,-1,0), new Vector3(0,-1,0),
        new Vector3(0,0.8f,0.2f), // Нормаль, соответствующая наклону крыши
        new Vector3(0,0.8f,0.2f),
        new Vector3(0,0.8f,0.2f),
        new Vector3(0,0.8f,0.2f),
        };

        // Индексы
        uint[] indices =
        {
        // Основной корпус (6 граней)
        0,1,2, 2,3,0,             // Перед
        4,5,6, 6,7,4,             // Зад
        8,9,10, 10,11,8,          // Левая
        12,13,14, 14,15,12,       // Правая
        16,17,18, 18,19,16,       // Пол
        20,21,22, 22,23,20,       // Потолок

        //Крыша 
        24, 25, 26,              // Передний треугольник 
        27, 28, 26,              // Задний треугольник 
        29, 30, 31, 31, 32, 29,  // Нижняя грань
        33, 34, 26,              // Левая боковая грань
        35, 36, 26,              // Правая боковая грань

        // Дымоход (4 грани)
        37,38,39, 39,40,37,     // Передняя грань
        41,42,43, 43,44,41,     // Задняя грань
        37,41,44, 44,40,37,     // Левая грань
        38,42,43, 43,39,38,     // Правая грань
        40,39,43, 43,44,40,     // Верхняя грань
        37,38,42, 42,41,37      // Нижняя грань
        };

        int VAO;                                // Vertex Array Object
        int VBO;                                // Vertex Buffer Object
        int EBO;                                // Element Buffer Object

        Shader shaderProgram = new Shader();    // Шейдерная программа 

        // Текстуры
        int textureWalls;     // Стены
        int textureWood;      // Дерево (нужна для крыши, пола и потолка)
        int textureChimney;   // Дымоход

        // Камера и параметры управления 
        Camera camera;
        float xRot = 0f;                     // Угол вращения по Х
        float yRot = 0f;                     // Угол вращения по Y
        private int cubeDistance = 3;        // Дистанция до объекта 
        const float rotationSpeed = 0.5f;    // Скорость вращения 
        private float _brightness = 1.0f;    // Освещение 


        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(width, height));
            this.height = height;
            this.width = width;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Генерация и привязка VAO/ VBO / EBO
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            textureChimney = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureChimney);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);
            GL.BindVertexArray(0);

            // Компилирование и загрузка шейдеров
            shaderProgram.LoadShader();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Загрузка текстуры для стен (i.png)
            textureWalls = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureWalls);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult wallsTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/i.png"), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, wallsTexture.Width, wallsTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, wallsTexture.Data);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Загрузка тесктурв для крыши (other.png)
            textureWood = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureWood);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            ImageResult woodTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/wood.png"), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, woodTexture.Width, woodTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, woodTexture.Data);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Загрузка текстуры для дымохода
            textureChimney = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, textureChimney);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult chimneyTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/rock.png"), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, chimneyTexture.Width, chimneyTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, chimneyTexture.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Включение теста глубины
            GL.Enable(EnableCap.DepthTest);

            // Инициализация камеры
            camera = new Camera(width, height, Vector3.Zero);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteTexture(textureWalls);
            GL.DeleteTexture(textureWood);

            shaderProgram.DeleteShader();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            // Очистка буфера цвета и глубины
            GL.ClearColor(0.3f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Активация шейдера
            shaderProgram.UseShader();

            // Создание матрицы модели (вращение + позиция)
            Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(xRot)) *
                           Matrix4.CreateRotationY(MathHelper.DegreesToRadians(yRot));
            model *= Matrix4.CreateTranslation(0f, 0f, -cubeDistance);

            // Получение матриц вида и проекции из камеры
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjection();

            // Передача матриц в шейдер
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram.shaderHandle, "model"), true, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram.shaderHandle, "view"), true, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram.shaderHandle, "projection"), true, ref projection);

            // Настройка освещения
            Vector3 lightPos = camera.position + new Vector3(2f, 2f, 0f);
            GL.Uniform3(GL.GetUniformLocation(shaderProgram.shaderHandle, "lightPos"), lightPos);
            // Передача яркости освещения
            GL.Uniform1(GL.GetUniformLocation(shaderProgram.shaderHandle, "lightIntensity"), _brightness);

            // Привязка VAO и EBO
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            // Отрисовка стен
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureWalls);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram.shaderHandle, "texture0"), 0);
            GL.DrawElements(PrimitiveType.Triangles, 24, DrawElementsType.UnsignedInt, 0);

            // Отрисовка пола и потолка
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureWood);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram.shaderHandle, "texture0"), 1);
            GL.DrawElements(PrimitiveType.Triangles, 12, DrawElementsType.UnsignedInt, 24 * sizeof(uint));

            // Отрисовка крыши
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureWood);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram.shaderHandle, "textureWood"), 1);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 36 * sizeof(uint));
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 42 * sizeof(uint));
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 48 * sizeof(uint));



            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureWood);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram.shaderHandle, "texture0"), 1);
            GL.DrawElements(PrimitiveType.Triangles, 5, DrawElementsType.UnsignedInt, 24 * sizeof(uint));

            // Отрисовка дымохода (с заданной позицией относительно дома)
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, textureChimney);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram.shaderHandle, "texture0"), 2);
            float heightCorrection = 0.6f * 0.1f; 
            Matrix4 chimneyModel = model *
                Matrix4.CreateTranslation(
                    0.6f + 10f,
                    0.4f + heightCorrection,
                    0.4f
                );
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 54 * sizeof(uint));

            // Обмен буферов и завершение рендера
            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _brightness = Math.Clamp(_brightness + e.OffsetY * 0.1f, 0.0f, 3.0f);
            base.OnMouseWheel(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            KeyboardState input = KeyboardState;

            // Закрытие игры при нажатии Esc
            if (input.IsKeyDown(Keys.Escape))
                Close();

            // Обработка вращения клавишами WASD
            if (input.IsKeyDown(Keys.W))
                xRot += rotationSpeed;
            if (input.IsKeyDown(Keys.S))
                xRot -= rotationSpeed;
            if (input.IsKeyDown(Keys.A))
                yRot += rotationSpeed;
            if (input.IsKeyDown(Keys.D))
                yRot -= rotationSpeed;

            base.OnUpdateFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }
    }
}