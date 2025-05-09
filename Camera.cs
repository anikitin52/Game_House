using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Open_TK
{
    internal class Camera
    {
        // Параметры камеры
        private float SCREENWIDTH;         // Ширина окна  
        private float SCREENHEIGHT;        // Высота окна 

        public Vector3 position;           // Позиция камеры в пространстве

        // Векторы ориентации камеры
        Vector3 up = Vector3.UnitY;        // Вектор "вверх" (по оси Y)
        Vector3 front = -Vector3.UnitZ;    // Вектор "вперед" (по отрицательной оси Z)

        // Конструктор камеры. Фиксация начальной позиции
        public Camera(int width, int height, Vector3 position)
        {
            SCREENWIDTH = width;
            SCREENHEIGHT = height;
            this.position = position;
            this.position.Z = 1f;
        }

        // Создание матрицы вида
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }

        // Создание матрицы перспективной проекции
        // для 3D рендеринга
        public Matrix4 GetProjection()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), SCREENWIDTH / SCREENHEIGHT, 0.1f, 100f);
        }
    }
}