namespace Doobry
{
    public class Result
    {
        public Result(int row, string data)
        {
            Row = row;
            Data = data;
        }

        public int Row { get; }

        public string Data { get; }
    }
}