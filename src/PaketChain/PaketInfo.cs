namespace PaketChain
{
    internal class PaketInfo
    {
        public string PaketPath { get; }
        public PaketType ToolType { get; }

        public PaketInfo(string paketPath, PaketType toolType)
        {
            PaketPath = paketPath;
            ToolType = toolType;
        }
    }
}