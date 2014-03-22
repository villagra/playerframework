using Microsoft.Media.AdaptiveStreaming.Dash.Smooth;
using Microsoft.Media.AdaptiveStreaming;
using Microsoft.Media.ISO;
using Microsoft.Media.ISO.Boxes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Microsoft.Media.AdaptiveStreaming.Dash
{
    internal class OfflineCffFileParser : CffFileParser
    {
        public IStorageFile StorageFile { get; set; }
        
        /// <summary>
        /// Opens a local (offline) file and initializes the CFF data that can be used for 
        /// manifest generation.
        /// </summary>
        /// <param name="path">The file URI of the resource to be opened. i.e. ms-appx:////Big_Buck_Bunny.uvu </param>
        public override async Task Parse(Uri path)
        {
            if (StorageFile == null)
            {
                StorageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(path);
            }

            using (var fileStream = await StorageFile.OpenStreamForReadAsync())
            {
                var reader = new BoxBinaryReader(fileStream);
                Box box = null;

                do
                {
                    box = reader.ReadNextBox();

                    if (box != null)
                    {
                        this.Boxes.Add(box);

                        if (box.Type == BoxType.Moov)
                        {
                            // There may be an mdat after the moov, if so parse it
                            if (reader.PeekNextBoxType() == BoxType.Mdat)
                            {
                                box = reader.ReadNextBox();
                                this.Boxes.Add(box);
                            }

                            // After parsing the moov and optional mdat after it, skip to the mfra
                            // this will jump past the moof and mdats which we don't need to process
                            reader.GotoMovieFragmentRandomAccess();
                        }
                    }
                } while (box != null);
            }

            this.InitializeTrackRegistry();
        }

        /// <summary>
        /// Gets a fragment of track data from the file that is being streamed locally.
        /// </summary>
        /// <param name="trackType">The type of track being requested (video, audio, text)</param>
        /// <param name="bitrate">The bitrate of the track that should be returned.</param>
        /// <param name="timeOffset">The time offset of the fragment of data being requested.</param>
        /// <param name="language">The ISO language code of the track if there is one.</param>
        /// <param name="callback">The callback that will be invoked when the fragment has been retrieved.
        /// The callback will have the stream to the data.</param>
        /// <remarks>The fragment that will be returned will be a moof and mdat pair of boxes from the file.</remarks>
        public override async Task<WebRequestorResponse> GetTrackFragmentStream(ManifestTrackType trackType, uint bitrate, ulong timeOffset, string language)
        {
            if (language == string.Empty) language = null;

            var track = this.ManifestTracks
                    .Where(t => t.Type == trackType && t.Bitrate == bitrate && t.Language == language)
                    .SingleOrDefault();

            if (track != null)
            {
                var entry = track.Fragments.TrackFragmentRandomAccessEntries.FirstOrDefault(e => e.Time == timeOffset);

                if (entry != null)
                {
                    ulong moofSize = 0;
                    ulong mdatSize = 0;
                    long moofOffset = 0;

                    using (var fileStream = await StorageFile.OpenStreamForReadAsync())
                    {
                        var reader = new BoxBinaryReader(fileStream);
                        moofOffset = (long)entry.MoofOffset;
                        reader.BaseStream.Seek(moofOffset, SeekOrigin.Begin);

                        moofSize = this.ReadBoxSize(reader);

                        // Run to the end of the moof to get to its mdat box
                        reader.BaseStream.Seek(moofOffset + (long)moofSize, SeekOrigin.Begin);

                        mdatSize = this.ReadBoxSize(reader);

                        // And back to the beginning of the moof so we can read both boxes into the stream
                        reader.BaseStream.Seek(moofOffset, SeekOrigin.Begin);

                        var size = moofSize + mdatSize;
                        var fragment = reader.ReadBytes((int)size);

                        var stream = new MemoryStream(fragment);
                        return new WebRequestorResponse(stream, System.Net.HttpStatusCode.OK, null, string.Empty);
                    }
                }
            }
            throw new WebRequestorFailure(System.Net.HttpStatusCode.NotFound, null);
        }

        public override void Close()
        {
            base.Close();
            StorageFile = null;
        }
    }
}
