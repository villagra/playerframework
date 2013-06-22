
namespace Microsoft.Media.ISO.Boxes
{
    public class AVCNALBox : Box
    {
        public AVCNALBox(long offset, long size)
            : base(offset, size, BoxType.Avcn)
        {
        }

        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
