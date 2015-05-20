using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Provides a view model specific to nonlinear VPAID ad players.
    /// This helps the control panel properly display and function during a nonlinear ad.
    /// </summary>
    public class VpaidNonLinearAdViewModel : InteractiveViewModel
    {
        /// <summary>
        /// HACK: Allows an instance to be created from Xaml. Without this, xamltypeinfo is not generated and binding will not work.
        /// </summary>
        public VpaidNonLinearAdViewModel() { }

        /// <summary>
        /// The VPAID player playing a nonlinear ad.
        /// </summary>
        public IVpaid Vpaid { get; private set; }

        internal VpaidNonLinearAdViewModel(IVpaid vpaid, MediaPlayer mediaPlayer)
            : base(mediaPlayer)
        {
            Vpaid = vpaid;
        }

        /// <inheritdoc /> 
        protected override void OnPause()
        {
            Vpaid.PauseAd();
            base.OnPause();
        }

        /// <inheritdoc /> 
        protected override void OnPlayResume()
        {
            Vpaid.ResumeAd();
            base.OnPlayResume();
        }
    }
}
