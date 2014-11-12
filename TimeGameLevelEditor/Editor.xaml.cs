using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

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
        int blockIndex;
        Point blockStartPoint;
        int gridSize;
        Ellipse pointer;
        ObservableCollection<string> actions;

        bool topHandle;
        bool leftHandle;
        bool bottomHandle;
        bool rightHandle;

        public Editor()
        {
            InitializeComponent();
            editMode = EditMode.Add;
            blockIndex = -1;
            gridSize = 5;
            topHandle = false;
            leftHandle = false;
            bottomHandle = false;
            rightHandle = false;
            actions = new ObservableCollection<string>();
            pointer = new Ellipse();
            pointer.Fill = new SolidColorBrush(Colors.Red);
            pointer.Width = 7;
            pointer.Height = 7;
            SetPosition(pointer, new Point(0, 0));
            canvas.Children.Add(pointer);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            ClearOutlines();
            if (editMode == EditMode.Add)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentMousePosition = SnapToGrid(e.GetPosition(canvas));
                    SetPointerPosition(currentMousePosition);
                    int width = (int)(currentMousePosition.X - mouseStartPoint.X);
                    int height = (int)(currentMousePosition.Y - mouseStartPoint.Y);
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
                OutlineBlocks(e);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (blockIndex != -1)
                    {
                        Point mouseCurrentPoint = e.GetPosition(canvas);
                        Vector mouseDelta = mouseStartPoint - mouseCurrentPoint;
                        if (!topHandle && !leftHandle && !bottomHandle && !rightHandle)
                        {
                            SetPosition(canvas.Children[blockIndex],
                                new Point(blockStartPoint.X - mouseDelta.X,
                                    blockStartPoint.Y - mouseDelta.Y));
                        }
                        else
                        {
                            if (topHandle)
                            {
                                Point blockPosition = GetPosition(canvas.Children[blockIndex]);
                                SetPosition(canvas.Children[blockIndex], new Point(blockPosition.X,
                                    blockStartPoint.Y - mouseDelta.Y));
                                ((Rectangle)canvas.Children[blockIndex]).Height += blockStartPoint.Y - mouseDelta.Y;
                            }
                        }
                    }
                }
                else if (e.LeftButton == MouseButtonState.Released)
                {
                    topHandle = false;
                    leftHandle = false;
                    bottomHandle = false;
                    rightHandle = false;
                }
            }
            else if (editMode == EditMode.Delete)
            {
                OutlineBlocks(e);
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gridSizeTextBox.IsFocused)
            {
                gridSizeTextBox_LostFocus(null, null);
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (editMode == EditMode.Add)
                {
                    mouseStartPoint = SnapToGrid(e.GetPosition(canvas));
                    SetPointerPosition(mouseStartPoint);
                    Rectangle tmp = new Rectangle();
                    tmp.Fill = new SolidColorBrush(Colors.White);
                    SetPosition(tmp, mouseStartPoint);
                    canvas.Children.Add(tmp);
                    xPosTextBox.Text = mouseStartPoint.X.ToString();
                    yPosTextBox.Text = mouseStartPoint.Y.ToString();
                    actions.Add("Added block " + (canvas.Children.Count - 1));
                }
                else if (editMode == EditMode.Move)
                {
                    if (canvas.Children.Count > 0)
                    {
                        blockIndex = -1;
                        for (int i = canvas.Children.Count - 1; i >= 1; i--)
                        {
                            if (Contains((Rectangle)canvas.Children[i], e.GetPosition(canvas)))
                            {
                                blockIndex = i;
                                List<Rect> handleZones = GetRectangleHandles();
                                mouseStartPoint = e.GetPosition(canvas);
                                for (int j = 0; j < 4; j++)
                                {
                                    if (handleZones[j].Contains(mouseStartPoint))
                                    {
                                        if (j == 0)
                                        {
                                            topHandle = true;
                                        }
                                        else if (j == 1)
                                        {
                                            leftHandle = true;
                                        }
                                        else if (j == 2)
                                        {
                                            bottomHandle = true;
                                        }
                                        else if (j == 3)
                                        {
                                            rightHandle = true;
                                        }
                                    }
                                }
                                blockStartPoint = GetPosition(canvas.Children[i]);
                                xPosTextBox.Text = GetPosition(canvas.Children[i]).X.ToString();
                                yPosTextBox.Text = GetPosition(canvas.Children[i]).Y.ToString();
                                widthTextBox.Text = ((Rectangle)canvas.Children[i]).Width.ToString();
                                heightTextBox.Text = ((Rectangle)canvas.Children[i]).Height.ToString();
                                actions.Add("Moved block " + i);
                                break;
                            }
                        }
                    }
                }
                else if (editMode == EditMode.Delete)
                {
                    if (canvas.Children.Count > 0)
                    {
                        blockIndex = -1;
                        for (int i = canvas.Children.Count - 1; i >= 1; i--)
                        {
                            if (Contains((Rectangle)canvas.Children[i], e.GetPosition(canvas)))
                            {
                                actions.Add("Deleted block " + i);
                                canvas.Children.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        List<Rect> GetRectangleHandles()
        {
            List<Rect> handleZones = new List<Rect>();
            Rectangle rectangle = (Rectangle)canvas.Children[blockIndex];
            Point rectanglePosition = GetPosition(rectangle);
            Rect top = new Rect(rectanglePosition.X, rectanglePosition.Y, rectangle.Width, 10);
            Rect left = new Rect(rectanglePosition.X, rectanglePosition.Y, 10, rectangle.Height);
            Rect bottom = new Rect(rectanglePosition.X, rectanglePosition.Y + rectangle.Height - 10, rectangle.Width, 10);
            Rect right = new Rect(rectanglePosition.X + rectangle.Width - 10, rectanglePosition.Y, 10, rectangle.Height);
            handleZones.Add(top);
            handleZones.Add(left);
            handleZones.Add(bottom);
            handleZones.Add(right);
            return handleZones;
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

        void ClearOutlines()
        {
            for (int i = 1; i < canvas.Children.Count; i++)
            {
                ((Rectangle)canvas.Children[i]).StrokeThickness = 0;
            }
        }

        void OutlineBlocks(MouseEventArgs e)
        {
            if (canvas.Children.Count > 0)
            {
                blockIndex = -1;
                for (int i = canvas.Children.Count - 1; i >= 1; i--)
                {
                    if (Contains((Rectangle)canvas.Children[i], e.GetPosition(canvas)))
                    {
                        blockIndex = i;
                        ((Rectangle)canvas.Children[i]).StrokeThickness = 5;
                        ((Rectangle)canvas.Children[i]).Stroke = new SolidColorBrush(Colors.Black);
                        break;
                    }
                }
            }
        }

        void ClearScreen()
        {
            for (int i = 1; i < canvas.Children.Count; i++)
            {
                canvas.Children.RemoveAt(i);
                i--;
            }
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

        void SetPointerPosition(Point position)
        {
            position = SnapToGrid(position);
            Canvas.SetLeft(pointer, position.X - 3.5);
            Canvas.SetTop(pointer, position.Y - 3.5);
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
            if (canvas.Children.Count > 1)
            {
                return (Rectangle)canvas.Children[canvas.Children.Count - 1];
            }
            else
            {
                return null;
            }
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

        private void gridSizeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int gridSizeTmp = int.Parse(gridSizeTextBox.Text);
                if (gridSizeTmp < 0)
                {
                    MessageBox.Show("The grid size must be positive");
                    gridSizeTextBox.Text = gridSize.ToString();
                    return;
                }
                gridSizeTextBox.Text = gridSizeTmp.ToString();
                gridSize = gridSizeTmp;
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                gridSizeTextBox.Text = gridSize.ToString();
                MessageBox.Show("The grid size must be a number");
            }
        }

        private void actionsListView_Loaded(object sender, RoutedEventArgs e)
        {
            actionsListView.ItemsSource = actions;
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "JSON files (.json)|*.json*";
            if (openFileDialog.ShowDialog() == true)
            {
                ClearScreen();
                StreamReader streamReader = new StreamReader(File.Open(openFileDialog.FileName, FileMode.Open));
                JArray blocksData = JArray.Parse(streamReader.ReadToEnd());
                foreach (JObject blockData in blocksData)
                {
                    Rectangle tmp = new Rectangle();
                    tmp.Width = (float)blockData["width"];
                    tmp.Height = (float)blockData["height"];
                    tmp.Fill = new SolidColorBrush(Colors.White);
                    Point blockPosition = new Point((float)blockData["x"], (float)blockData["y"]);
                    SetPosition(tmp, blockPosition);
                    canvas.Children.Add(tmp);
                }
                streamReader.Close();
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.Filter = "JSON files (.json)|*.json*";
            if (saveFileDialog.ShowDialog() == true)
            {
                StreamWriter streamWriter = new StreamWriter(File.Create(saveFileDialog.FileName));
                JArray blocksData = new JArray();
                for (int i = 1; i < canvas.Children.Count; i++)
                {
                    JObject blockData = new JObject();
                    Point blockPosition = GetPosition(canvas.Children[i]);
                    blockData["x"] = (float)blockPosition.X;
                    blockData["y"] = (float)blockPosition.Y;
                    blockData["width"] = (float)((Rectangle)canvas.Children[i]).Width;
                    blockData["height"] = (float)((Rectangle)canvas.Children[i]).Height;
                    blocksData.Add(blockData);
                }
                streamWriter.Write(blocksData.ToString());
                streamWriter.Close();
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearScreen();
        }
    }
}
