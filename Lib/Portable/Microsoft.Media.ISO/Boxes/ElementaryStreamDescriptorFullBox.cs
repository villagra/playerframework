using Microsoft.Media.ISO.Boxes.Descriptors;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Describes the base elements of an audio or video stream track
    /// </summary>
    public class ElementaryStreamDescriptorFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementaryStreamDescriptorFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public ElementaryStreamDescriptorFullBox(long offset, long size)
            : base(offset, size, BoxType.Esds)
        {

        }

        /// <summary>
        /// Gets the stream descriptor.
        /// </summary>
        public Microsoft.Media.ISO.Boxes.Descriptors.Descriptor StreamDescriptor { get; private set; }


        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            StreamDescriptor = Microsoft.Media.ISO.Boxes.Descriptors.Descriptor.GetNextDescriptorFromStream(reader);
        }
    }
}
