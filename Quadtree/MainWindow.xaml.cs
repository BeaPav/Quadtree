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
using System.Diagnostics;



public class node
{
    public bool isOccupied;
    public bool hasSons;
    public bool PointInRect = false;

    public int level;
    public Point RLcorner;
    public Point LUcorner;
    public Point Middle;
    

    private List<node> sons;
    private Point P = new Point(-1,-1);
    private int maxLevel = 6;
    



    public node(int _level, Point RL, Point LU) //constructor
    {
        level = _level;
        RLcorner = RL;
        LUcorner = LU;
        Middle = new Point((LUcorner.X + RLcorner.X) / 2, (LUcorner.Y + RLcorner.Y) / 2);

        isOccupied = false;
        hasSons = false;
    }




    public node getSon(int i)
    {
        return sons[i];
    }

    public bool createSons(int a, int b)
    {
        if (level < maxLevel - 1)
        {

            hasSons = true;
            sons = new List<node>();



            sons.Add(new node(level + 1, Middle, LUcorner));
            sons.Add(new node(level + 1, new Point(RLcorner.X, Middle.Y), new Point(Middle.X, LUcorner.Y)));
            sons.Add(new node(level + 1, RLcorner, Middle));
            sons.Add(new node(level + 1, new Point(Middle.X, RLcorner.Y), new Point(LUcorner.X, Middle.Y)));



            return true;

        }
        else if (level == maxLevel - 1)
        {
            if (a != b)             //a, b vravia o kvadrantoch, v ktorých by boli body synov,
                                    //táto podmienka zabráni rozdeleniu bunky, ak bod nakoniec nepridáme 
            {
                hasSons = true;
                sons = new List<node>();



                sons.Add(new node(level + 1, Middle, LUcorner));
                sons.Add(new node(level + 1, new Point(RLcorner.X, Middle.Y), new Point(Middle.X, LUcorner.Y)));
                sons.Add(new node(level + 1, RLcorner, Middle));
                sons.Add(new node(level + 1, new Point(Middle.X, RLcorner.Y), new Point(LUcorner.X, Middle.Y)));



                return true;
            }
            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }

    }







    public Point getPoint()
    {
        return P;
    }

    public void setPoint(Point _P)
    {
        isOccupied = true;
        P = _P;
    }
    
   
}









class tree
{
    public node N;

    public tree(node M) //constructor
    {
        N = M;
    }


    public void addPoint(Point L, node _N)
    {
        if (!(_N.isOccupied))
        {
            _N.setPoint(L);
            
        }
        else //isOccupied == true
        { 
            if (!(_N.hasSons))
                
            {
                
                Point M = _N.getPoint();
                int Mj,Lj;

                //zistim kam by patril uz ulozeny bod
                if (M.X < _N.Middle.X & M.Y < _N.Middle.Y)               // son0
                {
                    Mj = 0;
                }
                else if (M.X >= _N.Middle.X & M.Y < _N.Middle.Y)         // son1
                {
                    Mj = 1;
                }
                else if (M.X >= _N.Middle.X & M.Y >= _N.Middle.Y)        // son2
                {
                    Mj = 2;
                }
                else                                                      // son3
                {
                    Mj = 3;
                }

                //zistim kam by patril pridavany bod L
                if (L.X < _N.Middle.X & L.Y < _N.Middle.Y)               // son0
                {
                    Lj = 0;
                }
                else if (L.X >= _N.Middle.X & L.Y < _N.Middle.Y)         // son1
                {
                    Lj = 1;
                }
                else if (L.X >= _N.Middle.X & L.Y >= _N.Middle.Y)        // son2
                {
                    Lj = 2;
                }
                else                                                      // son3
                {
                    Lj = 3;
                }


                //test ci mozem delit - podla max urovne
                bool b = _N.createSons(Lj,Mj);

                if (b)  
                {


                    //upracem najprv povodny bod do podnode-u
                    _N.getSon(Mj).setPoint(M);


                    //idem umiestnovat bod ktory mi prisiel na vstupe
                    addPoint(L, _N.getSon(Lj));
                }

            }

            else // _N.hasSons == true
            {
                
                //rovnako testujeme synov
                if (L.X < _N.Middle.X & L.Y < _N.Middle.Y)                 
                {
                    addPoint(L, _N.getSon(0));
                }
                else if (L.X >= _N.Middle.X & L.Y < _N.Middle.Y)            
                {
                    addPoint(L, _N.getSon(1));
                }
                else if (L.X >= _N.Middle.X & L.Y >= _N.Middle.Y)           
                {
                    addPoint(L, _N.getSon(2));
                }
                else                                                        
                {
                    addPoint(L, _N.getSon(3));
                }
            }
        }
    }



}













namespace Quadtree
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        
        node activePointNode;
        bool activePoint = false;
        node N;
        tree T;
        bool drawingRectangle = false;
        Rectangle Rect;
        Point RectDown;
        Point RectMove;
        



        public MainWindow()
        {
            InitializeComponent();
            N = new node(0, new Point(C.Width,C.Height), new Point(0, 0));
            T = new tree(N);
            Rect = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(100, 0, 200, 150)),
                Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 250, 100)),
                StrokeThickness = 0.5,

            };

        }


        //vykresľovanie bodov zo stromu
        public void DrawPoints(node M)
        {
            if (M.isOccupied)
            {
                if (M.hasSons)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        DrawPoints(M.getSon(i));
                    }
                }

                else
                {
                    Point V = M.getPoint();

                    Ellipse E = new Ellipse
                    {
                        Fill = new SolidColorBrush(Colors.Black),
                        Width = 4,
                        Height = 4
                    };

                    E.MouseRightButtonDown += new MouseButtonEventHandler(PointClick);

                    if (M == activePointNode & activePoint == true)     //ak máme označený bod
                    {
                        E.Fill = new SolidColorBrush(Colors.Red);
                    }

                    else if (M.PointInRect & drawingRectangle)      //ak máme body pod obdĺždnikom
                    {
                        E.Fill = new SolidColorBrush(Colors.Aquamarine);
                    }


                    Canvas.SetLeft(E, V.X - 2);
                    Canvas.SetTop(E, V.Y - 2);
                    Canvas.SetZIndex(E, 50);

                    C.Children.Add(E);

                }
            }
        }


        // zistujeme ci ideme oznacit bod
        private void PointClick(object sender, MouseButtonEventArgs e)
        {
            PointTest(T.N, e);

        }

        public void PointTest(node M, MouseButtonEventArgs e)
        {
            if (M.isOccupied)
            {
                if (M.hasSons)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        PointTest(M.getSon(i), e);
                    }
                }
                else
                {
                    Point H = M.getPoint();

                    if ((H.X - 2 < e.GetPosition(C).X) & (H.X + 2 > e.GetPosition(C).X) &
                        (H.Y - 2 < e.GetPosition(C).Y) & (H.Y + 2 > e.GetPosition(C).Y))
                    {
                        activePoint = true;
                        activePointNode = M;
                        DrawPoints(T.N);

                        //vypisat cestu
                        Path.Content = WritePath(T.N, H);

                    }
                   

                }
            }
            
        }









        //vykresľovanie rozdeľujúcich čiar
        public void DrawLines(node _N)
            {
            if (_N.hasSons)
            {
                Line L1 = new Line  //horizontálna
                {
                    X1 = _N.Middle.X,
                    Y1 = _N.LUcorner.Y,
                    X2 = _N.Middle.X,
                    Y2 = _N.RLcorner.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(Convert.ToByte((200 * _N.level) % 255),
                                                               Convert.ToByte((60 * _N.level) % 255),
                                                               Convert.ToByte((80* _N.level) % 255))),
                    StrokeThickness = 1
                };

                Line L2 = new Line  //vertikálna
                {
                    X1 = _N.LUcorner.X,
                    Y1 = _N.Middle.Y,
                    X2 = _N.RLcorner.X,
                    Y2 = _N.Middle.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(Convert.ToByte((200 * _N.level) % 255),
                                                               Convert.ToByte((60 * _N.level) % 255),
                                                               Convert.ToByte((80 * _N.level) % 255))),
                    StrokeThickness = 1
                };

                C.Children.Add(L1);
                C.Children.Add(L2);

                for (int i = 0; i<4; i++)
                {
                    DrawLines(_N.getSon(i));
                }

            }
            
        }











       
        //vypísanie cesty označeného vrchola
        public string WritePath(node M, Point P)
        {
            if (M.isOccupied)
            {
                if (M.hasSons)
                {
                    if (P.X < M.Middle.X & P.Y < M.Middle.Y)               // son0
                    {
                        return "0, " + WritePath(M.getSon(0), P);
                    }
                    else if (P.X >= M.Middle.X & P.Y < M.Middle.Y)         // son1
                    {
                        return "1, " + WritePath(M.getSon(1), P);
                    }
                    else if (P.X >= M.Middle.X & P.Y >= M.Middle.Y)        // son2
                    {
                        return "2, " + WritePath(M.getSon(2), P);
                    }
                    else                                                   // son3
                    {
                        return "3, " + WritePath(M.getSon(3), P);
                    }
                }
                else
                {
                    if (P == M.getPoint())
                    {
                        return "Point";
                    }
                }
            }
            
            return "";
            
            
        }



        //prienik obdĺždnika a bunky
        public bool Intersection(node M)
        {
            Point L = new Point(Canvas.GetLeft(Rect), Canvas.GetTop(Rect));     //Ľavý horný roh obdĺždnika
            Point R = new Point(L.X + Rect.Width, L.Y + Rect.Height);           //Pravý dolný roh obdĺždnika
            
            if (R.X < M.LUcorner.X)          //obdĺždnik je vľavo
            {
                return false;
            }
            else if (L.X > M.RLcorner.X)    //obdĺždnik je vpravo
            {
                return false;
            }
            else if (R.Y < M.LUcorner.Y)    //obdĺždnik je nad bunkou
            {
                return false;
            }
            else if (L.Y > M.RLcorner.Y)    //obdĺždnik je pod bunkou
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //označenie bodov v obdĺždniku
        public void FindPointsInRect(node M)
        {
            if (M.isOccupied & Intersection(M))
            {
                if (M.hasSons)
                {
                    for (int i = 0; i<4; i++)
                    {
                        FindPointsInRect(M.getSon(i));
                    }
                }
                else
                {
                    Point L = new Point(Canvas.GetLeft(Rect), Canvas.GetTop(Rect));     //Ľavý horný roh obdĺždnika
                    Point R = new Point(L.X + Rect.Width, L.Y + Rect.Height);           //Pravý dolný roh obdĺždnika
                    Point V = M.getPoint();                                             //Bod v bunke

                    if (L.X <= V.X & R.X >= V.X & L.Y <= V.Y & R.Y >= V.Y)
                    {
                        M.PointInRect = true;
                    }
                }


            }
        }

        public void DeactivatePointsInRect(node M)
        {
            if (M.isOccupied)
            {
                if (M.hasSons)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        DeactivatePointsInRect(M.getSon(i));
                    }
                }
                else
                {
                    M.PointInRect = false;
                }
            }
        }




















        //pridanie bodu po kliknutí
        private void C_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            activePoint = false;
            Path.Content = "";

           
            T.addPoint(e.GetPosition(C), T.N);
            
            
            C.Children.Clear();
            DrawPoints(T.N);
            DrawLines(T.N);
        }


        //delete
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            
            C.Children.Clear();
            activePoint = false;
            Path.Content = "";

            N = new node(0, new Point(C.Width, C.Height), new Point(0, 0));
            T = new tree(N);
        }




        //pridanie náhodných bodov do existujúceho stromu
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            
            var rnd = new Random();
            Point P;

            for (int i = 0; i < 20; i++)
            {
                P = new Point(rnd.NextDouble()*C.ActualWidth, rnd.NextDouble()*C.ActualHeight);
                T.addPoint(P, T.N);
            }

            C.Children.Clear();
            DrawPoints(T.N);
            DrawLines(T.N);
        }

        //vygenerovanie nového stromu
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
           

            N = new node(0, new Point(C.Width, C.Height), new Point(0, 0));
            T = new tree(N);


            var rnd = new Random();
            Point P;

            for (int i = 0; i < 100; i++)
            {
                P = new Point(rnd.NextDouble() * C.ActualWidth, rnd.NextDouble() * C.ActualHeight);
                T.addPoint(P, T.N);
            }

            C.Children.Clear();
            DrawPoints(T.N);
            DrawLines(T.N);
        }








        //obdĺždnik
        

        //zaznamenanie prvého kliku
        private void C_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                drawingRectangle = true;
                RectDown = new Point(e.GetPosition(C).X, e.GetPosition(C).Y);
            }



        }

        private void C_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawingRectangle)
            {
                RectMove = new Point(e.GetPosition(C).X, e.GetPosition(C).Y);
                

                Rect.Height = Math.Abs(RectDown.Y - RectMove.Y);
                Rect.Width = Math.Abs(RectDown.X - RectMove.X);
                

                if (RectDown.X < RectMove.X)    //nastavenie ľavého horného a pravého spodného bodu obdĺždnika podľa toho ako ťaháme myšou
                {
                    if (RectDown.Y < RectMove.Y)
                    {
                        Canvas.SetLeft(Rect, RectDown.X);
                        Canvas.SetTop(Rect, RectDown.Y);
                    }
                    else 
                    {
                        Canvas.SetLeft(Rect, RectDown.X);
                        Canvas.SetTop(Rect, RectMove.Y);
                    
                    }
                }
                else
                {
                    if (RectDown.Y < RectMove.Y)
                    {
                        Canvas.SetLeft(Rect,RectMove.X);
                        Canvas.SetTop(Rect, RectDown.Y);
                    }
                    else
                    {
                        Canvas.SetLeft(Rect, RectMove.X);
                        Canvas.SetTop(Rect, RectMove.Y);
                    }
                }


                C.Children.Clear();
                C.Children.Add(Rect);

                DrawLines(T.N);

                DeactivatePointsInRect(T.N);
                FindPointsInRect(T.N);
                DrawPoints(T.N);
                
            }

            
             
            
        }

        private void C_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            C.Children.Remove(Rect);
            drawingRectangle = false;
            DeactivatePointsInRect(T.N);
            DrawPoints(T.N);
            activePoint = false;

        }
    }



}
