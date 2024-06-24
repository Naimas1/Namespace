namespace Namespace
{
    internal class Program
    {
        public static object update { get; private set; }
        public static object AddRatingToInfo { get; private set; }

        private static object Main(string[] args)
        {
            dotnet ef migrations add AddRatingToInfo;
}
    }

    internal class dotnet
    {
    }
}