using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The Box (synonymous with Atom) is the basic unit of organization in a MP4 file.
    /// </summary>
    public abstract class Box
    {
        /// <summary>
        /// Gets the offset of the box inside the stream.
        /// </summary>
        public long Offset { get; private set; }

        /// <summary>
        /// Gets the offset of the inner boxes inside the stream.
        /// </summary>
        public long? InnerBoxOffset { get; private set; }

        /// <summary>
        /// Identifies the box type. 
        /// Standard boxes use a compact type, which is normally four printable characters to permit ease of identification. 
        /// </summary>
        public BoxType Type { get; private set; }

        /// <summary>
        /// User extensions types — in this case, the <see cref="Type"/> is set to 'uuid'.
        /// </summary>
        public Guid ExtendedType { get; private set; }

        /// <summary>
        /// Specifies the number of bytes in this box, including all its fields and contained boxes. 
        /// If size is 1, then the actual size is in the field largesize. 
        /// If size is 0, then this box is the last one in the file and its contents extend to the end of the file (normally only used for a Media Data Box)
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Gets or sets the inner boxes of this box.
        /// </summary>
        public List<Box> InnerBoxes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        public Box()
        {
            this.InnerBoxes = new List<Box>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        /// <param name="boxType">Type of the box.</param>
        public Box(long offset, long size, BoxType boxType)
            : this()
        {
            this.Offset = offset;
            this.Size = size;
            this.Type = boxType;
        }

        /// <summary>
        /// Creates a new Box instance that matches the BoxType specified. This will position the 
        /// reader to the end of the box that was read in.
        /// </summary>
        /// <param name="stream">The stream that contains the bytes for the box.</param>
        /// <returns>Returns the box read from the stream; if there was no box to read it 
        /// will return null.</returns>
        public static Box Create(BoxBinaryReader stream)
        {
            Box box = null;
            var offset = stream.Offset;
            var size = stream.ReadBoxSize();
            var boxType = stream.ReadBoxType();

            switch (boxType)
            {
                case BoxType.Moov:
                    box = new MovieBox(offset, size);
                    break;
                case BoxType.Mvhd:
                    box = new MovieHeaderFullBox(offset, size);
                    break;
                case BoxType.Trak:
                    box = new TrackBox(offset, size);
                    break;
                case BoxType.Tkhd:
                    box = new TrackHeaderFullBox(offset, size);
                    break;
                case BoxType.Tref:
                    box = new TrackReferenceBox(offset, size);
                    break;
                case BoxType.Mdia:
                    box = new MediaBox(offset, size);
                    break;
                case BoxType.Mdhd:
                    box = new MediaHeaderFullBox(offset, size);
                    break;
                case BoxType.Hdlr:
                    box = new HandlerReferenceFullBox(offset, size);
                    break;
                case BoxType.Minf:
                    box = new MediaInformationBox(offset, size);
                    break;
                case BoxType.Vmhd:
                    box = new VideoMediaHeaderFullBox(offset, size);
                    break;
                case BoxType.Smhd:
                    box = new SoundMediaHeaderFullBox(offset, size);
                    break;
                case BoxType.Nmhd:
                    box = new NullMediaHeaderFullBox(offset, size);
                    break;
                case BoxType.Dinf:
                    box = new DataInformationBox(offset, size);
                    break;
                case BoxType.Stbl:
                    box = new SampleTableBox(offset, size);
                    break;
                case BoxType.Stts:
                    box = new DecodingTimeToSampleFullBox(offset, size);
                    break;
                case BoxType.Stss:
                    box = new SyncSamplesBox(offset, size);
                    break;
                case BoxType.Ctts:
                    box = new CompositionTimeToSampleFullBox(offset, size);
                    break;
                case BoxType.Stsd:
                    box = new SampleDescriptionFullBox(offset, size);
                    break;
                case BoxType.Mp4a:
                case BoxType.Wma:
                case BoxType.Ac_3:
                case BoxType.Ec_3:
                case BoxType.Mlpa:
                case BoxType.Dtsc:
                case BoxType.Dtsh:
                case BoxType.Dtsl:
                case BoxType.Dtse:
                    box = new AudioSampleEntryBox(offset, size);
                    break;
                case BoxType.Dec3:
                    box = new EC3SpecificBox(offset, size);
                    break;
                case BoxType.Dac3:
                    box = new AC3SpecificBox(offset, size);
                    break;
                case BoxType.Dmlp:
                    box = new MLPSpecificBox(offset, size);
                    break;
                case BoxType.Ddts:
                    box = new DTSSpecificBox(offset, size);
                    break;
                case BoxType.Avc1:
                case BoxType.Vc_1:
                    box = new VisualSampleEntryBox(offset, size);
                    break;
                case BoxType.Stpp:
                    box = new SubtitleSampleEntryBox(offset, size);
                    break;
                case BoxType.Encv:
                case BoxType.Enca:
                case BoxType.Enct:
                case BoxType.Encs:
                    box = new ProtectedSampleEntryBox(offset, size, boxType);
                    break;
                case BoxType.Esds:
                    box = new ElementaryStreamDescriptorFullBox(offset, size);
                    break;
                case BoxType.Avcc:
                    box = new AdvancedVideoCodingBox(offset, size);
                    break;
                case BoxType.Wfex:
                    box = new WaveFormatExBox(offset, size);
                    break;
                case BoxType.Dvc1:
                    box = new DigitalVideoCodingBox(offset, size);
                    break;
                case BoxType.Mvex:
                    box = new MovieExtendsBox(offset, size);
                    break;
                case BoxType.Ftyp:
                    box = new FileTypeBox(offset, size);
                    break;
                case BoxType.Mdat:
                    box = new MediaDataBox(offset, size);
                    break;
                case BoxType.Moof:
                    box = new MovieFragmentBox(offset, size);
                    break;
                case BoxType.Mfra:
                    box = new MovieFragmentRandomAccessBox(offset, size);
                    break;
                case BoxType.Tfra:
                    box = new TrackFragmentRandomAccessFullBox(offset, size);
                    break;
                case BoxType.Mfro:
                    box = new MovieFragmentRandomAccessOffsetFullBox(offset, size);
                    break;
                case BoxType.Mfhd:
                    box = new MovieFragmentHeaderFullBox(offset, size);
                    break;
                case BoxType.Traf:
                    box = new TrackFragmentBox(offset, size);
                    break;
                case BoxType.Tfhd:
                    box = new TrackFragmentHeaderFullBox(offset, size);
                    break;
                case BoxType.Trun:
                    box = new TrackFragmentRunFullBox(offset, size);
                    break;
                case BoxType.Sdtp:
                    box = new IndependentAndDisposableSamplesFullBox(offset, size);
                    break;
                case BoxType.Uuid:
                    box = ExtensionBox.GetNextBoxFromStream(stream, offset, (uint)size);
                    box.ExtendedType = stream.ReadGuid();
                    break;
                case BoxType.Pdin:
                    box = new ProgressiveDownloadInfoFullBox(offset, size);
                    break;
                case BoxType.Bloc:
                    box = new BaseLocationFullBox(offset, size);
                    break;
                case BoxType.Ainf:
                    box = new AssetInformationFullBox(offset, size);
                    break;
                case BoxType.Xml:
                    box = new XmlFullBox(offset, size);
                    break;
                case BoxType.Bxml:
                    box = new BinaryXmlFullBox(offset, size);
                    break;
                case BoxType.Iloc:
                    box = new ItemLocationFullBox(offset, size);
                    break;
                case BoxType.Idat:
                    box = new ItemDataBox(offset, size);
                    break;
                case BoxType.Meta:
                    box = new MetaFullBox(offset, size);
                    break;
                case BoxType.Free:
                case BoxType.Skip:
                    box = new FreeSpaceBox(offset, size);
                    break;
                case BoxType.Trik:
                    box = new TrickPlayFullBox(offset, size);
                    break;
                case BoxType.Btrt:
                    box = new BitRateBox(offset, size);
                    break;
                case BoxType.Sthd:
                    box = new SubtitleMediaHeaderFullBox(offset, size);
                    break;
                case BoxType.Subs:
                    box = new SubSampleInformationFullBox(offset, size);
                    break;
                case BoxType.Tfdt:
                    box = new TrackFragmentBaseMediaDecodeTimeFullBox(offset, size);
                    break;
                case BoxType.Saiz:
                    box = new SampleAuxiliaryInformationSizesFullBox(offset, size);
                    break;
                case BoxType.Saio:
                    box = new SampleAuxiliaryInformationOffsetsFullBox(offset, size);
                    break;
                case BoxType.Prft:
                    box = new ProducerReferenceTimeFullBox(offset, size);
                    break;
                case BoxType.Pssh:
                    box = new ProtectionSystemSpecificHeaderFullBox(offset, size);
                    break;
                case BoxType.Tenc:
                    box = new TrackEncryptionFullBox(offset, size);
                    break;
                case BoxType.Senc:
                    box = new SampleEncryptionFullBox(offset, size);
                    break;
                case BoxType.Schi:
                    box = new SchemeInformationBox(offset, size);
                    break;
                case BoxType.Sinf:
                    box = new ProtectedSchemeInformationBox(offset, size);
                    break;
                case BoxType.Avcn:
                    box = new AVCNALBox(offset, size);
                    break;
                case BoxType.Sidx:
                    box = new SegmentIndexBox(offset, size);
                    break;
                case BoxType.Stsc:
                    box = new SampleChunkBox(offset, size);
                    break;
                case BoxType.Stsz:
                    box = new SampleSizeBox(offset, size);
                    break;
                case BoxType.Stco:
                    box = new SampleChunkOffsetBox(offset, size);
                    break;
                case BoxType.Iods:
                    box = new InitialObjectDescriptorBox(offset, size);
                    break;
                default:
                    stream.GotoEndOfBox(offset, size);
                    break;
            }

            if (box != null)
            {
                box.ReadBoxFromStream(stream);
            }

            return box;
        }

        internal void ReadBoxFromStream(BoxBinaryReader reader)
        {
            if (this.Size == 0)
            {
                throw new BoxException("Zero size not supported!");
            }

            if (this.Size == 1)
            {
                this.Size = (long)reader.ReadUInt64();
            }

            ReadBoxPropertiesFromStream(reader);
            
            if (reader.Offset != this.Size + this.Offset)
            {
                string message = string.Format("The box was not totally read from the stream.\n\tBox Type: {0}, Reader Offset: {1}, Box Size: {2}, Box Offset {3}",
                    this.GetType().Name, reader.Offset, this.Size, this.Offset);

#if DEBUG
                System.Diagnostics.Debug.WriteLine(message);
#endif
                reader.GotoEndOfBox(this.Offset, this.Size);
            }
        }

        protected void ReadInnerBoxes(BoxBinaryReader reader, params BoxType[] expectedBoxTypes)
        {
            InnerBoxOffset = reader.Offset;
            while (reader.Offset < this.Offset + this.Size)
            {
                var box = reader.ReadNextBox();

                if (box != null)
                {
                    if (box.Type != BoxType.Unknown && !expectedBoxTypes.Where(t => t == BoxType.Any || t == box.Type).Any())
                    {
                        throw new BoxException(string.Format("The child box {0} was not expected inside box {1}", box.Type, this.Type));
                    }

                    this.InnerBoxes.Add(box);
                }
            }
        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected abstract void ReadBoxPropertiesFromStream(BoxBinaryReader reader);
    }
}
