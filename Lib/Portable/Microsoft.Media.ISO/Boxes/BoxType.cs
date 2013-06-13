using System;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Enumerates types of boxes
    /// </summary>
    public enum BoxType
    {
        Unknown,
        Any,
        Moof,
        Mdat,
        Mfhd,
        Traf,
        Tfhd,
        Trun,
        Sdtp,
        Uuid,
        Ftyp,
        Moov,
        Mvhd,
        Trak,
        Tkhd,
        Tref,
        Mdia,
        Mdhd,
        Hdlr,
        Minf,
        Vmhd,
        Smhd,
        Nmhd,
        Dinf,
        Stbl,
        Stts,
        Stss,
        Ctts,
        Stsd,
        Soun,
        Vide,
        Ovc1,
        Owma,
        Encv,
        Enca,
        Enct,
        Encs,
        Mvex,
        Mfra,
        Tfra,
        Mfro,
        Stsc,
        Stco,
        Stsz,
        Esds,
        Sinf,
        Frma,
        Schm,
        Schi,
        Avcc,
        Mp4a,
        Avc1,
        Wma,
        Wfex,
        Vc_1,
        Dvc1,
        Null,
        /* CFF Boxes */
        Pdin,
        Bloc,
        Ainf,
        Xml,
        Bxml,
        Iloc,
        Idat,
        Meta,
        Free,
        Skip,
        Trik,
        Btrt,
        Sthd,
        Stpp,
        Subs,
        Ec_3,
        Dec3,
        Mlpa,
        Dtsc,
        Dtsh,
        Dtsl,
        Dtse,
        Ddts,
        Tfdt,
        Ac_3,
        Dac3,
        Dmlp,
        Saiz,
        Saio,
        Prft,
        Tenc,
        Senc,
        Sbgp,
        Pssh,
        Subt,
        Avcn,
        Sidx,
        Iods
    }

    public static class BoxTypeHelpers
    {
        public static bool TryParse(string boxTypeCode, out BoxType boxType)
        {
#if true // not supported in PCL
            try
            {
                boxType = (BoxType)Enum.Parse(typeof(BoxType), boxTypeCode.Replace('-', '_'), true);
                return true;
            }
            catch (ArgumentException)
            {
                boxType = default(BoxType);
                return false;
            }
#else
            return Enum.TryParse<BoxType>(boxTypeCode.Replace('-', '_'), true, out boxType);
#endif
        }
    }
}
