using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

static public class Ex
{
    // http://stackoverflow.com/a/28873998/847349
    public static IObservable<TResult> CombineWithPrevious<TSource, TResult>(
this IObservable<TSource> source,
Func<TSource, TSource, TResult> resultSelector)
    {
        return source.Scan(
            Tuple.Create(default(TSource), default(TSource)),
            (previous, current) => Tuple.Create(previous.Item2, current))
            .Select(t => resultSelector(t.Item1, t.Item2));
    }
}

class Program
{
    public class MyPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
        }
    }

    // http://stackoverflow.com/a/28873952/847349
    static void Main(string[] args)
    {
        List<MyPoint> points = new List<MyPoint>();

        for (int i = 0; i < 10; i++)
        {
            points.Add(new MyPoint { X = i, Y = i * 10 });
        }

        IObservable<MyPoint> pointObservable = points.ToObservable();

        //var res = pointObservable.Zip(
        //    pointObservable.Skip(1),
        //    (p1, p2) => new { A = p1, B = p2 }
        //);

        //var res = pointObservable
        //    .CombineWithPrevious((p1, p2) => new { A = p1, B = p2 })
        //    .Skip(1);

        var res = pointObservable
            .Publish(po =>
                po.Zip(
                    po.Skip(1),
                    (p1, p2) => new { A = p1, B = p2 }
                )
            );

        res.Subscribe(Console.WriteLine);
    }
}
