using System;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Represents an user type extension box.
    /// </summary>
    public static class ExtensionBox
    {
        /// <summary>
        /// Gets the next extension box from stream.
        /// </summary>
        /// <param name="reader">The box stream reader.</param>
        /// <param name="offset">The current box stream offset.</param>
        /// <param name="size">The box size.</param>
        /// <returns></returns>
        public static Box GetNextBoxFromStream(BoxBinaryReader reader, long offset, uint size)
        {
            Guid extendedType = reader.PeekEntensionType(size == 1 ? 8 : 0);

            switch (extendedType.ToString().ToUpper())
            {
                case "A2394F52-5A9B-4F14-A244-6C427C648DF4":
                    return new SampleEncryptionFullBox(offset, size);
                case "6D1D9B05-42D5-44E6-80E2-141DAFF757B2":
                    return new TrackFragmentExtendedHeaderBox(offset, size);
                //case "6B6840F2-5F24-4FC5-BA39-A51BCF0323F3": ???
                //case "D08A4F18-10F3-4A82-B6C8-32D8ABA183D3":
                //    return new ProtectionSystemSpecificHeaderFullBox(offset, size);
                default:
                    return new UnknownBox(offset, size);
                    //reader.GotoEndOfBox(offset, size);
                    //return null;
            }
        }
    }
}
