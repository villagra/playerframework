using System;
using System.IO;
using System.Text;
using Microsoft.Media.ISO.Boxes;
using System.Collections.Generic;

namespace Microsoft.Media.ISO
{
    /// <summary>
    /// Implements a box binary stream reader
    /// </summary>
    public class BoxBinaryReader : BinaryReader
    {
        /// <summary>
        /// Field that maintains the mfra offset value. This value indicates the offset
        /// of the mfra box from the end of the stream.
        /// </summary>
        private uint mfraOffset = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxBinaryReader"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        public BoxBinaryReader(Stream inputStream)
            : base(inputStream, Encoding.UTF8)
        { }

        /// <summary>
        /// Gets the current offset of the stream.
        /// </summary>
        public long Offset { get { return this.BaseStream.Position; } }

        /// <summary>
        /// Tells whether this <see cref="BoxBinaryReader"/> is at end of stream.
        /// </summary>
        /// <value>
        ///   <c>true</c> if at end of stream; otherwise, <c>false</c>.
        /// </value>
        public bool AtEndOfStream { get { return this.BaseStream.Position == this.BaseStream.Length; } }

        /// <summary>
        /// Reads the size of the box.
        /// </summary>
        /// <returns></returns>
        public uint ReadBoxSize()
        {
            return this.ReadUInt32();
        }

        /// <summary>
        /// Reads the box type from the stream and converts it to a strongly
        /// typed BoxType.
        /// </summary>
        /// <returns>Returns the type of box the stream is at; otherwise null if
        /// the box is not supported.</returns>
        public BoxType ReadBoxType()
        {
            var boxString = this.ReadString(4).Replace("-", "_");
            BoxType boxType;

            if (!BoxTypeHelpers.TryParse(boxString, out boxType))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Unhandled Box: {0}", boxString);
#endif
                boxType = BoxType.Null;
            }
            return boxType;
        }

        /// <summary>
        /// Reads characters from the stream until it finds the '\0' termination.
        /// </summary>
        /// <returns></returns>
        public string ReadNullTerminatedString()
        {
            char ch;
            StringBuilder builder = new StringBuilder();
            while ((ch = this.ReadChar()) != '\0')
            {
                builder.Append(ch);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Peeks the value of the entension type.
        /// </summary>
        /// <param name="numBytesToSkip">The num bytes to skip before trying to read the type.</param>
        public Guid PeekEntensionType(long numBytesToSkip)
        {
            var offset = this.Offset;
            this.BaseStream.Seek(numBytesToSkip, SeekOrigin.Current);
            var extensionType = this.ReadGuid();
            this.BaseStream.Seek(offset, SeekOrigin.Begin);

            return extensionType;
        }

        /// <summary>
        /// Places the stream reader at the end of the specified box.
        /// </summary>
        /// <param name="boxOffset">The box offset.</param>
        /// <param name="boxSize">The box size.</param>
        public void GotoEndOfBox(long boxOffset, long boxSize)
        {
            this.BaseStream.Seek(boxSize - (this.Offset - boxOffset), System.IO.SeekOrigin.Current);
        }

        /// <summary>
        /// Reads a GUID from the stream.
        /// </summary>
        public Guid ReadGuid()
        {
            uint a = this.ReadUInt32();
            ushort b = this.ReadUInt16();
            ushort c = this.ReadUInt16();
            byte[] buffer = this.ReadBytes(8);
            return new Guid((int)a, (short)b, (short)c, buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7]);
        }

        /// <summary>
        /// Reads a string from the stream with the specified number of characters.
        /// </summary>
        /// <param name="numChars">The number of characters.</param>
        /// <returns></returns>
        public string ReadString(int numChars)
        {
            return new string(this.ReadChars(numChars));
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream and advances the position of the stream by two bytes.
        /// </summary>
        /// <returns>
        /// A 2-byte unsigned integer read from this stream.
        /// </returns>
        /// <exception cref="T:System.IO.EndOfStreamException">The end of the stream is reached. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override ushort ReadUInt16()
        {
            byte num1 = this.ReadByte();
            byte num2 = this.ReadByte();
            return (ushort)((num1 << 8) + num2);
        }

        /// <summary>
        /// Reads a 3-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
        /// </summary>
        /// <returns>
        /// A 3-byte unsigned integer read from this stream.
        /// </returns>
        /// <exception cref="T:System.IO.EndOfStreamException">The end of the stream is reached. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>        
        public uint ReadUInt24()
        {
            uint num1 = this.ReadByte();
            uint num2 = this.ReadByte();
            uint num3 = this.ReadByte();
            return (((num1 << 8) + num2) << 8) + num3;
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
        /// </summary>
        /// <returns>
        /// A 4-byte unsigned integer read from this stream.
        /// </returns>
        /// <exception cref="T:System.IO.EndOfStreamException">The end of the stream is reached. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override uint ReadUInt32()
        {
            uint num1 = this.ReadByte();
            uint num2 = this.ReadByte();
            uint num3 = this.ReadByte();
            uint num4 = this.ReadByte();
            return (uint)((((((num1 << 8) + num2) << 8) + num3) << 8) + num4);
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight bytes.
        /// </summary>
        /// <returns>
        /// An 8-byte unsigned integer read from this stream.
        /// </returns>
        /// <exception cref="T:System.IO.EndOfStreamException">The end of the stream is reached. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream is closed. </exception>
        public override ulong ReadUInt64()
        {
            ulong num1 = this.ReadUInt32();
            ulong num2 = this.ReadUInt32();
            return ((num1 << 0x20) + num2);
        }

        /// <summary>
        /// Reads an unsigned 16 bit integer from the current stream and converts it to three characters and advances
        /// the position of the stream by four bytes.
        /// </summary>
        /// <returns>Returns a string of three characters read and unpacked from the stream.</returns>
        /// <exception cref="T:System.IO.EndOfStreamException">The end of the stream is reached. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The stream is closed. </exception>
        public string ReadUInt16PackedCharacters()
        {
            UInt16 charBits = this.ReadUInt16();
            char[] unpackedChars = new char[3];

            for (int i = 0; i < 3; i++)
            {
                int c = (charBits >> (2 - i) * 5) & 0x1f;
                unpackedChars[i] = (char)(c + 0x60);
            }

            return new string(unpackedChars);
        }

        /// <summary>
        /// Read the next box from the stream.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        public Box ReadNextBox()
        {
            if (this.AtEndOfStream) return null;
            return Box.Create(this);
        }

        /// <summary>
        /// Peeks the type of the next box.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        /// <returns></returns>
        public BoxType PeekNextBoxType()
        {
            if (this.AtEndOfStream) return BoxType.Null;

            var offset = this.Offset;               // Hang onto the offset so we can go back to it
            this.GotoPosition(this.Offset + 4);     // Jump to the box type
            var boxType = this.ReadBoxType();

            this.GotoPosition(offset);
            return boxType;
        }

        /// <summary>
        /// Places the stream reader position in the specified offset.
        /// </summary>
        /// <param name="offset">The stream offset.</param>
        public void GotoPosition(long offset)
        {
            this.BaseStream.Position = offset;
        }

        /// <summary>
        /// Places the stream reader position at the beginning of the 
        /// mfra (Movie Fragment Random Access) box.
        /// </summary>
        /// <remarks>The mfra location is determined by the last four 
        /// bytes of the file.</remarks>
        public void GotoMovieFragmentRandomAccess()
        {
            if (this.mfraOffset == 0)
            {
                // Figure out the offset
                this.BaseStream.Seek(-4, SeekOrigin.End);
                mfraOffset = this.ReadUInt32();
            }

            // Move from the end of the file to the mfra
            this.BaseStream.Seek(-mfraOffset, SeekOrigin.End);
        }

        /// <summary>
        /// Gets all boxes.
        /// </summary>
        /// <returns>The collection of boxes read.</returns>
        public IList<Box> GetAllBoxes()
        {
            var result = new List<Box>();

            do
            {
                var box = ReadNextBox();
                if (box == null) break;
                result.Add(box);

            } while (true);

            return result;
        }
    }
}
