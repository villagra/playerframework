
namespace Microsoft.Media.ISO.Boxes.Descriptors
{
    /// <summary>
    /// Enumerates the possible values for a <see cref="Descriptor"/> tag.
    /// </summary>
    public enum DescriptorTag
    {
        DECODER_CONFIG = 4,
        DECODER_SPECIFIC_INFO = 5,
        ES = 3,
        ES_ID_INC = 14,
        ES_ID_REF = 15,
        IOD = 2,
        IPMP_DESCRIPTOR = 11,
        IPMP_DESCRIPTOR_POINTER = 10,
        MP4_IOD = 0x10,
        MP4_OD = 0x11,
        OD = 1,
        SL_CONFIG = 6
    }
}
