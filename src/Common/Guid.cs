
namespace Penta.EeWin.Common
{
    public static class Guid
    {
        public const string GUID_FORMAT_NONE = "N";
        public const string GUID_FORMAT_HYPHENS = "D";
        public const string GUID_FORMAT_BRACES = "B";
        public const string GUID_FORMAT_PARENTHESES = "P";
        public const string GUID_FORMAT_HEX = "X";

        public static System.Guid motpCp { get; } = new System.Guid("EBB1B1BE-3091-406D-8F31-3987AC85BC71");
        public static System.Guid faceCp { get; } = new System.Guid("EBB1B1BE-3091-406D-8F31-3987AC85BC76");
        public static System.Guid qrCp { get; } = new System.Guid("EBB1B1BE-3091-406D-8F31-3987AC85BC7B");
        public static System.Guid simpleCp { get; } = new System.Guid("EBB1B1BE-3091-406D-8F31-3987AC85BC82");
    }
}
