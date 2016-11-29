namespace Doobry.Features.QueryDeveloper
{
    public class Result
    {
        public Result(int row, string data)
        {
            DocumentId id;
            DocumentId.TryParse(data, out id);
            Id = id;

            Row = row;
            Data = data;
        }

        public DocumentId Id { get; }

        public int Row { get; }

        public string Data { get; }

    }
}