using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeGameLevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Editor : Window
    {
        enum EditMode
        {
            Add,
            Move,
            Delete
        }
        EditMode editMode;
        Point mouseStartPoint;
        List<Rectangle> blocks;
        int blockIndex;
        Point blockStartPoint;
        int gridSize;

        public Editor()
        {
            InitializeComponent();
            editMode = EditMode.Add;
            blocks = new List<Rectangle>();
            blockIndex = -1;
            gridSize = 5;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (editMode == EditMode.Add)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    int width = (int)(e.GetPosition(canvas).X - mouseStartPoint.X);
                    int height = (int)(e.GetPosition(canvas).Y - mouseStartPoint.Y);
                    GetLastRect(canvas).Width = Math.Abs(width);
                    GetLastRect(canvas).Height = Math.Abs(height);
                    widthTextBox.Text = Math.Abs(width).ToString();
                    heightTextBox.Text = Math.Abs(height).ToString();
                    if (width < 0)
                    {
                        xPosTextBox.Text = e.GetPosition(canvas).X.ToString();
                        SetPosition(GetLastRect(canvas), e.GetPosition(canvas));
                    }
                    if (height < 0)
                    {
                        yPosTextBox.Text = e.GetPosition(canvas).Y.ToString();
                        SetPosition(GetLastRect(canvas), e.GetPosition(canvas));
                    }
                }
            }
            else if (editMode == EditMode.Move)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (blockIndex != -1)
                    {
                        Point mouseCurrentPoint = e.GetPosition(canvas);
                        Vector mouseDelta = mouseStartPoint - mouseCurrentPoint;
                        SetPosition(canvas.Children[blockIndex],
                            new Point(blockStartPoint.X - mouseDelta.X,
                                blockStartPoint.Y - mouseDelta.Y));
                    }
                }
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (editMode == EditMode.Add)
                {
                    mouseStartPoint = SnapToGrid(e.GetPosition(canvas));
                    Rectangle tmp = new System.Windows.Shapes.Rectangle();
                    tmp.Fill = new SolidColorBrush(Colors.White);
                    SetPosition(tmp, mouseStartPoint);
                    canvas.Children.Add(tmp);
                    blocks.Add(tmp);
                    xPosTextBox.Text = mouseStartPoint.X.ToString();
                    yPosTextBox.Text = mouseStartPoint.Y.ToString();
                }
                else if (editMode == EditMode.Move)
                {
                    if (canvas.Children.Count > 0)
                    {
                        blockIndex = -1;
                        for (int i = canvas.Children.Count - 1; i >= 0; i--)
                        {
                            if (Contains((Rectangle)canvas.Children[i], e.GetPosition(canvas)))
                            {
                                blockIndex = i;
                                mouseStartPoint = e.GetPosition(canvas);
                                blockStartPoint = GetPosition(canvas.Children[i]);
                                xPosTextBox.Text = GetPosition(canvas.Children[i]).X.ToString();
                                yPosTextBox.Text = GetPosition(canvas.Children[i]).Y.ToString();
                                widthTextBox.Text = ((Rectangle)canvas.Children[i]).Width.ToString();
                                heightTextBox.Text = ((Rectangle)canvas.Children[i]).Height.ToString();
                                break;
                            }
                        }
                    }
                }
            }
        }

        Point SnapToGrid(Point point)
        {
            int x = (int)point.X % gridSize;
            int y = (int)point.Y % gridSize;
            Point tmp = new Point();
            if (x <= (gridSize / 2 + gridSize % 2))
            {
                tmp.X = point.X - x;
            }
            else if (x > (gridSize / 2 + gridSize % 2))
            {
                tmp.X = point.X + (gridSize - x);
            }

            if (y <= (gridSize / 2 + gridSize % 2))
            {
                tmp.Y = point.Y - y;
            }
            else if (y > (gridSize / 2 + gridSize % 2))
            {
                tmp.Y = point.Y + (gridSize - y);
            }
            return tmp;
        }

        bool Contains(Rectangle element, Point point)
        {
            Point corner = GetPosition(element);
            return (point.X >= corner.X && point.X <= corner.X + element.Width &&
                point.Y >= corner.Y && point.Y <= corner.Y + element.Height);
        }

        void SetPosition(UIElement element, Point position)
        {
            position = SnapToGrid(position);
            Canvas.SetLeft(element, position.X);
            Canvas.SetTop(element, position.Y);
        }

        Point GetPosition(UIElement element)
        {
            Point tmp = new Point();
            tmp.X = Canvas.GetLeft(element);
            tmp.Y = Canvas.GetTop(element);
            return tmp;
        }

        Rectangle GetLastRect(Canvas canvas)
        {
            return (Rectangle)canvas.Children[canvas.Children.Count - 1];
        }

        private void addRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            editMode = EditMode.Add;
        }

        private void moveRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            editMode = EditMode.Move;
        }

        private void deleteRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            editMode = EditMode.Delete;
        }
    }
}
